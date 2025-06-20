using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProceduralDungeonGenerator.Model.Objects;

namespace ProceduralDungeonGenerator.Configuration
{
    public class EnemyConfig
    {
        public required string EnemyID { get; set; }
        public required EnemyType Type{ get; set; }
        public int Weight { get; set; }
        public required string Style { get; set; }

        public static List<EnemyConfig> LoadFromCsv(string path)
        {
            var enemyConfigs = new List<EnemyConfig>();

            if (!File.Exists(path))
                throw new FileNotFoundException($"Configuration file does not exist: {path}");

            var lines = File.ReadAllLines(path);

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(';');

                if (parts.Length != 4)
                    throw new FormatException($"Invalid number of columns in a row : {line}");

                try
                {
                    var config = new EnemyConfig
                    {
                        EnemyID = parts[0].Trim(),
                        Style = parts[1].Trim(),
                        Type = Enum.Parse<EnemyType>(parts[2].Trim()),
                        Weight = int.Parse(parts[3].Trim())
                    };

                    enemyConfigs.Add(config);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while processing configuration file: {line}\nDetails: {ex.Message}");
                }
            }

            return enemyConfigs;
        }
    }
}
