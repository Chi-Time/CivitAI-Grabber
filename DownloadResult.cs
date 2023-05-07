// Copyright (c) 2022 James Johnson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
namespace CivitAI_Grabber
{
    /// <summary>The result value of a download operation.</summary>
    public enum DownloadResult
    {
        /// <summary>Represents a sucessful download operation.</summary>
        Success,
        /// <summary>Represents a failed download operation.</summary>
        Failed,
        /// <summary>Represents an operation that stopped as the file already exists.</summary>
        Exists
    };
}
