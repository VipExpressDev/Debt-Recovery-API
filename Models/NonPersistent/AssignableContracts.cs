using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class AssignableContracts
    {
        public int DebtItemId { get; set; }
        public string ContractNo { get; set; }
        public string ContractVIPLevel { get; set; }
        public string ContractStatus { get; set; }
        public string ClientFullName { get; set; }
        public DateTime? DateOfSale { get; set; }
        public decimal AmountDue { get; set; }
        public int TotalTransactions { get; set; }
        public string AllocatedTo { get; set; }

        public AssignableContracts()
        {

        }

        public AssignableContracts(string _contractNo, string _contractVIPLevel, string _contractStatus, string _clientFullName, DateTime? _dateOfSale, decimal _amountDue, int _totalTransactions, int _debtItemId)
        {
            ContractNo = _contractNo;
            ContractVIPLevel = _contractVIPLevel;
            ContractStatus = _contractStatus;
            ClientFullName = _clientFullName;
            DateOfSale = _dateOfSale;
            AmountDue = _amountDue;
            TotalTransactions = _totalTransactions;
            DebtItemId = _debtItemId;
        }

        public AssignableContracts(string _contractNo, string _contractVIPLevel, string _contractStatus, string _clientFullName, DateTime? _dateOfSale, decimal _amountDue, int _totalTransactions, string _allocatedTo)
        {
            ContractNo = _contractNo;
            ContractVIPLevel = _contractVIPLevel;
            ContractStatus = _contractStatus;
            ClientFullName = _clientFullName;
            DateOfSale = _dateOfSale;
            AmountDue = _amountDue;
            TotalTransactions = _totalTransactions;
            AllocatedTo = _allocatedTo;
        }
    }
}
