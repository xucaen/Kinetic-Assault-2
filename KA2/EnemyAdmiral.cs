using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace KA2
{
    public class EnemyAdmiral
    {
        private List<Enemy> _enemies;

        private Texture2D _enemyTexture;
        public static Texture2D _meteorTexture;
        public EnemyAdmiral()
        {
            _enemies = new List<Enemy>();
        }

        public void LoadContent(ContentManager content)
        {

            // YOU NEED THESE TWO LINES!
            _enemyTexture = content.Load<Texture2D>("enemy_sprites");
            _meteorTexture = content.Load<Texture2D>("meteors");

            // 1. Define the path data here (This will eventually come from your JSON)
            List<Vector2> enterPath = new List<Vector2>
                {
                    new Vector2(-50, -50),   // Start off-screen
                    new Vector2(320, 240),  // Swoop to center
                    new Vector2(320, 100)   // Destination area
                };

            // 2. Create the Behavior object
            Behavior swoopIn = new Behavior(enterPath);

            // 3. Call the single method with the data it needs
            SpawnFormation(10, swoopIn);
        }

        private void SpawnFormation(int numOfEnemies, Behavior behavior)
        {
            _enemies.Clear();

            for (int n = 0; n < numOfEnemies; ++n)
            {
                // Each enemy gets the shared behavior and its index (n)
                _enemies.Add(new Enemy(_enemyTexture, behavior, n));
            }
        }

        public void Update(GameTime gameTime)
        {
            // Update all enemies and remove dead ones
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                _enemies[i].Update(gameTime);

                if (!_enemies[i].IsActive)
                {
                    _enemies.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var enemy in _enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

        internal List<Enemy> GetEnemies()
        {
            return _enemies;
        }
    }
}