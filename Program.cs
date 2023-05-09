// Copyright (c) 2023 James Johnson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using CivitAI_Grabber.Models;
using System.Text;

namespace CivitAI_Grabber
{
    internal class Program
    {
        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger ();
        private static readonly List<(DownloadResult Result, string Url)> _Results = new ();

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

            foreach (var link in links)
            {
                if (link.ShouldSkip)
                    continue;

                string modelId = link.Url.Replace ("https://civitai.com/models/", "");
                modelId = modelId.Split ("/")[0];

                var json = WebUtility.GetJSON ("https://civitai.com/api/v1/models/" + modelId);
                if (string.IsNullOrWhiteSpace (json))
                {
                    Console.WriteLine ($"Failed to retrieve JSON data from {link.Url}");
                    _Results.Add ((DownloadResult.Failed, link.Url));
                    continue;
                }

                var model = Model.Deserialise (json);
                if (model == null)
                {
                    Console.WriteLine ($"Failed to deserialize JSON data from {link.Url}");
                    _Results.Add ((DownloadResult.Failed, link.Url));
                    continue;
                }

                model!.Description = WebUtility.RemoveHTML (model.Description);

                Console.WriteLine ($"Downloading: {model.Name} - From Url: {link.Url}");
                DownloadResult result = Downloader.DownloadModel (model, link, config);
                _Results.Add ((result, link.Url));

                switch (result)
                {
                    case DownloadResult.Success:
                        Console.WriteLine ("Model File Downloaded.");
                        break;
                    case DownloadResult.Failed:
                        Console.WriteLine ("Model File Failed.");
                        break;
                    case DownloadResult.Exists:
                        Console.WriteLine ("Model File Already Saved.");
                        break;
                    default:
                        break;
                }

                Console.WriteLine ($"{model.Name} Downloaded.\n");
            }

            WriteResults ();
            Console.ReadKey ();
        }

        private static void InitialiseNlog ()
        {
            const string seperator = "\n====================\n";
            const string messageFormat = seperator + "[${level:uppercase=true}]\n[${longdate}] ${logger}\n${message:exceptionSeparator=\n:withexception=true}" + seperator;
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

        private static void WriteResults ()
        {
            var failures = _Results.FindAll (x => x.Result == DownloadResult.Failed);
            var sucesses = _Results.FindAll (x => x.Result == DownloadResult.Success);
            var existers = _Results.FindAll (x => x.Result == DownloadResult.Exists);

            StringBuilder message = new StringBuilder ();
            foreach (var failure in failures)
                message.AppendLine ($"\tModel file: {failure.Url} failed");
            _Logger.Info ("Failures:\n" + message.ToString ());

            message.Clear ();
            foreach (var success in sucesses)
                message.Append ($"\tModel file: {success.Url} downloaded.");
            _Logger.Info ("Successes:\n" + message.ToString ());

            message.Clear ();
            foreach (var exists in existers)
                message.Append ($"\tModel file: {exists.Url} already exists.");
            _Logger.Info ("Exists:\n" + message.ToString ());
        }

        public static void Terminate ()
        {
            Console.WriteLine ("\nExecution stopped consult the logfile for more info.\nPress any key to close.");
            Console.ReadKey ();
            Environment.Exit (0);
        }
    }
}