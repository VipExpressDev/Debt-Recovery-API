using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class DebtReminder
    {
        public string ContractNo { get; set; }
        public string BookingRef { get; set; }
        public string CollectorName { get; set; }
        public string ManagerName { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderType { get; set; }

        public DebtReminder()
        {

        }

        public DebtReminder(string _contractNo, string _bookingRef, string _collectorName, string _managerName, DateTime _reminderDate, string _reminderType)
        {
            ContractNo = _contractNo;
            BookingRef = _bookingRef;
            CollectorName = _collectorName;
            ManagerName = _managerName;
            ReminderDate = _reminderDate;
            ReminderType = _reminderType;
        }
    }
}
