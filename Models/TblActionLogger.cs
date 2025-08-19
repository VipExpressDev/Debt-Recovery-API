using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models
{
    public class TblActionLogger : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string Action { get; set; }
        public string View { get; set; }
    }
}
