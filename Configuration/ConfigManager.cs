using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Configuration
{
    public enum DungeonStyle
    {
        Dungeon,
        Spaceship
    }

    public static class ConfigManager
    {

        // general configuration
        public static int dungeonWidth = 1200;
        public static int dungeonHeight = 900;

        // configuration from files
        public static List<RoomConfig> RoomConfigs { get; private set; } = new List<RoomConfig>();
        public static List<EnemyConfig> EnemyConfigs { get; private set; } = new List<EnemyConfig>();
        public static List<ArtifactConfig> ArtifactConfigs { get; private set; } = new List<ArtifactConfig>();
        public static List<ItemConfig> ItemConfigs { get; private set; } = new List<ItemConfig>();

        public static void LoadAllConfigs(string style)
        {
            RoomConfigs = RoomConfig.LoadFromCsv(@"Resources\config_room.csv")
                .Where(r => r.Style.Equals(style, StringComparison.OrdinalIgnoreCase)).ToList();
            EnemyConfigs = EnemyConfig.LoadFromCsv(@"Resources\config_enemy.csv")
                .Where(e => e.Style.Equals(style, StringComparison.OrdinalIgnoreCase)).ToList();
            ArtifactConfigs = ArtifactConfig.LoadFromCsv(@"Resources\config_artifact.csv")
                .Where(a => a.Style.Equals(style, StringComparison.OrdinalIgnoreCase)).ToList();
            ItemConfigs = ItemConfig.LoadFromCsv(@"Resources\config_item.csv")
                .Where(a => a.Style.Equals(style, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
