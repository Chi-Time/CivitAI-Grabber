using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
