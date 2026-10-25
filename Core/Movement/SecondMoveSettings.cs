using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Movement
{
    public readonly record struct SecondMoveSettings(
    bool IsActive,
    bool IsConditional,
    bool InverseAxes,
    int MinCombinedDistance,
    int MaxCombinedDistance)
    {
        public SecondMoveSettings() : this(false, false, false, 1, 3)
        {
        }
    }
}
