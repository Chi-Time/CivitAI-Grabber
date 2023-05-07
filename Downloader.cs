﻿using CivitAI_Grabber.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

            DownloadPreviews (version, downloadDirectory);
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

        private static void DownloadPreviews (ModelVersion modelVersion, string downloadDirectory)
        {
            var modelFile = modelVersion.GetModelFile ();
            if (modelFile == null)
                return;

            StringBuilder failMessage = new ();
            for (int i = 0; i < modelVersion.Images.Count; i++)
            {
                var currentImage = modelVersion.Images[i];

                if (string.IsNullOrWhiteSpace (currentImage.Url))
                    continue;

                string extension = Path.GetExtension (currentImage.Url);
                string filePath = downloadDirectory + Path.GetFileNameWithoutExtension (modelFile.Name);

                // Format first found preview for Auto1111 previews.
                if (i == 0)
                    filePath += ".preview" + extension;
                else
                    filePath += "_" + i.ToString ("D2") + extension;

                if (File.Exists (filePath))
                    continue;

                bool result = WebUtility.DownloadFile (currentImage.Url, filePath);
                if (result == false)
                    failMessage.AppendLine ($"\tUnable to download preview image {currentImage.Url} from model file {modelVersion.Name}.");
            }

            if (failMessage.Length > 0)
                _Logger.Warn (failMessage.ToString ());
        }
    }
}
