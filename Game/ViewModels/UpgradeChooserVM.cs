using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Engine.models;
using Game.utils;

namespace Game.ViewModels
{
    public class Upgrade
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Effect { get; set; }

        public ICommand UpgradeBuilding { get; set; } // Komenda otwierająca panel ulepszeń

    }

    public class UpgradeChooserVM
    {
        public ObservableCollection<Upgrade> Upgrades { get; set; }



        public UpgradeChooserVM()
        {
            Upgrades = new();
        }

        public void AddUpgrade(int level, string name, int price, string effect, Action<object> action, Predicate<object> canUpgrade)
        {
            Upgrades.Add(new Upgrade
            {
                Level = level,
                Name = name,
                Price = price,
                Effect = effect,
                UpgradeBuilding = new RelayCommand(action, canUpgrade)
            });
        }
    }
}
