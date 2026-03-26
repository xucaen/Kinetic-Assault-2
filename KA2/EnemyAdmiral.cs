using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using static KA2.EnemyColonel;

namespace KA2
{
    public class EnemyAdmiral
    {
        private List<Enemy> _enemies;
        private Dictionary<string, Wave> _waveLibrary;
        private string _currentWaveName;
        private EnemyColonel _activeBoss;

        private Texture2D _enemyTexture;
        public static Texture2D _meteorTexture;
        public EnemyAdmiral()
        {
            _enemies = new List<Enemy>();
        }

        public void LoadContent(ContentManager content)
        {

            _enemyTexture = content.Load<Texture2D>("enemy_sprites");
            _meteorTexture = content.Load<Texture2D>("meteors");

            // Load and store the library
            _waveLibrary = Wave.LoadBehaviorLibrary("Wave.dat");

            if (_waveLibrary != null && _waveLibrary.Count > 0)
            {
                // Dynamically pick the very first wave defined in the file
                _currentWaveName = _waveLibrary.Keys.First();
                SpawnWave(_waveLibrary[_currentWaveName]);
            }
        }

        private void SpawnWave(Wave wave)
        {
            _enemies.Clear();
            _activeBoss = null;

            if (wave.BossParts.Count > 0)
            {
                _activeBoss = new EnemyColonel(wave);
            }
            else
            { 
                for (int n = 0; n < wave.EnemyCount; ++n)
                {
                    // Pass all required arguments to the Wave constructor
                    var individualPath = new Wave(
                        wave.Segments,
                        wave.Speed,
                        wave.DelayBetweenEnemies,
                        wave.EnemyCount,
                        wave.HitsToKill,
                        wave.NextWaveName
                    );

                    var enemy = new Enemy(_enemyTexture, individualPath, n);
                    _enemies.Add(enemy);
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

            // Logic for Next Wave
            if (_enemies.Count == 0 && _waveLibrary != null && _currentWaveName != null)
            {
                string next = _waveLibrary[_currentWaveName].NextWaveName;

                if (!string.IsNullOrWhiteSpace(next) && _waveLibrary.ContainsKey(next))
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