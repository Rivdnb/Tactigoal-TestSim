using Core.GameLogic;
using Core.GameObjects;
using Core.Movement;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace WPF.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public ConfigManager ConfigManager { get; init; }
        public GameConfig GameConfig { get; init; }
        public Board DemoBoard { get; init; }
        public MoveValidator MoveValidator { get; init; }
        public string ConfigDirectory { get; init; }
        public string DataDirectory { get; init; }
        public List<UnitType> Archetypes { get; set; }
        private Button[,] _boardButtons;
        private List<Position> _legalMoves;
        private bool _dragStarted = false;
        public int DemoBoardRows { get; } = 9;
        public int DemoBoardColumns { get; } = 9;
        public UnitType SelectedUnitType { get; private set; }
        public Position DemoPosition { get; init; }
        public SettingsPage()
        {
            ConfigDirectory = System.IO.Path.Combine(AppContext.BaseDirectory, "ConfigFiles");
            DataDirectory = System.IO.Path.Combine(AppContext.BaseDirectory, "Data");
            DemoBoard = new Board(DemoBoardRows, DemoBoardColumns);
            MoveValidator = new MoveValidator(DemoBoard);
            ConfigManager = new ConfigManager();
            try { GameConfig = ConfigManager.LoadGameConfig(ConfigDirectory, DataDirectory); }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(-1);
            }

            InitializeComponent();

            BoardRows.ValueChanged += RowSliderValueChanged;
            BoardColumns.ValueChanged += ColumnSliderValueChanged;
            BoardRows.Value = GameConfig.BoardRows;
            BoardColumns.Value = GameConfig.BoardColumns;

            _boardButtons = new Button[DemoBoardRows, DemoBoardColumns];
            _legalMoves = [];
            DemoPosition = new Position(DemoBoardRows / 2, DemoBoardColumns / 2);
            Archetypes = GameConfig.UnitTypes;
            SelectedUnitType = Archetypes.First();
            CreateBoardGrid();
            RefreshBoard();

            this.DataContext = this;

        }
        private bool CombinedDistanceValidation()
        {
            if (!Int32.TryParse(MinCombinedDistanceBox.Text, out int min) ||
                !Int32.TryParse(MaxCombinedDistanceBox.Text, out int max))
            {
                MessageBox.Show("Invalid input: Only numbers are allowed");
                return false;
            }
            if (min > max)
            {
                MessageBox.Show("Invalid input: Minimum cannot be higher than maximum");
                return false;
            }
            if (min < 0)
            {
                MessageBox.Show("Invalid input: Minimum cannot be below 0");
                return false;
            }
            if (max == 0)
            {
                MessageBox.Show("Invalid input: Maximum must be above 0");
                return false;
            }
            return true;
        }
        private void SliderDragStarted(object sender, DragStartedEventArgs e) => _dragStarted = true;
        private void RowSliderDragCompleted(object sender, DragCompletedEventArgs e)
        {
            GameConfig.UpdateBoardRows((int)BoardRows.Value);
            _dragStarted = false;
        }
        private void RowSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_dragStarted)
                GameConfig.UpdateBoardRows((int)BoardRows.Value);
        }
        private void ColumnSliderDragCompleted(object sender, DragCompletedEventArgs e)
        {
            GameConfig.UpdateBoardColumns((int)BoardColumns.Value);
            _dragStarted = false;
        }

        private void ColumnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_dragStarted)
                GameConfig.UpdateBoardColumns((int)BoardColumns.Value);
        }
        private void LoadSecondMoveSettings()
        {
            IsActiveCB.IsChecked = SelectedUnitType.SecondMoveSettings.IsActive;
            IsConditionalCB.IsChecked = SelectedUnitType.SecondMoveSettings.IsConditional;
            InverseAxesCB.IsChecked = SelectedUnitType.SecondMoveSettings.InverseAxes;
            MinCombinedDistanceBox.Text = SelectedUnitType.SecondMoveSettings.MinCombinedDistance.ToString();
            MaxCombinedDistanceBox.Text = SelectedUnitType.SecondMoveSettings.MaxCombinedDistance.ToString();
            // visibility
            SecondGrid.Visibility = SelectedUnitType.SecondMoveSettings.IsActive ? Visibility.Visible : Visibility.Hidden;
            ConditionCB.Visibility = SelectedUnitType.SecondMoveSettings.IsActive ? Visibility.Visible : Visibility.Hidden;
            ConditionsCB.Visibility = SelectedUnitType.SecondMoveSettings.IsConditional ? Visibility.Visible : Visibility.Hidden;
        }
        private void ArchetypeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            SelectedUnitType = (UnitType)ArchetypeCombo.SelectedValue;
            Unit selectedUnit = new Unit(SelectedUnitType, SelectedUnitType.Name, new Player("DemoName", true, true), DemoPosition);
            DemoBoard.UpdateBoard(selectedUnit, DemoPosition);
            _legalMoves = MoveValidator.GetLegalMoves(selectedUnit);
            RefreshBoard();
            LoadSecondMoveSettings();
        }
        private void CreateBoardGrid()
        {
            DemoBoardGrid.Rows = DemoBoardRows;
            DemoBoardGrid.Columns = DemoBoardColumns;

            for (int row = DemoBoardRows - 1; row >= 0; row--)
            {
                for (int column = 0; column < DemoBoardColumns; column++)
                {
                    Button button = new Button();
                    button.Tag = new Position(row, column);
                    button.Background = Brushes.ForestGreen;
                    button.BorderBrush = Brushes.Black;
                    _boardButtons[row, column] = button;
                    DemoBoardGrid.Children.Add(button);
                }
            }
        }
        private ImageSource LoadImage(Unit unit)
        {
            if (unit == null || unit.Type == null)
                throw new Exception("Error when loading unit/unit type");
            return new BitmapImage(
                new Uri($"../Assets/{unit.Type.Name}P1.png",
                        UriKind.Relative));
        }
        private void RefreshBoard()
        {
            Unit selectedUnit = new Unit(SelectedUnitType!, SelectedUnitType!.Name, new Player("DemoName", true, true), DemoPosition);
            _legalMoves = MoveValidator.GetLegalMoves(selectedUnit);
            for (int row = DemoBoardRows - 1; row >= 0; row--)
            {
                for (int column = 0; column < DemoBoardColumns; column++)
                {
                    Unit? unit = DemoBoard.GetUnitAt(new Position(row, column));
                    if (unit != null)
                    {
                        _boardButtons[row, column].Content = new Image
                        {
                            Source = LoadImage(unit),
                            Width = 45,
                            Height = 45,
                            Stretch = Stretch.Uniform
                        };
                    }
                    else { _boardButtons[row, column].Content = ""; }
                    if (_legalMoves.Contains(new Position(row, column)))
                    {
                        _boardButtons[row, column].Background = Brushes.Blue;
                    }
                    else
                    {
                        _boardButtons[row, column].Background = Brushes.ForestGreen;
                    }
                }
            }
        }
        public void Setting_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && !CombinedDistanceValidation())
                return;

            SecondMoveSettings newSettings = sender switch
            {
                CheckBox cb when cb == IsActiveCB => SelectedUnitType.SecondMoveSettings with
                {
                    IsActive = (IsActiveCB.IsChecked == true)
                },
                CheckBox cb when cb == IsConditionalCB => SelectedUnitType.SecondMoveSettings with
                {
                    IsConditional = (IsConditionalCB.IsChecked == true)
                },
                CheckBox cb when cb == InverseAxesCB => SelectedUnitType.SecondMoveSettings with
                {
                    InverseAxes = (InverseAxesCB.IsChecked == true)
                },
                var tb when tb == MinCombinedDistanceBox => SelectedUnitType.SecondMoveSettings with
                {
                    MinCombinedDistance = Int32.Parse(MinCombinedDistanceBox.Text) // valid input
                },
                var tb when tb == MaxCombinedDistanceBox => SelectedUnitType.SecondMoveSettings with
                {
                    MaxCombinedDistance = Int32.Parse(MaxCombinedDistanceBox.Text) // valid input
                },
                _ => SelectedUnitType.SecondMoveSettings
            };
            SelectedUnitType.UpdateSecondMoveSettings(newSettings);
            if (SelectedUnitType.SecondMoveSettings.IsActive && SelectedUnitType.SecondMovementRules == null)
            { // Create second move rules if they enabled it (dirty fix TODO?)
                MessageBox.Show("Creating new second movement rules");
                SelectedUnitType.CreateSecondMovementRules();
            }
            // dirty fix
            SecondGrid.Visibility = SelectedUnitType.SecondMoveSettings.IsActive ? Visibility.Visible : Visibility.Hidden;
            ConditionCB.Visibility = SelectedUnitType.SecondMoveSettings.IsActive ? Visibility.Visible : Visibility.Hidden;
            ConditionsCB.Visibility = SelectedUnitType.SecondMoveSettings.IsConditional ? Visibility.Visible : Visibility.Hidden;
            RefreshBoard();
        }
        public void OpenMovementRangeWindow(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            bool IsFirstMovement = (button.Parent == FirstGrid);

            MoveRangesWindow moveWindow = new MoveRangesWindow(SelectedUnitType, (DirectionName)button.Tag, IsFirstMovement);
            MessageBox.Show($"{SelectedUnitType.SecondMoveSettings.IsActive},{SelectedUnitType.SecondMoveSettings.IsConditional}," +
                 $"{SelectedUnitType.SecondMoveSettings.InverseAxes},{SelectedUnitType.SecondMoveSettings.MinCombinedDistance}," +
                 $"{SelectedUnitType.SecondMoveSettings.MaxCombinedDistance}");
            moveWindow.Show();
            moveWindow.Closing += WindowCloseHandler;
        }
        public void WindowCloseHandler(object? sender, CancelEventArgs? e)
        {
            GameConfig.UpdateUnitTypes(Archetypes);
            RefreshBoard();
        }
        private void SaveExitClick(object sender, RoutedEventArgs e)
        {
            ConfigManager.StoreGameConfig(GameConfig, ConfigDirectory);
            NavigationService.GoBack();
        }
    }
}
