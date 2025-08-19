using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models
{
    public class TblDebtCollectors : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string NameAndSurname { get; set; }
        public int PersonnelCode { get; set; }
        public bool isManager { get; set; }
        public int RegionID { get; set; }
    }
}
