using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models
{
    public class TblClientProfileHistory : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string ContractNo { get; set; }
        public string BookingRef { get; set; }
        public DateTime DateActioned { get; set; }
        public int ActionedByID { get; set; }
        public int StatusID { get; set; }
        public DateTime StatusDate { get; set; }
        public string Comment { get; set; }
    }
}
