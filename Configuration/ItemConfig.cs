using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralDungeonGenerator.Model.Objects;
using ProceduralDungeonGenerator.Model.Structure;

namespace ProceduralDungeonGenerator.Configuration
{
    public class ItemConfig
    {
        public required string ItemID { get; set; }
        public string Style { get; set; }
        public ItemCategory Category { get; set; }
        public string Name { get; set; }
        public RoomType RoomType { get; set; }
        public PlacementType Placement { get; set; }
        public int Weight { get; set; }

        public static List<ItemConfig> LoadFromCsv(string path)
        {
            var itemConfigs = new List<ItemConfig>();

            if (!File.Exists(path))
                throw new FileNotFoundException($"Plik konfiguracyjny nie został znaleziony: {path}");

            var lines = File.ReadAllLines(path);

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) 
                    continue;

                var parts = line.Split(';');

                if (parts.Length != 7)
                    throw new FormatException($"Nieprawidłowa liczba kolumn w wierszu: {line}");

                try
                {
                    var config = new ItemConfig()
                    {
                        ItemID = parts[0].Trim(),
                        Style = parts[1].Trim(),
                        Category = Enum.Parse<ItemCategory>(parts[2].Trim()),
                        Name = parts[3].Trim(),
                        RoomType = Enum.Parse<RoomType>(parts[4].Trim(), true),
                        Placement = Enum.Parse<PlacementType>(parts[5].Trim(), true),
                        Weight = int.Parse(parts[6].Trim())
                    };

                    itemConfigs.Add(config);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Błąd przetwarzania wiersza: {line}\nSzczegóły: {ex.Message}");
                }
                }

                return itemConfigs;
        }
    }

}
