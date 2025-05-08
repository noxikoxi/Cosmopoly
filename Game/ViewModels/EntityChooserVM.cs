using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.models;
using System.Windows.Input;
using System.Windows.Media;
using Game.utils;


namespace Game.ViewModels
{
    public class EntityDisplayData
    {
        public string Name { get; set; }
        public Brush Color { get; set; }
        public ICommand OpenDetailsCommand { get; set; } // Komenda otwierająca panel ulepszeń
    }

    public class EntityChooserVM
    {
        public ObservableCollection<EntityDisplayData> Entities { get; set; }

        public EntityChooserVM()
        {
            Entities = new();
        }
            
        public void AddEntity(string name, Brush color, Action<object> action)
        {
            Entities.Add(new EntityDisplayData
            {
                Name = name,
                Color = color,
                OpenDetailsCommand = new RelayCommand(action)
            });
        }
    }
}
