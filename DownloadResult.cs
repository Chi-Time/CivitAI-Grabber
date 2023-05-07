using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
