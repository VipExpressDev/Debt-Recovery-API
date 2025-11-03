using DebtRecoveryPlatform.Models.NonPersistent;
using DebtRecoveryPlatform.Models.ResponseObject;
using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using DebtRecoveryPlatform.Models.DTO;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using DebtRecoveryPlatform.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DebtRecoveryPlatform.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly RepositoryInterface<TblDebtRecoveryData> _DebtRecoveryDataRepository;
        private readonly RepositoryInterface<TblDebtCollectors> _DebtCollectorsRepository;
        private readonly RepositoryInterface<TblPrimaryStatus> _PrimaryStatusRepository;
        private readonly RepositoryInterface<TblClientProfileHistory> _ClientProfileHistoryRepository;
        private IConfiguration _Configuration { get; }
        public ReportController(RepositoryInterface<TblDebtRecoveryData> DebtRecoveryDataRepository, RepositoryInterface<TblClientProfileHistory> ClientProfileHistoryRepository, RepositoryInterface<TblDebtCollectors> DebtCollectorsRepository, RepositoryInterface<TblPrimaryStatus> PrimaryStatusRepository, IConfiguration configuration)
        {
            _DebtRecoveryDataRepository = DebtRecoveryDataRepository;
            _DebtCollectorsRepository = DebtCollectorsRepository;
            _PrimaryStatusRepository = PrimaryStatusRepository;
            _Configuration = configuration;
            _ClientProfileHistoryRepository = ClientProfileHistoryRepository;
        }


        // GET: /Report/  (All)
        [HttpGet("{managerCode}", Name = "CollectorGetPerformances")]
        public async Task<IActionResult> Get(int managerCode, DateTime? from, DateTime? to, int? collector,  [FromHeader] string Authorization)
        {
            List<CollectorPerformanceDto> collectorPerformanceList = new List<CollectorPerformanceDto>();

            var allDebtCollectors = await _DebtCollectorsRepository.GetAll();

            var isManager = allDebtCollectors.Where(x => x.PersonnelCode == managerCode && x.isManager && x.GCRecord == null).FirstOrDefault()?.PersonnelCode;



            var useDebtData = await _DebtRecoveryDataRepository.GetAll();
            var allDebtData = useDebtData.Where(w => managerCode == isManager ? w.AllocatedBy == managerCode : w.AllocatedTo == managerCode
             && (!from.HasValue || w.TransactionDate >= from.Value) &&
               (!to.HasValue || w.TransactionDate <= to.Value)
               &&((!collector.HasValue || collector == 0) || collector == w.AllocatedTo)
            ).ToList();

            
            var debtStatus = await _PrimaryStatusRepository.GetAll();
            int actionReqCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Action")) == null ? 4 : debtStatus.FirstOrDefault(f => f.Description.Contains("Action")).Code;

            foreach (var item in allDebtCollectors.GroupBy(gb => gb.PersonnelCode))
            {
                decimal AmtAssignedTotal = allDebtData.Where(w => w.AllocatedTo == item.Key).Sum(s => s.TransactionAmount);
                decimal AmtCollected = allDebtData.Where(w => w.AllocatedTo == item.Key).Sum(s => s.CollectedAmount);
                //decimal AmtOutstanding = allDebtData.Where(w => w.AllocatedTo == item.Key).Sum(s => s.AmountDue);
                decimal AmtOutstanding = AmtAssignedTotal - AmtCollected;
                decimal AmtUnactionedTotal = allDebtData.Where(w => w.AllocatedTo == item.Key && w.StatusID == actionReqCode).Sum(s => s.TransactionAmount);

                CollectorPerformanceDto collectorPerformanceAccumilative = new CollectorPerformanceDto()
                {
                    CollectorName = allDebtCollectors.FirstOrDefault(w => w.PersonnelCode == item.Key)?.NameAndSurname,
                    AmountAssigned = AmtAssignedTotal,
                    AmountCollected = AmtCollected,
                    Outstanding = AmtOutstanding,
                    Unactioned = AmtUnactionedTotal,
                    CollectionRate = AmtCollected == 0 ? 0: AmtCollected / AmtAssignedTotal //* 100
                };

               

                if (collectorPerformanceAccumilative.AmountAssigned > 0)
                {
                    collectorPerformanceList.Add(collectorPerformanceAccumilative);
                }
                
            }

            var reportService = new ReportService();
            var fileBytes = reportService.GenerateCollectorPerformanceReport(collectorPerformanceList);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CollectorReport.xlsx");

            //return new OkObjectResult(new ResponseObject<CollectorPerformanceSummary>(debtPerformanceList, Authorization));
        }

        // GET: /Report/GetDebtorEngagementSummary
        [HttpGet("GetDebtorEngagementSummary/{managerCode}", Name = "GetDebtorEngagementSummary")]
        public async Task<IActionResult> GetDebtorEngagementSummary(int managerCode, DateTime? from, DateTime? to, int? collector, [FromHeader] string Authorization)
        {
            List<DebtorEngagementSummaryDTO> DebtorEngagementSummaryList = new List<DebtorEngagementSummaryDTO>();

            var allDebtCollectors = await _DebtCollectorsRepository.GetAll();
            var isManager = allDebtCollectors.Where(x => x.PersonnelCode == managerCode && x.isManager && x.GCRecord == null).FirstOrDefault()?.PersonnelCode;

            var allData = await _DebtRecoveryDataRepository.GetAll();

            //var debtRecoveryData = allData.Where(w => managerCode != isManager || w.AllocatedBy == managerCode
            // && (!from.HasValue || w.TransactionDate >= from.Value) &&
            //   (!to.HasValue || w.TransactionDate <= to.Value)
            //   && ((!collector.HasValue || collector == 0) || collector == w.AllocatedTo)
            //).GroupBy(gb => gb.BookingRef).ToList();

            var debtRecoveryData = allData
                .Where(w =>
                    (managerCode == isManager? w.AllocatedBy == managerCode : w.AllocatedTo == managerCode) &&
                    (!from.HasValue || w.TransactionDate >= from.Value) &&
                    (!to.HasValue || w.TransactionDate <= to.Value) &&
                    ((!collector.HasValue || collector == 0) || collector == w.AllocatedTo)
                )
                .GroupBy(gb => gb.ContractNo)
                .Select(g => new
                {
                    ContractNo = g.Key,
                    AmountDue = g.Sum(x => x.AmountDue),
                    TransactionAmount = g.Sum(x => x.TransactionAmount),
                    StatusID = g.FirstOrDefault()?.StatusID,
                    DebtSatisfied = g.FirstOrDefault()?.DebtSatisfied
                })
                .ToList();


            var debtStatus = await _PrimaryStatusRepository.GetAll();

            //decimal AmtCollected = debtRecoveryData.Where(w => w.AllocatedTo == item.Key).Sum(s => s.CollectedAmount);
            decimal TotalAmount = debtRecoveryData.Sum(s => s.AmountDue);

            foreach (var item in debtRecoveryData.Where(x => x.DebtSatisfied != true ).GroupBy(gb => gb.StatusID))
            {
                string status = debtStatus.FirstOrDefault(f => f.Code == item.Key) == null ? "No Status" : debtStatus.FirstOrDefault(f => f.Code == item.Key).Description;
                int statusCode = debtStatus.FirstOrDefault(f => f.Code == item.Key) == null ? 0 : debtStatus.FirstOrDefault(f => f.Code == item.Key).Code;

                decimal StatusTotalAmount = debtRecoveryData.Where(w => w.StatusID == item.Key && w.DebtSatisfied != true).Sum(s => s.TransactionAmount);
                int Count = debtRecoveryData.Where(w => w.StatusID == item.Key && w.DebtSatisfied != true).Count();


                DebtorEngagementSummaryDTO debtorEngagementSummary = new DebtorEngagementSummaryDTO()
                {
                    Status = status,
                    Count = Count,
                    TotalAmount = StatusTotalAmount,
                    PortfolioPercent = Math.Truncate(StatusTotalAmount) == 0 ? 0 : StatusTotalAmount / TotalAmount //* 100,
                };

                DebtorEngagementSummaryList.Add(debtorEngagementSummary);

            }


            //Get values for collected amounts

            decimal CollectedTotalAmount = debtRecoveryData.Where(w => w.DebtSatisfied == true).Sum(s => s.TransactionAmount);
            int CollectedCount = debtRecoveryData.Where(w => w.DebtSatisfied == true).Count();

            //DebtorEngagementSummaryDTO debtorEngagementSummary2 = new DebtorEngagementSummaryDTO()
            //{
            //    Status = "Collected",
            //    Count = CollectedCount,
            //    TotalAmount = CollectedTotalAmount,
            //    PortfolioPercent = CollectedTotalAmount == 0 ? 0 : (decimal)CollectedTotalAmount / TotalAmount //* 100,
            //};

            //DebtorEngagementSummaryList.Add(debtorEngagementSummary2);


            var reportService = new ReportService();
            var fileBytes = reportService.DebtorEngagementSummaryReport(DebtorEngagementSummaryList);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Debtor Engagement Summary Report.xlsx");

            //return new OkObjectResult(new ResponseObject<CollectorPerformanceSummary>(debtPerformanceList, Authorization));
        }

        ///////
        ///

        // GET: /Report/GetDebtorFeedbacks
        [HttpGet("GetDebtorFeedbacks/{managerCode}", Name = "GetDebtorFeedbacks")]
        public async Task<IActionResult> GetDebtorFeedbacks(int ? managerCode, DateTime? from, DateTime? to, int? collector, [FromHeader] string Authorization)
        {
            List<DebtorFeedbackDTO> DebtorFeedbackList = new List<DebtorFeedbackDTO>();

            var allDebtCollectors = await _DebtCollectorsRepository.GetAll();
            var isManager = allDebtCollectors.Where(x => x.PersonnelCode == managerCode && x.isManager && x.GCRecord == null).FirstOrDefault();

            var collectors = new List<(int, string)>();
            var collectorCode = new List<int>();


            collectors = allDebtCollectors
                .Where(x => x.RegionID == isManager?.RegionID || x.PersonnelCode == managerCode)
                .Select(x => (Code: x.PersonnelCode, Name: x.NameAndSurname))
                .ToList();

            collectorCode = collectors.Select(x => x.Item1)
                .ToList();


            var allClientHistory = await _ClientProfileHistoryRepository.GetAll();

            //var allData = await _DebtRecoveryDataRepository.GetAll();
            var clientHistoryData = allClientHistory.Where(x => collectors == null || collectorCode.Contains(x.CreatedBy)
                && (!from.HasValue || x.CreatedDate >= from.Value) &&
               (!to.HasValue || x.CreatedDate <= to.Value)
               && ((!collector.HasValue || collector == 0) || collector == x.ActionedByID)
            ).ToList();

            //decimal AmtCollected = debtRecoveryData.Where(w => w.AllocatedTo == item.Key).Sum(s => s.CollectedAmount);

            foreach (var item in clientHistoryData.OrderBy(x=> x.BookingRef))
            {
                DebtorFeedbackDTO debtorFeedbackDTO = new DebtorFeedbackDTO()
                {
                    ContractNo = item.ContractNo,
                    DateCreated = item.CreatedDate,
                    CreatedBy = collectors.Where(x => x.Item1 == item.CreatedBy).FirstOrDefault().Item2,
                    Comment = item.Comment

                };

                DebtorFeedbackList.Add(debtorFeedbackDTO);

            }

            var reportService = new ReportService();
            var fileBytes = reportService.FeedBackReport(DebtorFeedbackList);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Debtor Feedback Report.xlsx");

            //return new OkObjectResult(new ResponseObject<CollectorPerformanceSummary>(debtPerformanceList, Authorization));
        }

    }
}
