// Copyright (c) 2022 James Johnson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
namespace CivitAI_Grabber.Models
{
    /// <summary>Hashes associated with a model file.</summary>
    public class Hashes
    {
        public string AutoV1 { get; set; } = "";
        public string AutoV2 { get; set; } = "";
        public string SHA356 { get; set; } = "";
        public string CRC32 { get; set; } = "";
        public string BLAKE3 { get; set; } = "";
    }
}
