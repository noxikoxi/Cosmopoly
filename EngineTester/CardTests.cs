using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.models;
using Engine;
using Engine.utils;
using Engine.strategies;

// 18 Asserts

namespace EngineTester
{
    [TestClass]
    public sealed class CardTests
    {
        private Game game;

        [TestInitialize]
        public void Setup()
        {
            game = new Game([new Player("Player1"), new Player("Player2")], "valid_game_config");
        }


        [TestMethod]
        public void TestCardInitialization()
        {
            var cards = ConfigLoader.LoadCardConfig("valid_game_config/Cards.json");
            Assert.IsNotNull(cards, "Cards should be loaded successfully.");
            Assert.AreEqual(7, cards.Count, "There should be 3 cards loaded.");
        }

        [TestMethod]
        public void Test_Shield_Strategy()
        {
            Card card = new(title: "a", description: "b", strategies : [new CardStrategy("Shield", 0)], applyTogether: true);
            Assert.AreEqual(strategyType.Shield, card.strategies.First().Type, "Card should have Shield strategy.");
            game.ApplyCard(card);
            Assert.AreEqual(1, game.CurrentPlayer.ShieldCards);
        }

        [TestMethod]
        public void Test_Move_Strategy()
        {
            Card card = new(title: "a", description: "b", strategies: [new CardStrategy("Move", 0)], applyTogether: true);
            Assert.AreEqual(strategyType.Move, card.strategies.First().Type, "Card should have Move strategy.");
            game.ApplyCard(card, 10);
            Assert.AreEqual(10, game.CurrentPlayer.position);
        }

        [TestMethod]
        public void Test_Give_Credits_Strategy()
        {
            Card card = new(title: "a", description: "b", strategies: [new CardStrategy("GiveCredits", 1000)], applyTogether: true);
            Assert.AreEqual(strategyType.GiveCredits, card.strategies.First().Type, "Card should have GiveCredits strategy.");
            game.ApplyCard(card);
            Assert.AreEqual(1000, game.CurrentPlayer.Credits);
        }

        [TestMethod]
        public void Test_Remove_Credits_Strategy()
        {
            Card card = new(title: "a", description: "b", strategies: [new CardStrategy("TakeCredits", 1000)], applyTogether: true);
            Assert.AreEqual(strategyType.TakeCredits, card.strategies.First().Type, "Card should have TakeCredits strategy.");
            game.ApplyCard(card);
            Assert.AreEqual(-1000, game.CurrentPlayer.Credits);
        }

        [TestMethod]
        public void Test_Cannot_Create_Card_With_Neagtive_Value()
        {
            Assert.ThrowsException<System.ArgumentException>(() =>
            {
                Card card = new(title: "a", description: "b", strategies: [new CardStrategy("TakeCredits", -1000)], applyTogether: true);
            });
        }

        [TestMethod]
        public void Test_Use_Shield_Strategy()
        {
            Card card = new(title: "a", description: "b", strategies: [new CardStrategy("UseShield", 0)], applyTogether: true);
            game.CurrentPlayer.ShieldCards = 1;
            game.ApplyCard(card);
            Assert.AreEqual(0, game.CurrentPlayer.ShieldCards, "Shield card should be used and count should be 0.");
        }

        [TestMethod]
        public void Test_Block_Turn_Strategy()
        {
            Card card = new(title: "a", description: "b", strategies: [new CardStrategy("BlockTurn", 2)], applyTogether: true);
            game.ApplyCard(card);
            Assert.AreEqual(2, game.CurrentPlayer.BlockedTurns, "Blocked Turns should be 2.");
        }

        [TestMethod]
        public void Test_Destroy_Strategy()
        {
            Card card = new(title: "a", description: "b", strategies: [new CardStrategy("Destroy", 2)], applyTogether: true);
            game.CurrentPlayer.Credits = 100000;
            (game.Entities[1] as HabitablePlanet).Owner  = game.CurrentPlayer;
            (game.Entities[2] as HabitablePlanet).Owner = game.CurrentPlayer;
            (game.Entities[3] as HabitablePlanet).Owner = game.CurrentPlayer;
            (game.Entities[4] as HabitablePlanet).Owner = game.CurrentPlayer;

            game.NextPlayer();
            game.NextPlayer();
            var playerSystem = game.GetPlayerSystems();
            Assert.IsTrue(playerSystem.Count() == 1, "Player should have 1 system before destroy strategy.");
            Assert.IsTrue(game.GetPossibleSystemUpgrades(playerSystem[0]).Keys.Count() > 0);
            game.UpgradeSystem(playerSystem[0], "Shipyard");
            Assert.IsTrue(game.GetPossibleSystemUpgrades(playerSystem[0]).Keys.Count() == 0, "Player should not have any upgrades after GalacticShipyard upgrade.");
            Assert.IsTrue(playerSystem[0].IsGalacticShipyardBuilt, "GalacticShipyard should be built in the player's system.");

            game.ApplyCard(card);

            Assert.IsFalse(playerSystem[0].IsGalacticShipyardBuilt);
        }
    }
}
