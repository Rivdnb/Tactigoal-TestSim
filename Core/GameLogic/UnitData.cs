using Core.GameObjects;
using Core.Movement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.GameLogic
{
    public class UnitData()
    {
        public string Name { get; init; } = "";
        public Position CurrentPosition { get; init; }
    }
}