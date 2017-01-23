using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Excel_App.Models
{
    public class ProbeData
    {
        public int ProbeId { get; set; }
        public string ProbeName { get; set; }
        public virtual ICollection<SCImport> SCImports { get; set; }
    }
}