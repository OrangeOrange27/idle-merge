using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Package.Logger.Abstraction;
using ZLogger;

namespace Common.Server
{
    public class ResponseModel<T>
    {
        private static readonly ILogger Logger = LogManager.GetLogger<ResponseModel<T>>();
        /// <summary>
        /// Indicates whether the API call was successful, based on the "status" field.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The data returned from the API, deserialized from the "data" field.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// The error message if the API call failed, based on the "error" field.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Static method to create a success response model.
        /// </summary>
        /// <param name="data">The data to be returned in the success response.</param>
        /// <returns>A ResponseModel with IsSuccess set to true and the provided data.</returns>
        public static ResponseModel<T> Success(T data)
        {
            return new ResponseModel<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        /// <summary>
        /// Static method to create an error response model.
        /// </summary>
        /// <param name="errorMessage">The error message to be returned.</param>
        /// <returns>A ResponseModel with IsSuccess set to false and the provided error message.</returns>
        public static ResponseModel<T> Error(string errorMessage)
        {
            return new ResponseModel<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        /// <summary>
        /// Static method to create a response model based on the server response.
        /// </summary>
        /// <param name="serverResponse">The raw JSON response from the server.</param>
        /// <returns>A ResponseModel with IsSuccess, Data, and ErrorMessage set appropriately.</returns>
        public static ResponseModel<T> FromServerResponse(string serverResponse)
        {
            BaseResponse<T> baseResponse;
            
            try
            {
                baseResponse = JsonConvert.DeserializeObject<BaseResponse<T>>(serverResponse);
            }
            catch (JsonException ex)
            {
               Logger.ZLogError(ex, "JSON Deserialization");
                return Error("Failed to deserialize response.");
            }
            

            if (baseResponse == null)
            {
                return Error("Can't deserialize response");
            }
            
            return baseResponse.Status == "OK" ? Success(baseResponse.Data) : Error(baseResponse.Error);
        }

        /// <summary>
        /// Class to match the server's response structure for deserialization.
        /// </summary>
        private class BaseResponse<TData>
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("error")]
            public string Error { get; set; }

            [JsonProperty("data")]
            public TData Data { get; set; }
            
            [JsonProperty("meta")]
            public TData Meta { get; set; }
        }
    }
}
