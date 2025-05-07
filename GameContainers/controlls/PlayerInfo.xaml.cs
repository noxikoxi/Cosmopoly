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

        public string PlayerNameText
        {
            get => PlayerName.Content.ToString();
            set => PlayerName.Content = value;
        }

        public long CreditsAmount
        {
            get => long.Parse(Credits.Content.ToString());
            set => Credits.Content = value.ToString();
        }

        public int PirateCardCount
        {
            get => int.Parse(PirateCards.Content.ToString());
            set => PirateCards.Content = value.ToString();
        }
    }
}
