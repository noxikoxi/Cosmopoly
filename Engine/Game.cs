using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Engine.managers;
using Engine.models;
using Engine.utils;

namespace Engine
{
    public class Game
    {
        public Player[] players;

        public int Turn { get; private set; }

        public Player CurrentPlayer { get; private set; }

        private int _currPlayerIndex;

        private Random _random = new Random();

        private Dictionary<HabitablePlanet, bool> upgradedPlanetsThisTurn;

        private Dictionary<PlanetarySystem, bool> upgradedSystemThisTurn;

        private FinanceManager _fManager;

        private int _total_entities;
        private int _players_count;

        public List<SpaceEntity> Entities { get; private set; }
        public List<PlanetarySystem> PlanetarySystems { get; private set; }

        private Card _pirateCard;

        public Game(Player[] players, string configFolderPath)
        {
            this.players = players;
            Turn = 0;
            _currPlayerIndex = 0;
            CurrentPlayer = players[_currPlayerIndex];

            var cards = ConfigLoader.LoadCardConfig(Path.Combine(configFolderPath, "Cards.json"));
            // Zawsze będzie
            foreach (var card in cards)
            {
                if (card.Title == "Atak piratów")
                {
                    _pirateCard = card;
                    break;
                }
            }
            Debug.WriteLine("\n[Cosmopoly Engine] Loaded Cards");

            (Entities, PlanetarySystems) = ConfigLoader.LoadGalaxyConfig(Path.Combine(configFolderPath, "Galaxy.json"));

            if (Entities.Count == 0 || PlanetarySystems.Count == 0)
            {
                throw new Exception("Unable to load galaxy config");
            }

            Debug.WriteLine("\n[Cosmopoly Engine] Loaded Galaxy");

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

            if (manager == null)
            {
                throw new Exception("Unable to load Finance Config");
            }

            _fManager = manager;
            Debug.WriteLine("\n[Cosmopoly Engine] Loaded Finance Manager");

            upgradedPlanetsThisTurn = new();
            upgradedSystemThisTurn = new();
        }

        public long GetPlayerPassiveIncome(Player player)
        {
            return _fManager.GetPlayerPassiveIncome(player, Entities, PlanetarySystems);
        }

        public void AddCredits(Player player, long credits)
        {
            player.Credits += credits;
        }



        public bool TransferCredits(Player from, Player to, long credits)
        {
            if (from.Credits < credits)
            {
                return false;
            }
            from.Credits -= credits;
            to.Credits += credits;
            return true;
        }

        public void SetInitialCredits()
        {
            foreach (var player in this.players)
            {
                player.Credits = _fManager.GetInitialPlayerCredits();
            }
        }

        public void RemoveCredits(Player player, long credits)
        {
            player.Credits -= credits;
        }

        public bool IsPlayerInBankruptcy(Player player)
        {
            return player.Credits < 0;
        }

        public Random GetRandom()
        {
            return this._random;
        }

        public void RemoveBankruptPlayer(Player player)
        {
            player.Credits = 0;
            player.BlockedTurns = 2;
            player.position = 0;
            player.ShieldCards = 0;
            foreach (SpaceEntity entity in Entities)
            {
                if (entity is HabitablePlanet planet && planet.Owner == player)
                {
                    planet.resetOwnership();
                }
            }

            player.IsBankrupt = true;
        }

        public long GetCurrentPositionHousingCost()
        {
            if (Entities[CurrentPlayer.position] is HabitablePlanet planet)
            {
                return _fManager.GetHousingCost(planet);
            }

            return 0;
        }

        public Dictionary<Station, int> GetStationWithIdx()
        {
            return OwnershipManager.GetStationWithIdx(Entities);
        }

        public long GetCurrentPlayerPropertyTax(double taxPercentage)
        {
            return _fManager.GetPropertyTax(CurrentPlayer, Entities, taxPercentage);
        }

        public void NextPlayer()
        {
            if (CurrentPlayer.SkippedTurns == 1)
            {
                CurrentPlayer.ResetSkippedTurns();
            }
            ++this._currPlayerIndex;
            if (this._currPlayerIndex >= _players_count)
            {
                this.Turn++;
                this._currPlayerIndex %= _players_count;
            }
            CurrentPlayer = this.players[this._currPlayerIndex];
            if (CurrentPlayer.IsBankrupt)
            {
                NextPlayer();
            }
            else if (CurrentPlayer.BlockedTurns > 0)
            {
                CurrentPlayer.BlockedTurns--;
                NextPlayer();
            }
            ResetPlayerUpgradesThisTurn();
        }

        public int RollDice()
        {
            int diceRoll = this._random.Next(6) + 1; // from 1 to 6
            return diceRoll;
        }

        public void MovePlayerToPosition(int newPosition)
        {

            if (newPosition >= 0 && newPosition < _total_entities)
            {
                CurrentPlayer.position = newPosition;
            }
            else
            {
                throw new Exception("Invalid position");
            }
        }

        public int MovePlayerByPoint()
        {
            var pl = CurrentPlayer;
            int newPostion = (pl.position + 1) % _total_entities;
            pl.position = newPostion;

            if (newPostion == 0){
                AddCredits(pl, GetPlayerPassiveIncome(pl));
            }

            return newPostion;
        }

        public SpaceEntity GetCurrentPlayerPositionEntity()
        {
            return this.Entities[CurrentPlayer.position];
        }

