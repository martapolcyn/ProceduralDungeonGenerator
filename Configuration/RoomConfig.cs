using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProceduralDungeonGenerator.Model.Structure;

namespace ProceduralDungeonGenerator.Configuration
{
    public class RoomConfig
    {
        public required string RoomID { get; set; }
        public RoomType Type { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }
        public RoomSize Size { get; set; }
        public RoomShape Shape { get; set; }
        public int MinEnemies { get; set; }
        public int MaxEnemies { get; set; }
        public int MinArtifacts { get; set; }
        public int MaxArtifacts { get; set; }
        public required string Style { get; set; }

        public static List<RoomConfig> LoadFromCsv(string path)
        {
            var roomConfigs = new List<RoomConfig>();

            if (!File.Exists(path))
                throw new FileNotFoundException($"Configuration file not found: {path}");

            var lines = File.ReadAllLines(path);

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(';');

                if (parts.Length != 11)
                    throw new FormatException($"Invalid number of columns in row: {line}");

                try
                {
                    var config = new RoomConfig
                    {
                        RoomID = parts[0].Trim(),
                        Style = parts[1].Trim(),
                        Type = Enum.Parse<RoomType>(parts[2].Trim(), true),
                        MinCount = int.Parse(parts[3].Trim()),
                        MaxCount = int.Parse(parts[4].Trim()),
                        Size = Enum.Parse<RoomSize>(parts[5].Trim(), true),
                        Shape = Enum.Parse<RoomShape>(parts[6].Trim(), true),
                        MinEnemies = int.Parse(parts[7].Trim()),
                        MaxEnemies = int.Parse(parts[8].Trim()),
                        MinArtifacts = int.Parse(parts[9].Trim()),
                        MaxArtifacts = int.Parse(parts[10].Trim())
                    };

                    roomConfigs.Add(config);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while processing configuration file:  {line} \nDetails: {ex.Message}");
                }
            }

            return roomConfigs;
        }
    }
}
