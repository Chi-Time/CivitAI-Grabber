namespace CivitAI_Grabber.Models
{
    /// <summary>A version of the model.</summary>
    public class ModelVersion
    {
        public int Id { get; set; } = -1;
        public int ModelId { get; set; } = -1;
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string BaseModel { get; set; } = "";
        public string Description { get; set; } = "";
        public string DownloadUrl { get; set; } = "";
        public string[] TrainedWords { get; set; } = new string[0];
        public List<ModelFile> Files { get; set; } = new ();
        public List<Image> Images { get; set; } = new ();
    }
}
