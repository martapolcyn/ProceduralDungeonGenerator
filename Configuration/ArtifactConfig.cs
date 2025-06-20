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
                throw new FileNotFoundException($"Config file not found: {path}");

            var lines = File.ReadAllLines(path);

            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(';');

                if (parts.Length != 4)
                    throw new FormatException($"Incorrect column count: {line}");

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
                    throw new Exception($"Error while processing configuration file: {line}\nDetails: {ex.Message}");
                }

            }

            return artifactConfigs;
        }
    }
}
