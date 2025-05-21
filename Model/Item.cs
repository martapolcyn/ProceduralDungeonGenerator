using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
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

    public class Item
    {
        public string Id { get; set; }
        public string Style { get; set; }
        public ItemCategory Category { get; set; }
        public string Name { get; set; }
        public RoomType RoomType { get; set; }
        public PlacementType Placement { get; set; }
        public int Weight { get; set; }

        public Item(string id, string style, ItemCategory category, string name, RoomType roomType, PlacementType placement, int weight)
        {
            Id = id;
            Style = style;
            Category = category;
            Name = name;
            RoomType = roomType;
            Placement = placement;
            Weight = weight;
        }

        public override string ToString()
        {
            return $"Item: Category={Category}, Name={Name}";
        }
    }

}
