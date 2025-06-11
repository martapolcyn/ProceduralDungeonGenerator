using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralDungeonGenerator.Model.Structure;

namespace ProceduralDungeonGenerator.Model.Objects
{
    public class ArchitecturalItem : Item
    {
        public override ItemCategory Category => ItemCategory.Architecture;

        public ArchitecturalItem(string id, string style, string name, RoomType roomType, PlacementType placement, int weight)
            : base(id, style, name, roomType, placement, weight)
        {
        }
    }
}
