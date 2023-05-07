// Copyright (c) 2023 James Johnson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using CivitAI_Grabber.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CivitAI_Grabber.Models
{
    /// <summary>Base representation of a CivitAI Model.</summary>
    public class Model
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";
        public bool POI { get; set; } = false;
        public bool NSFW { get; set; } = false;
        public Tag[] Tags { get; set; } = new Tag[0];
        public Dictionary<string, string> Creator { get; set; } = new ();
        public List<ModelVersion> ModelVersions { get; set; } = new ();

        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger ();

        /// <summary>Serialise the current <see cref="Model"/> instance to a json string.</summary>
        /// <param name="prettyPrint">Should the json string be formatted and readable?</param>
        /// <returns>A json string of the current <see cref="Model"/> instance.</returns>
        public string Serialise (bool prettyPrint = true)
        {
            try
            {
                var options = new JsonSerializerOptions ()
                {
                    WriteIndented = prettyPrint,
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
                };

                options.Converters.Add (new NullToEmptyStringConverter ());
                return JsonSerializer.Serialize (this, options);
            }
            catch (Exception e)
            {
                _Logger.Error (e, "Unable to serialise model file.");
                return "";
            }
        }

        /// <summary>Deserialise the given json string to a <see cref="Model"/> instance.</summary>
        /// <param name="json">The json string to deserialise from.</param>
        /// <returns>A <see cref="Model"/> instance if successful. <see langword="null"/> if failed.</returns>
        public static Model? Deserialise (string json)
        {
            try
            {
                var options = new JsonSerializerOptions ()
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true,
                    UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
                };

                //TODO: Switch back to Newtonsoft eventually as we can't populate null fields automatically. Awesome
                //https://stackoverflow.com/questions/72661011/system-text-json-defaultvaluehandling-defaultvaluehandling-ignore-alternativ
                options.Converters.Add (new NullToEmptyStringConverter ());
                return JsonSerializer.Deserialize<Model> (json, options);
            }
            catch (Exception e)
            {
                _Logger.Warn (e, "Could not deserialise model from provided json.");
                return null;
            }
        }

        /// <summary>Retrieves the model version matching the id if one exists.</summary>
        /// <param name="versionID">The id of the model version to find.</param>
        /// <returns>A <see cref="ModelVersion"/> instance if one matches. <see langword="null"/> if none found.</returns>
        public ModelVersion? GetModelVersion (int versionID = -1)
        {
            if (versionID == -1)
                return GetMostRecent ();

            return ModelVersions.Find (x => x.Id == versionID);
        }

        /// <summary>Retrieves the most recently added model version.</summary>
        /// <returns>The most recent <see cref="ModelVersion"/> instance if one exists. <see langword="null"/> if none found.</returns>
        public ModelVersion? GetMostRecent ()
        {
            if (ModelVersions.Count <= 0)
                return null;

            if (ModelVersions.Count == 1)
                return ModelVersions[0];

            var recentVersions = ModelVersions.OrderByDescending (x => x.CreatedAt).ToList ();
            return recentVersions[0];
        }
    }
}
