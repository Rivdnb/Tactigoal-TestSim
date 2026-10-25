using Core.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.GameLogic
{
    public class GameState // TODO: Extend class when needed
    {
        //public Player P1 { get; }
        //public Player P2 { get; }
        //public Player CurrentPlayer { get; private set; }
        public List<Unit> Units { get; private set; }
        public GameState(List<Unit> units)
        {
            //P1 = p1;
            //P2 = p2;
            //CurrentPlayer = currentPlayer;
            Units = units;
        }
    }
}
