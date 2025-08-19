using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class AllocatedDebt : BaseClass
    {
        public string ContractStatus { get; set; }
        public int ContractsCount { get; set; }
        public int ActionRequiredCount { get; set; }
        public int FollowUpCount { get; set; }
        public int PTPCount { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool isAccu { get; set; }
    }
}
