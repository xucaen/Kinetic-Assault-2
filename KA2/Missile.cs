using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KA2
{
    public class Missile
    {
        public Vector2 Position;
        private Texture2D _texture;
        private float _speed;
        private Rectangle? _sourceRect; // Nullable so bullets can draw the whole texture

        public bool IsExpired { get; set; }

        // Use the source rect width if available, otherwise the full texture width
        public int Width => _sourceRect?.Width ?? _texture.Width;
        public int Height => _sourceRect?.Height ?? _texture.Height;

        public Missile(Texture2D texture, Vector2 startPosition, float speed, Rectangle? sourceRect = null)
        {
            _texture = texture;
            Position = startPosition;
            _speed = speed;
            _sourceRect = sourceRect;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Adding speed to Y. 
            // Use a negative speed for things going UP (Bullets)
            // Use a positive speed for things going DOWN (Meteors)
            Position.Y += _speed * dt;

            if (IsOffScreen())
            {
                IsExpired = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // The Draw overload handles null sourceRects by drawing the whole texture
            spriteBatch.Draw(_texture, Position, _sourceRect, Color.White);
        }

        public bool IsOffScreen()
        {
            int screenWidth = Game1.NativeRenderTarget.Width;
            int screenHeight = Game1.NativeRenderTarget.Height;

            // Check all four bounds based on the calculated Width/Height
            if (Position.Y < -Height) return true;
            if (Position.Y > screenHeight) return true;
            if (Position.X < -Width) return true;
            if (Position.X > screenWidth) return true;

            return false;
        }
    }
}