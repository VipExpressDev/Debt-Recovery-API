using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models
{
    public class TblRejectedMandates : BaseClass
    {
        [Key]
        public int Id { get; set; }
        public string MandateCode { get; set; }
        public string ContractNo { get; set; }
        public string PayerName { get; set; }
        public string Biller { get; set; }
        public string BillersBank { get; set; }
        public string Product { get; set; }
        public decimal Amount { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerBank { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public DateTime MandateEffectiveDate { get; set; }
        public DateTime MandateExpiryDate { get; set; }
        public int Channel { get; set; }
        public string Status { get; set; }
        public int DebitFrequency { get; set; }
        public DateTime LineCreatedDate { get; set; }
        public string LineCreatedBy { get; set; }
        public DateTime DateVerified { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? DateApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? DateAuthorized { get; set; }
        public string AuthorizedBy { get; set; }
    }
}
