using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Core.Movement
{
    public readonly record struct MovementVector(
        int RowDelta,
        int ColumnDelta)
    {
        public MovementVector Reverse()
        {
            return new MovementVector(
                RowDelta * -1,
                ColumnDelta * -1);
        }
    }
}
