using Microsoft.VisualBasic.ApplicationServices;
using ProceduralDungeonGenerator.Model;
using System.Resources;

namespace ProceduralDungeonGenerator
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Config.LoadAllConfigs();
            //IDungeonStyle style = new DungeonStyleDungeon();
            IDungeonStyle style = new DungeonStyleSpaceship();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var dungeon = new Dungeon(style);
            dungeon.GenerateDungeon();


            Application.Run(new DungeonForm(dungeon));
        }
    }
}