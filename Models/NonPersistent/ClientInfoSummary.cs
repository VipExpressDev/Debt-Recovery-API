using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class ClientInfoSummary : BaseClass
    {
        public string ContractNo { get; set; }
        public string BookingRef { get; set; }
        public string TransactionType { get; set; }
        public string TypeCount { get; set; }
        public decimal TotalDue { get; set; }
    }
}
