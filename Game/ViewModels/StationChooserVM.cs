
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.models;

namespace Game.ViewModels
{
    public class StationChooserVM
    {
        public ObservableCollection<Station> Stations { get; set; }

        public StationChooserVM()
        {
            Stations =
            [
                new Station("Stacja A"),
                new Station("Stacja B"),
                new Station ("Stacja C")
            ];
        }
    }
}
