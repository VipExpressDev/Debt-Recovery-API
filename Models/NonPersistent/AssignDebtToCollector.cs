using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class AssignDebtToCollector
    {
        public string ContractNo { get; set; }
        public int? AllocatedTo { get; set; }
        public int? AllocatedBy { get; set; }

        public AssignDebtToCollector()
        {

        }

        public AssignDebtToCollector(string _contractNo, int? _allocatedTo, int? _allocatedBy)
        {
            ContractNo = _contractNo;
            AllocatedTo = _allocatedTo;
            AllocatedBy = _allocatedBy;
        }
    }
}
