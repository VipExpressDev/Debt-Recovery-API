using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class DebtRecoveryData
    {
        public DateTime? LastDateAssigned { get; set; }
        public string ContractNo { get; set; }
        public string DebtStatus { get; set; }
        public string AssignedTo { get; set; }
        public string ContractStatus { get; set; }
        public string ContractLevel { get; set; }
        public string ClientName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public DateTime? PromiseToPayDate { get; set; }
        public DateTime? LastCollectedDate { get; set; }
        public bool isAccumilative { get; set; }

        public DebtRecoveryData()
        {

        }

        public DebtRecoveryData(DateTime? _lastDateAssigned, string _contractNo, string _debtStatus, string _assignedTo, string _contractStatus, string _contractLevel, string _clientName, decimal _totalAmount, DateTime? _followUpDate, DateTime? _promiseToPayDate, DateTime? _lastCollectedDate, bool _isAccumilative)
        {
            LastDateAssigned = _lastDateAssigned;
            ContractNo = _contractNo;
            DebtStatus = _debtStatus;
            AssignedTo = _assignedTo;
            ContractStatus = _contractStatus;
            ContractLevel = _contractLevel;
            ClientName = _clientName;
            TotalAmount = _totalAmount;
            FollowUpDate = _followUpDate;
            PromiseToPayDate = _promiseToPayDate;
            LastCollectedDate = _lastCollectedDate;
            isAccumilative = _isAccumilative;
        }
    }
}
