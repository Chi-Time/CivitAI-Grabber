using System.Text.Json.Serialization;

namespace CivitAI_Grabber.Models
{
    /// <summary>Generation parameters of the models image.</summary>
    public class Meta
    {
        public string Size { get; set; } = "";
        public string Model { get; set; } = "";
        public int Steps { get; set; } = 0;
        public Dictionary<string, string> Hashes { get; set; } = new ();
        public string Prompt { get; set; } = "";
        public string NegativePrompt { get; set; } = "";
        public string Sampler { get; set; } = "";
        public float CFGScale { get; set; } = 0.0f;
        // API name requires space
        // Allow reading from string as "Clip skip" value is a string value for some weird reason.
        [JsonPropertyName ("Clip skip")]
        [JsonNumberHandling (JsonNumberHandling.AllowReadingFromString)]
        public int ClipSkip { get; set; }
    }
}
