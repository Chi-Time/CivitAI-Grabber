// Copyright (c) 2023 James Johnson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System.Text;

namespace CivitAI_Grabber
{
    /// <summary>Represents a download link and it's parsed directories.</summary>
    public class DownloadLink
    {
        /// <summary>The CivitAI URL to download data from.</summary>
        public string Url { get; set; } = "";
        /// <summary>The model version Id number if one exists.</summary>
        public int ModelVersionId { get; set; } = -1;
        /// <summary>Should the URL be skipped from being downloaded?</summary>
        public bool ShouldSkip { get; set; } = false;
        /// <summary>The folder hierarchy to store the downloaded data into.</summary>
        public List<string> Directories { get; set; } = new List<string>();

        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger ();

        /// <summary>Parses and loads the download links from the provided file.</summary>
        /// <param name="filePath">The file path to read from.</param>
        /// <returns>A list of parsed download links.</returns>
        public static List<DownloadLink> LoadLinks (string filePath)
        {
            if (File.Exists (filePath) == false)
            {
                _Logger.Error ($"Could not find links.txt at expected location: {filePath}\nPlease ensure a links.txt file exists with links provided to download.");
                Program.Terminate ();
            }

            var links = new List<DownloadLink> ();
            using (var sr = new StreamReader (filePath))
            {
                string? line = "";

                while (( line = sr.ReadLine () ) != null)
                    if (DownloadLink.TryParse (line, out DownloadLink link))
                        links.Add (link);
            }

            return links;
        }

        /// <summary>Try and parse the given line as a <see cref="DownloadLink"/> instance.</summary>
        /// <param name="line">The line to parse.</param>
        /// <param name="link">The <see cref="DownloadLink"/> instance to fill.</param>
        /// <returns><see langword="true"/> if parse was successful. <see langword="false"/> if failed.</returns>
        public static bool TryParse (string line, out DownloadLink link)
        {
            link = new DownloadLink ();
            if (string.IsNullOrWhiteSpace (line))
                return false;

            // Split the given line and parse the tokens.
            string[] tokens = line.Split (" ");
            for (int i = 0; i < tokens.Length - 1; i++)
            {
                // Check for skip identifier.
                if (tokens[i].StartsWith ("#"))
                {
                    link.ShouldSkip = true;
                    continue;
                }

                link.Directories.Add (tokens[i]);
            }

            // Validate link.
            if (tokens[^1].Contains ("https://civitai.com/") == false)
                return false;

            // Add final token as the url and parse any id if available.
            link.Url = tokens[^1];
            link.ModelVersionId = ExtractModelVersionId (link.Url);
            return true;
        }

        private static int ExtractModelVersionId (string url)
        {
            // Look for model version specifier in url.
            if (url.Contains ("modelVersionId="))
            {
                string[] tokens = url.Split ("=");

                if (int.TryParse (tokens[^1], out int result))
                    return result;
            }

            // If parsing failed or not found, return skip id.
            return -1;
        }

        /// <summary>Constructs a directory path from the links stored directories and the given root and subfolder if one is provided.</summary>
        /// <param name="root">The base root to start from.</param>
        /// <param name="subFolder">The subfolder to place at the end of the directory path.</param>
        /// <returns>A formatted directory path.</returns>
        public string GetDirectoryPath(string root = "", string subFolder = "")
        {
            StringBuilder directoryBuilder = new();
            directoryBuilder.Append(root + Path.DirectorySeparatorChar);
            foreach (var directory in Directories)
                directoryBuilder.Append(directory + Path.DirectorySeparatorChar);

            directoryBuilder.Append(subFolder);
            return directoryBuilder.ToString();
        }
    }
}
