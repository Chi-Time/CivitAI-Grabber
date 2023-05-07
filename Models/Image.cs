// Copyright (c) 2022 James Johnson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
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
