using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Engine.models;
using Newtonsoft.Json.Bson;

// 41 Asserts

namespace EngineTester
{
    [TestClass]
    public sealed class GameTests
    {
        private Game game;

        [TestInitialize]
        public void Setup()
        {
            game = new Game([new Player("Player1"), new Player("Player2")], "valid_game_config");
        }


        [TestMethod]
        public void TestGameInitialization()
        {
            Assert.IsNotNull(game, "Game should be initialized.");
            Assert.AreEqual("Player1", game.CurrentPlayer.Name, "Current Player should be Player 1)");
            Assert.IsTrue(game.Entities.Count() > 0, "Game Entities cannot be empty");
            Assert.IsTrue(game.PlanetarySystems.Count() > 0, "Game planetary systems cannot be empty");
        }

        [TestMethod]
        public void TestDiceRollBetween_1_6()
        {
            for (int i = 0; i < 100; i++)
            {
                int roll = game.RollDice();
                Assert.IsTrue(roll >= 1 && roll <= 6, $"Dice roll should be between 1 and 6, but got {roll}.");
            }
        }

        [TestMethod]
        public void TestPlayerTurnSwitch()
        {
            Player initialPlayer = game.CurrentPlayer;
            game.NextPlayer();
            Assert.AreNotEqual(initialPlayer, game.CurrentPlayer, "Current player should switch after next turn.");
            Assert.AreEqual("Player2", game.CurrentPlayer.Name, "Current Player should be Player 2 after first turn.");
            game.NextPlayer();
            Assert.AreEqual("Player1", game.CurrentPlayer.Name, "Current Player should be Player 1");
        }

        [TestMethod]
        public void TestPlayerMoveByPoint()
        {
            Player player = game.CurrentPlayer;
            int initialPosition = player.position;
            game.MovePlayerByPoint();
            Assert.AreEqual(initialPosition + 1, player.position, $"Player position should be {initialPosition + 1} after moving.");
        }

        [TestMethod]
        public void TestMovePlayerToPosition()
        {
            Player player = game.CurrentPlayer;
            int initialPosition = player.position;
            game.MovePlayerToPosition(10);
            Assert.AreEqual(10, player.position, $"Player position should be 10 after moving.");
        }

        [TestMethod]
        public void TestPlayerSkipTurn_can_skip()
        {
            Player player = game.CurrentPlayer;
            game.SkipPlayerTurn();
            Assert.AreNotEqual(game.CurrentPlayer, player);
        }

        [TestMethod]
        public void TestPlayerSkipTurn_canot_skip_more_than_twice_in_a_row()
        {
            Player player = game.CurrentPlayer;
            game.SkipPlayerTurn();
            game.NextPlayer();
            game.SkipPlayerTurn();
            game.NextPlayer();
            game.SkipPlayerTurn();
            Assert.AreEqual(game.CurrentPlayer, player, "Player cannot skip more than twice in a row");
        }

        [TestMethod]
        public void Test_Player_Settle_Planet_Valid()
        {
            Player player = game.CurrentPlayer;
            game.MovePlayerToPosition(1);
            game.SetInitialCredits();
            Assert.AreEqual(1, player.position);
            game.SettlePlanet();
            HabitablePlanet planet = game.Entities[1] as HabitablePlanet;
            Assert.AreEqual(player, planet.Owner, "Player should own the planet after settling.");
            Assert.AreEqual(1, planet.HotelLevel);
            Assert.AreEqual(0, planet.MineLevel);
            Assert.AreEqual(0, planet.FarmLevel);
        }

        [TestMethod]
        public void Test_Player_Settle_Planet_Not_Habitable_Planet()
        {
            Player player = game.CurrentPlayer;
            Assert.ThrowsException<Exception>(() =>
            {
                game.SettlePlanet();
            });
        }

        [TestMethod]
        public void Test_Player_Settle_Planet_Already_Owned()
        {
            Player player = game.CurrentPlayer;
            game.SetInitialCredits();
            game.MovePlayerByPoint();
            game.SettlePlanet();
            game.NextPlayer();
            game.MovePlayerByPoint();
            Assert.ThrowsException<Exception>(() =>
            {
                game.SettlePlanet();
            });
        }

