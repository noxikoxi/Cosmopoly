using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Engine.models
{
    public class PlanetarySystem
    {

        private List<byte> planetsIds;

        public string Name { get; private set; }

        public bool IsGalacticShipyardBuilt { get; private set; }

        // Kopalnia w pasach asteroid
        public byte MineLevel { get; private set; }
        public byte MaxMineLevel { get; private set; }  // Max 5

        public PlanetarySystem(string name)
        {
            planetsIds = new List<byte>();
            MineLevel = 0;
            MaxMineLevel = 5;
            Name = name;
        }

        public void UupgradeMine()
        {
            ++MineLevel;
        }


        public void BuildGalacticShipyhard()
        {
            IsGalacticShipyardBuilt = true;
        }

        public void DestroyGalacticShipyard()
        {
            IsGalacticShipyardBuilt = false;
        }

        public void AddPlanet(byte planetId)
        {
            planetsIds.Add(planetId);
        }

        public List<byte> GetPlanetsIds()
        {
            return planetsIds;
        }

        public override string ToString()
        {
            return $"Układ planetarny {Name}; zawiera planety {string.Join(", ", planetsIds)}";
        }

    }
}
