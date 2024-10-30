<<<<<<< HEAD
using ModBusMaster;

namespace ModBusSimMaster
=======
namespace ModBusMaster
>>>>>>> 5eacb85e602b94055b4f4ae2c1f4d40e591d0c2a
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}