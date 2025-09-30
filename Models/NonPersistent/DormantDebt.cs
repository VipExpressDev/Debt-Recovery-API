using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class DormantDebt
    {
        public string ContractNo { get; set; }
        public string BookingRef { get; set; }
        public DateTime LastActionedDate { get; set; }

        public DormantDebt()
        {

        }

        public DormantDebt(string _bookingRef, DateTime _lastActionedDate)
        {
            //ContractNo = _contractNo;
            BookingRef = _bookingRef;
            LastActionedDate = _lastActionedDate;
        }
    }
}
