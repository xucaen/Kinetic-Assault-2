using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KA2
{
    public class Wave
    {
        public List<Vector2> PathPoints { get; private set; }
        public bool IsFinished => PathPoints.Count == 0;

        public float Speed { get; private set; }
        public float DelayBetweenEnemies { get; private set; }
        public int EnemyCount { get; private set; }
        public string NextWaveName { get; private set; } // Added this

        public int HitsToKill { get; private set; } = 1; // Default to 1
        public Wave(List<Vector2> points, float speed, float delay, int count, int hitsToKill = 1, string nextWave = null)
        {
            PathPoints = new List<Vector2>(points);
            Speed = speed;
            DelayBetweenEnemies = delay;
            EnemyCount = count;
            HitsToKill = hitsToKill;
            NextWaveName = nextWave;
        }

        public static Dictionary<string, Wave> LoadBehaviorLibrary(string filePath)
        {
            var library = new Dictionary<string, Wave>();
            if (!File.Exists(filePath)) return library;

            string[] lines = File.ReadAllLines(filePath);
            Wave currentWave = null;
            string currentSection = "";
            bool inPathBlock = false;

            foreach (var line in lines)
            {
                string trimmed = line.Split('#')[0].Trim(); // Remove comments
                if (string.IsNullOrEmpty(trimmed)) continue;

                // Section Header: [Swoopers]
                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    currentSection = trimmed.Substring(1, trimmed.Length - 2);
                    // Create a placeholder Wave - we'll fill data as we go
                    currentWave = new Wave(new List<Vector2>(), 0, 0, 0,0, null);
                    library[currentSection] = currentWave;
                    inPathBlock = false;
                    continue;
                }

                if (currentWave == null) continue;

                // Metadata Parsing
                if (trimmed.Contains("="))
                {
                    var parts = trimmed.Split('=');
                    string key = parts[0].Trim().ToLower();
                    string val = parts[1].Trim();

                    switch (key)
                    {
                        case "count": currentWave.EnemyCount = int.Parse(val); break;
                        case "next": currentWave.NextWaveName = val; break;
                        // Strip 'ms' from speed/delay
                        case "speed": currentWave.Speed = float.Parse(val.Replace("ms", "")); break;
                        case "delay": currentWave.DelayBetweenEnemies = float.Parse(val.Replace("ms", "")); break;
                        case "hits": currentWave.HitsToKill = int.Parse(val); break;
                    }
                }

                // Path Parsing
                if (trimmed == "paths") { inPathBlock = true; continue; }
                if (trimmed == "}") { inPathBlock = false; continue; }

                if (inPathBlock)
                {
                    // Extract all [X,Y] patterns from the line
                    string[] pairs = trimmed.Split(new[] { "],[" }, StringSplitOptions.None);
                    foreach (var p in pairs)
                    {
                        var clean = p.Replace("[", "").Replace("]", "").Split(':').Last(); // Handle 'a: [x,y]'
                        var coords = clean.Split(',');
                        if (coords.Length == 2)
                        {
                            currentWave.PathPoints.Add(new Vector2(float.Parse(coords[0]), float.Parse(coords[1])));
                        }
                    }
                }
            }
            return library;
        }

    }
}