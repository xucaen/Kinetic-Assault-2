using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KA2
{
    public enum PathType { Linear, Curved }

    public class PathSegment
    {
        public List<Vector2> Points = new List<Vector2>();
        public PathType Type;
    }
    public class Wave
    {
        public List<PathSegment> Segments { get; private set; } = new List<PathSegment>();
        public bool IsFinished => Segments.Count == 0;

        public float Speed { get; private set; }
        public float DelayBetweenEnemies { get; private set; }
        public int EnemyCount { get; private set; }
        public string NextWaveName { get; private set; } 

        public int HitsToKill { get; private set; } = 1; 
        public Wave(List<PathSegment> segments, float speed, float delay, int count, int hitsToKill = 1, string nextWave = null)
        {
            Segments = segments;
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
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                // Section Header: [Swoopers]
                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    currentSection = trimmed.Substring(1, trimmed.Length - 2);
                    // Create a placeholder Wave - we'll fill data as we go
                    currentWave = new Wave(new List<PathSegment>(), 0, 0, 0,1, null);
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
                        case "speed": currentWave.Speed = float.Parse(val); break;
                        case "delay": currentWave.DelayBetweenEnemies = float.Parse(val); break;
                        case "hits": currentWave.HitsToKill = int.Parse(val); break;
                    }
                }

                // Path Parsing
                if (trimmed == "paths") { inPathBlock = true; continue; }
                if (trimmed == "}") { inPathBlock = false; continue; }

                if (inPathBlock)
                {
                    // Split into Label and Data: "line: [50,50]..." -> ["line", " [50,50]..."]
                    var parts = trimmed.Split(new[] { ':' }, 2);
                    if (parts.Length < 2) continue;

                    string label = parts[0].Trim().ToLower();
                    string coordData = parts[1].Trim(); // This is the actual [X,Y] string

                    var segment = new PathSegment();
                    segment.Type = label.Contains("path") ? PathType.Curved : PathType.Linear;

                    // Use coordData instead of the original 'trimmed' string
                    string[] pairs = coordData.Split(new[] { "],[" }, StringSplitOptions.None);
                    foreach (var p in pairs)
                    {
                        // Thoroughly scrub everything that isn't a digit, comma, or minus sign
                        var clean = p.Replace("[", "").Replace("]", "").Trim();
                        var coords = clean.Split(',');

                        if (coords.Length == 2)
                        {
                            // Use TryParse to prevent the game from silently failing if data is messy
                            if (float.TryParse(coords[0], out float x) && float.TryParse(coords[1], out float y))
                            {
                                segment.Points.Add(new Vector2(x, y));
                            }
                        }
                    }

                    if (segment.Points.Count > 0)
                    {
                        currentWave.Segments.Add(segment);
                    }
                }
            }
            return library;
        }

    }
}