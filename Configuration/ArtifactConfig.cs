using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralDungeonGenerator.Model.Objects;


namespace ProceduralDungeonGenerator.Configuration
{
    public class ArtifactConfig
    {
        public required string ArtifactID { get; set; }
        public ArtifactName Name { get; set; }
        public int Weight { get; set; }
        public required string Style { get; set; }

        public static List<ArtifactConfig> LoadFromCsv(string path)
        {
            var artifactConfigs = new List<ArtifactConfig>();

            if (!File.Exists(path))
                throw new FileNotFoundException($"Plik konfiguracyjny nie został znaleziony: {path}");

            var lines = File.ReadAllLines(path);

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(';');

                if (parts.Length != 4)
                    throw new FormatException($"Nieprawidłowa liczba kolumn w wierszu: {line}");

                try
                {
                    var config = new ArtifactConfig
                    {
                        ArtifactID = parts[0].Trim(),
                        Style = parts[1].Trim(),
                        Name = Enum.Parse<ArtifactName>(parts[2].Trim()),
                        Weight = int.Parse(parts[3].Trim())
                    };

                    artifactConfigs.Add(config);
                } 
                catch(Exception ex)
                {
                    throw new Exception($"Błąd przetwarzania wiersza: {line}\nSzczegóły: {ex.Message}");
                }

            }

            return artifactConfigs;
        }
    }
}
