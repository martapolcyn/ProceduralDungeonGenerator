using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
{
    public enum ArtifactName
    {
        Coin,
        Sword,
        Elixir,
        Diamond
    }

    public class Artifact
    {
        public ArtifactName Name { get; private set; }

        public Artifact(ArtifactName name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"Artifact: {Name}";
        }
    }
}
