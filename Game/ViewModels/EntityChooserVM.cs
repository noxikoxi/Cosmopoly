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
        public ImageSource Image { get; set; }
        public ICommand OpenDetailsCommand { get; set; } // Komenda otwierająca panel ulepszeń
    }

    public class EntityChooserVM
    {
        public ObservableCollection<EntityDisplayData> Entities { get; set; }

        public EntityChooserVM()
        {
            Entities = new();
        }
            
        public void AddEntity(string name, ImageSource img, Action<object> action)
        {
            Entities.Add(new EntityDisplayData
            {
                Name = name,
                Image = img,
                OpenDetailsCommand = new RelayCommand(action)
            });
        }
    }
}
