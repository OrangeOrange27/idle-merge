using System;
using Newtonsoft.Json;

namespace Common.PlayerData
{
    public class PlayerLoginData
    {
        [JsonProperty("createdAt")]
        private long _createdAt;

        [JsonProperty("lastLogin")]
        private long _lastLogin;

        public DateTime CreatedAt
        {
            get => DateTimeOffset.FromUnixTimeMilliseconds(_createdAt).UtcDateTime;
            set => _createdAt = new DateTimeOffset(value).ToUnixTimeMilliseconds();
        }

        public DateTime LastLogin
        {
            get => DateTimeOffset.FromUnixTimeMilliseconds(_lastLogin).UtcDateTime;
            set => _lastLogin = new DateTimeOffset(value).ToUnixTimeMilliseconds();
        }
        
        public override string ToString()
        {
            return $"PlayerLoginData: CreatedAt: {CreatedAt}, LastLogin: {LastLogin}";
        }
    }
}