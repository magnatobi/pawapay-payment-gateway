namespace PawaPayGateway.Domain.Responses
{
    public class ApiResponseCodes
    {
        public const string SUCCESSFUL = "00";

        //ERRORS
        public const string VALIDATION_ERROR = "01";

        public const string INVALID_OTP = "02";

        public const string EXPIRED_OTP = "03";

        public const string EXISTING_USER = "04";

        public const string SIM_ALREADY_VERIFIED = "05";

        public const string INVALID_LOGIN_CREDENTIALS = "06";

        public const string LOCKED_PROFILE = "07";

        public const string OPERATION_FAILED = "08";

        public const string EXISTING_SIM = "09";

        public const string SIM_NOT_FOUND = "10";

        public const string INVALID_SIM_STATE = "11";

        public const string UNVERIFIED_SIM = "12";

        public const string SIM_LIMIT_REACHED = "13";

        public const string INVALID_EMAIL = "14";

        public const string INVALID_PIN = "15";

        public const string PASSWORD_RESET_FAILED = "16";

        public const string INVALID_PHONE_NUMBER = "17";

        public const string BAD_PASSWORD = "18";

        public const string INVALID_REGISTRATION = "19";

        public const string INVALIDATED_TRACKING_NUMBER = "20";

        public const string SIM_MAX_DELETION_LIMIT = "21";

        public const string RECHARGE_FAILED = "22";

        public const string PROCESSING = "23";

        public const string PARTNER_TOKEN_ERROR = "24";

        public const string SERVER_ERROR = "25";

        public const string NO_AVAILABLE_WALLET = "26";

        public const string INSUFFICIENT_FUNDS = "27";

        public const string USER_NOT_FOUND = "28";

        public const string PAYMENT_FAILED = "29";

        public const string BAD_REGISTRATION = "30";

        public const string INVALID_PASSWORD = "31";

        public const string ACCOUNT_DETAIL_INVALID = "32";

        public const string INVALID_USER_STATE = "33";

        public const string TRANSACTION_NOT_FOUND = "34";

        public const string INVALID_OPERATION = "35";

        public const string FEE_NOT_FOUND = "36";

        public const string OBJECT_NOT_FOUND = "37";

        public const string REWARD_NOT_FOUND = "38";

        public const string TRANSACTION_LIMIT_REACHED = "39";

        public const string USER_EMAIL_NOT_VERIFIED = "40";

        public const string KYC_ALREADY_EXIST = "41";

        public const string KYC_NOTFOUND = "42";

        public const string ISSUE_NOT_FOUND = "43";

        public static string KYC_VALIDATION_FAILED = "44";

        public const string INVALID_ID = "45";

        public const string PROMOTION_NOT_FOUND = "46";

        public const string INVALID_PARTNER_WALLET = "47";

        public const string PARTNER_WALLET_NOTFOUND = "48";

        public const string PARTNER_WALLET_RETREIVE_SUCCESSFULLY = "49";

        public const string SMS_GATEWAY_NOTFOUND = "50";

        public const string PARTNER_NOT_FOUND = "51";

        public const string SIM_SERIAL_NUMBER_UPDATED_SUCCESSFULLY = "52";

        public const string RECORD_NOT_FOUND = "53";

        public const string ACCOUNT_DELETED_IN_PAST_7_DAYS = "54";

        public const string INCORRECT_SIM_NETWORK = "55";

        public const string INVALID_BILLPAYMENT_ACCOUNTNUMBER = "56";

        public const string EMAIL_NOT_CONFIRMED = "57";

        public const string FEATURE_DISABLED = "60";

        public const string DESTINATION_NETWORK_DISABLED = "58";
    }
}
