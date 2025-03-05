using System;
using Common.Utils.Extensions;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using UnityEngine.Networking;
using ZLogger;

namespace Common.Server
{
    public static class ServerRequest
    {
        private static readonly ILogger Logger = LogManager.GetLogger(nameof(ServerRequest));
        /// <summary>
        /// Makes a GET request to the given URL with a Bearer token.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="bearerToken">The Bearer token for authorization.</param>
        /// <typeparam name="T">The type of data expected in the response.</typeparam>
        /// <returns>A ResponseModel containing the success status and data.</returns>
        public static async UniTask<ResponseModel<T>> GetRequest<T>(string url, string bearerToken)
        {
            using UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("Authorization", "Bearer " + bearerToken);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            return await SendRequest<T>(webRequest);
        }

        /// <summary>
        /// Makes a POST request to the given URL with a JSON body.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="jsonBody">The JSON data to be sent as the body of the request.</param>
        /// <param name="bearerToken">The Bearer token for authorization.</param>
        /// <typeparam name="T">The type of data expected in the response.</typeparam>
        /// <returns>A ResponseModel containing the success status and data.</returns>
        public static async UniTask<ResponseModel<T>> PostRequest<T>(string url, string jsonBody, string bearerToken = null)
        {
            UnityWebRequest request = CreateOutgoingRequest("POST", url, jsonBody, bearerToken);
            return await SendRequest<T>(request);
        }
        
        /// <summary>
        /// Makes a PATCH request to the given URL with a JSON body.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="jsonBody">The JSON data to be sent as the body of the request.</param>
        /// <param name="bearerToken">The Bearer token for authorization.</param>
        /// <typeparam name="T">The type of data expected in the response.</typeparam>
        /// <returns>A ResponseModel containing the success status and data.</returns>
        public static async UniTask<ResponseModel<T>> PatchRequest<T>(string url, string jsonBody, string bearerToken = null)
        {
            UnityWebRequest request = CreateOutgoingRequest("PATCH", url, jsonBody, bearerToken);
            return await SendRequest<T>(request);
        }
        
        /// <summary>
        /// Makes a DELETE request to the given URL with a JSON body.
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="jsonBody">The JSON data to be sent as the body of the request.</param>
        /// <param name="bearerToken">The Bearer token for authorization.</param>
        /// <typeparam name="T">The type of data expected in the response.</typeparam>
        /// <returns>A ResponseModel containing the success status and data.</returns>
        public static async UniTask<ResponseModel<T>> DeleteRequest<T>(string url, string bearerToken)
        {
            UnityWebRequest request = CreateOutgoingRequest("Request", url, null, bearerToken);
            return await SendRequest<T>(request);
        }

        private static UnityWebRequest CreateOutgoingRequest(string type, string url, string jsonBody = null, string bearerToken = null)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url, type);
            if (!jsonBody.IsNullOrEmpty())
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            webRequest.downloadHandler = new DownloadHandlerBuffer();

            if (!bearerToken.IsNullOrEmpty())
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + bearerToken);
            }
            
            webRequest.SetRequestHeader("Content-Type", "application/json");

            return webRequest;
        }

        /// <summary>
        /// Sends the request, processes the result, and converts the response to the desired type.
        /// </summary>
        /// <typeparam name="T">The type of data expected in the response.</typeparam>
        /// <param name="request">The UnityWebRequest to send.</param>
        /// <returns>A ResponseModel containing the success status and the data of type T, or error message if failed.</returns>
        private static async UniTask<ResponseModel<T>> SendRequest<T>(UnityWebRequest request)
        {
            try
            {
                await request.SendWebRequest();
            }
            catch (Exception e)
            {
                return ResponseModel<T>.Error(e.Message);
            }
            
            if (request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Logger.ZLogError("Error: " + request.error);
                return ResponseModel<T>.Error(request.error);
            }
            
            var responseText = request.downloadHandler.text;

            var responseData = ResponseModel<T>.FromServerResponse(responseText);

            return responseData;
        }
    }
}