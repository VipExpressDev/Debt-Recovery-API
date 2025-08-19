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
    public class CollectorPerformanceSummaryController : Controller
    {
        private readonly RepositoryInterface<TblDebtRecoveryData> _DebtRecoveryDataRepository;
        private readonly RepositoryInterface<TblDebtCollectors> _DebtCollectorsRepository;
        private readonly RepositoryInterface<TblPrimaryStatus> _PrimaryStatusRepository;
        private IConfiguration _Configuration { get; }

        public CollectorPerformanceSummaryController(RepositoryInterface<TblDebtRecoveryData> DebtRecoveryDataRepository, RepositoryInterface<TblDebtCollectors> DebtCollectorsRepository, RepositoryInterface<TblPrimaryStatus> PrimaryStatusRepository, IConfiguration configuration)
        {
            _DebtRecoveryDataRepository = DebtRecoveryDataRepository;
            _DebtCollectorsRepository = DebtCollectorsRepository;
            _PrimaryStatusRepository = PrimaryStatusRepository;
            _Configuration = configuration;
        }
        
        // GET: /CollectorPerformanceSummary/  (All)
        [HttpGet("{managerCode}", Name = "GetPerformances")]
        public async Task<IActionResult> Get(int managerCode, [FromHeader] string Authorization)
        {
            List<CollectorPerformanceSummary> debtPerformanceList = new List<CollectorPerformanceSummary>();

            var useDebtData =  await _DebtRecoveryDataRepository.GetAll();
            var allDebtData = useDebtData.Where(w => w.AllocatedBy == managerCode).ToList();
            var allDebtCollectors = await _DebtCollectorsRepository.GetAll();
            var debtStatus =    await _PrimaryStatusRepository.GetAll();
            int actionReqCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Action")) == null ? 4 : debtStatus.FirstOrDefault(f => f.Description.Contains("Action")).Code;

            foreach (var item in allDebtCollectors.GroupBy(gb => gb.PersonnelCode))
            {
                decimal AmtAssignedTotal = allDebtData.Where(w => w.AllocatedTo == item.Key).Sum(s => s.TransactionAmount);
                decimal AmtCollected = allDebtData.Where(w => w.AllocatedTo == item.Key).Sum(s => s.CollectedAmount);
                decimal AmtOutstanding = allDebtData.Where(w => w.AllocatedTo == item.Key).Sum(s => s.AmountDue);
                decimal AmtUnactionedTotal = allDebtData.Where(w => w.AllocatedTo == item.Key && w.StatusID == actionReqCode).Sum(s => s.TransactionAmount);

                CollectorPerformanceSummary debtorPerformanceAccumilative = new CollectorPerformanceSummary()
                {
                    CollectorName = allDebtCollectors.FirstOrDefault(w => w.PersonnelCode == item.Key)?.NameAndSurname,
                    AmtAssignedTotal = AmtAssignedTotal,
                    AmtCollected = AmtCollected,
                    AmtOutstanding = AmtOutstanding,
                    AmtUnactionedTotal = AmtUnactionedTotal,
                    isAccumilative = true
                };

                decimal AmtAssignedTotalM = allDebtData.Where(w => w.AllocatedTo == item.Key && w.TransactionDate >= DateTime.Now.AddMonths(-1)).Sum(s => s.TransactionAmount);
                decimal AmtCollectedM = allDebtData.Where(w => w.AllocatedTo == item.Key && w.LastCollectedDate >= DateTime.Now.AddMonths(-1)).Sum(s => s.CollectedAmount);
                decimal AmtOutstandingM = allDebtData.Where(w => w.AllocatedTo == item.Key && (w.DateSatisfied == null || w.DateSatisfied == DateTime.MinValue) && w.TransactionDate >= DateTime.Now.AddMonths(-1)).Sum(s => s.TransactionAmount);
                decimal AmtUnactionedTotalM = allDebtData.Where(w => w.AllocatedTo == item.Key && w.StatusID == actionReqCode && w.TransactionDate >= DateTime.Now.AddMonths(-1)).Sum(s => s.TransactionAmount);

                CollectorPerformanceSummary debtorPerformanceMonthly = new CollectorPerformanceSummary()
                {
                    CollectorName = allDebtCollectors.FirstOrDefault(w => w.PersonnelCode == item.Key)?.NameAndSurname,
                    AmtAssignedTotal = AmtAssignedTotalM,
                    AmtCollected = AmtCollectedM,
                    AmtOutstanding = AmtOutstandingM,
                    AmtUnactionedTotal = AmtUnactionedTotalM,
                    isAccumilative = false
                };

                if(debtorPerformanceAccumilative.AmtAssignedTotal > 0)
                {
                    debtPerformanceList.Add(debtorPerformanceAccumilative);
                }
                if(debtorPerformanceMonthly.AmtAssignedTotal > 0)
                {
                    debtPerformanceList.Add(debtorPerformanceMonthly);
                }
            }

            return new OkObjectResult(new ResponseObject<CollectorPerformanceSummary>(debtPerformanceList, Authorization));
        }
    }
}