        public Card GetCardFromSingularity()
        {
            var pl = CurrentPlayer;
            if (this.Entities[pl.position] is Singularity singualairty)
            {
                var card = singualairty.GetRandomCard(
                    OwnershipManager.GetPlayerSystemGalacticShipyards(
                        CurrentPlayer,
                        Entities,
                        PlanetarySystems
                        ),
                    _random);
                return card;
            }
            else
            {
                throw new Exception("Current position is not Singularity");
            }
     
        }

        public void BlockPlayer(int turns)
        {
            CurrentPlayer.BlockedTurns += turns;
        }

        public void SkipPlayerTurn()
        {
            if (CanSkipTurn())
            {
                var player = this.players[this._currPlayerIndex];
                NextPlayer();
                player.SkipTurn();
            }
        }

        public bool CanSkipTurn()
        {
            if (this.players[this._currPlayerIndex].SkippedTurns == 0)
            {
                return true;
            }
            return false;
        }

        public void SettlePlanet()
        {
            SpaceEntity currentEntity = this.Entities[CurrentPlayer.position];
            if (currentEntity is HabitablePlanet planet)
            {
                if (planet.Owner != null)
                {
                    throw new Exception("Planet already owned");
                }
                var hotelCost = _fManager.GetSettleCost();
                if (CurrentPlayer.Credits < hotelCost)
                {
                    throw new Exception("Not enough credits");
                }
                planet.Owner = CurrentPlayer;
                CurrentPlayer.Credits -= hotelCost;
                planet.UpgradeHotel();
            }
            else
            {
                throw new Exception("Current position is not habitable planet");
            }
        }

        public bool CanPlayerSettle()
        {
            SpaceEntity currentEntity = this.Entities[CurrentPlayer.position];
            if (currentEntity is HabitablePlanet planet)
            {
                if (planet.Owner != null)
                {
                    return false;
                }
                var hotelCost = _fManager.GetSettleCost();

                if ( CurrentPlayer.Credits < hotelCost)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public long GetHousingCost()
        {
            return _fManager.GetSettleCost();
        }

        public int GetCurrentPlayerIndex()
        {
            return this._currPlayerIndex;
        }

        public PlanetarySystem? GetPlanetarySystem(SpaceEntity entity)
        {
            return OwnershipManager.GetPlanetSystem((byte)Entities.IndexOf(entity), PlanetarySystems);
        }

        // Upgrades

        public void ResetPlayerUpgradesThisTurn()
        {
            upgradedPlanetsThisTurn.Clear();
            upgradedSystemThisTurn.Clear();
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
            return OwnershipManager.GetPlayerPlanets(Entities, CurrentPlayer);
        }

        public List<PlanetarySystem> GetPlayerSystems()
        {
            return OwnershipManager.GetPlayerSystemGalacticShipyards(CurrentPlayer, Entities, PlanetarySystems);
        }

        public Dictionary<string, int> GetPossibleSystemUpgrades(PlanetarySystem system)
        {
            // No more upgrades this turn
            if (upgradedSystemThisTurn.ContainsKey(system) && upgradedSystemThisTurn[system] == true)
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
            if (upgradedPlanetsThisTurn.ContainsKey(planet) && upgradedPlanetsThisTurn[planet] == true)
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
            var isSystemOwned = OwnershipManager.IsOwnedByPlayer(CurrentPlayer, Entities, planetSystem);

            List<string> buildings = UpgradeManager.GetPossiblePlanetUpgrades(planet, isSystemOwned);
            foreach (var building in buildings)
            {
                if (building == "Hotel")
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

        public string GetUpgradeEffect(string val)
        {
            return UpgradeManager.GetUpgradeEffect(val);
        }

        public void UpgradePlanet(HabitablePlanet planet, string building)
        {

            var (h_costs, m_costs, f_costs) = _fManager.GetPlanetUpgradeCosts(planet);
            if (building == "Hotel")
            {
                planet.UpgradeHotel();
                CurrentPlayer.Credits -= h_costs;
            }
            else if (building == "Mine")
            {
                planet.UpgradeMine();
                CurrentPlayer.Credits -= m_costs;
            }
            else if (building == "Farm")
            {
                planet.UpgradeFarm();
                CurrentPlayer.Credits -= f_costs;
            }
            upgradedPlanetsThisTurn[planet] = true;
        }

        public void UpgradeSystem(PlanetarySystem system, string building)
        {

            var (s_costs, m_costs) = _fManager.GetPlanetarySystemUpgradeCosts(system);
            if (building == "Shipyard")
            {
                system.BuildGalacticShipyhard();
                CurrentPlayer.Credits -= s_costs;
            }
            else if (building == "Mine")
            {
                system.UupgradeMine();
                CurrentPlayer.Credits -= m_costs;
            }

            upgradedSystemThisTurn[system] = true;

        }

        public Card GetPirateCard()
        {
            return _pirateCard;
        }

        // CARDS
        public int GetCurrentPlayerShieldCards()
        {
            return CurrentPlayer.ShieldCards;
        }

        public void ApplyCard(Card card, int startegyOption = 0)
        {
            // Shield
            if (card.strategies[0].Type == strategies.strategyType.Shield)
            {
                CurrentPlayer.ShieldCards++;
            } // Move
            else if (card.strategies[0].Type == strategies.strategyType.Move)
            {
                MovePlayerToPosition(startegyOption);
            }
            else
            {

                if (card.ApplyTogether)
                {
                    foreach (var strategy in card.strategies)
                    {
                        strategy.Apply(this);
                    }
                }
                else
                {
                    card.strategies[startegyOption].Apply(this);
                }
            }
        }
    }
}
