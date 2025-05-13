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

    public class Item
    {
        public string Id { get; set; }
        public string Style { get; set; }
        public ItemCategory Category { get; set; }
        public string Name { get; set; }
        public string RoomType { get; set; }
        public string Placement { get; set; }
        public int Weight { get; set; }

        public Item(string id, string style, ItemCategory category, string name, string roomType, string placement, int weight)
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
