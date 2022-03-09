using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProSuite.Support.WebAPI.Migration.Models
{
    public class PSResult
    {
        public List<string> Response { get; set; }
        public List<string> ErrorMessage { get; set; }
        public List<string> WarningMessage { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string scriptContent { get; set; }
    }
}
