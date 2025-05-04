using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.managers;
using Engine.models;
using Engine.utils;

namespace Engine
{
    public class Game
    {
        private Player[] _players;

        public int Turn { get; private set; }

        private int _currPlayer;

        private Random _random = new Random();

        private Dictionary<HabitablePlanet, bool> upgradedPlanetsThisTurn;

        private Dictionary<PlanetarySystem, bool> upgradedSystemThisTurn;

        private FinanceManager _fManager;

        private int _total_entities;
        private int _players_count;

        public List<SpaceEntity> Entities { get; private set; }
        public List<PlanetarySystem> PlanetarySystems { get; private set; }

        public Game(Player[] players, string configFolderPath)
        {
            _players = players;
            Turn = 0;
            _currPlayer = 0;
            upgradedPlanetsThisTurn = new();
            upgradedSystemThisTurn = new();

            var cards = ConfigLoader.LoadCardConfig(Path.Combine(configFolderPath, "Cards.json"));
            Console.WriteLine("\n[Cosmopoly Engine] Loaded Cards");

            (Entities, PlanetarySystems) = ConfigLoader.LoadGalaxyConfig(Path.Combine(configFolderPath, "Galaxy.json"));

            if(Entities.Count == 0 || PlanetarySystems.Count == 0)
            {
                throw new Exception("Unable to load galaxy config");
            }

            Console.WriteLine("\n[Cosmopoly Engine] Loaded Galaxy");

            foreach (SpaceEntity entity in Entities)
            {
                if (entity is Singularity singularity)
                {
                    singularity.SetPossibleCards(cards);
                }
            }

            _total_entities = Entities.Count;
            _players_count = players.Length;

            FinanceManager? manager = ConfigLoader.LoadFinanceManagerConfig(Path.Combine(configFolderPath, "Finance.json"));

            if(manager == null)
            {
                throw new Exception("Unable to load Finance Config");
            }

            _fManager = manager;
            Console.WriteLine("\n[Cosmopoly Engine] Loaded Finance Manager");
        }

        public long GetPlayerPassiveIncome(Player player)
        {
            return _fManager.GetPlayerPassiveIncome(player, Entities, PlanetarySystems);
        }

        public void AddCredits(Player player, long credits)
        {
            player.credits += credits;
        }

        public bool TransferCredits(Player from, Player to, long credits)
        {
            if (from.credits < credits)
            {
                return false;
            }
            from.credits -= credits;
            to.credits += credits;
            return true;
        }

        public void RemoveCredits(Player player, long credits)
        {
            player.credits -= credits;
        }

        public bool IsPlayerInBankruptcy(Player player)
        {
            return player.credits <= 0;
        }

        public void RemoveBankruptPlayer(Player player)
        {
            if (IsPlayerInBankruptcy(player))
            {
                player.credits = 0;
                player.BlockedTurns = 2;
                player.position = 0;
                player.cards.Clear();
            }

            foreach (SpaceEntity entity in Entities)
            {
                if (entity is HabitablePlanet planet && planet.Owner == player)
                {
                    planet.resetOwnership();
                }
            }

            player.IsBankrupt = true;
        }

        public long getCurrentPositionHousingCost()
        {
            if (Entities[GetCurrentPlayer().position] is HabitablePlanet planet)
            {
                return _fManager.GetHousingCost(planet);
            }

            return 0;
        }

        public void NextPlayer()
        {
            ++_currPlayer;
            if (_currPlayer >= _players_count)
            {
                Turn++;
                _currPlayer %= _players_count;
            }
            var player = GetCurrentPlayer();

            if (player.IsBankrupt)
            {
                NextPlayer();
            } else if (player.BlockedTurns > 0)
            {
                player.BlockedTurns--;
                NextPlayer();
            }
        }

        public int RollDice()
        {
            int diceRoll = _random.Next(6) + 1; // from 1 to 6
            return diceRoll;
        }

        public void MovePlayerToPostion(int newPosition)
        {

            if (newPosition >= 0 && newPosition < _total_entities)
            {
                _players[_currPlayer].position = newPosition;
            }
            else
            {
                throw new Exception("Invalid position");
            }
        }

        public int MovePlayerByPoints(int points)
        {
            int newPostion = (_players[_currPlayer].position + points) % _total_entities;
            _players[_currPlayer].position = newPostion;

            return newPostion;
        }

        public SpaceEntity GetCurrentPlayerPositionEntity()
        {
            return Entities[GetCurrentPlayer().position];
        }

        public Card GetCardFromSingularity()
        {
            if (Entities[GetCurrentPlayer().position].GetType() != typeof(Singularity))
            {
                throw new Exception("Current position is not Singularity");
            }
            return ((Singularity)Entities[GetCurrentPlayer().position]).GetRandomCard(PlanetarySystems, _random);
        }

