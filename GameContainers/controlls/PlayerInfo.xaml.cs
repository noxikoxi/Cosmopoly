using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameContainers.controlls
{
    /// <summary>
    /// Logika interakcji dla klasy PlayerInfo.xaml
    /// </summary>
    public partial class PlayerInfo : UserControl
    {
        public PlayerInfo()
        {
            InitializeComponent();
        }

        public void SetPlayer(string player_name)
        {
            PlayerName.Content = player_name;
        }
        public void SetCredits(long credits)
        {
            Credits.Content = credits.ToString();
        }

        public void SetCardsCount(int count)
        {
            PirateCards.Content = count;
        }
    }
}
