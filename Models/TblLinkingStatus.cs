using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models
{
    public class TblLinkingStatus : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public int PrimaryStatusId { get; set; }
        public int SecondaryStatusId { get; set; }
    }
}
