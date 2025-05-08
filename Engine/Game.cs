using System.Diagnostics;
using Engine.managers;
using Engine.models;
using Engine.utils;

namespace Engine
{
    public class Game
    {
        public Player[] players;

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
            this.players = players;
            Turn = 0;
            _currPlayer = 0;

            var cards = ConfigLoader.LoadCardConfig(Path.Combine(configFolderPath, "Cards.json"));
            Debug.WriteLine("\n[Cosmopoly Engine] Loaded Cards");

            //foreach (var card in cards)
            //{
            //    Console.WriteLine(card);
            //    foreach (var strategy in card.strategies)
            //    {
            //        Console.WriteLine(strategy);
            //    }
            //    Console.WriteLine("");
            //}

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

        public void SetInitialCredits()
        {
            foreach (var player in this.players)
            {
                player.credits = _fManager.GetInitialPlayerCredits();
            }
        }

        public void RemoveCredits(Player player, long credits)
        {
            player.credits -= credits;
        }

        public bool IsPlayerInBankruptcy(Player player)
        {
            return player.credits < 0;
        }

        public Random GetRandom()
        {
            return this._random;
        }

        public void RemoveBankruptPlayer(Player player)
        {
            player.credits = 0;
            player.BlockedTurns = 2;
            player.position = 0;
            player.cards.Clear();
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
            if (Entities[GetCurrentPlayer().position] is HabitablePlanet planet)
            {
                return _fManager.GetHousingCost(planet);
            }

            return 0;
        }

        public Dictionary<SpaceEntity, int> GetStationWithIdx()
        {
            return OwnershipManager.GetStationWithIdx(Entities);
        }

        public long GetCurrentPlayerPropertyTax(double taxPercentage)
        {
            return _fManager.GetPropertyTax(GetCurrentPlayer(), Entities, taxPercentage);
        }

        public void NextPlayer()
        {
            var player = GetCurrentPlayer();
            if (player.SkippedTurns == 1)
            {
                player.ResetSkippedTurns();
            }
            ++this._currPlayer;
            if (this._currPlayer >= _players_count)
            {
                this.Turn++;
                this._currPlayer %= _players_count;
            }
            player = GetCurrentPlayer();
            if (player.IsBankrupt)
            {
                NextPlayer();
            }
            else if (player.BlockedTurns > 0)
            {
                player.BlockedTurns--;
                NextPlayer();
            }
            ResetPlayerUpgradesThisTurn();
        }

        public int RollDice()
        {
            int diceRoll = this._random.Next(6) + 1; // from 1 to 6
            return diceRoll;
        }

        public void MovePlayerToPostion(int newPosition)
        {

            if (newPosition >= 0 && newPosition < _total_entities)
            {
                this.players[this._currPlayer].position = newPosition;
            }
            else
            {
                throw new Exception("Invalid position");
            }
        }

        public int MovePlayerByPoints(int points)
        {
            int newPostion = (this.players[this._currPlayer].position + points) % _total_entities;
            this.players[this._currPlayer].position = newPostion;

            return newPostion;
        }

        public SpaceEntity GetCurrentPlayerPositionEntity()
        {
            return this.Entities[GetCurrentPlayer().position];
        }

        public Card GetCardFromSingularity()
        {
            if (this.Entities[GetCurrentPlayer().position].GetType() != typeof(Singularity))
            {
                throw new Exception("Current position is not Singularity");
            }
            return ((Singularity)this.Entities[GetCurrentPlayer().position]).GetRandomCard(PlanetarySystems, _random);
        }

        public void BlockPlayer(int turns)
        {
            GetCurrentPlayer().BlockedTurns += turns;
        }

        public void SkipPlayerTurn()
        {
            if (CanSkipTurn())
            {
                var player = this.players[this._currPlayer];
                NextPlayer();
                player.SkipTurn();
            }
        }

        public bool CanSkipTurn()
        {
            if (this.players[this._currPlayer].SkippedTurns == 0)
            {
                return true;
            }
            return false;
        }

        public void SettlePlanet()
        {
            SpaceEntity currentEntity = this.Entities[GetCurrentPlayer().position];
            if (currentEntity is HabitablePlanet planet)
            {
                if (planet.Owner != null)
                {
                    throw new Exception("Planet already owned");
                }
                var hotelCost = _fManager.GetSettleCost();
                if (GetCurrentPlayer().credits < hotelCost)
                {
                    throw new Exception("Not enough credits");
                }
                planet.Owner = GetCurrentPlayer();
                GetCurrentPlayer().credits -= hotelCost;
                planet.UpgradeHotel();
            }
            else
            {
                throw new Exception("Current position is not habitable planet");
            }
        }

        public bool CanPlayerSettle()
        {
            SpaceEntity currentEntity = this.Entities[GetCurrentPlayer().position];
            if (currentEntity is HabitablePlanet planet)
            {
                if (planet.Owner != null)
                {
                    return false;
                }
                var hotelCost = _fManager.GetSettleCost();

                if ( GetCurrentPlayer().credits < hotelCost)
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

        public Player GetCurrentPlayer()
        {
            return this.players[this._currPlayer];
        }

        public int GetCurrentPlayerIndex()
        {
            return this._currPlayer;
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
            return OwnershipManager.GetPlayerPlanets(Entities, GetCurrentPlayer());
        }

        public List<PlanetarySystem> GetPlayerSystems()
        {
            return OwnershipManager.GetPlayerSystemGalacticShipyards(GetCurrentPlayer(), Entities, PlanetarySystems);
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
            var isSystemOwned = OwnershipManager.IsOwnedByPlayer(GetCurrentPlayer(), Entities, planetSystem);

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
        public int GetCurrentPlayerPirateCards()
        {
            var cards = 0;

            foreach(var card in this.players[_currPlayer].cards)
            {
                if (card.strategies[0].Type == Engine.strategies.strategyType.Shield)
                {
                    cards++;
                }
            }
            return cards;
        }

        public void ApplyCard(Card card, int startegyOption = 0)
        {
            // Move i Shield musze obsłużyc tutaj, reszta jest w CardStrategy
            if (card.strategies[0].Type == Engine.strategies.strategyType.Shield)
            {
                // W zasadzie to ta karta tylko anuluje atak piratów, więc wystraczy ją usunąc z ręki gracza
                this.players[_currPlayer].cards.Remove(card);

            }
            else if (card.strategies[0].Type == Engine.strategies.strategyType.Move)
            {
                // Należy wybrac nową pozycje, podana przez argument stategyOption jako index planety do której idzie gracz
                MovePlayerToPostion(startegyOption);
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
