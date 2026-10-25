using Core.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.GameLogic
{
    public class LineUp
    {
        public string Name { get; init; }
        public bool IsPlayerOne { get; init; }
        public int BoardRows { get; init; } // TODO: Create BoardSize struct?
        public int BoardColumns { get; init; }
        public List<UnitData> UnitsData { get; init; }
        public LineUp(string name, bool isPlayerOne, int boardRows, int boardColumns, List<UnitData> unitsData)
        {
            Name = name;
            IsPlayerOne = isPlayerOne;
            BoardRows = boardRows;
            BoardColumns = boardColumns;
            UnitsData = unitsData;
        }
    }
}
