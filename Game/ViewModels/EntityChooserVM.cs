using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.models;


namespace Game.ViewModels
{
    public class EntityChooserVM
    {
        public ObservableCollection<SpaceEntity> Entities { get; set; }

        public EntityChooserVM()
        {
            Entities =
            [
                new HabitablePlanet("Ziemia"),
                new HabitablePlanet("Mars"),
                new HabitablePlanet ("Wenus")   
            ];
        }
    }
}
