using System.Collections;
using Common.GlobalServiceLocator;
using Common.TimeService.Tests;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;
using VContainer;

namespace Features.Core.SupplySystem.Tests
{
    [TestFixture]
    [TestOf(typeof(SupplyManager))]
    public class SupplySystemTests : VContainerTestFixture
    {
        protected override void InstallDependencies(IContainerBuilder containerBuilder)
        {
            GlobalServices.Reset();

        }
        
        [UnityTest]
        public IEnumerator PlayFirstStreakInRow_NextStreakIndex_Offset_StreakCardsCountInLastStreak()
        {
            return UniTask.ToCoroutine(async () =>
            {
                Resolver.Resolve<InitStreakModel>().StreakCardsCountInLastStreak = 2;
                Resolver.Resolve<InitStreakModel>().CurrentStreakIndex = 0;
                
                var gameplayController = await Resolver.InitializeGameplayController(LevelConfig);

                var cardContext = gameplayController.CardContext;
                cardContext.StreakModel.StreakCardsCountInLastStreak.Value.Should().Be(2);
                cardContext.StreakModel.CurrentStreakIndex.Value.Should().Be(0);
                
                gameplayController.PlayCard(cardContext.GameAreaCards().First(model => model.IsOpened.Value));
                    
                cardContext.StreakModel.StreakCardsCountInLastStreak.Value.Should().Be(0);
                cardContext.StreakModel.CurrentStreakIndex.Value.Should().Be(1);
            });
        }
    }
}