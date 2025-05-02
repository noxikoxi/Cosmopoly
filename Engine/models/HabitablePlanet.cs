using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmopolyEngine.models
{
    public class HabitablePlanet : SpaceEntity
    {
        public byte HotelLevel { get; private set; } 
        public byte MaxHotelLevel { get; private set; } // Max 5 

        public byte FarmLevel { get; private set; } 
        public byte MaxFarmLevel { get; private set; } // Max 6

        public byte MineLevel { get; private set; }
        public byte MaxMineLevel { get; private set; } // Max 3

        public Player? Owner { get; set; }

        protected override string ToStringInternal()
        {
            return $"Planeta {GetName()}";
        }

        public HabitablePlanet(string name) : base(name, true)
        {
            HotelLevel = 0;
            FarmLevel = 0;
            MineLevel = 0;
            MaxHotelLevel = 5;
            MaxFarmLevel = 6;
            MaxMineLevel = 3;
            Owner = null;
        }

        public void resetOwnership()
        {
            Owner = null;
            HotelLevel = 0;
            FarmLevel = 0;
            MineLevel = 0;  
        }

        public void UpgradeHotel()
        {
            ++HotelLevel;
        }

        public void UpgradeFarm()
        { 
            ++FarmLevel;
        }

        public void UpgradeMine()
        {
            ++MineLevel;
        }

    }
}
