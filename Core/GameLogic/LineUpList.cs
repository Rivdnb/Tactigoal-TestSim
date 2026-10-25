using System;
using System.Collections.Generic;
using System.Text;

namespace Core.GameLogic
{
    public class LineUpList(List<LineUp> list)
    {
        public List<LineUp> List { get; init; } = list;
    }
}
