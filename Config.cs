using System.Text.Json;
using System.Text.Json.Serialization;

namespace CivitAI_Grabber
{
    public class Config
    {
        [JsonPropertyName ("checkpoint_directory")]
        public string CheckpointDirectory { get; set; } = "";
        [JsonPropertyName ("textual_inversion_directory")]
        public string TextualInversionDirectory { get; set; } = "";
        [JsonPropertyName ("hypernetwork_directory")]
        public string HypernetworkDirectory { get; set; } = "";
        [JsonPropertyName ("lora_directory")]
        public string LoraDirectory { get; set; } = "";
        [JsonPropertyName ("locon_directory")]
        public string LoConDirectory { get; set; } = "";
        [JsonPropertyName ("other_directory")]
        public string OtherDirectory { get; set; } = "";
        [JsonPropertyName ("request_timeout_length")]
        public TimeSpan _RequestTimeoutLength = new TimeSpan (0, 0, 100);

        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger ();

        /// <summary>Loads a <see cref="Config"/> document instance either from a file or auto-gen'd default.</summary>
        /// <param name="filePath">The filepath to load/write the config file from/to.</param>
        /// <returns>A loaded or default <see cref="Config"/> instance.</returns>
        public static Config Load (string filePath)
        {
            // Attempt to read file if one exists.
            if (File.Exists (filePath))
            {
                string json = File.ReadAllText (filePath);
                var config = Config.Deserialise (json);
                if (config != null)
                    return config;

                _Logger.Warn ("Config file could not be parsed.\nGenerating new config with default directories.");
                return GenerateConfigFile (filePath);
            }

            // Inform user no file was found. Create and populate default config.
            _Logger.Warn ("No config file found!\nGenerating new config with default directories.");
            return GenerateConfigFile (filePath);
        }

        /// <summary>Generate a default <see cref="Config"/> instance and write it to a file.</summary>
        /// <param name="filePath">The filepath to write the config instance to.</param>
        /// <returns>A default <see cref="Config"/> instance.</returns>
        public static Config GenerateConfigFile (string filePath)
        {
            string appdataPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
            Config config = new ()
            {
                CheckpointDirectory = appdataPath,
                TextualInversionDirectory = appdataPath,
                HypernetworkDirectory = appdataPath,
                LoraDirectory = appdataPath,
                LoConDirectory = appdataPath,
                OtherDirectory = appdataPath
            };

            File.WriteAllText (filePath, config.Serialise ());
            return config;
        }

        /// <summary>Determines if config directories are valid paths.</summary>
        /// <returns><see langword="true"/> if all directories are valid. <see langword="false"/> if not.</returns>
        public bool IsValid ()
        {
            return Directory.Exists (CheckpointDirectory)
                && Directory.Exists (TextualInversionDirectory)
                && Directory.Exists (HypernetworkDirectory)
                && Directory.Exists (LoraDirectory)
                && Directory.Exists (LoConDirectory)
                && Directory.Exists (OtherDirectory);
        }

        /// <summary>Serialise the current <see cref="Config"/> instance to a json string.</summary>
        /// <param name="prettyPrint">Should the json string be formatted and readable?</param>
        /// <returns>A json string of the current <see cref="Config"/> instance.</returns>
        public string Serialise (bool prettyPrint = true)
        {
            return JsonSerializer.Serialize (this, new JsonSerializerOptions ()
            {
                AllowTrailingCommas = true,
                WriteIndented = prettyPrint
            });
        }

        /// <summary>Deserialise the given json string to a <see cref="Config"/> instance.</summary>
        /// <param name="json">The json string to deserialise from.</param>
        /// <returns>A <see cref="Config"/> instance if successful. <see langword="null"/> if failed.</returns>
        public static Config? Deserialise (string json)
        {
            try
            {
                return JsonSerializer.Deserialize<Config> (json, new JsonSerializerOptions ()
                {
                    AllowTrailingCommas = true,
                    UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                });
            }
            catch (Exception e)
            {
                _Logger.Debug (e, "Could not deserialise config file.");
                return null;
            }   
        }

        /// <summary>Retrieve the download directory for the given model type.</summary>
        /// <param name="modelType">The model type to match the download directory to.</param>
        /// <returns>A download directory based on the model type provided.</returns>
        public string GetDirectoryFromModelType (string modelType)
        {
            modelType = modelType.ToLower ();

            if (modelType.Contains ("checkpoint"))
                return CheckpointDirectory;
            if (modelType.Contains ("textual"))
                return TextualInversionDirectory;
            if (modelType.Contains ("hypernetwork"))
                return HypernetworkDirectory;
            if (modelType.Contains ("lora"))
                return LoraDirectory;
            if (modelType.Contains ("locon"))
                return LoConDirectory;

            return OtherDirectory;
        }
    }
}
