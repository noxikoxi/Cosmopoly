using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.models
{
    public class HabitablePlanet(string name) : SpaceEntity(name, true)
    {
        public byte HotelLevel { get; private set; } = 0;
        public byte MaxHotelLevel { get; private set; } = 5;

        public byte FarmLevel { get; private set; } = 0;
        public byte MaxFarmLevel { get; private set; } = 6;

        public byte MineLevel { get; private set; } = 0;
        public byte MaxMineLevel { get; private set; } = 3;

        public Player? Owner { get; set; } = null;

        protected override string ToStringInternal()
        {
            return $"Planeta {Name}";
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
