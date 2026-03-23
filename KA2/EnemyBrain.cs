using Microsoft.Xna.Framework;
using System;

namespace KA2
{

    /// <summary>
    /// This is the enemy's State engine. 
    /// </summary>
    public class EnemyBrain
    {
        public EnemyState CurrentState { get; private set; } = EnemyState.Idle;
        public EnemyState PreviousState { get; private set; } = EnemyState.Idle;

        private double _stateTimer = 0;

        // Personality tuning (this is your future "difficulty system")
        public double IdleTime;
        public double ChargeTime = 1.0;
        public double FireTime = 0.5;


        private static Random _rng = new Random();

        public EnemyBrain()
        {
            // .Next(3, 11) gives a random integer from 3 up to (but not including) 11
            IdleTime = _rng.Next(3, 11);
        }
        public void Update(GameTime gameTime)
        {
            _stateTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (CurrentState == EnemyState.Idle && _stateTimer > IdleTime)
            {
                ChangeState(EnemyState.Charge);
            }
            else if (CurrentState == EnemyState.Charge && _stateTimer > ChargeTime)
            {
                ChangeState(EnemyState.Fire);
            }
            //leaving the fire state depends on if the meteor is still alive
        }

        private void ChangeState(EnemyState newState)
        {
            PreviousState = CurrentState;
            CurrentState = newState;
            _stateTimer = 0;
        }
        // New method to allow the Enemy class to force a state change
        public void ForceState(EnemyState newState)
        {
            ChangeState(newState);
        }
        public bool IsNewState()
        {
            return CurrentState != PreviousState;
        }
    }
}