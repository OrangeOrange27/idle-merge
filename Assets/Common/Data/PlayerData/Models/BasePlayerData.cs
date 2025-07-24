using Newtonsoft.Json;

namespace Common.PlayerData
{
    public class BasePlayerData
    {
        [JsonProperty("id")] public string ID;

        public string AuthToken;

        [JsonProperty("username")] public string UserName;

        [JsonProperty("email")] public string Email;

        [JsonProperty("loginData")] public PlayerLoginData LoginData = new();

        [JsonProperty("purchases")] public PlayerPurchaseData PurchaseData = new();
        
        public override string ToString()
        {
            return $"BasePlayerData:\n" +
                   $"ID: {ID}\n" +
                   $"UserName: {UserName}\n" +
                   $"Email: {Email}\n" +
                   $"LoginData: {LoginData}\n" +
                   $"PurchaseData: {PurchaseData}";
        }
    }
}