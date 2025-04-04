namespace PawaPayGateway.Domain.Responses
{
    /// <summary>
    /// Base response with data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiBaseResponse<T> : ApiBaseResponseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public T? data { get; set; }
        public List<string> errors { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public ApiBaseResponse(bool success, int code, string message)
        {
            this.success = success;
            this.code = code;
            this.message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public ApiBaseResponse(bool success, int code, string message, T data)
        {
            this.success = success;
            this.code = code;
            this.message = message;
            this.data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="errors"></param>
        public ApiBaseResponse(bool success, int code, string message, List<string> errors)
        {
            this.success = success;
            this.code = code;
            this.message = message;
            this.errors = errors;
        }
    }


    /// <summary>
    /// Base model for API response
    /// </summary>
    public abstract class ApiBaseResponseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; } = string.Empty;
    }
}
