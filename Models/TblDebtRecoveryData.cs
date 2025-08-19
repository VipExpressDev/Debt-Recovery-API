using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models
{
    public class TblDebtRecoveryData : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string PvOID { get; set; }
        public string Type { get; set; }
        public string ContractNo { get; set; }
        public string ClientFullName { get; set; }
        public string Cellphone { get; set; }
        public string VipLevel { get; set; }
        public string ContractStatus { get; set; }
        public DateTime? DateOfSale { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal AmountDue { get; set; }
        public int? AllocatedTo { get; set; }
        public DateTime? AllocatedDate { get; set; }
        public int? AllocatedBy { get; set; }
        public int StatusID { get; set; }
        public decimal CollectedAmount { get; set; }
        public DateTime? LastCollectedDate { get; set; }
        public string Comment { get; set; }
        public int? DebtReasonID { get; set; }
        public bool? DebtSatisfied { get; set; }
        public bool? Commissionable { get; set; }
        public DateTime? PromiseToPayDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public DateTime? DateSatisfied { get; set; }
        public bool Discontinued { get; set; }
        public int Region { get; set; }
    }
}
