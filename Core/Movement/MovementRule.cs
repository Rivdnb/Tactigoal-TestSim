using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Movement
{
    public class MovementRule
    {
        public DirectionName DirectionName { get; init; }
        public MovementVector MoveVector { get; init; }
        public int MinDistance { get; private set; }
        public int MaxDistance { get; private set; }
        public MovementRule(DirectionName directionName, MovementVector moveVector, int minDistance, int maxDistance)
        {
            DirectionName = directionName;
            MoveVector = moveVector;
            MinDistance = minDistance;
            MaxDistance = maxDistance;
        }
        public void UpdateMinDistance(int newValue)
        {
            MinDistance = newValue;
        }
        public void UpdateMaxDistance(int newValue)
        {
            MaxDistance = newValue;
        }
    }
}
