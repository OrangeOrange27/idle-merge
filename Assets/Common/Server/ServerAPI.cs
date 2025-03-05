using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Package.Logger.Abstraction;
using ZLogger;

namespace Common.Server
{
    public static partial class ServerAPI
    {
        private const string BaseUrl = "";
        
        public class Player
        {
            private static readonly ILogger Logger = LogManager.GetLogger<Player>();
            /// <summary>
            /// Retrieves the player data using the Bearer token.
            /// GET: /players/me
            /// </summary>
            /// <param name="bearerToken">The Bearer token for authorization (access token).</param>
            /// <returns>Returns the player data as a string.</returns>
            public static async UniTask<T> GetPlayerDataAsync<T>(string bearerToken)
            {
                string url = $"{BaseUrl}/players/me";

                ResponseModel<T> response = await ServerRequest.GetRequest<T>(url, bearerToken);

                if (!response.IsSuccess)
                {
                    Logger.ZLogError(response.ErrorMessage);
                    return default;
                }

                return response.Data;
            }

            /// <summary>
            /// Updates the player's data.
            /// Post: /players/me
            /// </summary>
            /// <param name="playerData">The player data, should derive from BasePlayerData</param>
            /// <param name="bearerToken">The Bearer token for authorization (access token).</param>
            /// <returns>Returns the player data as a string.</returns>
            public static async UniTask<T> UpdatePlayerDataAsync<T>(T playerData, string bearerToken)
            {
                string url = $"{BaseUrl}/players";
                string jsonBody = JsonConvert.SerializeObject(playerData);
                ResponseModel<T> response = await ServerRequest.PatchRequest<T>(url, jsonBody, bearerToken);

                if (!response.IsSuccess)
                {
                    Logger.ZLogError(response.ErrorMessage);
                    return default;
                }

                return response.Data;
            }
            
            public static async UniTask<T> DeleteUser<T>(string bearerToken)
            {
                string url = $"{BaseUrl}/players";
                ResponseModel<T> response = await ServerRequest.DeleteRequest<T>(url, bearerToken);

                if (!response.IsSuccess)
                {
                    Logger.ZLogError(response.ErrorMessage);
                    return default;
                }

                return response.Data;
            }
        }
    }
}