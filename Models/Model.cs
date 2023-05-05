using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        /// <summary>Retrieves the model version matching the id if one exists.</summary>
        /// <param name="versionID">The id of the model version to find.</param>
        /// <returns>A <see cref="ModelVersion"/> instance if one matches. <see langword="null"/> if none found.</returns>
        public ModelVersion? GetModelVersion (int versionID = -1)
        {
            if (versionID == -1)
                return GetMostRecentModelVersion ();

            return ModelVersions.Find (x => x.Id == versionID);
        }

        /// <summary>Retrieves the most recently added model version.</summary>
        /// <returns>The most recent <see cref="ModelVersion"/> instance if one exists. <see langword="null"/> if none found.</returns>
        public ModelVersion? GetMostRecentModelVersion ()
        {
            if (ModelVersions.Count <= 0)
                return null;

            if (ModelVersions.Count == 1)
                return ModelVersions[0];

            var recentVersions = ModelVersions.OrderByDescending (x => x.CreatedAt).ToList ();
            return recentVersions[0];
        }

        /// <summary>Deserialise the given json string to a <see cref="Model"/> instance.</summary>
        /// <param name="json">The json string to deserialise from.</param>
        /// <returns>A <see cref="Model"/> instance if successful. <see langword="null"/> if failed.</returns>
        public static Model? Deserialise (string json)
        {
            try
            {
                return JsonSerializer.Deserialize<Model> (json, new JsonSerializerOptions ()
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true,
                    UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
                });
            }
            catch (Exception e)
            {
                _Logger.Warn (e, "Could not deserialise model from provided json.");
                return null;
            }
        }
    }
}
