using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models
{
    public class TblStatusLinking
    {
        [Key]
        public int Id { get; set; }
        public int PrimaryStatusID { get; set; }
        public int SecondaryStatusID { get; set; }
    }
}