        [TestMethod]
        public void Test_Player_Can_Settle_Planet_No_Credits()
        {
            Player player = game.CurrentPlayer;
            game.MovePlayerByPoint();
            Assert.ThrowsException<Exception>(() =>
            {
                game.SettlePlanet();
            });
        }

        [TestMethod]
        public void Get_Curr_player_index_valid()
        {
            Player player = game.CurrentPlayer;
            int playerIndex = game.GetCurrentPlayerIndex();
            Assert.AreEqual(0, playerIndex);
        }

        [TestMethod]
        public void Get_Player_Planets_Empty_List()
        {
            Player player = game.CurrentPlayer;
            var planets = game.GetPlayerPlanets();
            Assert.AreEqual(0, planets.Count());
        }

        [TestMethod]
        public void Get_Player_Planets_Non_Empty_List()
        {
            Player player = game.CurrentPlayer;
            game.SetInitialCredits();
            game.MovePlayerByPoint();
            game.SettlePlanet();
            var planets = game.GetPlayerPlanets();
            Assert.AreEqual(1, planets.Count());
        }

        [TestMethod]
        public void Upgrades_Valid()
        {
            Player player = game.CurrentPlayer;
            game.MovePlayerByPoint();
            game.SetInitialCredits();
            game.SettlePlanet();
            HabitablePlanet pl = game.Entities[1] as HabitablePlanet;
            game.UpgradePlanet(pl, "Mine");
            var possibleUpdates = game.GetPossiblePlanetUpgrades(pl);
            Assert.AreEqual(0, possibleUpdates.Count());
            
            Assert.IsTrue(pl.MineLevel == 1);
            game.NextPlayer();
            game.NextPlayer();
            possibleUpdates = game.GetPossiblePlanetUpgrades(pl);
            Assert.AreEqual(3, possibleUpdates.Count());
            CollectionAssert.Contains(possibleUpdates.Keys, "Hotel");
            CollectionAssert.Contains(possibleUpdates.Keys, "Mine");
            CollectionAssert.Contains(possibleUpdates.Keys, "Farm");
        }

        [TestMethod]
        public void Upgrades_Cannot_Upgrade_Twice()
        {
            Player player = game.CurrentPlayer;
            game.MovePlayerByPoint();
            game.SetInitialCredits();
            game.AddCredits(player, 2000);
            HabitablePlanet pl = game.Entities[1] as HabitablePlanet;
            game.UpgradePlanet(pl, "Mine");
            Assert.IsTrue(pl.MineLevel == 1);
            Assert.AreEqual(0, game.GetPossiblePlanetUpgrades(pl).Keys.Count());
        }

        [TestMethod]
        public void Test_Transfer_Credits()
        {
            Player player = game.CurrentPlayer;
            game.NextPlayer();
            Player player1 = game.CurrentPlayer;
            game.SetInitialCredits();
            var credits = player.Credits;
            game.TransferCredits(player, player1, 100);
            Assert.AreEqual(credits - 100, player.Credits, "Player should have 100 credits less after transfer.");
            Assert.AreEqual(100 + credits, player1.Credits, "Player 1 should have 100 credits more after transfer.");
        }

        [TestMethod]
        public void Cannot_Get_Cart_From_Non_Singularity()
        {
            Player player = game.CurrentPlayer;
            game.MovePlayerByPoint();
            Assert.ThrowsException<Exception>(() =>
            {
                game.GetCardFromSingularity();
            });
            game.MovePlayerByPoint();
            Assert.ThrowsException<Exception>(() =>
            {
                game.GetCardFromSingularity();
            });
            game.MovePlayerToPosition(10);
            Assert.ThrowsException<Exception>(() =>
            {
                game.GetCardFromSingularity();
            });
        }

        [TestMethod]
        public void Test_Remove_Player()
        {
            Player player = game.CurrentPlayer;
            game.SetInitialCredits();
            game.MovePlayerByPoint();
            game.SettlePlanet();
            game.RemoveBankruptPlayer(player);

            Assert.IsTrue(player.IsBankrupt);
            Assert.AreEqual(0, player.Credits);
            Assert.IsTrue((game.Entities[1] as HabitablePlanet).Owner == null);
            Assert.IsTrue((game.Entities[1] as HabitablePlanet).HotelLevel == 0);
            Assert.AreEqual(0, player.position);
        }
    }
}
