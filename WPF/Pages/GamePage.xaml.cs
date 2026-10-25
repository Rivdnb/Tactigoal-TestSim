using Core.GameLogic;
using Core.GameObjects;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF.Pages
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private readonly Game _game;

        private Button[,] _boardButtons;

        private List<Position> _legalMoves;

        public GamePage()
        {

            InitializeComponent();

            Application.Current.MainWindow.Height = 1050;
            Application.Current.MainWindow.Width = 700;

            ConfigManager configManager = new ConfigManager();
            string configDirectory = System.IO.Path.Combine(AppContext.BaseDirectory, "ConfigFiles");
            string dataDirectory = System.IO.Path.Combine(AppContext.BaseDirectory, "Data");
            GameConfig config = configManager.LoadGameConfig(configDirectory, dataDirectory);

            _game = new Game(config);

            _boardButtons = new Button[config.BoardRows, config.BoardColumns];

            _legalMoves = [];

            CreateBoardGrid();
            _game.GameSetup();
            MessageBox.Show($"Startplayer: {_game.currentPlayer.Name}");
            RefreshBoard();


        }
        private SolidColorBrush GetBackgroundColour(int row) // TODO: Zones later?
        {
            return Brushes.ForestGreen;
        }

        private void TileClicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Position position = (Position)button.Tag;
            _game.HandleTileClick(position);
            if (_game.selectedUnit != null)
                _legalMoves = _game.MoveValidator.GetLegalMoves(_game.selectedUnit);
            else
                _legalMoves = [];
            if (_game.GameWon)
            {
                MessageBox.Show($"Goal! {_game.currentPlayer.Name} wins!");
            }
            RefreshBoard();
        }
        private void CreateBoardGrid()
        {
            BoardGrid.Rows = _game.Board.N_Rows;
            BoardGrid.Columns = _game.Board.N_Columns;

            for (int row = _game.Board.N_Rows - 1; row >= 0; row--)
            {
                for (int column = 0; column < _game.Board.N_Columns; column++)
                {
                    Button button = new Button();
                    button.Tag = new Position(row, column);
                    button.Click += TileClicked;
                    button.Background = GetBackgroundColour(row);
                    button.BorderBrush = Brushes.Black;
                    if (row == (_game.Board.N_Rows - 1) / 2)
                        button.BorderThickness = new Thickness(1, 3, 1, 1);
                    else
                        button.BorderThickness = new Thickness(1);

                    _boardButtons[row, column] = button;
                    BoardGrid.Children.Add(button);
                }
            }

        }
        private ImageSource LoadImage(Unit unit)
        {
            if (unit == null || unit.Type == null)
                throw new Exception("Error when loading unit/unit type");
            string player = (unit.Owner == _game.P1) ? "P1" : "P2";
            return new BitmapImage(
                new Uri($"../Assets/{unit.Type.Name}{player}.png",
                        UriKind.Relative));
        }
        private void RefreshBoard()
        {
            for (int row = _game.Board.N_Rows - 1; row >= 0; row--)
            {
                for (int column = 0; column < _game.Board.N_Columns; column++)
                {
                    Unit? unit = _game.Board.GetUnitAt(new Position(row, column));
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
                        _boardButtons[row, column].BorderBrush = Brushes.Blue;
                    }
                    else
                    {
                        _boardButtons[row, column].BorderBrush = Brushes.Black;
                    }
                }
            }
        }
    }
}
