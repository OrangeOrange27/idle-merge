using System;
using System.Collections.Generic;
using Common.Authentication.Providers;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ZLogger;

namespace Common.Server
{
    public static partial class ServerAPI
    {
        public class Login
        {
            private static readonly ILogger Logger = Package.Logger.Abstraction.LogManager.GetLogger<Login>();
            /// <summary>
            /// Authenticates the user using the specified login data.
            /// POST: /auth/login
            /// </summary>
            /// <param name="loginData">The login data struct to be sent in the request body.</param>
            /// <returns>Returns the server response as a string.</returns>
            public static async UniTask<LoginResponse> LoginAsync(LoginData loginData)
            {
                string url = $"{BaseUrl}/auth/login";

                string jsonBody = JsonConvert.SerializeObject(loginData);

                ResponseModel<LoginResponse> response = await ServerRequest.PostRequest<LoginResponse>(url, jsonBody);

                if (!response.IsSuccess)
                {
                    Logger.ZLogError(response.ErrorMessage);
                    return null;
                }

                return response.Data;
            }
        }
        
        public struct LoginData
        {
            [JsonProperty("authProvider")] public string AuthProvider;
            [JsonProperty("token")] public string Token;
            [JsonProperty("extra")] public Dictionary<string, object> Extra;

            public LoginData(AuthProvider authProvider, string token)
            {
                AuthProvider = authProvider.ToString().ToLower();
                Token = token;
                Extra = new();
            }
        }

        public class LoginResponse
        {
            [JsonProperty("tokens")] public Tokens Tokens { get; set; }

            public string GetAccessToken()
            {
                return Tokens.Access.Token;
            }
        }

        public struct Tokens
        {
            [JsonProperty("access")] public AccessToken Access { get; set; }
        }

        public struct AccessToken
        {
            [JsonProperty("token")] public string Token { get; set; }

            [JsonProperty("expires")] public DateTime Expires { get; set; }
        }
    }
}