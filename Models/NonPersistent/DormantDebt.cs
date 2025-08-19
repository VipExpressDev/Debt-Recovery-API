using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class DormantDebt
    {
        public string ContractNo { get; set; }
        public DateTime LastActionedDate { get; set; }

        public DormantDebt()
        {

        }

        public DormantDebt(string _contractNo, DateTime _lastActionedDate)
        {
            ContractNo = _contractNo;
            LastActionedDate = _lastActionedDate;
        }
    }
}