        public void BlockPlayer(int turns)
        {
            GetCurrentPlayer().BlockedTurns += turns;
        }

        public void SkipPlayerTurn()
        {
            if (_players[_currPlayer].SkippedTurns < 2)
            {
                _players[_currPlayer].SkipTurn();
                NextPlayer();
            }
        }

        public Player GetCurrentPlayer()
        {
            return _players[_currPlayer];
        }
           
        // Upgrades

        public void ResetPlayerUpgradesThisTurn()
        {
            upgradedPlanetsThisTurn = new();
            upgradedSystemThisTurn = new();
            foreach (HabitablePlanet planet in GetPlayerPlanets())
            {
                upgradedPlanetsThisTurn.Add(planet, false);
            }

            foreach (PlanetarySystem system in GetPlayerSystems())
            {
                upgradedSystemThisTurn.Add(system, false);
            }
        }

        public List<HabitablePlanet> GetPlayerPlanets()
        {
            return OwnershipManager.GetPlayerPlanets(Entities, GetCurrentPlayer());
        }

        public List<PlanetarySystem> GetPlayerSystems()
        {
            return OwnershipManager.GetPlayerSystemGalacticShipyards(GetCurrentPlayer(), Entities, PlanetarySystems);
        }

        public Dictionary<string, int> GetPossibleSystemUpgrades(PlanetarySystem system)
        {
            // No more upgrades this turn
            if(upgradedSystemThisTurn.ContainsKey(system) && upgradedSystemThisTurn[system] == false)
            {
                return new Dictionary<string, int>();
            }
            var (s_costs, m_costs) = _fManager.GetPlanetarySystemUpgradeCosts(system);
            List<string> buildings = UpgradeManager.GetPossibleSystemUpgrades(system);
            Dictionary<string, int> upgrades = new();
            foreach (var building in buildings)
            {
                if (building == "Shipyard")
                {
                    upgrades.Add("Shipyard", s_costs);
                }
                else if (building == "Mine")
                {
                    upgrades.Add("Mine", m_costs);
                }
            }

            return upgrades;
        }

        public Dictionary<string, int> GetPossiblePlanetUpgrades(HabitablePlanet planet)
        {
            if (upgradedPlanetsThisTurn.ContainsKey(planet) && upgradedPlanetsThisTurn[planet] == false)
            {
                return new Dictionary<string, int>();
            }
            var (h_costs, m_costs, f_costs) = _fManager.GetPlanetUpgradeCosts(planet);
            Dictionary<string, int> upgrades = new();
            var planetSystem = OwnershipManager.GetPlanetSystem((byte)Entities.IndexOf(planet), PlanetarySystems);
            if (planetSystem == null)
            {
                throw new Exception("Habitable Planet should have a system, unable to find it");
            }
            var isSystemOwned = OwnershipManager.IsOwnedByPlayer(GetCurrentPlayer(), Entities, planetSystem);

            List<string> buildings = UpgradeManager.GetPossiblePlanetUpgrades(planet, isSystemOwned);

            foreach (var building in buildings)
            {
                if(building == "Hotel")
                {
                    upgrades.Add("Hotel", h_costs);
                }
                else if (building == "Mine")
                {
                    upgrades.Add("Mine", m_costs);
                }
                else if (building == "Farm")
                {
                    upgrades.Add("Farm", f_costs);
                }
            }

            return upgrades;
        }

        public void UpgradePlanet(HabitablePlanet planet, string building)
        {

            var (h_costs, m_costs, f_costs) = _fManager.GetPlanetUpgradeCosts(planet);
            if (building == "Hotel")
            {
                planet.UpgradeHotel();
                GetCurrentPlayer().credits -= h_costs;
            }
            else if (building == "Mine")
            {
                planet.UpgradeMine();
                GetCurrentPlayer().credits -= m_costs;
            }
            else if (building == "Farm")
            {
                planet.UpgradeFarm();
                GetCurrentPlayer().credits -= f_costs;
            }
            upgradedPlanetsThisTurn[planet] = true;
        }

        public void UpgradeSystem(PlanetarySystem system, string building)
        {

            var (s_costs, m_costs) = _fManager.GetPlanetarySystemUpgradeCosts(system);
            if (building == "Shipyard")
            {
                system.BuildGalacticShipyhard();
                GetCurrentPlayer().credits -= s_costs;
            }
            else if (building == "Mine")
            {
                system.UupgradeMine();
                GetCurrentPlayer().credits -= m_costs;
            }

            upgradedSystemThisTurn[system] = true;

        }

        // CARDS
        public List<Card> GetCurrentPlayerCards()
        {
            return _players[_currPlayer].cards;
        }

        public void UsePlayerCard(int cardId)
        {
            // TODO:
            _players[_currPlayer].cards.RemoveAt(cardId);
        }
    }
}
