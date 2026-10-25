using System;
using System.Collections.Generic;
using System.Text;

namespace Core.GameObjects
{
    public readonly record struct Player(
        string Name, 
        bool IsPlayerOne, bool IsHuman);
}
