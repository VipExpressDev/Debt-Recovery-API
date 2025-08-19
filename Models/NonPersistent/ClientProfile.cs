using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class ClientProfile : BaseClass
    {
        public string ContractNo { get; set; }
        public string VipLevel { get; set; }
        public string ContractStatus { get; set; }
        public DateTime DateOfSale { get; set; }
        public DateTime DateDepositPaid { get; set; }
        public string Cellphone { get; set; }
        public string EmailAddress { get; set; }
    }
}
