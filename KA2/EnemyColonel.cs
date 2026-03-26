using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace KA2
{
    public class EnemyColonel
    {
        public List<Enemy> Parts = new List<Enemy>();
        public Vector2 Position = new Vector2(320, 150); // Default boss staging area

        // This is the constructor the error is asking for
        public EnemyColonel(Wave wave)
        {
            // Here is where you "finish" the code by turning 
            // the Wave's BossParts into actual Enemy objects
            foreach (var partDef in wave.BossParts)
            {
                // We treat each boss part as a specialized 'Enemy'
                // You will need to load the specific texture from partDef.TextureName
                // and assign the partDef.Offset to the enemy's position logic.
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var part in Parts)
            {
                part.Update(gameTime);
                // Synchronize parts to the boss position
                part.Position = this.Position + part.Offset;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var part in Parts)
            {
                part.Draw(spriteBatch);
            }
        }
    }
}