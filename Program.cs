using System.Text;

namespace CivitAI_Grabber
{
    internal class Program
    {
        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger ();

        static void Main (string[] args)
        {
            InitialiseNlog ();
            var config = Config.Load (@"config.json");
            if (config.IsValid () == false)
            {
                _Logger.Error ("Invalid config directories set.\nPlease ensure valid directories are assigned before using this application.");
                Terminate ();
            }

            var links = DownloadLink.LoadLinks (@"links.txt");
            if (links.Count <= 0)
            {
                _Logger.Error ("No valid download links provided.\nPlease provide valid links to download from before using this application.");
                Terminate ();
            }

            Console.ReadKey ();
        }

        private static void InitialiseNlog ()
        {
            const string messageFormat = "[${level:uppercase=true}][${longdate}] ${logger}\n${message:exceptionSeparator=\n:withexception=true}";
            var config = new NLog.Config.LoggingConfiguration ();
            var logFile = new NLog.Targets.FileTarget ("logfile")
            {
                FileName = "logfile.txt",
                DeleteOldFileOnStartup = true,
                Encoding = Encoding.UTF8,
                Layout = NLog.Layouts.Layout.FromString (messageFormat)
            };
            var logConsole = new NLog.Targets.ColoredConsoleTarget ("logconsole")
            {
                Layout = NLog.Layouts.Layout.FromString (messageFormat)
            };

            config.AddRule (NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logConsole);
            config.AddRule (NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logFile);
            NLog.LogManager.Configuration = config;
        }

        public static void Terminate ()
        {
            Console.WriteLine ("\nExecution stopped consult the logfile for more info.\nPress any key to close.");
            Console.ReadKey ();
            Environment.Exit (0);
        }
    }
}