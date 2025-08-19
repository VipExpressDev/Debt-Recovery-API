using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class AllocationHistory
    {
        public string ContractNo { get; set; }
        public string AllocatedBy { get; set; }
        public string AllocatedTo { get; set; }
        public DateTime DateAllocated { get; set; }

        public AllocationHistory()
        {

        }

        public AllocationHistory(string _contractNo, string _allocatedBy, string _allocatedTo, DateTime _dateAllocated)
        {
            ContractNo = _contractNo;
            AllocatedBy = _allocatedBy;
            AllocatedTo = _allocatedTo;
            DateAllocated = _dateAllocated;
        }
    }
}
