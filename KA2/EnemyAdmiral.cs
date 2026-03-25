using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KA2
{
    public class EnemyAdmiral
    {
        private List<Enemy> _enemies;
        private Dictionary<string, Wave> _waveLibrary;
        private string _currentWaveName;


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

            // Load and store the library
            _waveLibrary = Wave.LoadBehaviorLibrary("Wave.dat");

            if (_waveLibrary.ContainsKey("Swoopers"))
            {
                _currentWaveName = "Swoopers";
                SpawnWave(_waveLibrary[_currentWaveName]);
            }
        }

        private void SpawnWave(Wave wave)
        {
            _enemies.Clear();

            for (int n = 0; n < wave.EnemyCount; ++n)
            {
                // Pass all required arguments to the Wave constructor
                var individualPath = new Wave(
                    wave.PathPoints.ToList(),
                    wave.Speed,
                    wave.DelayBetweenEnemies,
                    wave.EnemyCount,
                    wave.NextWaveName
                );

                var enemy = new Enemy(_enemyTexture, individualPath, n);
                _enemies.Add(enemy);
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

            // Logic for Next Wave
            if (_enemies.Count == 0 && _waveLibrary != null && _currentWaveName != null)
            {
                string next = _waveLibrary[_currentWaveName].NextWaveName;

                if (!string.IsNullOrEmpty(next) && _waveLibrary.ContainsKey(next))
                {
                    _currentWaveName = next;
                    SpawnWave(_waveLibrary[_currentWaveName]);
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