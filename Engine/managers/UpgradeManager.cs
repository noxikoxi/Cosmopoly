using CosmopolyEngine.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmopolyEngine.managers
{
    static internal class UpgradeManager
    {

        public static List<string> GetPossiblePlanetUpgrades(HabitablePlanet planet, bool isSystemOwned)
        {
            List<string> buildings = [];

            if ( planet.HotelLevel == 0)
            {
                buildings.Add("Hotel");
                return buildings;
            }
            if (planet.MineLevel < planet.MaxMineLevel)
            {
                buildings.Add("Mine");
            }

            if (planet.FarmLevel < planet.MaxFarmLevel)
            {
                buildings.Add("Farm");
            }

            if (planet.HotelLevel < planet.MaxHotelLevel-1 || planet.HotelLevel == planet.MaxHotelLevel-1 && isSystemOwned)
            {
                buildings.Add("Hotel");
            }

            return buildings;

        }

        public static List<string> GetPossibleSystemUpgrades(PlanetarySystem system)
        {
            List<string> buildings = [];

            if(!system.IsGalacticShipyardBuilt)
            {
                buildings.Add("Shipyard");
            }
            if(system.MineLevel < system.MaxMineLevel)
            {
                buildings.Add("Mine");
            }

            return buildings;

        }
    }
}
