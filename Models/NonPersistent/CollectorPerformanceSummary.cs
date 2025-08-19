using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class CollectorPerformanceSummary
    {
        public string CollectorName { get; set; }
        public decimal AmtAssignedTotal { get; set; }
        public decimal AmtCollected { get; set; }
        public decimal AmtOutstanding { get; set; }
        public decimal AmtUnactionedTotal { get; set; }
        public bool isAccumilative { get; set; }
    }
}
