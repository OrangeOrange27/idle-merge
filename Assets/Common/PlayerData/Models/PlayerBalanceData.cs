using Newtonsoft.Json;

namespace Common.PlayerData
{
    public class PlayerBalanceData
    {
        [JsonProperty("coins")] public int Coins;
        [JsonProperty("gems")] public int Gems;
        [JsonProperty("energy")] public int Energy;
        
        public override string ToString()
        {
            return $"PlayerBalanceData: Coins: {Coins}, Gems: {Gems}, Energy: {Energy}";
        }
    }
}