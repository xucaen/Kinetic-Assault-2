using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KA2
{
    // These match the rows in your uploaded image!
    public enum EnemyState { Idle = 0, Charge = 1, Fire = 2, Recoil = 3, Desperation = 4, Variant = 5 }

    public class Enemy
    {
        //TODO: this must be a setting in wave.dat
        private const double CHARGE_DURATION = 1.0;

        private Texture2D _texture;
        public Vector2 Position;

        // CHANGE: Use a List and an Index instead of a Wave object
        private List<Vector2> _pathPoints;
        private int _currentPathIndex = 0;

        private float _speed;
        private int _health;
        private double _chargeTimer = 0;
        private float _startTimer;
        public bool IsActive = true;

        // 2. Animation Variables
        private int _frameWidth = 32;  // The width of one robot
        private int _frameHeight = 32; // The height of one robot
        private int _currentFrame = 0;
        private int _totalFrames = 5;  // 5 robots per row
        private double _frameTimer = 0;
        private double _fps = 0.15;    // Seconds per frame (Speed of animation)

        // 3. State Variables
        private EnemyBrain _brain;

        //bullets
        public  Missile _meteor;
        private bool _hasFiredThisCycle = false;

        // Constructor: Runs when you "New up" an enemy
        public Enemy(Texture2D texture, Wave wave, int index)
        {
            _texture = texture;
            // COPY the points from the wave master plan into this specific enemy
            _pathPoints = wave.PathPoints.ToList();

            _startTimer = index * (wave.DelayBetweenEnemies / 1000f);
            _speed = wave.Speed;
            _health = wave.HitsToKill;

            // Start at the first coordinate
            if (_pathPoints.Count > 0)
                Position = _pathPoints[0];

            _brain = new EnemyBrain();
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            
            
        }
        public void Update(GameTime gameTime)
        {

            // 1. UPDATE PROJECTILES FIRST
            // We do this before the "IsActive" check so that even if the 
            // enemy is dead, their meteor keeps moving and doesn't get stuck!
            if (_meteor != null)
            {
                _meteor.Update(gameTime);

                // If it goes off screen, clean it up
                if (_meteor.Position.Y > 480)
                {
                    _meteor = null;
                }
            }

            // 2. THE DEATH CHECK


            if (!IsActive) return;

            // NEW: If we haven't started yet, don't run brain or firing logic
            if (_startTimer > 0)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _startTimer -= dt;
                return;
            }

            // 1. Run the "Brain" (State logic)
            _brain.Update(gameTime);

            // 2. Run the "Body" (Animation logic)
            UpdateAnimation(gameTime);

            // 3. Handle Firing (Wait for meteor to finish)
            
            HandleFiringLogic(gameTime);

            // --- MOVEMENT LOGIC ---
            //

            HandleMovement(gameTime);

        }



        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive || _startTimer > 0) return;

            // Calculate the "Slice" of the sprite sheet
            // X shifts based on animation frame
            // Y shifts based on the Row (the Enum value)


            Rectangle sourceRect = new Rectangle(
                _currentFrame * _frameWidth,
                (int)_brain.CurrentState * _frameHeight,
                _frameWidth,
                _frameHeight
            );



            spriteBatch.Draw(_texture,
                Position, 
                sourceRect, 
                Color.White
    
                );

            // Draw the meteor
            _meteor?.Draw(spriteBatch);
        }

        public void TakeDamage()
        {
            // Transition to the explosion/recoil row in your spritesheet!
            _brain.ForceState(EnemyState.Recoil);

            //decrament the hits to kill value
            if (--_health <= 0)
            {
                IsActive = false;
            }
        }
        private void SpawnMeteor()
        {
            // Center the meteor on the robot
            int meteorWidth = 16;
            float x = Position.X + (_frameWidth / 2) - (meteorWidth / 2);
            float y = Position.Y + _frameHeight;

            //TODO: eventually, the Rectangle will be variable depending on evel.
            _meteor = new Missile(EnemyAdmiral._meteorTexture, new Vector2(x, y), 200f, new Rectangle(0, 0, 16, 16));
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            _frameTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_frameTimer >= _fps)
            {
                _currentFrame++;
                if (_currentFrame >= _totalFrames)
                {
                    _currentFrame = 0;
                    if (_brain.CurrentState == EnemyState.Recoil && IsActive)
                    {
                        _brain.ForceState(EnemyState.Idle);
                    }
                }
                _frameTimer = 0;
            }
        }
        private void HandleFiringLogic(GameTime gameTime)
        {

            // 1. If the brain says "Charge", start the timer
            if (_brain.CurrentState == EnemyState.Charge)
            {
                _chargeTimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (_chargeTimer >= CHARGE_DURATION)
                {
                    _chargeTimer = 0;
                    _brain.ForceState(EnemyState.Fire); // Move to firing state
                }
            }


            if (_brain.CurrentState == EnemyState.Fire)
            {
                // If we just entered Fire state and haven't spawned the meteor yet
                if (_meteor == null && !_hasFiredThisCycle)
                {
                    SpawnMeteor();
                    _hasFiredThisCycle = true;
                }

                // If the meteor is gone (either off-screen or destroyed), go back to Idle
                if (_meteor == null && _hasFiredThisCycle)
                {
                    _brain.ForceState(EnemyState.Idle);
                    _hasFiredThisCycle = false; // Reset for next time
                }
            }


            if (_meteor != null)
            {
                _meteor.Update(gameTime);

                // Use our new non-mystical check!
                if (_meteor.Position.Y > Game1.NativeRenderTarget.Height)
                {
                    _meteor = null;
                }
            }
        }

        private void HandleMovement(GameTime gameTime)
        {

            if (_pathPoints.Count == 0) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // 1. Get the current target from our list
            Vector2 target = _pathPoints[_currentPathIndex];
            Vector2 direction = target - Position;

            if (direction.Length() < 5f)
            {
                // 2. Advance the index
                _currentPathIndex++;

                // 3. THE LOOP: If we reached the end of the path (the end of 'end:'), 
                // set index back to 0 to restart at the beginning of 'start:'
                if (_currentPathIndex >= _pathPoints.Count)
                {
                    _currentPathIndex = 0;
                }
            }
            else
            {
                direction.Normalize();
                Position += direction * _speed * dt;
            }

        }

     
    }
}
