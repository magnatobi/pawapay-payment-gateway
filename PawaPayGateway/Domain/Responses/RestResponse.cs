namespace PawaPayGateway.Domain.Responses
{
    public class RestResponse
    {
        public bool IsSuccessStatusCode = false;
        public string StatusCode = string.Empty;
        public string reasonPhrase = string.Empty;
        public string message = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSuccessStatusCode"></param>
        /// <param name="StatusCode"></param>
        /// <param name="reasonPhrase"></param>
        /// <param name="result"></param>
        public RestResponse(bool isSuccessStatusCode, string StatusCode, string reasonPhrase, string result)
        {
            this.IsSuccessStatusCode = isSuccessStatusCode;
            this.StatusCode = StatusCode;
            this.reasonPhrase = reasonPhrase;
            Result = result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSuccessStatusCode"></param>
        /// <param name="StatusCode"></param>
        /// <param name="reasonPhrase"></param>
        /// <param name="result"></param>
        public RestResponse(bool isSuccessStatusCode, string StatusCode, string reasonPhrase, byte[] result)
        {
            this.IsSuccessStatusCode = isSuccessStatusCode;
            this.StatusCode = StatusCode;
            this.reasonPhrase = reasonPhrase;
            ResultByteAray = result;
        }

        public string Result { get; set; } = string.Empty;

        public byte[] ResultByteAray { get; set; }
    }
}
