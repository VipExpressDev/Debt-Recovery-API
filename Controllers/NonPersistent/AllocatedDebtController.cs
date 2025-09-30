using DebtRecoveryPlatform.Helpers;
using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Models.NonPersistent;
using DebtRecoveryPlatform.Models.ResponseObject;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace DebtRecoveryPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllocatedDebtController : ControllerBase
    {
        private readonly RepositoryInterface<TblDebtRecoveryData> _DebtRecoveryDataRepository;
        private readonly RepositoryInterface<TblPrimaryStatus> _PrimaryStatusRepository;

        public AllocatedDebtController(RepositoryInterface<TblDebtRecoveryData> DebtRecoveryDataRepository, RepositoryInterface<TblPrimaryStatus> PrimaryStatusRepository)
        {
            _DebtRecoveryDataRepository = DebtRecoveryDataRepository;
            _PrimaryStatusRepository = PrimaryStatusRepository;
        }

        // GET: /AllocatedDebt/ (All)
        public async Task<IActionResult> Get([FromHeader] string Authorization)
        {
            var allDebtData = await _DebtRecoveryDataRepository.GetAll();
            var debtStatus = await _PrimaryStatusRepository.GetAll();
            int actionReqCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Action")) == null ? 0 : debtStatus.FirstOrDefault(f => f.Description.Contains("Action")).Code;
            int followUpCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Follow")) == null ? 0 : debtStatus.FirstOrDefault(f => f.Description.Contains("Follow")).Code;
            int ptpCode = debtStatus.FirstOrDefault(f => f.Description.Contains("PTP")) == null ? 0 : debtStatus.FirstOrDefault(f => f.Description.Contains("PTP")).Code;
            List<AllocatedDebt> listOfDebt = new List<AllocatedDebt>();

            foreach (var debtItem in allDebtData.GroupBy(gb => gb.ContractStatus))
            {
                AllocatedDebt debtSummary = new AllocatedDebt()
                {
                    ContractStatus = debtItem.Key,
                    ContractsCount = allDebtData.Where(w => w.ContractStatus == debtItem.Key).Select(s => s.BookingRef).Distinct().Count(),
                    ActionRequiredCount = allDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == actionReqCode).Select(s => s.BookingRef).Distinct().Count(),
                    FollowUpCount = allDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == followUpCode).Select(s => s.BookingRef).Distinct().Count(),
                    PTPCount = allDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == ptpCode).Select(s => s.BookingRef).Distinct().Count(),
                    TotalAmount = allDebtData.Where(w => w.ContractStatus == debtItem.Key).Sum(sm => sm.AmountDue),
                    isAccu = true
                };

                listOfDebt.Add(debtSummary);
            }

            foreach (var debtItem in allDebtData.Where(w => w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).GroupBy(gb => gb.ContractStatus))
            {
                AllocatedDebt monthDebtSummary = new AllocatedDebt()
                {
                    ContractStatus = debtItem.Key,
                    ContractsCount = allDebtData.Where(w => w.ContractStatus == debtItem.Key && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Select(s => s.BookingRef).Distinct().Count(),
                    ActionRequiredCount = allDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == actionReqCode && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Select(s => s.BookingRef).Distinct().Count(),
                    FollowUpCount = allDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == followUpCode && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Select(s => s.BookingRef).Distinct().Count(),
                    PTPCount = allDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == ptpCode && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Select(s => s.BookingRef).Distinct().Count(),
                    TotalAmount = allDebtData.Where(w => w.ContractStatus == debtItem.Key && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Sum(sm => sm.AmountDue),
                    isAccu = false
                };

                listOfDebt.Add(monthDebtSummary);
            }

            return new OkObjectResult(new ResponseObject<AllocatedDebt>(listOfDebt, Authorization));
        }

        // GET: /AllocatedDebt/int (Single)
        [HttpGet("GetAllocatedDebt")]
        public async Task<IActionResult> Get(int debtCollectorID, [FromHeader] string Authorization)
        {
            var allDebtData = await _DebtRecoveryDataRepository.GetAll();
            var filteredDebtData = allDebtData.Where(w => w.AllocatedTo == debtCollectorID).ToList();
            var debtStatus = await _PrimaryStatusRepository.GetAll();
            int actionReqCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Action")) == null ? 0 : debtStatus.FirstOrDefault(f => f.Description.Contains("Action")).Code;
            int followUpCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Follow")) == null ? 0 : debtStatus.FirstOrDefault(f => f.Description.Contains("Follow")).Code;
            int ptpCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Promise")) == null ? 0 : debtStatus.FirstOrDefault(f => f.Description.Contains("Promise")).Code;
            List<AllocatedDebt> listOfDebt = new List<AllocatedDebt>();

            foreach (var debtItem in filteredDebtData.GroupBy(gb => gb.ContractStatus))
            {
                AllocatedDebt debtSummary = new AllocatedDebt()
                {
                    ContractStatus = debtItem.Key,
                    ContractsCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key).Select(s => s.BookingRef).Distinct().Count(),
                    ActionRequiredCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == actionReqCode).Select(s => s.BookingRef).Distinct().Count(),
                    FollowUpCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == followUpCode).Select(s => s.BookingRef).Distinct().Count(),
                    PTPCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == ptpCode).Select(s => s.BookingRef).Distinct().Count(),
                    TotalAmount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key).Sum(sm => sm.AmountDue),
                    isAccu = true
                };

                listOfDebt.Add(debtSummary);
            }

            foreach (var debtItem in filteredDebtData.Where(w => w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).GroupBy(gb => gb.ContractStatus))
            {
                AllocatedDebt monthDebtSummary = new AllocatedDebt()
                {
                    ContractStatus = debtItem.Key,
                    ContractsCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Select(s => s.BookingRef).Distinct().Count(),
                    ActionRequiredCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == actionReqCode && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Select(s => s.BookingRef).Distinct().Count(),
                    FollowUpCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == followUpCode && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Select(s => s.BookingRef).Distinct().Count(),
                    PTPCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == ptpCode && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Select(s => s.BookingRef).Distinct().Count(),
                    TotalAmount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Sum(sm => sm.AmountDue),
                    isAccu = false
                };

                listOfDebt.Add(monthDebtSummary);
            }

            return new OkObjectResult(new ResponseObject<AllocatedDebt>(listOfDebt, Authorization));
        }

        [HttpGet("GetAssignedDebt")]
        public async Task<IActionResult> GetAssignedDebt(int managerID, [FromHeader] string Authorization)
        {
            var allDebtData = await _DebtRecoveryDataRepository.GetAll();
            var filteredDebtData = allDebtData.Where(w => w.AllocatedBy == managerID).ToList();
            var debtStatus = await _PrimaryStatusRepository.GetAll();
            int actionReqCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Action")) == null ? 0 : debtStatus.FirstOrDefault(f => f.Description.Contains("Action")).Code;
            int followUpCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Follow")) == null ? 0 : debtStatus.FirstOrDefault(f => f.Description.Contains("Follow")).Code;
            int ptpCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Promise")) == null ? 0 : debtStatus.FirstOrDefault(f => f.Description.Contains("Promise")).Code;
            List<AllocatedDebt> listOfDebt = new List<AllocatedDebt>();

            foreach (var debtItem in filteredDebtData.GroupBy(gb => gb.ContractStatus))
            {
                AllocatedDebt debtSummary = new AllocatedDebt()
                {
                    ContractStatus = debtItem.Key,
                    ContractsCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key).Select(s => s.BookingRef).Distinct().Count(),
                    ActionRequiredCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == actionReqCode).Select(s => s.BookingRef).Distinct().Count(),
                    FollowUpCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == followUpCode).Select(s => s.BookingRef).Distinct().Count(),
                    PTPCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == ptpCode).Select(s => s.BookingRef).Distinct().Count(),
                    TotalAmount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key).Sum(sm => sm.AmountDue),
                    isAccu = true
                };

                listOfDebt.Add(debtSummary);
            }

            foreach (var debtItem in filteredDebtData.Where(w => w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).GroupBy(gb => gb.ContractStatus))
            {
                AllocatedDebt monthDebtSummary = new AllocatedDebt()
                {
                    ContractStatus = debtItem.Key,
                    ContractsCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Select(s => s.BookingRef).Distinct().Count(),
                    ActionRequiredCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == actionReqCode && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Select(s => s.BookingRef).Distinct().Count(),
                    FollowUpCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == followUpCode && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Select(s => s.BookingRef).Distinct().Count(),
                    PTPCount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.StatusID == ptpCode && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Select(s => s.BookingRef         ).Distinct().Count(),
                    TotalAmount = filteredDebtData.Where(w => w.ContractStatus == debtItem.Key && w.TransactionDate > new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 01)).Sum(sm => sm.AmountDue),
                    isAccu = false
                };

                listOfDebt.Add(monthDebtSummary);
            }

            return new OkObjectResult(new ResponseObject<AllocatedDebt>(listOfDebt, Authorization));
        }

        // POST: /AllocatedDebt/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] AllocatedDebt allocatedDebt, [FromHeader] string Authorization)
        {
            using (var scope = new TransactionScope())
            {
                //_AllocatedDebtRepository.Create(allocatedDebt);
                //_AllocatedDebtRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = allocatedDebt }, allocatedDebt);
            };
        }

        // PUT: /AllocatedDebt/int
        [HttpPut("")]
        public IActionResult Put([FromBody] AllocatedDebt allocatedDebt, [FromHeader] string Authorization)
        {
            if (allocatedDebt != null)
            {
                using (var scope = new TransactionScope())
                {
                    //_AllocatedDebtRepository.Update(allocatedDebt);
                    scope.Complete();
                    return new OkObjectResult(new ResponseObject<AllocatedDebt>(allocatedDebt, Authorization));
                }
            }
            return new NoContentResult();
        }

        // DELETE: /AllocatedDebt/int
        [HttpDelete("{allocatedDebtID}")]
        public IActionResult Delete(int allocatedDebtID, [FromHeader] string Authorization)
        {
            //_AllocatedDebtRepository.Delete(allocatedDebtID);
            return new OkResult();
        }
    }
}
