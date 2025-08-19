using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models
{
    public class TblSecondaryStatus : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public int Code { get; set; }
    }
}
