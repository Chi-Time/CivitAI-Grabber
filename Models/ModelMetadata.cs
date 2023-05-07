// Copyright (c) 2023 James Johnson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
namespace CivitAI_Grabber.Models
{
    /// <summary>The metadata associated a model file.</summary>
    public class ModelFileMetadata
    {
        public string FP { get; set; } = "";
        public string Size { get; set; } = "";
        public string Format { get; set; } = "";
    }
}
