using Cysharp.Threading.Tasks;

namespace Common.Authentication.Providers
{
    public interface IProvider
    {
        /// <summary>
        /// Initializes the provider with necessary configuration.
        /// </summary>
        /// <returns>true if init, false otherwise</returns>
        UniTask<bool> Initialize();

        /// <summary>
        /// Initiates the login process with the provider.
        /// </summary>
        /// <returns>The task result contains a tuple with a
        /// boolean indicating login success and a string with the token or
        /// error message.</returns>
        UniTask<(bool, string)> LoginAsync();

        /// <summary>
        /// Logs out the user from the provider.
        /// </summary>
        UniTask Logout();

        /// <summary>
        /// Checks if the user is currently logged in.
        /// </summary>
        /// <returns>True if the user is logged in, otherwise false.</returns>
        UniTask<bool> IsLoggedIn();
    }

    public enum AuthProvider
    {
        Guest,
        Facebook
    }
}