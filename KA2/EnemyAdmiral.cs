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
            // The Admiral manages the master texture for all enemies
            _enemyTexture = content.Load<Texture2D>("enemy_sprites");
            _meteorTexture = content.Load<Texture2D>("meteors");
            // For now, let's spawn a small formation (like Galaga)
            SpawnFormation();
        }

        private void SpawnFormation()
        {
            _enemies.Clear();

            // Example: Create a 5x2 grid of enemies
            for (int row = 0; row < 2; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    Vector2 pos = new Vector2(150 + (col * 80), 50 + (row * 60));
                    _enemies.Add(new Enemy(_enemyTexture, pos));
                }
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