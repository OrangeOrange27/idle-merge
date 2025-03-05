using Newtonsoft.Json;

namespace Common.PlayerData
{
    public class PlayerPurchaseData
    {
        [JsonProperty("items")] public string[] Items;
        [JsonProperty("ltv")] public int LTV;

        public override string ToString()
        {
            var itemsStr = Items != null ? string.Join(", ", Items) : "None";
            return $"PlayerPurchaseData: Items: [{itemsStr}], LTV: {LTV}";
        }
    }
}