using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models
{
    public class TblDebtAllocationHistory : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateAllocated { get; set; }
        public int AllocatedBy { get; set; }
        public int DebtItemID { get; set; }
        public int AllocatedTo { get; set; }
    }
}
