using Core.GameObjects;
using Core.Movement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;

namespace Core.GameLogic
{
    public class UnitType
    {
        public string Name { get; init; }
        public string Symbol { get; init; }
        public int AmountPerPlayer { get; private set; }
        public List<MovementRule> FirstMovementRules { get; private set; }
        public List<MovementRule> SecondMovementRules { get; private set; } = [];
        public SecondMoveSettings SecondMoveSettings { get; private set; } = new();
        public UnitType(string name, string symbol, int amountPerPlayer, List<MovementRule> firstMovementRules,
            List<MovementRule> secondMovementRules, SecondMoveSettings secondMoveSettings) 
        {
            Name = name;
            Symbol = symbol;
            AmountPerPlayer = amountPerPlayer;
            FirstMovementRules = firstMovementRules;
            SecondMovementRules = secondMovementRules;
            SecondMoveSettings = secondMoveSettings;
        }
        public void UpdateFirstMovementRule(MovementRule rule)
        {
            int index = FirstMovementRules.FindIndex(x => x.DirectionName == rule.DirectionName);
            if (index != -1)
                FirstMovementRules[index] = rule;
        }

        public void UpdateSecondMovementRule(MovementRule rule)
        {
            int index = SecondMovementRules.FindIndex(x => x.DirectionName == rule.DirectionName);
            if (index != -1)// TODO: error handling?
                SecondMovementRules[index] = rule;
        }

        public void UpdateSecondMoveSettings(SecondMoveSettings newSettings)
        {
            SecondMoveSettings = newSettings; // TODO?: any logic?
        }

        public void CreateSecondMovementRules()
        {
            SecondMovementRules = [];
            foreach (DirectionName d in Enum.GetValues<DirectionName>())
            {
                MovementVector MatchingVec = FirstMovementRules?.Find(x => x.DirectionName == d)?.MoveVector
                    ?? throw new Exception("Can't find first movement rules vectors"); // TODO?: more elegant solution?
                SecondMovementRules.Add(new MovementRule(d, MatchingVec, 0, 0));
            }
        }
    }
}
