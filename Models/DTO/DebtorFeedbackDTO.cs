using Microsoft.VisualBasic;
using System;

namespace DebtRecoveryPlatform.Models.DTO
{
    public class DebtorFeedbackDTO
    {
        public string ContractNo { get; set; }

        public DateTime DateCreated { get; set; }

        public string Comment { get; set; }

        public string CreatedBy { get; set; }
    }
}
