using Core.GameObjects;
using Core.Movement;
using System.Data.Common;
using System.Text.Json;

namespace Core.GameLogic
{
    public class Game
    {
        public bool GameWon { get; private set; }
        public Player P1 { get; }
        public Player P2 { get; }
        public Player currentPlayer { get; private set; }
        public Board Board { get; }
        public Unit? selectedUnit { get; private set; }
        public Unit? KeeperP1 { get; private set; }
        public Unit? KeeperP2 { get; private set; }
        public MoveValidator MoveValidator { get; }
        public GameConfig GameConfig { get; }
        public Game(GameConfig gameConfig)
        {
            GameWon = false;
            GameConfig = gameConfig;
            P1 = GameConfig.P1; 
            P2 = GameConfig.P2;
            currentPlayer = P1;
            Board = new Board(GameConfig.BoardRows, GameConfig.BoardColumns);
            selectedUnit = null; KeeperP1 = null; KeeperP2 = null;
            MoveValidator = new MoveValidator(Board);
        }
        public void LoadLineUp(string name, Player player)
        {
            string json = File.ReadAllText(GameConfig.LineUpsPath);
            LineUpList? lineUps = JsonSerializer.Deserialize<LineUpList>(json);
            LineUp lineup = lineUps?.List.Find(x => x.Name == name) ?? throw new ArgumentNullException(nameof(name)); // TODO: handle this

            if(lineup.BoardColumns != Board.N_Columns || lineup.BoardRows != Board.N_Rows)
                throw new Exception($"Failed to load line-up: Incompatible board size Lineup" +
                    $" ({lineup.BoardRows}, {lineup.BoardColumns}) while Game ({Board.N_Rows}, {Board.N_Columns})");

            UnitType? type = null;
            Unit? unit = null;
            int newRow, newCol;

            foreach (UnitData unitData in lineup.UnitsData)
            {
                type = GameConfig.UnitTypes.Find(t => t.Name == unitData.Name) 
                    ?? throw new ArgumentNullException(unitData.Name);
                if (lineup.IsPlayerOne && player == P2) // Move units in P1 line-up to P2 side
                {
                    newRow = (Board.N_Rows - 1) - unitData.CurrentPosition.Row;
                    newCol = (Board.N_Columns - 1) - unitData.CurrentPosition.Column;
                    unit = new(type, unitData.Name, player, new Position(newRow, newCol));

                }// TODO: P2 lineups -> P1
                else
                    unit = new(type, unitData.Name, player, unitData.CurrentPosition);

                Board.UpdateBoard(unit, unit.CurrentPosition);
            }
        }
        public bool Coinflip()
        {
            Random random = new Random();
            int rand = random.Next(1, 3);
            return (rand == 1); // true = P1 , false = P2
        }
        public bool GameSetup() 
        {
            currentPlayer = (Coinflip()) ? P1 : P2;

            UnitType keeperType = GameConfig.UnitTypes.Find(x => x.Name == "Keeper")
                ?? throw new Exception($"Failed to load unit type: Keeper");
            KeeperP1 = new Unit(keeperType, keeperType.Name, P1, new Position(0, (int)Math.Floor((decimal)Board.N_Columns/2)));
            KeeperP2 = new Unit(keeperType, keeperType.Name, P2, new Position(Board.N_Rows-1, (int)Math.Floor((decimal)Board.N_Columns / 2)));
            Board.UpdateBoard(KeeperP1, KeeperP1.CurrentPosition);
            Board.UpdateBoard(KeeperP2, KeeperP2.CurrentPosition);

            LoadLineUp("Default", P1);
            LoadLineUp("Default", P2);

            return currentPlayer.IsPlayerOne; 
        }

        public bool IsStalemate()
        {
            return false;
        }

        public void HandleTileClick(Position position) //TODO setup phase
        {
            Unit? unitAtPos = Board.GetUnitAt(position);
            {
                if (selectedUnit == null)// Check for selecting a currentPlayer's unit
                {
                    if (unitAtPos != null && unitAtPos.Owner == currentPlayer)
                        selectedUnit = unitAtPos;
                    return; // Select unit
                }
                if (MoveValidator.GetLegalMoves(selectedUnit).Contains(position))// Check for executing a legal move
                {
                    if(unitAtPos?.Type?.Name == "Keeper" && unitAtPos.Owner != currentPlayer)
                    {
                        GameWon = true; // current player wins
                        Board.UpdateBoard(selectedUnit, position);
                        selectedUnit.MoveUnit(position);
                        return;
                    }
                    Board.UpdateBoard(selectedUnit, position);
                    selectedUnit.MoveUnit(position);
                    selectedUnit = null;
                    currentPlayer = (currentPlayer == P1) ? P2 : P1;
                }
                selectedUnit = null;
                return;
            }
        }
    }
}
