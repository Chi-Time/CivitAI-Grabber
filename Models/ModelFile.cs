// Copyright (c) 2023 James Johnson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
namespace CivitAI_Grabber.Models
{
    /// <summary>A file associated with a model.</summary>
    public class ModelFile
    {
        public string Name { get; set; } = "";
        public int Id { get; set; } = -1;
        public double SizeKB { get; set; } = 0.0d;
        public string Type { get; set; } = "";
        public ModelFileMetadata Metadata { get; set; } = new ();
        public Hashes Hashes { get; set; } = new ();
        public string DownloadUrl { get; set; } = "";
        public bool Primary { get; set; } = false;
    } 
}
