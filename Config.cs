using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralDungeonGenerator.Configuration;

namespace ProceduralDungeonGenerator
{
    public static class Config
    {

        // general configuration
        public static int dungeonWidth = 1200;
        public static int dungeonHeight = 900;

        // configuration from files
        public static List<RoomConfig> RoomConfigs { get; private set; } = new List<RoomConfig>();
        public static List<EnemyConfig> EnemyConfigs { get; private set; } = new List<EnemyConfig>();

        public static void LoadAllConfigs()
        {
            RoomConfigs = RoomConfig.LoadFromCsv(@"Resources\config_room.csv");
            EnemyConfigs = EnemyConfig.LoadFromCsv(@"Resources\config_enemy.csv");
        }
    }
}
