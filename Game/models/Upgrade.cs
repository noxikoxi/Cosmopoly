using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.models
{
    public class Upgrade
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Effect { get; set; }

        public Upgrade(int level, string name, int price, string effect)
        {
            Level = level;
            Name = name;
            Price = price;
            Effect = effect;
        }
    }
}
