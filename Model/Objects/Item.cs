using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralDungeonGenerator.Model.Structure;

namespace ProceduralDungeonGenerator.Model.Objects
{
    public enum ItemCategory
    {
        Furniture,
        Architecture,
        Decoration
    }

    public enum PlacementType
    {
        Any,
        Wall,
        CorridorStart
    }

    public abstract class Item
    {
        public string Id { get; set; }
        public string Style { get; set; }
        public string Name { get; set; }
        public RoomType RoomType { get; set; }
        public PlacementType Placement { get; set; }
        public int Weight { get; set; }
        public Point? Position { get; set; }

        public abstract ItemCategory Category { get; }

        protected Item(string id, string style, string name, RoomType roomType, PlacementType placement, int weight)
        {
            Id = id;
            Style = style;
            Name = name;
            RoomType = roomType;
            Placement = placement;
            Weight = weight;
        }

        public override string ToString()
        {
            return $"Item: Category={Category}, Name={Name}, Position=({Position})";
        }
    }
}
