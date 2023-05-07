// Copyright (c) 2023 James Johnson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
namespace CivitAI_Grabber.Models
{
    /// <summary>A version of the model.</summary>
    public class ModelVersion
    {
        public int Id { get; set; } = -1;
        public int ModelId { get; set; } = -1;
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public DateTime UpdatedAt { get; set; } = DateTime.MinValue;
        public string BaseModel { get; set; } = "";
        public string Description { get; set; } = "";
        public string DownloadUrl { get; set; } = "";
        public string[] TrainedWords { get; set; } = new string[0];
        public List<ModelFile> Files { get; set; } = new ();
        public List<Image> Images { get; set; } = new ();

        /// <summary>Retrieve the first <see cref="ModelFile"/> instance matching a tensor model.</summary>
        /// <returns>The first found <see cref="ModelFile"/> instance.</returns>
        public ModelFile? GetModelFile ()
        {
            return GetModelFile (new string[] { "safetensor", "pickletensor" });
        }

        /// <summary>Retrieve the first <see cref="ModelFile"/> instance matching the formats provided if available.</summary>
        /// <param name="formats">The format types to match with.</param>
        /// <returns>The first found <see cref="ModelFile"/> instance.</returns>
        public ModelFile? GetModelFile (string[] formats)
        {
            if (Files.Count <= 0)
                return null;

            if (Files.Count == 1)
                return Files[0];
            
            //TODO: Offer option to download either fp32 or fp16 through parameter.
            // If more than one file exists then loop through them to find the first instance
            // of a valid format that we want.
            for (int i = 0; i < Files.Count; i++)
            {
                ModelFile current = Files[i];
                for (int j = 0; j < formats.Length; j++)
                {
                    if (current.Metadata.Format.ToLower () == formats[j].ToLower ())
                        return current;
                }
            }

            return null;
        }
    }
}
