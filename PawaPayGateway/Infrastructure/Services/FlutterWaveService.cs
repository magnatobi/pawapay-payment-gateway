using PawaPayGateway.Domain;
using PawaPayGateway.Domain.Entities;
using PawaPayGateway.Domain.Enums;
using PawaPayGateway.Domain.Interfaces;
using PawaPayGateway.Domain.Models.Flutterwave;
using PawaPayGateway.Domain.Responses;
using PawaPayGateway.Domain.Responses.Flutterwave;
using PawaPayGateway.Infrastructure.Configs;
using PawaPayGateway.Infrastructure.Implementations;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PawaPayGateway.Infrastructure.Services
{
    public class FlutterWaveService : TransactionProvider, IBankTransactionProvider
    {
        private const string TransferUrl = "https://api.flutterwave.com/v3/transfers";
        private const string BalanceEndpoint = "https://api.flutterwave.com/v3/balances";
        private const string ReceiveUrl = "https://api.flutterwave.com/v3/payments";
        private const string GetTransactionUrl = "https://api.flutterwave.com/v3/transactions";
        private const string ConfirmTransactionUrl = "https://api.flutterwave.com/v3/transactions/[id]/verify/";
        private const string AirtimeDisburseUrl = "https://api.flutterwave.com/v3/bills";
        private const string BankVerificationUrl = "https://api.flutterwave.com/v3/accounts/resolve";
        private const string CallBackUrl = "api/simtransactions/flutterwavecallback";
        private readonly HttpClient _client;
        private readonly ILogger<FlutterWaveService> _logger;
        private readonly FlutterWaveConfig _config;
        private readonly string _tingtelAPIHost;

        public FlutterWaveService(HttpClient client, FlutterWaveConfig config, ILogger<FlutterWaveService> logger,
            string tingtelAPIHost, Gateway gw)
        {
            _client = client;
            _logger = logger;
            _config = config;
            _tingtelAPIHost = tingtelAPIHost;
            GatewayId = Convert.ToInt32(gw.Id);
            Name = gw.Name;
            Priority = gw.Priority;
            CredentialKey = gw.CredentialKey;
            Url = gw.GatewayUrl;
            Status = gw.Status;
        }

        public override GatewayType GatewayType => GatewayType.FlutterWave;

        public async Task<ApiBaseResponse<TransactionStatusResponse>> QueryTransaction(string transactionReference, string collectionReference)
        {
            //check the format of the transactionReference
            // if numeric, it's the flutterwave generated transaction_id
            // if alphanumeric, it's a tingtel reference

            long transactionId = 0;
            bool isFlutterwaveId = long.TryParse(transactionReference, out transactionId); //i now = 108
            if (!isFlutterwaveId)
            {
                return await QueryTransactionByReference(transactionReference);
            }

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.SecretKey);

            var url = $"{ConfirmTransactionUrl.Replace("[id]", transactionReference)}";
            var response = await _client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var responseObject = JsonSerializer.Deserialize<FlutterBaseResponse>(responseString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                _logger.LogDebug($"Flutterwave QueryTransaction success {responseString}");

                return new ApiBaseResponse<TransactionStatusResponse>(true, (int)HttpStatusCode.OK, "Query successful", new TransactionStatusResponse
                {
                    TransactionStatus = MapTransactionStatus(responseObject?.data.status, responseObject?.data.processor_response),
                    status = responseObject.data.status,
                    amount = responseObject.data.amount,
                    settled_amount = responseObject.data.amount_settled,
                    statusDescription = responseObject.data.narration,
                    mReference = responseObject.data.reference,
                    TransactionInfo = string.Concat(responseObject.data.narration, "/", responseObject.data.processor_response)
                });
            }

            _logger.LogWarning($"Flutterwave QueryTransaction failed for {transactionReference}: {responseString}");
            return new ApiBaseResponse<TransactionStatusResponse>(false, (int)HttpStatusCode.BadRequest, $"Error fetching transfer status.");
        }

        private async Task<ApiBaseResponse<TransactionStatusResponse>> QueryTransactionByReference(string transactionReference)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.SecretKey);

            var url = $"{GetTransactionUrl}?tx_ref={transactionReference}";
            var response = await _client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var responseObject = JsonSerializer.Deserialize<FlutterSearchResponse>(responseString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                if (responseObject.data != null && responseObject.data.Length > 0)
                {
                    _logger.LogDebug($"Flutterwave QueryTransactionByReference success {responseString}");

                    return new ApiBaseResponse<TransactionStatusResponse>(true, (int)HttpStatusCode.OK, "Query successful", new TransactionStatusResponse
                    {
                        TransactionStatus = MapTransactionStatus(responseObject.data[0].status, responseObject.data[0].processor_response),
                        status = responseObject.data[0].status,
                        amount = responseObject.data[0].amount,
                        settled_amount = responseObject.data[0].amount_settled,
                        statusDescription = responseObject.data[0].narration,
                        mReference = responseObject.data[0].reference,
                        TransactionInfo = string.Concat(responseObject.data[0].narration, "/", responseObject.data[0].processor_response)
                    });
                }
            }

            _logger.LogWarning($"Flutterwave QueryTransactionByReference failed for {transactionReference}: {responseString}");
            return new ApiBaseResponse<TransactionStatusResponse>(false, (int)HttpStatusCode.BadRequest, $"Error fetching transfer status.");
        }

        public static TransactionStatusEnum MapTransactionStatus(string statusStr, string processorResponse)
        {
            return statusStr switch
            {
                null => TransactionStatusEnum.UNKNOWN,
                "successful" => TransactionStatusEnum.SUCCESS,
                "failed" => TransactionStatusEnum.CANCELLED,
                _ when "Transaction not completed by user".Equals(processorResponse, StringComparison.OrdinalIgnoreCase) => TransactionStatusEnum.CANCELLED,
                _ => TransactionStatusEnum.UNKNOWN
            };
        }

        public Task<PaymentProvider> ReceiveFundsDetails(ReceiveFundsRequest request)
        {
            return Task.FromResult(new PaymentProvider
            {
                GatewayId = this.GatewayId,
                Key1 = _config.EncryptKey,
                Key2 = _config.PublicKey,
                Name = "Flutterwave", // visible in the App
                GatewayType = GatewayType
            });
        }

        public async Task<RecievePaymentResponseModel> ReceiveFundsLink(ReceiveFundsRequest request)
        {
            var customer = new FlutterCustomer
            {
                email = request.Email
            };
            var flutterwaveRequest = new FluterwaveReceivePaymentRequest
            {
                amount = request.Amount.ToString(),
                tx_ref = request.Reference,
                currency = "NGN",
                payment_options = "card",
                redirect_url = $"{_tingtelAPIHost}/{CallBackUrl}",
                customer = customer
            };

            FlutterBaseResponse flutterBaseResponse = null;
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.SecretKey);
            var jsonObject = JsonSerializer.Serialize(flutterwaveRequest);
            _logger.LogDebug($"Flutterwave ReceiveFunds request {jsonObject}");
            var response = await _client.PostAsync(new Uri($"{ReceiveUrl}"), new StringContent(jsonObject, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                flutterBaseResponse = JsonSerializer.Deserialize<FlutterBaseResponse>(responseString, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                _logger.LogDebug($"Flutterwave ReceiveFunds response {responseString}");

                return new RecievePaymentResponseModel
                {
                    message = flutterBaseResponse.message,
                    status = true,
                    link = flutterBaseResponse.data.Link,
                    platformType = GatewayType
                };
            }

            _logger.LogError($"Bad Flutterwave ReceiveFunds response {response.StatusCode} {response.ReasonPhrase}");
            return new RecievePaymentResponseModel
            {
                message = response.ReasonPhrase,
                status = false,
                link = null
            };
        }

        public async Task<GatewayBaseResponseModel> SendFunds(string bankCode, string accountNumber, decimal amount, string reference,
            string accountName = null, string narration = "TINGTEL AIRTIME")
        {
            var callbackUrl = $"{_tingtelAPIHost}/{CallBackUrl}";
            var modelRequest = new TransferModelRequest
            {
                account_bank = bankCode,
                account_number = accountNumber,
                reference = reference,
                amount = (int)amount,
                callback_url = callbackUrl,
                debit_currency = "NGN",
                currency = "NGN",
                narration = narration
            };

            FlutterBaseResponse flutterBaseResponse = null;
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.SecretKey);
            var jsonObject = JsonSerializer.Serialize(modelRequest);
            _logger.LogDebug($"Flutterwave SendFunds request {jsonObject}");
            var response = await _client.PostAsync(new Uri($"{TransferUrl}"), new StringContent(jsonObject, Encoding.UTF8, "application/json"));
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Flutterwave SendFunds success {responseString}");
                flutterBaseResponse = JsonSerializer.Deserialize<FlutterBaseResponse>(responseString, new JsonSerializerOptions(JsonSerializerDefaults.Web));

                GatewayResponseEnum transactionStatus = flutterBaseResponse.status.Equals("success", StringComparison.OrdinalIgnoreCase) ? GatewayResponseEnum.Success : GatewayResponseEnum.Failed;

                return new GatewayBaseResponseModel
                {
                    Data = flutterBaseResponse,
                    Status = transactionStatus,
                    Message = flutterBaseResponse.message,
                };
            }

            _logger.LogError($"Bad Flutterwave SendFunds response {response.StatusCode} {responseString}");
            return new GatewayBaseResponseModel
            {
                Data = flutterBaseResponse,
                Status = GatewayResponseEnum.Failed,
                Message = flutterBaseResponse.message,
            };
        }

        public override void ProcessError(string httpStatusCode, string message)
        {
            throw new NotImplementedException();
        }

        public override async Task<GatewayBaseResponseModel> CheckAvailability()
        {
            var balance = await QueryGatewayBalance();
            return QueryGatwewayAvailablity(balance);
        }

        public override async Task<GatewayBalanceResponse> QueryGatewayBalance()
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "FLWSECK_TEST-81211e7e430a2859a7c95e6fd2837d99-X");
            var response = await _client.GetAsync(new Uri($"{BalanceEndpoint}"));
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Flutterwave wallets balance query failed, reason >> {response.ReasonPhrase}");
                return new GatewayBalanceResponse
                {
                    Balance = 0m,
                    Status = GatewayResponseEnum.Failed,
                    Data = null,
                };
            }

            var responseString = await response.Content.ReadAsStringAsync();
            _logger.LogDebug($"FLutterwave wallets balance query success {responseString}");
            var responseObject = JsonSerializer.Deserialize<FLutterBalance>(responseString, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return new GatewayBalanceResponse
            {
                //todo: confirm if this is correct: take the minimum of the balances returned (this currently is what the alert job expects.
                Balance = responseObject.data.Min(data => data.available_balance),
                Data = responseObject.data,
                Message = responseObject.message,
                Status = GatewayResponseEnum.Success
            };
        }

        public async Task<ApiBaseResponse<ValidateAccountNumberResponse>> GetAccountName(string accountNumber, string bankCode)
        {
            var request = new FlutterVerifyBankRequest
            {
                account_bank = bankCode,
                account_number = accountNumber
            };

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.SecretKey);
            var jsonObject = JsonSerializer.Serialize(request);
            var response = await _client.PostAsync(new Uri($"{BankVerificationUrl}"), new StringContent(jsonObject, Encoding.UTF8, "application/json"));
            var responseString = await response.Content.ReadAsStringAsync();

            try
            {
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogDebug($"Flutterwave GetAccountName response {responseString}");
                    var flutterResponse = JsonSerializer.Deserialize<FlutterBaseResponse>(responseString, new JsonSerializerOptions(JsonSerializerDefaults.Web));

                    if (flutterResponse.status == "success" && flutterResponse.data?.account_name != null)
                    {
                        return new ApiBaseResponse<ValidateAccountNumberResponse>(true, (int)HttpStatusCode.OK, "OPERATION_SUCCESSFUL",
                            new ValidateAccountNumberResponse
                            {
                                AccountName = flutterResponse.data.account_name,
                                AccountNumber = flutterResponse.data.account_number
                            });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }

            _logger.LogInformation($"Flutterwave failed to validate GetAccountName {accountNumber} {bankCode} {responseString}");
            return new ApiBaseResponse<ValidateAccountNumberResponse>(false, (int)HttpStatusCode.BadRequest, "FAILED_TO_VALIDATE_BANK_ACCOUNT");
        }
    }
}