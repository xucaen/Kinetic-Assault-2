using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KA2
{
    public class Bullet
    {
        public Vector2 Position;
        private Texture2D _texture;
        private float _speed = 600f; // Fast!
        public bool IsExpired { get; private set; }

        public int Width => _texture.Width;
        public int Height => _texture.Height;

        public Bullet(Texture2D texture, Vector2 startPosition)
        {
            _texture = texture;
            Position = startPosition;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move up the screen
            Position.Y -= _speed * dt;

            // Mark for deletion if it leaves the top of the screen
            if (IsOffScreen())
            {
                IsExpired = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color.White);
        }


        public bool IsOffScreen()
        {
            // 1. Get the screen dimensions from the Overlord
            int screenWidth = Game1.NativeRenderTarget.Width;
            int screenHeight = Game1.NativeRenderTarget.Height;

            // 2. Check Top: Is the BOTTOM of the bullet past the top?
            if (Position.Y < -Height) return true;

            // 3. Check Bottom: Is the TOP of the bullet past the bottom?
            if (Position.Y > screenHeight) return true;

            // 4. Check Left: Is the RIGHT edge past the left side?
            if (Position.X < -Width) return true;

            // 5. Check Right: Is the LEFT edge past the right side?
            if (Position.X > screenWidth) return true;

            return false;
        }

    }
}