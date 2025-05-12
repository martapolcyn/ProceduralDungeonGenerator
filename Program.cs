using Microsoft.VisualBasic.ApplicationServices;
using ProceduralDungeonGenerator.Configuration;
using ProceduralDungeonGenerator.Model;
using System.Resources;
using System.Security.Cryptography.X509Certificates;

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

            ApplicationConfiguration.Initialize();
            Application.Run(new DungeonForm());
        }
    }
}