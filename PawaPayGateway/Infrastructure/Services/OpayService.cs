using PawaPayGateway.Domain;
using PawaPayGateway.Domain.Entities;
using PawaPayGateway.Domain.Enums;
using PawaPayGateway.Domain.Interfaces;
using PawaPayGateway.Domain.Models.Opay;
using PawaPayGateway.Domain.Responses;
using PawaPayGateway.Domain.Responses.Opay;
using PawaPayGateway.Infrastructure.Configs;
using PawaPayGateway.Infrastructure.Implementations;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PawaPayGateway.Infrastructure.Services
{
    /// <summary>
    ///
    /// </summary>
    public class OpayService : TransactionProvider, IBankTransactionProvider
    {
        private readonly ILogger<OpayService> _logger;
        public override GatewayType GatewayType => GatewayType.Opay;
        private readonly IEmailService _emailService;
        private readonly OpayConfig _config;

        public OpayService(ILogger<OpayService> logger, IEmailService emailService, OpayConfig config, Gateway gw)
        {
            _logger = logger;
            _emailService = emailService;
            _config = config;
            Priority = gw.Priority;
            Status = gw.Status;
            GatewayId = Convert.ToInt32(gw.Id);
            Name = gw.Name;
            CredentialKey = gw.CredentialKey;
        }

        public override async Task<GatewayBaseResponseModel> CheckAvailability()
        {
            var balance = await QueryGatewayBalance();
            return QueryGatwewayAvailablity(balance);
        }

        public async Task<ApiBaseResponse<ValidateAccountNumberResponse>> GetAccountName(string accountNumber, string bankCode)
        {
            var restClient = new RestClient<GetAccountNameRequest>(_logger);

            var body = new GetAccountNameRequest
            {
                bankAccountNo = accountNumber,
                bankCode = bankCode,
                countryCode = "NG"
            };

            var response = await restClient.PostAsync($"{_config.Url}/verification/accountNumber/resolve", GetHeaders(), body);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Opay failed to validate account {accountNumber} {bankCode}: {response.Result}");
                return new ApiBaseResponse<ValidateAccountNumberResponse>(false, (int)HttpStatusCode.BadRequest, $"Error creating transfer recepient");
            }

            _logger.LogDebug($"Opay validated account {accountNumber} {bankCode}: {response.Result}");

            var data = JsonSerializer.Deserialize<GetAccountNameResponse>(response.Result);

            return new ApiBaseResponse<ValidateAccountNumberResponse>(true, (int)HttpStatusCode.OK, $"Operation successful",
                new ValidateAccountNumberResponse
                {
                    AccountName = data?.data.accountName,
                    AccountNumber = data?.data.accountNo
                });
        }

        private Dictionary<string, string> GetHeaders()
        {
            var headers = new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {_config.AuthToken}"},
                {"Content-Type", "application/json"},
                {"MerchantID", _config.MerchantId}
            };
            return headers;
        }

        private Dictionary<string, string> GetPaymentHeaders(string signature)
        {
            var headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json"},
                {"MerchantId", _config.MerchantId},
                {"Authorization", $"Bearer {signature}" }
            };

            return headers;
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

        public override async Task<GatewayBalanceResponse> QueryGatewayBalance()
        {
            var client = new RestClient<object>(_logger);

            var response = await client.PostAsync($"{_config.Url}/balances", GetHeaders(), null);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Opay balance query failed for ${nameof(OpayService)}: {response.Result}");
                return new GatewayBalanceResponse
                {
                    Balance = 0m,
                    Data = response?.Result,
                    Message = response?.message,
                    Status = GatewayResponseEnum.Failed
                };
            }

            _logger.LogDebug($"Opay Balance success {response.Result}");
            var data = JsonSerializer.Deserialize<QueryBalanceResponse>(response.Result);

            return new GatewayBalanceResponse
            {
                Balance = Convert.ToDecimal(data?.data?.cashBalance?.value) / 100,
                Data = data,
                Message = data.message,
                Status = GatewayResponseEnum.Success
            };
        }

        public async Task<ApiBaseResponse<TransactionStatusResponse>> QueryTransaction(string transactionReference, string orderNumber)
        {
            var client = new RestClient<QueryTransactionRequest>(_logger);
            var request = new QueryTransactionRequest
            {
                reference = transactionReference,
                orderNo = orderNumber
            };

            string signature = HashRequest(JsonSerializer.Serialize(request), _config.SecretKey);

            var response = await client.PostAsync($"{_config.Url}/transfer/status/toBank", GetPaymentHeaders(signature), request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Opay QueryTransaction failed for ${transactionReference}: {response.Result}");
                return new ApiBaseResponse<TransactionStatusResponse>(false, (int)HttpStatusCode.BadRequest, $"Error fetching transfer status.");
            }

            var result = JsonSerializer.Deserialize<QueryTransactionResponse>(response.Result);

            if (!result.code.Equals("00000"))
            {
                return new ApiBaseResponse<TransactionStatusResponse>(false, (int)HttpStatusCode.BadRequest, $"Error fetching transfer status.");
            }

            return new ApiBaseResponse<TransactionStatusResponse>(true, (int)HttpStatusCode.OK, "Query successful", new TransactionStatusResponse
            {
                status = result.code,
                amount = Convert.ToDecimal(result.data?.amount) / 100,
                settled_amount = Convert.ToDecimal(result.data?.amount) / 100,
                TransactionStatus = GetTransactionStatus(result?.data?.status)
            });
        }

        public Task<PaymentProvider> ReceiveFundsDetails(ReceiveFundsRequest request)
        {
            return Task.FromResult(new PaymentProvider
            {
                GatewayId = this.GatewayId,
                Key1 = _config.MerchantId,
                Key2 = _config.PublicKey,
                Key3 = _config.AuthToken,
                Name = "OPay", // visible in the App
                GatewayType = this.GatewayType
            });
        }

        public async Task<RecievePaymentResponseModel> ReceiveFundsLink(ReceiveFundsRequest request)
        {
            var client = new RestClient<InitializeCheckoutRequest>(_logger);
            var checkoutRequest = new InitializeCheckoutRequest
            {
                mchShortName = "Test",
                productName = "Test",
                productDesc = "Test",
                amount = (request.Amount * 100).ToString(),
                callbackUrl = request.CallbackUrl,
                currency = "NGN",
                expireAt = "10",
                payMethods = new string[] { "account", "qrcode", "bankCard", "bankAccount", "bankTransfer", "bankUSSD" },
                payTypes = new string[] { "BalancePayment", "BonusPayment", "OWealth" },
                reference = request.Reference,
                // userPhone = model.Phone,
                userRequestIp = request.UserRequestIPAddress,
                returnUrl = request.ReturnUrl
            };

            var response = await client.PostAsync($"{_config.Url}/cashier/initialize", GetHeaders(), checkoutRequest);
            if (response.IsSuccessStatusCode)
            {
                var paymentResult = JsonSerializer.Deserialize<InitializeCheckoutResponse>(response.Result);
                return new RecievePaymentResponseModel
                {
                    link = paymentResult.data.cashierUrl,
                    message = paymentResult.message,
                    platformType = GatewayType.Opay,
                    status = paymentResult.code == "00000"
                };
            }

            return new RecievePaymentResponseModel
            {
                link = string.Empty,
                message = "Not Successful",
                status = false,
            };
        }

        public async Task<GatewayBaseResponseModel> SendFunds(string bankCode, string accountNumber, decimal amount, string reference,
            string accountName = null, string narration = "")
        {
            var client = new RestClient<TransferToBankRequest>(_logger);
            var body = new TransferToBankRequest
            {
                amount = (amount * 100).ToString(),
                country = "NG",
                currency = "NGN",
                reason = narration,

                receiver = new TransferReceiver
                {
                    bankAccountNumber = accountNumber,
                    bankCode = bankCode,
                    name = accountName
                },
                reference = reference,
            };

            string bodyAsJsonString = JsonSerializer.Serialize(body);

            string signature = HashRequest(bodyAsJsonString, _config.SecretKey);

            var response = await client.PostAsync($"{_config.Url}/transfer/toBank", GetPaymentHeaders(signature), body);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Bad Opay SendFunds status code response {response.StatusCode} {response.Result}");

                return new GatewayBaseResponseModel
                {
                    Data = response,
                    Status = GatewayResponseEnum.Failed,
                    Message = response.message
                };
            }

            _logger.LogDebug($"Opay SendFunds success {response.Result}");
            var data = JsonSerializer.Deserialize<TransferToBankResponse>(response.Result);

            if (!data.code.Equals("00000"))
            {
                _logger.LogError($"Bad Opay SendFunds response code {data.code} {response.Result}");

                return new GatewayBaseResponseModel
                {
                    Data = response,
                    Status = GatewayResponseEnum.Failed,
                    Message = response.message
                };
            }

            return new GatewayBaseResponseModel
            {
                Status = GetGatewayResponse(data?.data?.status),
                Message = data.message,
                Data = data
            };
        }

        private TransactionStatusEnum GetTransactionStatus(string status)
        {
            return status switch
            {
                "SUCCESSFUL" => TransactionStatusEnum.SUCCESS,
                "INITIAL" => TransactionStatusEnum.SUCCESS,
                "FAILED" => TransactionStatusEnum.FAILED,
                "PENDING" => TransactionStatusEnum.SUCCESS,
                "CLOSED" => TransactionStatusEnum.CANCELLED,
                _ => TransactionStatusEnum.FAILED,
            };
        }

        private GatewayResponseEnum GetGatewayResponse(string status)
        {
            return status switch
            {
                "SUCCESSFUL" => GatewayResponseEnum.Success,
                "FAILED" => GatewayResponseEnum.Failed,
                "PENDING" => GatewayResponseEnum.Success,
                "INITIAL" => GatewayResponseEnum.Success,
                _ => GatewayResponseEnum.Failed,
            };
        }

        public static string HashRequest(string request, string secretKey)
        {
            byte[] key = Encoding.UTF8.GetBytes(secretKey);
            byte[] sourceBytes = Encoding.UTF8.GetBytes(request);
            using var hmacSha512 = new HMACSHA512(key);

            byte[] hashBytes = hmacSha512.ComputeHash(sourceBytes);
            string hash = string.Concat(Array.ConvertAll(hashBytes, b => b.ToString("X2"))).ToLower();

            return hash;
        }
    }
}