using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class ClientProfileHistory
    {
        public string ContractNo { get; set; }
        public string BookingRef { get; set; }
        public DateTime DateActioned { get; set; }
        public string ActionedBy { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public string Comment { get; set; }


        public ClientProfileHistory()
        {

        }

        public ClientProfileHistory(string _contractNo, string _bookingRef, string _actionedBy, string _status, DateTime _dateActioned, string _comment, DateTime _statusDate)
        {
            ContractNo = _contractNo;
            BookingRef = _bookingRef;
            ActionedBy = _actionedBy;
            Status = _status;
            DateActioned = _dateActioned;
            StatusDate = _statusDate;
            Comment = _comment;
        }
    }
}
