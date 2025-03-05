using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Common.PlayerData
{
    public class PlayerBalanceData
    {
        [JsonProperty("coins")] public int Coins;
        [JsonProperty("gems")] public int Gems;
        [JsonProperty("energy")] public int Energy;
        
        public event Action<PlayerBalanceAssetType, int> OnBalanceChanged;
        
        private Dictionary<PlayerBalanceAssetType, (Func<int> getter, Action<int> adder)> _balanceAccessors =>
            new()
            {
                {
                    PlayerBalanceAssetType.Coins, (() => Coins, value =>
                    {
                        Coins += value;
                        OnBalanceChanged?.Invoke(PlayerBalanceAssetType.Coins, Coins);
                    })
                },
                {
                    PlayerBalanceAssetType.Gems, (() => Gems, value =>
                    {
                        Gems += value;
                        OnBalanceChanged?.Invoke(PlayerBalanceAssetType.Gems, Gems);
                    })
                },
                {
                    PlayerBalanceAssetType.Energy, (() => Energy, value =>
                    {
                        Energy += value;
                        OnBalanceChanged?.Invoke(PlayerBalanceAssetType.Energy, Energy);
                    })
                },
            };
        
        public int GetBalance(PlayerBalanceAssetType balanceType)
        {
            return _balanceAccessors[balanceType].getter();
        }

        public void ChangeBalance(PlayerBalanceAssetType balanceType, int amount)
        {
            _balanceAccessors[balanceType].adder(amount);
        }
        
        public override string ToString()
        {
            return $"PlayerBalanceData: Coins: {Coins}, Gems: {Gems}, Energy: {Energy}";
        }
    }
}