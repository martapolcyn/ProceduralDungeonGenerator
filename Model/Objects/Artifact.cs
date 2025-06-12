using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ProceduralDungeonGenerator.Model.Objects
{
    public enum ArtifactName
    {
        // Dungeon artifacts
        Coin,
        Sword,
        Elixir,

        // Spaceship artifacts
        Diamond,

        // Cave artifacts
        Crystal,
        AncientRelic,
        HiddenMap,
        MysteriousFungus,
        CavePearl
    }


    public class Artifact
    {
        public ArtifactName Name { get; private set; }

        public Point? Position { get; set; }

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
