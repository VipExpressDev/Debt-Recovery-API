using System.ComponentModel.DataAnnotations;

namespace DebtRecoveryPlatform.Models
{
    public class TblBadDebtReasons : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public int Code { get; set; }
    }
}
