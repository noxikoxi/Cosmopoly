﻿using System;
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
    /// Logika interakcji dla klasy UpgradeChooser.xaml
    /// </summary>
    public partial class UpgradeChooser : UserControl
    {
        public event EventHandler Exit_Clicked;
        public UpgradeChooser()
        {
            InitializeComponent();
        }
        private void UpgradeChooser_Exit_Click(object sender, RoutedEventArgs e)
        {
            Exit_Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
