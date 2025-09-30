using DebtRecoveryPlatform.DBContext;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class PayPlanData
    {
        public string ContractNo { get; set; }
        public string BookingRef { get; set; }
        public string PaymentType { get; set; }
        public DateTime DateOfPayment { get; set; }
        public decimal Amount { get; set; }
        public string AccPayType { get; set; }
        public decimal AmtDue { get; set; }
        public DateTime? DateSatisfied { get; set; }
        public string PaymentStatus { get; set; }
        public string RowVariant { get; set; }

        public PayPlanData()
        {

        }

        public PayPlanData(string _contractNo, string _bookingRef, string _paymentType, DateTime _dateOfPayment, decimal _amount, string _accPayType, decimal _amtDue, DateTime? _dateSatisfied, string _paymentStatus, string _rowVariant)
        {
            ContractNo = _contractNo;
            BookingRef = _bookingRef;
            PaymentType = _paymentType;
            DateOfPayment = _dateOfPayment;
            Amount = _amount;
            AccPayType = _accPayType;
            AmtDue = _amtDue;
            DateSatisfied = _dateSatisfied;
            PaymentStatus = _paymentStatus;
            RowVariant = _rowVariant;
        }

        public static List<PayPlanData> GetPayPlanData(IConfiguration Configuration, string bookingRef)
        {
            string query = "SELECT C.ContractNo, C.BookingRef, PT.Description, PDP.DateOfPayment, PDP.Amount * C.ConversionRate AS [Converted Amount], IIF(PDP.AccountPaymentType = 1, 'Instalment', IIF(PDP.AccountPaymentType = 0, 'Deposit', 'Extension Fee'))  AS AccountPaymentType, PDP.AmtDue  * C.ConversionRate AS [Amount Due], PDP.DateSatisfied FROM TblPostDatedPayments PDP " +
                            "JOIN TblContract C ON PDP.ContractKey = C.OID " +
                            "JOIN TblPaymentTypes PT ON PDP.PaymentType = PT.OID " +
                            "WHERE C.BookingRef LIKE '%" + bookingRef + "%'";

            provisionDBContext dbCon = new provisionDBContext(Configuration);
            DataSet ds = dbCon.ReturnQueries("ProvisionDB", query);
            DataTable dt = ds.Tables[0];

            List<PayPlanData> payPlanData = new List<PayPlanData>();

            foreach (DataRow payline in dt.Rows)
            {
                DateTime DateOfPayment = (DateTime)(payline["DateOfPayment"]);
                DateTime? DateSatisfied = payline["DateSatisfied"] == DBNull.Value ? null : (DateTime?)(payline["DateSatisfied"]);
                decimal ExpectedAmount = (decimal)(payline["Converted Amount"]);
                decimal OutstandingAmount = payline["Amount Due"] == DBNull.Value ? 0 : (decimal)(payline["Amount Due"]);
                string paymentStatus;
                string rowVariant;

                if (DateSatisfied == null)
                {
                    if (DateOfPayment < DateTime.Now)
                    {
                        if(ExpectedAmount == OutstandingAmount)
                        {
                            paymentStatus = "Outstanding";
                            rowVariant = "danger";
                        }
                        else
                        {
                            paymentStatus = "Partially Satisfied";
                            rowVariant = "secondary";
                        }
                    }
                    else if (DateOfPayment.Month == DateTime.Now.Month && DateOfPayment.Year == DateTime.Now.Year)
                    {
                        if (ExpectedAmount == OutstandingAmount)
                        {
                            paymentStatus = "Current";
                            rowVariant = "warning";
                        }
                        else
                        {
                            paymentStatus = "Current Partially Satisfied";
                            rowVariant = "warning";
                        }                        
                    }
                    else
                    {
                        paymentStatus = "Upcoming";
                        rowVariant = "info";
                    }


                }
                else
                {
                    paymentStatus = "Fully Paid";
                    rowVariant = "success";
                }

                payPlanData.Add(new PayPlanData(payline["ContractNo"].ToString(), payline["bookingRef"].ToString(), payline["Description"].ToString(), DateOfPayment, (decimal)(payline["Converted Amount"]), payline["AccountPaymentType"].ToString(), OutstandingAmount, DateSatisfied, paymentStatus, rowVariant));
            }

            return payPlanData;
        }
    }
}
