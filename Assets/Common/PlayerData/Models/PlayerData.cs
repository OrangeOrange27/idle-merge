using System;
using Newtonsoft.Json;

namespace Common.PlayerData
{
    public class PlayerData : BasePlayerData
    {
        [JsonProperty("balance")] public PlayerBalanceData Balance;
        [JsonProperty("settings")] public PlayerSettingsData Settings = new();
        [JsonProperty("extras")] public ExtraPlayerData Extras = new();

        public static PlayerData CreateNew()
        {
            PlayerData playerData = new()
            {
                ID = Guid.NewGuid().ToString(),
                Settings = new PlayerSettingsData { MusicEnabled = true, SoundEnabled = true, VibrationsEnabled = true },
                Balance = new PlayerBalanceData(),
                Extras = new ExtraPlayerData
                {
                    //optional data here
                },
            };

            return playerData;
        }
        
        public override string ToString()
        {
            return base.ToString() +
                   "\nSoloPlayerData:\n" +
                   $"Settings: {Settings}\n" +
                   $"Balance: {Balance}";
        }
    }
}