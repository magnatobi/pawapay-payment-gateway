using PawaPayGateway.Domain;
using PawaPayGateway.Domain.Entities;
using PawaPayGateway.Domain.Enums;
using PawaPayGateway.Domain.Interfaces;
using PawaPayGateway.Domain.Models.Paystack;
using PawaPayGateway.Domain.Responses;
using PawaPayGateway.Infrastructure.Configs;
using PawaPayGateway.Infrastructure.Implementations;
using System.Net;
using System.Text.Json;

namespace PawaPayGateway.Infrastructure.Services
{
    public class PaystackTransferService : TransactionProvider, IBankTransactionProvider
    {
        private const string BaseUrl = "https://api.paystack.co";
        private const string PaymentLinkEndpoint = "/transaction/initialize";
        private const string TransferRecipientEndpoint = "/transferrecipient";
        private const string TransferEndpoint = "/transfer";
        private const string PaymentStatusEndpoint = "/transaction/verify";
        private const string CallBackUrl = "api/simtransactions/paystackcallback";
        private const string BalanceEndpoint = "/balance";
        private readonly PaystackConfig _config;
        private readonly ILogger<PaystackTransferService> _logger;
        private readonly IEmailService _emailService;
        private string _domainAPIHost;

        public PaystackTransferService(PaystackConfig config, ILogger<PaystackTransferService> logger, string domainAPIHost, IEmailService emailService, Gateway gw)
        {
            _config = config;
            _logger = logger;
            _emailService = emailService;
            _domainAPIHost = domainAPIHost;
            GatewayId = Convert.ToInt32(gw.Id);
            Name = gw.Name;
            Priority = gw.Priority;
            CredentialKey = gw.CredentialKey;
            Url = gw.GatewayUrl;
            Status = gw.Status;
        }

        public override GatewayType GatewayType => GatewayType.Paystack;

        public async Task<ApiBaseResponse<TransactionStatusResponse>> QueryTransaction(string transactionReference, string collectionReference)
        {
            var client = new RestClient<bool>(_logger);

            var headers = new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {_config.SecretKey}"}
            };

            var response = await client.GetAsync($"{BaseUrl}{PaymentStatusEndpoint}/{transactionReference}", headers);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Paystack QueryTransaction failed for ${transactionReference}: {response.Result}");
                return new ApiBaseResponse<TransactionStatusResponse>(false, (int)HttpStatusCode.BadRequest, $"Error fetching transfer status.");
            }

            var result = JsonSerializer.Deserialize<TransferStatus>(response.Result, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            _logger.LogDebug($"Paystack QueryTransaction success {response.Result}");
            return new ApiBaseResponse<TransactionStatusResponse>(true, (int)HttpStatusCode.OK, "Query successful",
                new TransactionStatusResponse
                {
                    TransactionStatus = MapTransactionStatus(result?.data?.status),
                    status = result.data.status,
                    amount = (result.data.amount ?? 0) / 100,
                    settled_amount = ((result.data.amount ?? 0) - (result.data.fees ?? 0)) / 100
                });
        }

        public static TransactionStatusEnum MapTransactionStatus(string statusStr)
        {
            return statusStr switch
            {
                null => TransactionStatusEnum.UNKNOWN,
                "success" => TransactionStatusEnum.SUCCESS,
                "abandoned" => TransactionStatusEnum.CANCELLED,
                "cancelled" => TransactionStatusEnum.CANCELLED,
                _ => TransactionStatusEnum.UNKNOWN
            };
        }

        public async Task<ApiBaseResponse<ValidateAccountNumberResponse>> GetAccountName(string accountNumber, string bankCode)
        {
            var client = new RestClient<bool>(_logger);

            var headers = new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {_config.SecretKey}"}
            };

