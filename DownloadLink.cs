using NLog.Config;
using System.Text;

namespace CivitAI_Grabber
{
    /// <summary>Represents a download link and it's parsed directories.</summary>
    public class DownloadLink
    {
        /// <summary>The CivitAI URL to download data from.</summary>
        public string Url { get; set; } = "";
        /// <summary>Should the URL be skipped from being downloaded?</summary>
        public bool ShouldSkip { get; set; } = false;
        /// <summary>The folder hierarchy to store the downloaded data into.</summary>
        public List<string> Directories { get; set; } = new List<string>();

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

            //TODO: Validate link.
            // Add final token as it should be the download link.
            link.Url = tokens[^1];
            return true;
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
