namespace CivitAI_Grabber.Models
{
    /// <summary>A generated preview image for the model.</summary>
    public class Image
    {
        public string Url { get; set; } = "";
        public string NSFW { get; set; } = "";
        public uint Width { get; set; } = 0;  
        public uint Height { get; set; } = 0;
        public Meta Meta { get; set; } = new Meta ();
    }
}
