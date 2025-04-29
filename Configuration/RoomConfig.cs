using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public static List<RoomConfig> LoadFromCsv(string path)
        {
            var roomConfigs = new List<RoomConfig>();

            if (!File.Exists(path))
                throw new FileNotFoundException($"Plik konfiguracyjny nie został znaleziony: {path}");

            var lines = File.ReadAllLines(path);

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');

                if (parts.Length != 8)
                    throw new FormatException($"Nieprawidłowa liczba kolumn w wierszu: {line}");

                try
                {
                    var config = new RoomConfig
                    {
                        RoomID = parts[0].Trim(),
                        Type = Enum.Parse<RoomType>(parts[1].Trim(), true),
                        MinCount = int.Parse(parts[2].Trim()),
                        MaxCount = int.Parse(parts[3].Trim()),
                        Size = Enum.Parse<RoomSize>(parts[4].Trim(), true),
                        Shape = Enum.Parse<RoomShape>(parts[5].Trim(), true),
                        MinEnemies = int.Parse(parts[6].Trim()),
                        MaxEnemies = int.Parse(parts[7].Trim())
                    };

                    roomConfigs.Add(config);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Błąd przetwarzania wiersza: {line}\nSzczegóły: {ex.Message}");
                }
            }

            return roomConfigs;
        }
    }
}
