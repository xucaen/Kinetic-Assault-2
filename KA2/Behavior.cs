using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KA2
{
    public class Behavior
    {
        public Queue<Vector2> PathPoints { get; private set; }
        public bool IsFinished => PathPoints.Count == 0;

        public Behavior(List<Vector2> points)
        {
            PathPoints = new Queue<Vector2>(points);
        }

        public Vector2 GetNextTarget() => PathPoints.Peek();
        public void ReachTarget() => PathPoints.Dequeue();
    }
}
