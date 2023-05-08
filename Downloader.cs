// Copyright (c) 2023 James Johnson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using CivitAI_Grabber.Models;
using System.Text;

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
            WriteInfoFile (model, downloadLink, downloadDirectory);
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

        // Download image previews associated with the given model version.
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

        // Write the info file about the download to the directory given.
        public static void WriteInfoFile (Model model, DownloadLink downloadLink, string downloadDirectory)
        {
            var modelVersion = model.GetModelVersion (downloadLink.ModelVersionId);
            if (modelVersion == null)
                modelVersion = new ModelVersion ();

            var infoBuilder = new StringBuilder ();
            infoBuilder.AppendLine ($"Model Url: {downloadLink.Url}");
            infoBuilder.AppendLine ($"Base Model: {modelVersion.BaseModel}");
            infoBuilder.AppendLine ($"Keywords: {string.Join (", ", modelVersion.TrainedWords)}");
            infoBuilder.AppendLine ($"--------DESCRIPTION---------");
            infoBuilder.AppendLine ();
            infoBuilder.AppendLine (WebUtility.RemoveHTML (model.Description));
            infoBuilder.AppendLine ($"----------------------------");
            infoBuilder.AppendLine ();
            infoBuilder.AppendLine ($"-----ABOUT THIS VERSION-----");
            infoBuilder.AppendLine (WebUtility.RemoveHTML (modelVersion.Description));
            infoBuilder.AppendLine ($"----------------------------");
            infoBuilder.AppendLine ();
            infoBuilder.AppendLine ($"------PREVIEWS METADATA-----");
            foreach (var image in modelVersion.Images)
                infoBuilder.AppendLine (image.Meta.ToString ());
            infoBuilder.AppendLine ($"----------------------------");
            infoBuilder.AppendLine ();
            infoBuilder.AppendLine ($"------ENTIRE LINK JSON------");
            infoBuilder.AppendLine (model.Serialise ());
            infoBuilder.AppendLine ($"----------------------------");

            var modelFile = modelVersion.GetModelFile ();
            if (modelFile == null)
                modelFile = new ModelFile ();

            string filePath = downloadDirectory + modelFile.Name + "-info.txt";
            using (FileStream fs = File.Create (filePath))
            {
                byte[] infoBytes = new UTF8Encoding (true).GetBytes (infoBuilder.ToString ());
                fs.Write (infoBytes, 0, infoBytes.Length);
            }
        }
    }
}
