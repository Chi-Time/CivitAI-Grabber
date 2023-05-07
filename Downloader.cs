using CivitAI_Grabber.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CivitAI_Grabber
{
    /// <summary>Helper class for downloading models and associated files.</summary>
    public static class Downloader
    {
        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger ();

        /// <summary>Download the model file for the provided model.</summary>
        /// <param name="model">The model to download the file from.</param>
        /// <param name="downloadLink">The download link settings to use.</param>
        /// <param name="config">The config settings to use.</param>
        /// <returns>A <see cref="DownloadResult"/> based on the status of the operation.</returns>
        public static DownloadResult DownloadModel (Model model, DownloadLink downloadLink, Config config)
        {
            // Build directory paths for file download.
            string configDirectory = config.GetDirectoryFromModelType (model.Type);
            string downloadDirectory = downloadLink.GetDirectoryPath (configDirectory);

            // Attempt to create the download path.
            if (CreateDownloadDirectory (downloadDirectory) == false)
                return DownloadResult.Failed;

            ModelVersion? version = model.GetModelVersion (downloadLink.ModelVersionId)!;
            if (version == null)
            {
                _Logger.Warn ("No model version found for model: " + downloadLink.Url);
                return DownloadResult.Failed;
            }

            ModelFile? file = version.GetModelFile ();
            if (file == null)
            {
                _Logger.Warn ("No model file found for model: " + downloadLink.Url);
                return DownloadResult.Failed;
            }

            // Determine if file is already downloaded into path.
            if (File.Exists (downloadDirectory + file.Name))
                return DownloadResult.Exists;

            bool result = WebUtility.DownloadFile (file.DownloadUrl, downloadDirectory + file.Name);
            if (result == false)
                return DownloadResult.Failed;

            return DownloadResult.Success;
        }

        // Create the download directory specified and return status of result.
        private static bool CreateDownloadDirectory (string directory)
        {
            if (Directory.Exists (directory) == false)
            {
                try
                {
                    Directory.CreateDirectory (directory);
                    return true;
                }
                catch (Exception e)
                {
                    _Logger.Error (e, $"Unable to create sub directory path: {directory}.");
                    return false;
                }
            }

            return true;
        }
    }
}
