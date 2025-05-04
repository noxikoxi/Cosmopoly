using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.models;
using Game.models;

namespace Game.ViewModels
{
    public class UpgradeChooserVM
    {
        public ObservableCollection<Upgrade> Upgrades { get; set; }

        public UpgradeChooserVM()
        {
            Upgrades =
            [
                new Upgrade(level: 1, name: "Kopalnia", price: 100, effect: "Zwiększone wydobycie o 10%"),
                new Upgrade(level: 2, name: "Hotel", price: 1000, effect: "Zwiększony koszt przebywania"),
                new Upgrade(level: 3, name: "Farma", price: 500, effect: "Uprawy wydajniejsze o 20%"),
                new Upgrade(level: 3, name: "Kopalnia Asteroid", price: 2000, effect: "Zwiększone wydobycie o 30%")
            ];
        }
    }
}
