using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameContainers.converters
{
    public static class UpgradesNames
    {

        public static string ConvertName(string name, int level)
        {
            switch (name)
            {
                case "Hotel":
                    switch (level)
                    {
                        case 1:
                            return "Habitaty mieszkalne";
                        case 2:
                            return "Kolonia";
                        case 3:
                            return "Hotel galaktyczny";
                        case 4:
                            return "Sieć hoteli planetarnych";
                        default:
                            return name;
                    }
                case "Mine":
                    return $"Kopalnia {level + 1} stopnia";
                case "Farm":
                    return $"Farma {level + 1} stopnia";
                case "Shipyard":
                    return "Port Galaktyczny";
                default:
                    return name;
            }

        }
    }
}
