using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class AssignDebtToCollector
    {
        public string ContractNo { get; set; }
        public string BookingRef { get; set; }
        public int? AllocatedTo { get; set; }
        public int? AllocatedBy { get; set; }

        public AssignDebtToCollector()
        {

        }

        public AssignDebtToCollector(string _contractNo, string _bookingRef, int? _allocatedTo, int? _allocatedBy)
        {
            ContractNo = _contractNo;
            BookingRef = _bookingRef;
            AllocatedTo = _allocatedTo;
            AllocatedBy = _allocatedBy;
        }
    }
}
