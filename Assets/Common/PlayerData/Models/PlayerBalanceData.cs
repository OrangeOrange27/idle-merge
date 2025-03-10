using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utils.Extensions;
using Features.Core.Placeables.Models;
using Newtonsoft.Json;

namespace Common.PlayerData
{
    public class PlayerBalanceData
    {
        [JsonProperty("coins")] public int Coins;
        [JsonProperty("gems")] public int Gems;
        [JsonProperty("energy")] public int Energy;
        [JsonProperty("collectibles")] public Collectible[] Collectibles;

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

        public event Action<CollectibleType, int> OnCollectibleAmountChanged;
        private Dictionary<CollectibleType, (Func<int> getter, Action<int> adder)> _collectiblesAccessors;

        public PlayerBalanceData()
        {
            EnsureCollectiblesArraySize();

            _collectiblesAccessors = new Dictionary<CollectibleType, (Func<int> getter, Action<int> adder)>();

            foreach (var collectibleType in EnumExtensions.EnumToList<CollectibleType>())
            {
                _collectiblesAccessors.Add(collectibleType, (() => GetCollectibleAmountInternal(collectibleType),
                    value =>
                    {
                        ChangeCollectibleAmountInternal(collectibleType, value);
                        OnCollectibleAmountChanged?.Invoke(collectibleType,
                            GetCollectibleAmountInternal(collectibleType));
                    }));
            }
        }

        public int GetBalance(PlayerBalanceAssetType balanceType)
        {
            return _balanceAccessors[balanceType].getter();
        }

        public void ChangeBalance(PlayerBalanceAssetType balanceType, int amount)
        {
            _balanceAccessors[balanceType].adder(amount);
        }

        public int GetCollectibleAmount(CollectibleType collectibleType)
        {
            return _collectiblesAccessors[collectibleType].getter();
        }

        public void ChangeCollectibleAmount(CollectibleType collectibleType, int amount)
        {
            _collectiblesAccessors[collectibleType].adder(amount);
        }

        public override string ToString()
        {
            return $"PlayerBalanceData: Coins: {Coins}, Gems: {Gems}, Energy: {Energy}";
        }

        private int GetCollectibleAmountInternal(CollectibleType collectibleType)
        {
            return GetCollectible(collectibleType)?.Amount ?? 0;
        }

        private void ChangeCollectibleAmountInternal(CollectibleType collectibleType, int amount)
        {
            var collectible = GetCollectible(collectibleType);
            if (collectible == null)
                throw new ArgumentNullException(nameof(collectible));

            collectible.Amount += amount;
        }

        private void EnsureCollectiblesArraySize()
        {
            var enumValuesCount = EnumExtensions.GetValuesCount<CollectibleType>();

            if(Collectibles.IsNullOrEmpty())
                Collectibles = new Collectible[enumValuesCount];
            
            if (Collectibles.Length == enumValuesCount)
                return;

            var collectibles = new Collectible[enumValuesCount];
            if (collectibles == null)
                throw new ArgumentNullException(nameof(collectibles));

            foreach (var type in EnumExtensions.EnumToList<CollectibleType>())
            {
                var amount = Collectibles.FirstOrDefault(collectible => collectible.CollectibleType == type)?.Amount ??
                             0;
                collectibles[(int)type] = new Collectible { CollectibleType = type, Amount = amount };
            }
        }

        private Collectible GetCollectible(CollectibleType collectibleType)
        {
            return Collectibles.FirstOrDefault(collectible => collectible.CollectibleType == collectibleType);
        }
    }
}