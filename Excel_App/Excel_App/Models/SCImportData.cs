using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Excel_App.Models
{
    public class SCImportData
    {
        public int SCImportID { get; set; }
        public string RelateIPAddress { get; set; }
        public string RelatedNode { get; set; }
        public int ProbeID { get; set; }
        public string Job { get; set; }
        public string CIType { get; set; }
        public Nullable<System.DateTime> TimeErrorReported { get; set; }
        public string Severity { get; set; }
        public string ErrorMessage { get; set; }
        public Nullable<int> Count { get; set; }
        public string ErrorSummary { get; set; }
        public Nullable<System.DateTime> LastTimeActive { get; set; }

        public virtual Probe Probe { get; set; }
    }
}