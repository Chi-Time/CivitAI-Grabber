using System.Text;
using System.Text.Json.Serialization;

namespace CivitAI_Grabber.Models
{
    /// <summary>Generation parameters of the models image.</summary>
    public class Meta
    {
        [JsonNumberHandling (JsonNumberHandling.AllowReadingFromString)]
        public uint ENSD { get; set; } = 0;
        public string Size { get; set; } = "";
        public ulong Seed { get; set; } = 0L;
        public string Model { get; set; } = "";
        public int Steps { get; set; } = 0;
        public Dictionary<string, string> Hashes { get; set; } = new ();
        public string Prompt { get; set; } = "";
        public string Sampler { get; set; } = "";
        public float CFGScale { get; set; } = 0.0f;
        // API name requires space for some of these.
        // Allow reading from string as number values are a string value for some weird reason.
        [JsonPropertyName ("Clip skip")]
        [JsonNumberHandling (JsonNumberHandling.AllowReadingFromString)]
        public int ClipSkip { get; set; } = 0;
        [JsonPropertyName ("Model hash")]
        public string ModelHash { get; set; } = "";
        [JsonPropertyName ("Hires steps")]
        [JsonNumberHandling (JsonNumberHandling.AllowReadingFromString)]
        public int HiresSteps { get; set; } = 0;
        [JsonPropertyName ("Hires pscale")]
        [JsonNumberHandling (JsonNumberHandling.AllowReadingFromString)]
        public int HiresUpscale { get; set; } = 0;
        [JsonPropertyName ("Hires upscaler")]
        public string HiresUpscaler { get; set; } = "";
        public string NegativePrompt { get; set; } = "";
        [JsonPropertyName ("Denoising strength")]
        [JsonNumberHandling (JsonNumberHandling.AllowReadingFromString)]
        public float DenoisingStrength { get; set; } = 0.0f

        public override string ToString ()
        {
            StringBuilder metaBuilder = new ();
            metaBuilder.AppendLine ("Prompt:");
            metaBuilder.AppendLine ($"\t{Prompt}");
            metaBuilder.AppendLine ("Negative Prompt:");
            metaBuilder.AppendLine ($"\t{NegativePrompt}");
            metaBuilder.AppendLine ("Settings:");
            metaBuilder.AppendLine ($"\tENSD: {ENSD}");
            metaBuilder.AppendLine ($"\tSize: {Size}");
            metaBuilder.AppendLine ($"\tSeed: {Seed}");
            metaBuilder.AppendLine ($"\tModel: {Model}");
            metaBuilder.AppendLine ($"\tSteps: {Steps}");
            metaBuilder.AppendLine ($"\tSampler: {Sampler}");
            metaBuilder.AppendLine ($"\tCFG Scale: {CFGScale}");
            metaBuilder.AppendLine ($"\tClip Skip: {ClipSkip}");
            metaBuilder.AppendLine ($"\tModel Hash: {ModelHash}");
            metaBuilder.AppendLine ($"\tHires Steps: {HiresSteps}");
            metaBuilder.AppendLine ($"\tHires Upscale: {HiresUpscale}");
            metaBuilder.AppendLine ($"\tHires Upscaler: {HiresUpscaler}");
            metaBuilder.AppendLine ($"\tDenoising Strength: {DenoisingStrength}");

            return metaBuilder.ToString ();
        }
    }
}
