using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Game.ViewModels;
using GameContainers;

namespace Game
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new EntityChooserVM();
            var stationModel = new StationChooserVM();
            var upgradeModel = new UpgradeChooserVM();
            MyEntityChooser.DataContext = viewModel;
            StationChooser.DataContext = stationModel;
            UpgradeChooser.DataContext = upgradeModel;
        }
    }
}