using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Engine.models;

namespace Engine.managers
{
    internal class FinanceManager
    {
        [JsonInclude]
        [JsonPropertyName("passiveBase")]
        private int passiveBase;

        [JsonInclude]
        [JsonPropertyName("passiveGalacticShipyard")]
        private int passiveByGalacticShipyard;

        [JsonInclude]
        [JsonPropertyName("passiveMine")]
        private int[] passiveByMineLevel;

        [JsonInclude]
        [JsonPropertyName("passiveFarm")]
        private int[] passiveByFarmLevel;

        [JsonInclude]
        [JsonPropertyName("passiveAsteroidMine")]
        private int[] passiveByAsteroidMineLevel;

        [JsonInclude]
        [JsonPropertyName("costHotel")]
        private int[] costbyHotelLevel;

        [JsonInclude]
        [JsonPropertyName("costUpgradeHotel")]
        private int[] costUpgradeHotel;

        [JsonInclude]
        [JsonPropertyName("costUpgradeMine")]
        private int[] costUpgradeMine;

        [JsonInclude]
        [JsonPropertyName("costUpgradeFarm")]
        private int[] costUpgradeFarm;

        [JsonInclude]
        [JsonPropertyName("costUpgradeAsteroidMine")]
        private int[] costUpgradeAsteroidMine;

        [JsonInclude]
        [JsonPropertyName("costBuildShipyard")]
        private int costBuildShipyard;

        public FinanceManager()
        {
            passiveBase = 0;
            passiveByGalacticShipyard = 0;
            passiveByMineLevel = [0];
            passiveByFarmLevel = [0];
            passiveByAsteroidMineLevel = [0];
            costbyHotelLevel = [0];
            costUpgradeHotel = [0];
            costUpgradeMine = [0];
            costUpgradeFarm = [0];
            costUpgradeAsteroidMine = [0];
            costBuildShipyard = 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"passiveBase: {passiveBase}");
            sb.AppendLine($"passiveByGalacticShipyard: {passiveByGalacticShipyard}");
            sb.AppendLine($"passiveByMineLevel: {string.Join(", ", passiveByMineLevel)}");
            sb.AppendLine($"passiveByFarmLevel: {string.Join(", ", passiveByFarmLevel)}");
            sb.AppendLine($"passiveByAsteroidMineLevel: {string.Join(", ", passiveByAsteroidMineLevel)}");
            sb.AppendLine($"costbyHotelLevel: {string.Join(", ", costbyHotelLevel)}");
            sb.AppendLine($"costUpgradeHotel: {string.Join(", ", costUpgradeHotel)}");
            sb.AppendLine($"costUpgradeMine: {string.Join(", ", costUpgradeMine)}");
            sb.AppendLine($"costUpgradeFarm: {string.Join(", ", costUpgradeFarm)}");
            sb.AppendLine($"costUpgradeAsteroidMine: {string.Join(", ", costUpgradeAsteroidMine)}");
            sb.AppendLine($"costBuildShipyard: {costBuildShipyard}");
            return sb.ToString();
        }

        public FinanceManager(
            int passiveBase,
            int passiveByGalacticShipyard,
            int[] passiveByMineLevel,
            int[] passiveByFarmLevel,
            int[] passiveByAsteroidMineLevel,
            int[] costbyHotelLevel,
            int[] costUpgradeHotel,
            int[] costUpgradeMine,
            int[] costUpgradeFarm,
            int[] costUpgradeAsteroidMine,
            int costBuildShipyard
            )
        {
            this.passiveBase = passiveBase;
            this.passiveByGalacticShipyard = passiveByGalacticShipyard;
            this.passiveByMineLevel = passiveByMineLevel;
            this.passiveByFarmLevel = passiveByFarmLevel;
            this.passiveByAsteroidMineLevel = passiveByAsteroidMineLevel;
            this.costbyHotelLevel = costbyHotelLevel;
            this.costUpgradeHotel = costUpgradeHotel;
            this.costUpgradeMine = costUpgradeMine;
            this.costUpgradeFarm = costUpgradeFarm;
            this.costUpgradeAsteroidMine = costUpgradeAsteroidMine;
            this.costBuildShipyard = costBuildShipyard;

        }

        private bool IsInRange(int pos, int[] arr)
        {
            if (pos >= arr.Length)
            {
                return false;
            }
            return true;
        }

        public (int, int, int) GetPlanetUpgradeCosts(HabitablePlanet planet)
        {
            int hotelCost = IsInRange(planet.HotelLevel, costUpgradeHotel) ? costUpgradeHotel[planet.HotelLevel] : 0;
            int mineCost = IsInRange(planet.MineLevel, costUpgradeMine) ? costUpgradeMine[planet.MineLevel] : 0;
            int farmCost = IsInRange(planet.FarmLevel, costUpgradeFarm) ? costUpgradeFarm[planet.FarmLevel] : 0;
            return (hotelCost, mineCost, farmCost);
        }

        public long GetInitialPlayerCredits()
        {
            return this.passiveBase * 5;
        }

        public (int, int) GetPlanetarySystemUpgradeCosts(PlanetarySystem system)
        {
            int mineCost = IsInRange(system.MineLevel,costUpgradeAsteroidMine) ? costUpgradeAsteroidMine[system.MineLevel] : 0;
            int galacticShipyardCost = 0;
            if (!system.IsGalacticShipyardBuilt)
            {
                galacticShipyardCost = costBuildShipyard;
            }
            return (mineCost, galacticShipyardCost);
        }

        public long GetPropertyTax(Player player, List<SpaceEntity> entities, double taxPercentage = 0.03)
        {
            var playerPlanets = OwnershipManager.GetPlayerPlanets(entities, player);
            long tax = 0;
            foreach (var planet in playerPlanets)
            {
                tax += this.costUpgradeHotel[planet.HotelLevel];
                tax += this.costUpgradeFarm[planet.FarmLevel];
                tax += this.costUpgradeMine[planet.MineLevel];
            }

            return (long)(tax * taxPercentage);
        }

        public long GetHousingCost(HabitablePlanet planet)
        {
            return costbyHotelLevel[planet.HotelLevel];
        }

        public long GetSettleCost()
        {
            return costbyHotelLevel[0];
        }

        private long GetPassiveByPlanet(HabitablePlanet planet)
        {
            long passiveIncome = 0;
            passiveIncome += passiveByFarmLevel[planet.FarmLevel];
            passiveIncome += passiveByMineLevel[planet.MineLevel];
            return passiveIncome;
        }

        public long GetPlayerPassiveIncome(Player player, List<SpaceEntity> entities, List<PlanetarySystem> systems)
        {
            long passiveIncome = passiveBase;
            foreach (SpaceEntity entity in entities)
            {
                if (entity.IsHabitable && ((HabitablePlanet)entity).Owner == player)
                {

                    passiveIncome += GetPassiveByPlanet((HabitablePlanet)entity);
                }
            }

            foreach (PlanetarySystem system in systems)
            {

                if (OwnershipManager.IsOwnedByPlayer(player, entities, system))
                {
                    passiveIncome += passiveByAsteroidMineLevel[system.MineLevel];
                    if (system.IsGalacticShipyardBuilt)
                    {
                        passiveIncome += passiveByGalacticShipyard;
                    }

                }

            }
            return passiveIncome;
        }


    }
}
