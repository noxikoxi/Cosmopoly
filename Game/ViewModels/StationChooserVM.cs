
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Engine.models;
using Game.utils;

namespace Game.ViewModels
{
    public class StationData
    {
        public string Name { get; set; }
        public ICommand Move { get; set; }
    }

    public class StationChooserVM : INotifyPropertyChanged
    {
        public ObservableCollection<StationData> Stations { get; set; }

        public int StationCount => Stations.Count;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public StationChooserVM()
        {
            Stations = new();
            Stations.CollectionChanged += (s, e) => OnPropertyChanged(nameof(StationCount));
        }

        public void AddStation(string name, Action<object> action)
        {
            Stations.Add(new StationData
            {
                Name = name,
                Move = new RelayCommand(action)
            });
        }

    }
}
