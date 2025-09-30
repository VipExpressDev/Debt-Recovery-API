using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models
{
    public class TblActionedReminder : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string ContractNo { get; set; }
        public string BookingRef { get; set; }
        public int ActionedByID { get; set; }
        public int ManagerID { get; set; }
        public DateTime ReminderDate { get; set; }
        public int ReminderTypeID { get; set; }
    }
}