            var response = await client.GetAsync($"{BaseUrl}/bank/resolve?account_number={accountNumber}&bank_code={bankCode}", headers);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Paystack failed to validate account {accountNumber} {bankCode}: {response.Result}");
                return new ApiBaseResponse<ValidateAccountNumberResponse>(false, (int)HttpStatusCode.BadRequest, $"Error creating transfer recepient");
            }

            _logger.LogDebug($"Paystack validated account {accountNumber} {bankCode}: {response.Result}");
            var account = JsonSerializer.Deserialize<AccountResolveResponse>(response.Result, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return new ApiBaseResponse<ValidateAccountNumberResponse>(true, (int)HttpStatusCode.OK, $"Operation successful",
                new ValidateAccountNumberResponse
                {
                    AccountName = account.data.account_name,
                    AccountNumber = account.data.account_number
                });
        }

        public async Task<GatewayBaseResponseModel> SendFunds(string bankCode, string accountNumber, decimal amount, string reference,
            string accountName = null, string narration = "")
        {
            var client = new RestClient<CreateRecipientRequest>(_logger);
            var recipient = new CreateRecipientRequest
            {
                type = "nuban",
                name = accountName,
                account_number = accountNumber,
                bank_code = bankCode,
                currency = "NGN"
            };

            var headers = new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {_config.SecretKey}"}
            };

            //Create transfer recipient by posting to https://api.paystack.co/transferrecipient:
            //with payload i.e.{"type":"nuban","name":null,"account_number":"8012345678","bank_code":"50515","currency":"NGN"}
            var response = await client.PostAsync($"{BaseUrl}{TransferRecipientEndpoint}", headers, recipient);

            //Check transaction status
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Bad Paystack SendFunds create recipient response {response.StatusCode} {response.Result}");

                return new GatewayBaseResponseModel
                {
                    Data = response,
                    Status = GatewayResponseEnum.Failed,
                    Message = response.message
                };
                //return new ApiBaseResponse<TransferToAccountResponse>(ApiResponseCodes.OPERATION_FAILED, nameof(ApiResponseCodes.OPERATION_FAILED), $"Error creating transfer recepient");
            }

            //Deserialize transfer recipient response from paystack
            var result = JsonSerializer.Deserialize<CreateRecipientResponse>(response.Result, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            //Build transfer request
            var client2 = new RestClient<InitiateTransferRequest>(_logger);
            var transferRequest = new InitiateTransferRequest
            {
                source = "balance",
                amount = ((int)(amount * 100)).ToString(), //paystack transacts in kobo,
                recipient = result.data.recipient_code,
                reason = narration,
                reference = reference
            };

            //Initialise bank transfer request
            var transferResponse = await client2.PostAsync($"{BaseUrl}{TransferEndpoint}", headers, transferRequest);

            //Check bank transfer status code or
            if (!transferResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"Bad Paystack SendFunds response {transferResponse.StatusCode} {transferResponse.Result}");

                //Deserilize bad response bank transfer to check for validation errors i.e. duplicate transactions
                var bankTransferBadResponse = JsonSerializer.Deserialize<BankTransferBadResponse>(transferResponse.Result, new JsonSerializerOptions(JsonSerializerDefaults.Web));

                //Process duplicate transaction reference from response
                //i.e. bad response from https://api.paystack.co/transfer:
                //{"status":false, "message":"Please provide a unique reference. Reference already exists on a transfer",
                //"meta":{"nextStep":"Create a new reference for the new transfer"}, "type":"validation_error", "code":"duplicate_transfer_reference"}
                if (bankTransferBadResponse?.code == "duplicate_transfer_reference")
                {
                    return new GatewayBaseResponseModel
                    {
                        Data = transferResponse,
                        Status = GatewayResponseEnum.Success,
                        Message = transferResponse.message
                    };
                }

                return new GatewayBaseResponseModel
                {
                    Data = transferResponse,
                    Status = GatewayResponseEnum.Failed,
                    Message = transferResponse.message
                };
            }

            _logger.LogDebug($"Paystack SendFunds success {transferResponse.Result}");
            var transferResult = JsonSerializer.Deserialize<InitiateTransferResponse>(transferResponse.Result, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            //todo: shouldn't we interrogate the response to see what it says first?
            if (transferResult.data.status.Equals("pending", StringComparison.OrdinalIgnoreCase))
            {
                return new GatewayBaseResponseModel
                {
                    Data = transferResult,
                    Status = GatewayResponseEnum.Success,
                    Message = transferResult.message
                };
            }

            return new GatewayBaseResponseModel
            {
                Data = transferResult,
                Status = GatewayResponseEnum.Failed,
                Message = transferResult.message
            };
        }

        public async Task<PaymentProvider> ReceiveFundsDetails(ReceiveFundsRequest request)
        {
            string link;
            if (request.DoGenerateLinks)
            {
                var linkResp = await ReceiveFundsLink(request);
                link = linkResp.status ? linkResp.link : null;
            }
            else
            {
                link = null;
            }

            return new PaymentProvider
            {
                GatewayId = this.GatewayId,
                Key1 = _config.PublicKey,
                Name = "Paystack", // visible in the App
                GatewayType = this.GatewayType,
                PaymentLink = link
            };
        }

        public async Task<RecievePaymentResponseModel> ReceiveFundsLink(ReceiveFundsRequest request)
        {
            PaystackRequest paystack = new PaystackRequest
            {
                reference = request.Reference,
                callback_url = $"{_domainAPIHost}/{CallBackUrl}",
                amount = decimal.ToInt32(request.Amount * 100).ToString(),
                email = request.Email,
            };

            var restClient = new RestClient<PaystackRequest>(_logger);
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {_config.SecretKey}");
            headers.Add("Cache-Control", "no-cache");

            var response = await restClient.PostAsync($"{BaseUrl}{PaymentLinkEndpoint}", headers, paystack);
            if (response.IsSuccessStatusCode)
            {
                var paymentResult = JsonSerializer.Deserialize<PaymentResponse>(response.Result, new JsonSerializerOptions(JsonSerializerDefaults.Web));

                return new RecievePaymentResponseModel
                {
                    link = paymentResult.Data.authorization_url,
                    message = paymentResult.Message,
                    status = true,
                    platformType = GatewayType
                };
            }

            return new RecievePaymentResponseModel
            {
                link = string.Empty,
                message = "Not Successful",
                status = false,
                //platformType = PlatformName.Paystack
            };
        }

        public override void ProcessError(string httpStatusCode, string message)
        {
            var emailModel = new MailRequest
            {
                BodyText =
                    $"Dear Operations, The following exception occurred at the gateway end. Please see gateway response: {message} => with httpsStatusCode of {httpStatusCode}",
                Subject = "Gateway Exception"
            };
            _emailService.SendInternalSupportEmail(emailModel);
        }

        public override async Task<GatewayBaseResponseModel> CheckAvailability()
        {
            var balance = await QueryGatewayBalance();
            return QueryGatwewayAvailablity(balance);
        }

        public override async Task<GatewayBalanceResponse> QueryGatewayBalance()
        {
            var client = new RestClient<bool>(_logger);

            var headers = new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {_config.SecretKey}"}
            };

            var response = await client.GetAsync($"{BaseUrl}{BalanceEndpoint}", headers);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Paystack balance query failed for ${nameof(PaystackTransferService)}: {response.Result}");
                return new GatewayBalanceResponse
                {
                    Balance = 0m,
                    Data = response?.Result,
                    Message = response?.message,
                    Status = GatewayResponseEnum.Failed
                };
            }

            _logger.LogDebug($"Paystack Balance success {response.Result}");
            var balanceResult = JsonSerializer.Deserialize<PaystackBalance>(response.Result, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return new GatewayBalanceResponse
            {
                //todo: confirm if this is correct: take the minimum of the balances returned (this currently is what the alert job expects.
                Balance = balanceResult.data.Min(data => data.balance),
                Data = balanceResult.data,
                Status = GatewayResponseEnum.Success,
                Message = balanceResult.message
            };
        }
    }
}