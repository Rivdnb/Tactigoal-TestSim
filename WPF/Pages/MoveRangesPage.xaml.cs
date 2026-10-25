using Core.GameLogic;
using Core.Movement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF.Pages
{
    /// <summary>
    /// Interaction logic for MoveRangesPage.xaml
    /// </summary>
    public partial class MoveRangesPage : Page
    {
        public UnitType SelectedUnitType { get; init; }
        public DirectionName SelectedDirectionName { get; init; }
        public MovementRule NewMovementRule { get; init; }
        public bool IsFirstMovement { get; init; }
        public MoveRangesPage(UnitType unitType, DirectionName directionName, bool isFirstMovement)
        {
            InitializeComponent();
            SelectedDirectionName = directionName;
            SelectedUnitType = unitType;
            TextUnitName.Text = SelectedUnitType.Name;

            IsFirstMovement = isFirstMovement;
            MovementStep.Text = IsFirstMovement ? "First" : "Second";

            List<MovementRule> rules = IsFirstMovement ?
                SelectedUnitType.FirstMovementRules : SelectedUnitType.SecondMovementRules;

            NewMovementRule = rules?.Find(x => x.DirectionName == SelectedDirectionName)
                ?? throw new Exception("Could not find movement rule that matches the direction name");
  
            MinDistanceBox.Text = NewMovementRule.MinDistance.ToString();
            MaxDistanceBox.Text = NewMovementRule.MaxDistance.ToString();
        }
        private int GetMinValue()
        {
            if (!Int32.TryParse(MinDistanceBox.Text, out int x))
                throw new Exception("Invalid input: input must be a number");
            return x;
        }
        private int GetMaxValue()
        {
            if (!Int32.TryParse(MaxDistanceBox.Text, out int x))
                throw new Exception("Invalid input: input must be a number");
            return x;
        }
        private bool ValuesValidation()
        {
            if(!Int32.TryParse(MinDistanceBox.Text, out int min) ||
                !Int32.TryParse(MaxDistanceBox.Text, out int max))
            {
                MessageBox.Show("Invalid input: Only numbers are allowed");
                return false;
            }
            if (min > max)
            {
                MessageBox.Show("Invalid input: Minimum cannot be higher than maximum");
                return false;
            }
            if (min < 0 || min > 1)
            {
                MessageBox.Show("Invalid input: Minimum cannot be below 0 or above 1");
                return false;
            }
            if (max != 0 && min == 0)
            {
                MessageBox.Show("Invalid input: Minimum must be 1 if maximum is not 0. " +
                    "Minimum = 0 is only valid, when maximum is also 0 (this combination signals" +
                    " that the unit cannot take steps in this direction.");
                return false;
            }
            return true;
        }
        private void SaveExitClick(object sender, RoutedEventArgs e)
        {
            if (ValuesValidation())
            {
                NewMovementRule.UpdateMinDistance(GetMinValue());
                NewMovementRule.UpdateMaxDistance(GetMaxValue());

                if (IsFirstMovement) SelectedUnitType.UpdateFirstMovementRule(NewMovementRule);
                else { SelectedUnitType.UpdateSecondMovementRule(NewMovementRule); }

                Window.GetWindow(this).Close();
            }
        }
    }
}
