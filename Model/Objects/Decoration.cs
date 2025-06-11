using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralDungeonGenerator.Model.Structure;

namespace ProceduralDungeonGenerator.Model.Objects
{
    public class Decoration : Item
    {
        public override ItemCategory Category => ItemCategory.Decoration;

        public Decoration(string id, string style, string name, RoomType roomType, PlacementType placement, int weight)
            : base(id, style, name, roomType, placement, weight)
        {
        }
    }

}
