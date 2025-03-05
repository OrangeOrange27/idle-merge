using Common.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.Authentication.Providers.Anonymous
{
    public class GuestSignInProvider : IProvider
    {
        private bool isLoggedIn;
        private string userToken;
        
        public UniTask<bool> Initialize()
        {
            return UniTask.FromResult(true);
        }
        
        public UniTask<(bool, string)> LoginAsync()
        {
            userToken = SystemInfo.deviceUniqueIdentifier;

            if (!userToken.IsNullOrEmpty())
            {
                isLoggedIn = true;
                return UniTask.FromResult((true, userToken));
            }

            return UniTask.FromResult((false, "Failed to retrieve device unique identifier."));
        }
        
        public UniTask Logout()
        {
            isLoggedIn = false;
            userToken = null;

            return UniTask.FromResult(true);
        }
        
        public UniTask<bool> IsLoggedIn()
        {
            return UniTask.FromResult(isLoggedIn);
        }
    }
}
