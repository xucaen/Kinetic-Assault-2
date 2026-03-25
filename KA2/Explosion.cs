using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Explosion
{
    private Texture2D _texture;
    private Vector2 _position;

    // Constant values based on your specific sprite sheet
    private const int TileSize = 100;
    private const int Columns = 10;
    private const int TotalFrames = 50;

    private int _currentFrame = 0;
    private double _timer;
    private double _fps = 0.04; // Adjust speed (lower is faster)
    public bool IsFinished = false;


    /// <summary>
    /// Explosion texture courtesy if
    ///
    ///https://opengameart.org/content/explosion-7
    ///Artist: BenHickling
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="pos"></param>
    public Explosion(Texture2D texture, Vector2 pos)
    {
        _texture = texture;

        // Offset position so the 100x100 explosion centers on the object
        // Adjust these subtractions based on the size of the object that blew up
        _position = new Vector2(pos.X - 32, pos.Y - 32);
    }

    public void Update(GameTime gameTime)
    {
        _timer += gameTime.ElapsedGameTime.TotalSeconds;
        if (_timer >= _fps)
        {
            _currentFrame++;
            _timer = 0;
            if (_currentFrame >= TotalFrames) IsFinished = true;
        }
    }

    public void Draw(SpriteBatch sb)
    {
        // Calculate which row and column we are on
        int column = _currentFrame % Columns;
        int row = _currentFrame / Columns;

        Rectangle sourceRect = new Rectangle(
            column * TileSize,
            row * TileSize,
            TileSize,
            TileSize
        );

        sb.Draw(_texture, _position, sourceRect, Color.White);
    }
}