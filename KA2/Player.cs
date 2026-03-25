using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace KA2
{
    public class Player
    {
        // State
        public bool IsAlive = true;
        public Vector2 Position;
        private Texture2D[] _textures;
        private float _speed = 300f;

        // Animation
        private int _currentFrame = 0;
        private double _timer = 0;
        private double _frameTime = 100; // Milliseconds per frame


        //bullets
        public  Missile Bullet;
        private Texture2D _bulletTexture;


       

        public int Width => _textures[0].Width;
        public int Height => _textures[0].Height;


        public Player(Vector2 startPosition)
        {
            Position = startPosition;
            _textures = new Texture2D[4];
        }

        public Player()
        {
            _textures = new Texture2D[4];
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            _textures[0] = content.Load<Texture2D>("player_fighter_3a");
            _textures[1] = content.Load<Texture2D>("player_fighter_3b");
            _textures[2] = content.Load<Texture2D>("player_fighter_3c");
            _textures[3] = content.Load<Texture2D>("player_fighter_3d");

            // ... your existing texture loads ...
            _bulletTexture = content.Load<Texture2D>("bullet"); // Make sure you have a bullet image!

        }

        public void Update(GameTime gameTime, InputController input, int screenWidth)
        {

            if (!IsAlive) return; // Stop processing movement/shooting if dead

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Movement
            Position.X += input.Movement.X * _speed * dt;

            // Clamping (using the width of the first frame)
            Position.X = MathHelper.Clamp(Position.X, 0, screenWidth - Width);

            // Animation
            _timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_timer >= _frameTime)
            {
                _currentFrame = (_currentFrame + 1) % _textures.Length;
                _timer -= _frameTime;
            }


            // 1. Handle Shooting
            // 2. Check for the Fire Button (Now this works!)
            // We check the 'IsFiring' bool inside the InputController
            if (input.IsFiring && Bullet == null)
            {
                SpawnBullet();
            }

            // 3. Update the single active bullet
            if (Bullet != null)
            {
                Bullet.Update(gameTime);

                // If it goes off the top of the screen, delete it so we can fire again
                if (Bullet.IsExpired)
                {
                    Bullet = null;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsAlive) return;

            spriteBatch.Draw(_textures[_currentFrame], Position, Color.White);
            // Draw all bullets

            if (Bullet != null)
            {
                Bullet.Draw(spriteBatch);
            }
          
        }


        private void SpawnBullet()
        {
            // 1. Calculate the X position so the bullet comes out of the center of the ship
            // Formula: (Ship X + half of Ship Width) - (half of Bullet Width)
            float bulletX = Position.X + (Width / 2) - (_bulletTexture.Width / 2);

            // 2. Start the bullet at the top of the player ship
            float bulletY = Position.Y;

            // 3. Create the new bullet instance
            // We assign it to _activeBullet. Since our Update loop only fires if 
            // _activeBullet is null, this "locks" the gun until this bullet is gone.
            Bullet = new Missile(_bulletTexture, new Vector2(bulletX, bulletY),-600f,null);
        }

        public void Reset(Vector2 startPosition)
        {
            Position = startPosition;
            IsAlive = true;
            Bullet = null; // Clean up any stray bullets
        }
    }
}