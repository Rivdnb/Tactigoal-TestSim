using System;
using System.Collections.Generic;
using System.Text;
using Core.Movement;

namespace Core.GameObjects
{
    public class Board(int n_rows, int n_columns)
    {
        public int N_Rows { get; } = n_rows;
        public int N_Columns { get; } = n_columns;

        private Unit?[,] _squares = new Unit[n_rows, n_columns];

        public bool IsValidPosition(Position pos)
        {
            if (pos.Row < 0 || pos.Row >= N_Rows) { return false; }
            if (pos.Column < 0 || pos.Column >= N_Columns) { return false; }

            return true;
        }
        public Unit? GetUnitAt(Position pos)
        {
            return IsValidPosition(pos) ? _squares[pos.Row, pos.Column] : null;
        }
        public void UpdateBoard(Unit unit, Position pos)
        {
            if (!IsValidPosition(pos)) 
                return;
            _squares[unit.CurrentPosition.Row, unit.CurrentPosition.Column] = null;
            _squares[pos.Row, pos.Column] = unit;
        }
    }
}
