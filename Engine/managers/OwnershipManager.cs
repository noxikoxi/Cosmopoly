using Engine.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.managers
{
    static class OwnershipManager
    {

        public static bool IsOwnedByPlayer(Player player, List<SpaceEntity> entities, PlanetarySystem system)
        {
            bool isOwner = true;

            foreach (int planetId in system.GetPlanetsIds())
            {
                if (entities[planetId].IsHabitable && ((HabitablePlanet)entities[planetId]).Owner != player)
                {
                    isOwner = false;
                    break;
                }
            }

            return isOwner;
        }

        public static List<HabitablePlanet> GetPlayerPlanets(List<SpaceEntity> entities, Player player)
        {
            List<HabitablePlanet> upgradablePlanets = [];
            foreach (SpaceEntity entity in entities)
            {
                if (entity is HabitablePlanet planet && planet.Owner == player)
                {
                    upgradablePlanets.Add(planet);
                }
            }
            return upgradablePlanets;
        }

        public static List<PlanetarySystem> GetPlayerSystems(List<PlanetarySystem> systems, List<SpaceEntity> entities, Player player)
        {
            List<PlanetarySystem> upgradableSystems = [];
            foreach (PlanetarySystem system in systems)
            {
                if (IsOwnedByPlayer(player, entities, system))
                {
                    upgradableSystems.Add(system);
                }
            }
            return upgradableSystems;
        }

        public static PlanetarySystem? GetPlanetSystem(byte planetID, List<PlanetarySystem> systems)
        {
            foreach (PlanetarySystem system in systems)
            {
                if (system.GetPlanetsIds().Contains(planetID))
                {
                    return system;
                }
            }

            return null;
        }

        public static List<PlanetarySystem> GetPlayerSystemGalacticShipyards(Player player, List<SpaceEntity> entities, List<PlanetarySystem> systems)
        {
            List<PlanetarySystem> output = [];

            foreach(PlanetarySystem system in systems)
            {
                if(IsOwnedByPlayer(player, entities, system) && system.IsGalacticShipyardBuilt)
                {
                    output.Add(system);
                }
            }

            return output;

        }
    }
}
