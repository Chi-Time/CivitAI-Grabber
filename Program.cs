using System.Text;

namespace CivitAI_Grabber
{
    internal class Program
    {
        static void Main (string[] args)
        {
            InitialiseNlog ();
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
    }
}