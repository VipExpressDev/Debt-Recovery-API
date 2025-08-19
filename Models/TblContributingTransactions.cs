using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models
{
    public class TblContributingTransactions : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public int DebtLineID { get; set; }
        public Guid TransLinkID { get; set; }
        public decimal AmountContributed { get; set; }
        public DateTime DateContributed { get; set; }
    }
}
