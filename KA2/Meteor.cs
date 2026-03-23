using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KA2
{
    public class Meteor
    {
        public Vector2 Position;
        private Texture2D _texture;
        private float _speed = 200f; // slow...
        public bool IsExpired { get;  set; }

        // This is the "Magic" that fixes it. 
        // We only take 16 pixels of width from the 128 pixel image.
        private Rectangle _sourceRect = new Rectangle(0, 0, 16, 16);

        public int Width => _texture.Width;
        public int Height => _texture.Height;

        public Meteor(Texture2D texture, Vector2 startPosition)
        {
            _texture = texture;
            Position = startPosition;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move DOWN the screen
            Position.Y += _speed * dt;

            // Mark for deletion if it leaves the top of the screen
            if (Position.Y > Game1.NativeRenderTarget.Height)
            {
                IsExpired = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, _sourceRect, Color.White);
        }
    }
}