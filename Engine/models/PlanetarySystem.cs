using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CosmopolyEngine.models
{
    public class PlanetarySystem
    {

        private List<byte> planetsIds;

        private string name;

        public bool IsGalacticShipyardBuilt { get; private set; }

        // Kopalnia w pasach asteroid
        public byte MineLevel { get; private set; }
        public byte MaxMineLevel { get; private set; }  // Max 5

        public PlanetarySystem(string name)
        {
            planetsIds = new List<byte>();
            MineLevel = 0;
            MaxMineLevel = 5;
            this.name = name;
        }

        public void UupgradeMine()
        {
            ++MineLevel;
        }

        public string GetName()
        {
            return name;
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
            return $"Układ planetarny {GetName()}; zawiera planety {string.Join(", ", planetsIds)}";
        }

    }
}
