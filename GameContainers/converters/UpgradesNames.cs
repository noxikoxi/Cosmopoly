using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameContainers.converters
{
    public static class UpgradesNames
    {

        public static string ConvertName(string name)
        {
            switch (name)
            {
                case "Hotel":
                    return "Hotel";
                case "Mine":
                    return "Kopalnia";
                case "Farm":
                    return "Farma";
                case "Shipyard":
                    return "Port Galaktyczny";
                default:
                    return name;
            }

        }
    }
}
