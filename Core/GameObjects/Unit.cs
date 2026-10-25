using Core.GameLogic;
using Core.Movement;

namespace Core.GameObjects
{
    public class Unit(UnitType type, string name, Player player, Position currentPos)
    {
        public UnitType Type { get; private set; } = type;
        public string Name { get; init; } = name;
        public Player Owner { get; private set; } = player;
        public Position CurrentPosition { get; private set; } = currentPos;

        public void MoveUnit(Position nextPos) => CurrentPosition = nextPos; // TODO error checks

    }
}
