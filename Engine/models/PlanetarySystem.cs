using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Engine.models
{
    public class PlanetarySystem(string name)
    {

        private List<byte> planetsIds = new List<byte>();

        public string Name{ get; private set; } = name;

        public bool IsGalacticShipyardBuilt { get; private set; }

        // Kopalnia w pasach asteroid
        public byte MineLevel { get; private set; } = 0;
        public byte MaxMineLevel { get; private set; } = 5;

        public void UupgradeMine()
        {
            ++MineLevel;
        }

        public void BuildGalacticShipyhard()
        {
            IsGalacticShipyardBuilt=true;
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
