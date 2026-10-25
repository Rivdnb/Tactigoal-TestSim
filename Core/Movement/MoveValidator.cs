using Core.GameObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Movement
{
    public class MoveValidator
    {
        public Board Board { get; }
        public MoveValidator(Board board)
        {
            Board = board;
        }

        public List<Position> GetLegalMovesForRules(Unit unit, Position currentPos, List<MovementRule> movementRules)
        {
            List<Position> legalMoves = new List<Position>();
            Position nextPos;
            Unit? unitAtNextPos;
            MovementVector moveVec;

            // Movement check for each direction (for this to work: no units can skip tiles)
            foreach (MovementRule rule in movementRules) // One rule for each of the 8 directions 
            {
                nextPos = currentPos; // Start at current position
                for (int i = rule.MinDistance; i <= rule.MaxDistance; i++)  // Trace path per direction
                {
                    if (rule.MaxDistance == 0) break; // Forbidden direction for this unit -> skip

                    moveVec = (unit.Owner.IsPlayerOne) ? rule.MoveVector : rule.MoveVector.Reverse(); // account for P2 move direction
                    nextPos = nextPos.Add(moveVec);// Take a step
                    if (!Board.IsValidPosition(nextPos)) break; // Out of bounds -> skip

                    unitAtNextPos = Board.GetUnitAt(nextPos);
                    if (unitAtNextPos != null)// Path blocked 
                        if (unitAtNextPos?.Type?.Name == "Keeper" && unitAtNextPos?.Owner != unit.Owner)
                        {
                            legalMoves.Add(nextPos); // Exception: Enemy keeper
                        }
                        else { break; }
                    legalMoves.Add(nextPos);
                }
            }
            return legalMoves;
        }

        public List<Position> GetLegalMoves(Unit unit)
        {
            List<Position> legalMoves = new List<Position>();
            List<Position> secondLegalMoves = new List<Position>();
            if (unit == null)
                return legalMoves;
            List<MovementRule> firstMovementRules = unit.Type?.FirstMovementRules
                ?? throw new ArgumentNullException(nameof(unit));            

            legalMoves = GetLegalMovesForRules(unit, unit.CurrentPosition, firstMovementRules); // Get legal moves for the First move
                                                                                      
            if (unit.Type.SecondMoveSettings.IsActive) // Second move if applicable
                {
                List<Position> legalMovesCopy = new List<Position>(legalMoves);
                List <MovementRule> secondMovementRules = unit.Type.SecondMovementRules;
                foreach (Position pos in legalMovesCopy) // all possible positions after the first move
                {
                    
                    if (!unit.Type.SecondMoveSettings.IsConditional) // 1.  Unconditional
                    {
                        // Add legal positions of all possible second moves, while skipping duplicate positions
                        secondLegalMoves = GetLegalMovesForRules(unit, pos, secondMovementRules);
                        legalMoves.AddRange(secondLegalMoves.Except(legalMoves)); 
                    }
                    else // 2.  Conditional
                    {
                        //2a. Inverse axes, distance requirement

                        //2b. No inverse axes, distance requirement only
                    }


                }
            }
            return legalMoves;
        }
    }
}
