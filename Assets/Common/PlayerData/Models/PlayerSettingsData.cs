using Newtonsoft.Json;

namespace Common.PlayerData
{
    public class PlayerSettingsData
    {
        [JsonProperty("isMusicEnabled")] public bool MusicEnabled { get; set; }

        [JsonProperty("isSoundEnabled")] public bool SoundEnabled { get; set; }

        [JsonProperty("isVibrationsEnabled")] public bool VibrationsEnabled { get; set; }
        [JsonProperty("languageCode")] public string LanguageCode { get; set; }

        public bool NotificationsEnabled { get; set; }

        public override string ToString()
        {
            return
                $"PlayerSettingsData: MusicEnabled: {MusicEnabled}, SoundEnabled: {SoundEnabled}, VibrationsEnabled: {VibrationsEnabled}, NotificationsEnabled: {NotificationsEnabled}";
        }
    }
}