using Common.Utils;
using Common.Utils.Extensions;
using Cysharp.Threading.Tasks;
using Facebook.Unity;
using UnityEngine;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.Authentication.Providers.Facebook
{
    public class FacebookProvider : IProvider
    {
        private static readonly ILogger Logger = Package.Logger.Abstraction.LogManager.GetLogger<FacebookProvider>();
        private const string EDITOR_TOKEN_KEY = "FB_TestToken";
        
        /// <summary>
        /// Initializes the Facebook SDK.
        /// </summary>
        public async UniTask<bool> Initialize()
        {
            UniTaskCompletionSource<bool> tcs = new();

            if (!FB.IsInitialized)
            {
                FB.Init(() =>
                {
                    if (FB.IsInitialized)
                    {
                        FB.ActivateApp();
                    }
                    tcs.TrySetResult(FB.IsInitialized);
                });
            }
            else
            {
                FB.ActivateApp();
                tcs.TrySetResult(true);
            }

            return await tcs.Task;
        }

        /// <summary>
        /// Initiates the Facebook login process.
        /// </summary>
        public async UniTask<(bool, string)> LoginAsync()
        {
            if (!FB.IsInitialized)
            {
                Logger.ZLogWarning("Facebook SDK was not init. Attempting init.");
                bool didInit = await Initialize();
                if (!didInit)
                {
                    return (false, "Facebook SDK is not initialized.");
                }
            }

            UniTaskCompletionSource<(bool, string)> tcs = new();

            //For Editor test purposes
            if (EnvInfo.IsEditor)
            {
                string testToken = "TEST TOKEN HERE"; //TODO: Add test token here
                tcs.TrySetResult((true, testToken));
                PlayerPrefs.SetString(EDITOR_TOKEN_KEY, testToken);
                return await tcs.Task;
            }
            
            FB.LogInWithReadPermissions(new[] { "public_profile", "email" }, result =>
            {
                if (result.Cancelled)
                {
                    tcs.TrySetResult((false, "Facebook login was cancelled."));
                }
                else if (!result.Error.IsNullOrEmpty())
                {
                    tcs.TrySetResult((false, result.Error));
                }
                else if (result.AccessToken != null)
                {
                    tcs.TrySetResult((true, result.AccessToken.TokenString));
                }
                else
                {
                    tcs.TrySetResult((false, "Facebook login failed. Unknown error."));
                }
            });

            return await tcs.Task;
        }

        /// <summary>
        /// Logs out the user from Facebook.
        /// </summary>
        public async UniTask Logout()
        {
            if (FB.IsLoggedIn)
            {
                FB.LogOut();
                Logger.ZLogInformation("Logged out from Facebook.");
            }
            else
            {
                Logger.ZLogInformation("Not logged in to Facebook.");
            }

            await UniTask.CompletedTask;
        }

        /// <summary>
        /// Checks if the user is currently logged in to Facebook.
        /// </summary>
        public UniTask<bool> IsLoggedIn()
        {
            if (EnvInfo.IsEditor)
            {
                return UniTask.FromResult(PlayerPrefs.HasKey(EDITOR_TOKEN_KEY));
            }
            
            return UniTask.FromResult(FB.IsLoggedIn);
        }
    }
}
