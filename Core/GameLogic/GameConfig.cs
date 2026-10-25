using Core.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.GameLogic
{
    public class GameConfig
    {
        public Player P1 { get; init; }
        public Player P2 { get; init; }
        public int BoardRows { get; private set; }
        public int BoardColumns { get; private set; }
        public List<UnitType> UnitTypes { get; private set; }
        public List<string> UnitTypePaths { get; init; }
        public string LineUpsPath { get; init; }
        public GameConfig(Player p1, Player p2, int boardRows, int boardColumns, List<UnitType> unitTypes,
            List<string> unitTypePaths, string lineUpsPath)
        {
            P1 = p1;
            P2 = p2;
            BoardRows = boardRows;
            BoardColumns = boardColumns;
            UnitTypes = unitTypes;
            UnitTypePaths = unitTypePaths;
            LineUpsPath = lineUpsPath;
        }
        public void UpdateBoardRows(int newValue) => BoardRows = newValue;
        public void UpdateBoardColumns(int newValue) => BoardColumns = newValue;
        public void UpdateUnitTypes(List<UnitType> unitTypes) => UnitTypes = unitTypes;

    }
}
