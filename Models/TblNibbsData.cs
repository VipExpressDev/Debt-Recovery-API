using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models
{
    public class TblNibbsData : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string MandateCode { get; set; }
        public string BillerName { get; set; }
        public string ContractNo { get; set; }
        public string BookingRef { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string PayerName { get; set; }
        public string CustomerBank { get; set; }
        public string Product { get; set; }
        public decimal Amount { get; set; }
        public int TrailNo { get; set; }
        public string SessionId { get; set; }
        public int DebitStatus { get; set; }
        public int CreditStatus { get; set; }
        public DateTime TransactionDate { get; set; }
        public string DebitStatusDescription { get; set; }
        public string CreditStatusDescription { get; set; }
    }
}