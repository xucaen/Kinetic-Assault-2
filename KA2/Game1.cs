using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace KA2
{
    public class Game1 : Game
    {

        // --- THE GLOBAL STATICS ---
        public static RenderTarget2D NativeRenderTarget;
        public static InputController Input;
        public static EnemyAdmiral Admiral;
        // ------


        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player _player;
        private Texture2D _explosionTexture;
        private List<Explosion> _explosions = new List<Explosion>();



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set the window to retro 4:3
            _graphics.PreferredBackBufferWidth = 640;
            _graphics.PreferredBackBufferHeight = 480;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            
            Input = new InputController();
            NativeRenderTarget = new RenderTarget2D(GraphicsDevice, 640, 480);
            Admiral = new EnemyAdmiral(); // Initialize the Admiral
            base.Initialize();
        }

        protected override void LoadContent()
        {

            ////load the player
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load your ship here! Use the filename WITHOUT the .png extension
            // 1. Initialize the player and load textures
            _player = new Player(Vector2.Zero); // Start at zero, move it in a second
            _player.LoadContent(Content);


            // Load the new explosion sheet (ensure the file is in Content as "explosions")
            _explosionTexture = Content.Load<Texture2D>("explosion1_5");


            // 2. Use the virtual resolution (Native Render Target) for calculations
            int screenWidth = NativeRenderTarget.Width;
            int screenHeight = NativeRenderTarget.Height;

            // Use the first frame to calculate positioning
            _player.SetPosition(new Vector2(
                (screenWidth / 2) - (_player.Width / 2),
                screenHeight - _player.Height - 50));

            // Tell the Admiral to load textures and spawn enemies
            Admiral.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            // Update the input state
            Input.ProcessInput();

            // Check for Exit (Xbox 'Back' button or Escape)
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Use the Movement vector from our input controller
            _player.Update(gameTime, Input, NativeRenderTarget.Width);


            // Admiral handles all enemy updates now
            Admiral.Update(gameTime);

            CheckCollisions();

            // Update explosions and remove finished ones
            for (int i = _explosions.Count - 1; i >= 0; i--)
            {
                _explosions[i].Update(gameTime);
                if (_explosions[i].IsFinished)
                    _explosions.RemoveAt(i);
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // 1. Draw to the virtual 640x480 canvas
            GraphicsDevice.SetRenderTarget(NativeRenderTarget);
            GraphicsDevice.Clear(Color.Black); // Retro games look best on black!

            _spriteBatch.Begin();
            _player.Draw(_spriteBatch); // Let the player draw itself
            // Admiral handles all enemy drawing
            Admiral.Draw(_spriteBatch);
            // Draw all active explosions
            foreach (var explosion in _explosions)
            {
                explosion.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            // 2. Draw that canvas to the actual window (with letterboxing)
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.DimGray); // Background color for the "bars"

            // SamplerState.PointClamp keeps the pixels from getting blurry when scaled
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            _spriteBatch.Draw(NativeRenderTarget, GetScaleRectangle(), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private Rectangle GetScaleRectangle()
        {
            var backBufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            var backBufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // We want 640x480 (4:3 ratio)
            float targetAspectRatio = 640f / 480f;
            float screenAspectRatio = (float)backBufferWidth / (float)backBufferHeight;

            int width = backBufferWidth;
            int height = backBufferHeight;
            int x = 0;
            int y = 0;

            if (screenAspectRatio > targetAspectRatio)
            {
                // Screen is wider than 4:3 (Letterbox on sides)
                width = (int)(backBufferHeight * targetAspectRatio);
                x = (backBufferWidth - width) / 2;
            }
            else
            {
                // Screen is taller than 4:3 (Letterbox on top/bottom)
                height = (int)(backBufferWidth / targetAspectRatio);
                y = (backBufferHeight - height) / 2;
            }

            return new Rectangle(x, y, width, height);
        }

        private void CheckCollisions()
        {
            // 1. Get the list of enemies from the Admiral
            var enemies = Admiral.GetEnemies();

            // 2. Check Player's Bullet vs Aliens
            if (_player.Bullet != null)
            {


                // Create a hit-box for the bullet
                Rectangle bulletRect = new Rectangle(
                    (int)_player.Bullet.Position.X,
                    (int)_player.Bullet.Position.Y,
                    _player.Bullet.Width,
                    _player.Bullet.Height
                );

                foreach (var alien in enemies)
                {
                    if (!alien.IsActive) continue;

                    // Create a hit-box for the alien
                    Rectangle alienRect = new Rectangle(
                        (int)alien.Position.X,
                        (int)alien.Position.Y,
                        32, 32 // Your robot frame size
                    );

                    if (bulletRect.Intersects(alienRect))
                    {
                        // BOOM!
                        // ALIEN EXPLOSION: 32x32 size, starting at Y=16 on the sheet
                        _explosions.Add(new Explosion(_explosionTexture, alien.Position));
                        alien.TakeDamage();// this set's the alien's state
                        _player.Bullet = null; // Destroy bullet on impact
                        break; // Stop checking this bullet, it's gone
                    }

                    var meteor = alien._meteor;
                    if (meteor != null)
                    {
                        // Create a hit-box for the meteor
                        Rectangle meteorRect = new Rectangle(
                        (int)meteor.Position.X,
                            (int)meteor.Position.Y,
                            meteor.Width,
                            meteor.Height
                        );


                        if (bulletRect.Intersects(meteorRect))
                        {
                            // Explosion for the meteor
                            _explosions.Add(new Explosion(_explosionTexture, meteor.Position));
                        }
                    }
                }
            }


            //Did an alien meteor hit the player?

            foreach (var alien in enemies)
            {
                if (!alien.IsActive) continue;

                var meteor = alien._meteor;
                if (meteor != null)
                {
                    // Create a hit-box for the meteor
                    Rectangle meteorRect = new Rectangle(
                    (int)meteor.Position.X,
                        (int)meteor.Position.Y,
                        meteor.Width,
                        meteor.Height
                    );

                    // Create a hit-box for the alien
                    Rectangle playerRect = new Rectangle(
                        (int)_player.Position.X,
                        (int)_player.Position.Y,
                        _player.Width, _player.Height
                    );
                    if (meteorRect.Intersects(playerRect))
                    {
                        // BOOM!
                        // PLAYER EXPLOSION: 64x64 size, starting at Y=48
                        _explosions.Add(new Explosion(_explosionTexture, _player.Position));
                        meteor.IsExpired = true;
                        //TODO: set Player state for taking damage or  blowing up

                    }
                }

            }
        }
    }
}
