using DebtRecoveryPlatform.Helpers;
using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Models.NonPersistent;
using DebtRecoveryPlatform.Models.ResponseObject;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Controllers.NonPersistent
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientInfoSummaryController : Controller
    {
        private readonly RepositoryInterface<TblDebtRecoveryData> _DebtRecoveryDataRepository;
        private IConfiguration configuration { get; }

        public ClientInfoSummaryController(RepositoryInterface<TblDebtRecoveryData> DebtRecoveryDataRepository, IConfiguration configuration)
        {
            _DebtRecoveryDataRepository = DebtRecoveryDataRepository;
            this.configuration = configuration;
        }

        // GET: /ClientInfoSummary/string (Single)
        [HttpGet()]
        public async Task<IActionResult> Get(string bookingRef, int userCode, [FromHeader] string Authorization)
        {            
            var contractTransactions = await _DebtRecoveryDataRepository.GetAll();
            List<TblDebtRecoveryData> filteredTransactions = contractTransactions.Where(w => w.BookingRef.Contains(bookingRef) && (w.AllocatedTo == userCode || w.AllocatedBy == userCode)).ToList();
            List<ClientInfoSummary> summaryOfTransactions = new List<ClientInfoSummary>();
            List<TblDebtRecoveryData> transactionsToUse = new List<TblDebtRecoveryData>();
            List<PayPlanData> PayPlanDataList = PayPlanData.GetPayPlanData(configuration, bookingRef);

            List<TblDebtRecoveryData> depositList = filteredTransactions.Where(w => w.Type == "Deposit").ToList();

            if(depositList.Count() > 0)
            {
                foreach (var item in depositList)
                {
                    transactionsToUse.Add(item);
                }
            }
            else
            {
                foreach (var item in filteredTransactions)
                {
                    transactionsToUse.Add(item);
                }
            }

            foreach (var item in transactionsToUse.GroupBy(gb => gb.Type))
            {
                int debtCount = transactionsToUse.Where(w => w.Type.Contains(item.Key) && w.TransactionDate <= DateTime.Now && (w.DateSatisfied == null || w.DateSatisfied == DateTime.MinValue)).Count();
                int payPlanCount = PayPlanDataList.Where(w => w.AccPayType == item.Key && w.DateOfPayment <= DateTime.Now).Count();

                ClientInfoSummary typeSummary = new ClientInfoSummary()
                {
                    ContractNo = transactionsToUse.Where(w => w.Type.Contains(item.Key)).Distinct().FirstOrDefault().ContractNo,
                    BookingRef = transactionsToUse.Where(w => w.Type.Contains(item.Key)).Distinct().FirstOrDefault().BookingRef,
                    TransactionType = item.Key,
                    TypeCount = String.Format("{0} out of {1}", debtCount, payPlanCount),
                    TotalDue = transactionsToUse.Where(w => w.Type.Contains(item.Key) && w.TransactionDate <= DateTime.Now).Sum(sm => sm.AmountDue)
                };

                summaryOfTransactions.Add(typeSummary);
            }

            return new OkObjectResult(new ResponseObject<ClientInfoSummary>(summaryOfTransactions, Authorization));
        }
    }
}
