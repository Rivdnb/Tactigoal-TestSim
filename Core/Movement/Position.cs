using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Core.Movement
{
    public readonly record struct Position(
        int Row,
        int Column)
    {
        public Position Add(MovementVector moveVector)
        {
            return new Position(
                Row + moveVector.RowDelta,
                Column + moveVector.ColumnDelta);
        }
    }   
}
