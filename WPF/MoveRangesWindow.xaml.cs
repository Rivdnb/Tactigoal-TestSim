using Core.GameLogic;
using Core.Movement;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MoveRangesWindow.xaml
    /// </summary>
    public partial class MoveRangesWindow : Window
    {
        public MoveRangesWindow(UnitType unitType, DirectionName directionName, bool IsFirstMovement)
        {
            InitializeComponent();
            SubFrame.Navigate(new Pages.MoveRangesPage(unitType, directionName, IsFirstMovement));
        }
    }
}
