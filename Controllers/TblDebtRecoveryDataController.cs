using DebtRecoveryPlatform.Helpers;
using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Models.NonPersistent;
using DebtRecoveryPlatform.Models.ResponseObject;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DebtRecoveryPlatform.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    [ApiController]
    public class TblDebtRecoveryDataController : ControllerBase
    {
        private readonly RepositoryInterface<TblDebtRecoveryData> _DebtRecoveryDataRepository;
        private readonly RepositoryInterface<TblPrimaryStatus> _PrimaryStatusRepository;
        private readonly RepositoryInterface<TblDebtCollectors> _DebtCollectorsRepository;

        public TblDebtRecoveryDataController(RepositoryInterface<TblDebtRecoveryData> DebtRecoveryDataRepository, RepositoryInterface<TblPrimaryStatus> PrimaryStatusRepository, RepositoryInterface<TblDebtCollectors> DebtCollectorsRepository)
        {
            _DebtRecoveryDataRepository = DebtRecoveryDataRepository;
            _PrimaryStatusRepository = PrimaryStatusRepository;
            _DebtCollectorsRepository = DebtCollectorsRepository;
        }

        // GET: /TblDebtRecoveryData/ (All)
        public async Task<IActionResult> Get([FromHeader] string Authorization)
        {
            var debtRecoveryData = await _DebtRecoveryDataRepository.GetAll();
            return new OkObjectResult(new ResponseObject<TblDebtRecoveryData>(debtRecoveryData, Authorization));
        }

        [HttpGet("GetQuoteOfTheDay")]
        public async Task<IActionResult> GetQuoteOfTheDay([FromHeader] string Authorization)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage responseMessage = await client.GetAsync("http://zenquotes.io/api/random");

            return new OkObjectResult(responseMessage.Content.ReadAsStringAsync());
        }

        [HttpGet("GetAllocatableDebt")]
        public async Task<IActionResult> GetAllocatableDebt(int managerRegion,[FromHeader] string Authorization)
        {
            var debtRecoveryData = await _DebtRecoveryDataRepository.GetAll();

            List<TblDebtRecoveryData> unassignedDebt = debtRecoveryData.Where(w => (w.AllocatedTo == null || w.AllocatedTo == 0) && w.Region == managerRegion && w.AmountDue > 0).ToList();
            List<AssignableContracts> AssignableContracts = new List<AssignableContracts>();

            foreach (var debtLine in unassignedDebt.GroupBy(gb => gb.ContractNo))
            {
                if(unassignedDebt.Where(w => w.ContractNo == debtLine.Key).Count() <= 14)
                {
                    AssignableContracts UnassignedContract = new AssignableContracts()
                    {
                        DebtItemId = unassignedDebt.FirstOrDefault(w => w.ContractNo == debtLine.Key).Id,
                        ContractNo = debtLine.Key,
                        ContractVIPLevel = unassignedDebt.FirstOrDefault(w => w.ContractNo == debtLine.Key).VipLevel,
                        ContractStatus = unassignedDebt.FirstOrDefault(w => w.ContractNo == debtLine.Key).ContractStatus,
                        ClientFullName = unassignedDebt.FirstOrDefault(w => w.ContractNo == debtLine.Key).ClientFullName,
                        DateOfSale = unassignedDebt.FirstOrDefault(w => w.ContractNo == debtLine.Key).DateOfSale,
                        AmountDue = unassignedDebt.Where(w => w.ContractNo == debtLine.Key).Sum(s => s.AmountDue),
                        TotalTransactions = unassignedDebt.Where(w => w.ContractNo == debtLine.Key).Count()
                    };

                    AssignableContracts.Add(UnassignedContract);
                }                
            }

            return new OkObjectResult(new ResponseObject<AssignableContracts>(AssignableContracts, Authorization));
        }

        [HttpGet("GetAllocatedDebt")]
        public async Task<IActionResult> GetAllocatedDebt(int managerRegion, [FromHeader] string Authorization)
        {
            var debtRecoveryData = await _DebtRecoveryDataRepository.GetAll();

            List<TblDebtRecoveryData> assignedDebt = debtRecoveryData.Where(w => (w.AllocatedTo != null && w.AllocatedTo != 0) && w.Region == managerRegion).ToList();
            List<AssignableContracts> ReassignableContracts = new List<AssignableContracts>();
            var debtCollectors = await _DebtCollectorsRepository.GetAll();

            foreach (var debtLine in assignedDebt.GroupBy(gb => gb.ContractNo))
            {
                var allocatedToID = assignedDebt.OrderByDescending(ob => ob.TransactionDate).FirstOrDefault(f => f.ContractNo == debtLine.Key).AllocatedTo;
                string collectorName = debtCollectors.Where(w => w.PersonnelCode == allocatedToID).FirstOrDefault().NameAndSurname;
                string contractStatus = assignedDebt.FirstOrDefault(w => w.ContractNo == debtLine.Key).ContractStatus;

                if(!contractStatus.Contains("Paid"))
                {
                    AssignableContracts AssignedContract = new AssignableContracts()
                    {
                        DebtItemId = assignedDebt.FirstOrDefault(w => w.ContractNo == debtLine.Key).Id,
                        ContractNo = debtLine.Key,
                        ContractVIPLevel = assignedDebt.FirstOrDefault(w => w.ContractNo == debtLine.Key).VipLevel,
                        ContractStatus = assignedDebt.FirstOrDefault(w => w.ContractNo == debtLine.Key).ContractStatus,
                        ClientFullName = assignedDebt.FirstOrDefault(w => w.ContractNo == debtLine.Key).ClientFullName,
                        DateOfSale = assignedDebt.FirstOrDefault(w => w.ContractNo == debtLine.Key).DateOfSale,
                        AmountDue = assignedDebt.Where(w => w.ContractNo == debtLine.Key).Sum(s => s.AmountDue),
                        TotalTransactions = assignedDebt.Where(w => w.ContractNo == debtLine.Key).Count(),
                        AllocatedTo = collectorName
                    };

                    ReassignableContracts.Add(AssignedContract);
                }                
            }

            return new OkObjectResult(new ResponseObject<AssignableContracts>(ReassignableContracts, Authorization));
        }

        // GET: /TblDebtRecoveryData/int (Single)
        [HttpGet("GetActionRequiredData")]
        public async Task<IActionResult> GetActionRequiredData(int debtRecoveryDataID, [FromHeader] string Authorization)
        {
            var h = DateTime.Now.AddMonths(-1);

            var debtCollectors = await _DebtCollectorsRepository.GetAll();
            var debtRecoveryData = await   _DebtRecoveryDataRepository.GetAll();
            var filteredDebtRecoveryData = debtRecoveryData.Where(w => (w.AllocatedTo == debtRecoveryDataID || w.AllocatedBy == debtRecoveryDataID) && w.TransactionDate <= DateTime.Now).ToList();
            var nonAccumilativeRecoveryData = filteredDebtRecoveryData.Where(w => w.TransactionDate > DateTime.Now.AddMonths(-1)).ToList();

            var debtStatus = await _PrimaryStatusRepository.GetAll();
            var actionReq = debtStatus.FirstOrDefault(f => f.Description.Contains("Action")) == null ? null : debtStatus.FirstOrDefault(f => f.Description.Contains("Action"));

            List<DebtRecoveryData> listOfDebt = new List<DebtRecoveryData>();

            foreach(var actionReqItem in nonAccumilativeRecoveryData.Where(w => w.StatusID == actionReq.Code).GroupBy(gb => gb.ContractNo))
            {
                int? collectorId = nonAccumilativeRecoveryData.FirstOrDefault(w => w.ContractNo == actionReqItem.Key).AllocatedTo;
                string collectorName = debtCollectors.FirstOrDefault(f => f.PersonnelCode == collectorId)?.NameAndSurname;

                DebtRecoveryData debtSummary = new DebtRecoveryData()
                {
                    LastDateAssigned = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == actionReqItem.Key).AllocatedDate,
                    ContractNo = actionReqItem.Key,
                    DebtStatus = actionReq.Description,
                    AssignedTo = collectorName,
                    ContractStatus = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == actionReqItem.Key).ContractStatus,
                    ContractLevel = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == actionReqItem.Key).VipLevel,
                    ClientName = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == actionReqItem.Key).ClientFullName,
                    TotalAmount = nonAccumilativeRecoveryData.Where(f => f.ContractNo == actionReqItem.Key).Sum(s => s.AmountDue),
                    FollowUpDate = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == actionReqItem.Key).FollowUpDate,
                    PromiseToPayDate = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == actionReqItem.Key).PromiseToPayDate,
                    isAccumilative = false
                };

                listOfDebt.Add(debtSummary);
            }

            foreach (var actionReqItem in filteredDebtRecoveryData.Where(w => w.StatusID == actionReq.Code).GroupBy(gb => gb.ContractNo))
            {
                int? collectorId = filteredDebtRecoveryData.FirstOrDefault(w => w.ContractNo == actionReqItem.Key).AllocatedTo;
                //string collectorName = debtCollectors.FirstOrDefault(f => f.PersonnelCode == collectorId).NameAndSurname;
                string collectorName = debtCollectors.Where(f => f.PersonnelCode == collectorId).Select(item => item.NameAndSurname).FirstOrDefault();

                DebtRecoveryData debtSummary = new DebtRecoveryData()
                {
                    LastDateAssigned = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == actionReqItem.Key).AllocatedDate,
                    ContractNo = actionReqItem.Key,
                    DebtStatus = actionReq.Description,
                    AssignedTo = collectorName,
                    ContractStatus = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == actionReqItem.Key).ContractStatus,
                    ContractLevel = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == actionReqItem.Key).VipLevel,
                    ClientName = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == actionReqItem.Key).ClientFullName,
                    TotalAmount = filteredDebtRecoveryData.Where(f => f.ContractNo == actionReqItem.Key).Sum(s => s.AmountDue),
                    FollowUpDate = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == actionReqItem.Key).FollowUpDate,
                    PromiseToPayDate = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == actionReqItem.Key).PromiseToPayDate,
                    isAccumilative = true
                };

                listOfDebt.Add(debtSummary);
            }

            return new OkObjectResult(new ResponseObject<DebtRecoveryData>(listOfDebt, Authorization));
        }

        // GET: /TblDebtRecoveryData/int (Single)
        [HttpGet("GetFollowUpData")]
        public async Task<IActionResult> GetFollowUpData(int debtRecoveryDataID, [FromHeader] string Authorization)
        {
            var debtCollectors = await _DebtCollectorsRepository.GetAll();
            var debtRecoveryData = await _DebtRecoveryDataRepository.GetAll();
            var filteredDebtRecoveryData = debtRecoveryData.Where(w => (w.AllocatedTo == debtRecoveryDataID || w.AllocatedBy == debtRecoveryDataID) && w.TransactionDate <= DateTime.Now).ToList();
            var nonAccumilativeRecoveryData = filteredDebtRecoveryData.Where(w => w.TransactionDate > DateTime.Now.AddMonths(-1)).ToList();

            var debtStatus = await _PrimaryStatusRepository.GetAll();
            var followUp = debtStatus.FirstOrDefault(f => f.Description.Contains("Follow")) == null ? null : debtStatus.FirstOrDefault(f => f.Description.Contains("Follow"));

            List<DebtRecoveryData> listOfDebt = new List<DebtRecoveryData>();

            foreach (var followUpItem in nonAccumilativeRecoveryData.Where(w => w.StatusID == followUp.Code).GroupBy(gb => gb.ContractNo))
            {
                int? collectorId = nonAccumilativeRecoveryData.FirstOrDefault(w => w.ContractNo == followUpItem.Key).AllocatedTo;
                string collectorName = debtCollectors.FirstOrDefault(f => f.PersonnelCode == collectorId).NameAndSurname;

                DebtRecoveryData debtSummary = new DebtRecoveryData()
                {
                    LastDateAssigned = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == followUpItem.Key).AllocatedDate,
                    ContractNo = followUpItem.Key,
                    DebtStatus = followUp.Description,
                    AssignedTo = collectorName,
                    ContractStatus = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == followUpItem.Key).ContractStatus,
                    ContractLevel = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == followUpItem.Key).VipLevel,
                    ClientName = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == followUpItem.Key).ClientFullName,
                    TotalAmount = nonAccumilativeRecoveryData.Where(f => f.ContractNo == followUpItem.Key).Sum(s => s.AmountDue),
                    FollowUpDate = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == followUpItem.Key).FollowUpDate,
                    PromiseToPayDate = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == followUpItem.Key).PromiseToPayDate,
                    isAccumilative = false
                };

                listOfDebt.Add(debtSummary);
            }

            foreach (var followUpItem in filteredDebtRecoveryData.Where(w => w.StatusID == followUp.Code).GroupBy(gb => gb.ContractNo))
            {
                int? collectorId = filteredDebtRecoveryData.FirstOrDefault(w => w.ContractNo == followUpItem.Key).AllocatedTo;
                string collectorName = debtCollectors.FirstOrDefault(f => f.PersonnelCode == collectorId)?.NameAndSurname;

                DebtRecoveryData debtSummary = new DebtRecoveryData()
                {
                    LastDateAssigned = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == followUpItem.Key).AllocatedDate,
                    ContractNo = followUpItem.Key,
                    DebtStatus = followUp.Description,
                    AssignedTo = collectorName,
                    ContractStatus = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == followUpItem.Key).ContractStatus,
                    ContractLevel = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == followUpItem.Key).VipLevel,
                    ClientName = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == followUpItem.Key).ClientFullName,
                    TotalAmount = filteredDebtRecoveryData.Where(f => f.ContractNo == followUpItem.Key).Sum(s => s.AmountDue),
                    FollowUpDate = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == followUpItem.Key).FollowUpDate,
                    PromiseToPayDate = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == followUpItem.Key).PromiseToPayDate,
                    isAccumilative = true
                };

                listOfDebt.Add(debtSummary);
            }

            return new OkObjectResult(new ResponseObject<DebtRecoveryData>(listOfDebt, Authorization));
        }

        // GET: /TblDebtRecoveryData/int (Single)
        [HttpGet("GetPromiseToPayData")]
        public async Task<IActionResult> GetPromiseToPayData(int debtRecoveryDataID, [FromHeader] string Authorization)
        {
            var debtCollectors = await _DebtCollectorsRepository.GetAll();
            var debtRecoveryData = await _DebtRecoveryDataRepository.GetAll();
            var filteredDebtRecoveryData = debtRecoveryData.Where(w => (w.AllocatedTo == debtRecoveryDataID || w.AllocatedBy == debtRecoveryDataID) && w.TransactionDate <= DateTime.Now && w.AmountDue > 0).ToList();
            var nonAccumilativeRecoveryData = filteredDebtRecoveryData.Where(w => w.TransactionDate > DateTime.Now.AddMonths(-1)).ToList();

            var debtStatus = await _PrimaryStatusRepository.GetAll();
            var ptp = debtStatus.FirstOrDefault(f => f.Description.Contains("Promise")) == null ? null : debtStatus.FirstOrDefault(f => f.Description.Contains("Promise"));

            List<DebtRecoveryData> listOfDebt = new List<DebtRecoveryData>();

            foreach (var ptpItem in nonAccumilativeRecoveryData.Where(w => w.StatusID == ptp.Code).GroupBy(gb => gb.ContractNo))
            {
                int? collectorId = nonAccumilativeRecoveryData.FirstOrDefault(w => w.ContractNo == ptpItem.Key).AllocatedTo;
                string collectorName = debtCollectors.FirstOrDefault(f => f.PersonnelCode == collectorId)?.NameAndSurname;

                DebtRecoveryData debtSummary = new DebtRecoveryData()
                {
                    LastDateAssigned = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == ptpItem.Key).AllocatedDate,
                    ContractNo = ptpItem.Key,
                    DebtStatus = ptp.Description,
                    AssignedTo = collectorName,
                    ContractStatus = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == ptpItem.Key).ContractStatus,
                    ContractLevel = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == ptpItem.Key).VipLevel,
                    ClientName = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == ptpItem.Key).ClientFullName,
                    TotalAmount = nonAccumilativeRecoveryData.Where(f => f.ContractNo == ptpItem.Key).Sum(s => s.AmountDue),
                    FollowUpDate = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == ptpItem.Key).FollowUpDate,
                    PromiseToPayDate = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == ptpItem.Key).PromiseToPayDate,
                    isAccumilative = false
                };

                listOfDebt.Add(debtSummary);
            }

            foreach (var ptpItem in filteredDebtRecoveryData.Where(w => w.StatusID == ptp.Code).GroupBy(gb => gb.ContractNo))
            {
                int? collectorId = filteredDebtRecoveryData.FirstOrDefault(w => w.ContractNo == ptpItem.Key).AllocatedTo;
                string collectorName = debtCollectors.FirstOrDefault(f => f.PersonnelCode == collectorId).NameAndSurname;

                DebtRecoveryData debtSummary = new DebtRecoveryData()
                {
                    LastDateAssigned = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == ptpItem.Key).AllocatedDate,
                    ContractNo = ptpItem.Key,
                    DebtStatus = ptp.Description,
                    AssignedTo = collectorName,
                    ContractStatus = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == ptpItem.Key).ContractStatus,
                    ContractLevel = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == ptpItem.Key).VipLevel,
                    ClientName = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == ptpItem.Key).ClientFullName,
                    TotalAmount = filteredDebtRecoveryData.Where(f => f.ContractNo == ptpItem.Key).Sum(s => s.AmountDue),
                    FollowUpDate = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == ptpItem.Key).FollowUpDate,
                    PromiseToPayDate = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == ptpItem.Key).PromiseToPayDate,
                    isAccumilative = true
                };

                listOfDebt.Add(debtSummary);
            }

            return new OkObjectResult(new ResponseObject<DebtRecoveryData>(listOfDebt, Authorization));
        }

        // GET: /TblDebtRecoveryData/int (Single)
        [HttpGet("GetNoPaymentData")]
        public async Task<IActionResult> GetNoPaymentData(int debtRecoveryDataID, [FromHeader] string Authorization)
        {
            var debtCollectors = await _DebtCollectorsRepository.GetAll();
            var debtRecoveryData = await _DebtRecoveryDataRepository.GetAll();
            var filteredDebtRecoveryData = debtRecoveryData.Where(w => (w.AllocatedTo == debtRecoveryDataID || w.AllocatedBy == debtRecoveryDataID) && w.TransactionDate <= DateTime.Now).ToList();
            var nonAccumilativeRecoveryData = filteredDebtRecoveryData.Where(w => w.TransactionDate > DateTime.Now.AddMonths(-1)).ToList();

            var debtStatus =  await _PrimaryStatusRepository.GetAll();
            var noPayment = debtStatus.FirstOrDefault(f => f.Description.Contains("Payment")) == null ? null : debtStatus.FirstOrDefault(f => f.Description.Contains("Payment"));

            List<DebtRecoveryData> listOfDebt = new List<DebtRecoveryData>();

            foreach (var noPaymentItem in nonAccumilativeRecoveryData.Where(w => w.StatusID == noPayment.Code).GroupBy(gb => gb.ContractNo))
            {
                int? collectorId = nonAccumilativeRecoveryData.FirstOrDefault(w => w.ContractNo == noPaymentItem.Key).AllocatedTo;
                string collectorName = debtCollectors.FirstOrDefault(f => f.PersonnelCode == collectorId)?.NameAndSurname;

                DebtRecoveryData noPaymentDebt = new DebtRecoveryData()
                {
                    LastDateAssigned = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == noPaymentItem.Key).AllocatedDate,
                    ContractNo = noPaymentItem.Key,
                    DebtStatus = noPayment.Description,
                    AssignedTo = collectorName,
                    ContractStatus = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == noPaymentItem.Key).ContractStatus,
                    ContractLevel = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == noPaymentItem.Key).VipLevel,
                    ClientName = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == noPaymentItem.Key).ClientFullName,
                    TotalAmount = nonAccumilativeRecoveryData.Where(f => f.ContractNo == noPaymentItem.Key).Sum(s => s.AmountDue),
                    FollowUpDate = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == noPaymentItem.Key).FollowUpDate,
                    PromiseToPayDate = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == noPaymentItem.Key).PromiseToPayDate,
                    isAccumilative = false
                };

                listOfDebt.Add(noPaymentDebt);
            }

            foreach (var noPaymentItem in filteredDebtRecoveryData.Where(w => w.StatusID == noPayment.Code).GroupBy(gb => gb.ContractNo))
            {
                int? collectorId = filteredDebtRecoveryData.FirstOrDefault(w => w.ContractNo == noPaymentItem.Key).AllocatedTo;
                string collectorName = debtCollectors.FirstOrDefault(f => f.PersonnelCode == collectorId)?.NameAndSurname; //debtCollectors.FirstOrDefault(f => f.PersonnelCode == collectorId).NameAndSurname;

                DebtRecoveryData noPaymentDebt = new DebtRecoveryData()
                {
                    LastDateAssigned = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == noPaymentItem.Key).AllocatedDate,
                    ContractNo = noPaymentItem.Key,
                    DebtStatus = noPayment.Description,
                    AssignedTo = collectorName,
                    ContractStatus = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == noPaymentItem.Key).ContractStatus,
                    ContractLevel = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == noPaymentItem.Key).VipLevel,
                    ClientName = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == noPaymentItem.Key).ClientFullName,
                    TotalAmount = filteredDebtRecoveryData.Where(f => f.ContractNo == noPaymentItem.Key).Sum(s => s.AmountDue),
                    FollowUpDate = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == noPaymentItem.Key).FollowUpDate,
                    PromiseToPayDate = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == noPaymentItem.Key).PromiseToPayDate,
                    isAccumilative = true
                };

                listOfDebt.Add(noPaymentDebt);
            }

            return new OkObjectResult(new ResponseObject<DebtRecoveryData>(listOfDebt, Authorization));
        }

        // GET: /TblDebtRecoveryData/int (Single)
        [HttpGet("GetCollectedDebtData")]
        public async Task<IActionResult> GetCollectedDebtData(int debtRecoveryDataID, [FromHeader] string Authorization)
        {
            var debtCollectors = await _DebtCollectorsRepository.GetAll();
            var debtRecoveryData = await _DebtRecoveryDataRepository.GetAll();
            var filteredDebtRecoveryData = debtRecoveryData.Where(w => (w.AllocatedTo == debtRecoveryDataID || w.AllocatedBy == debtRecoveryDataID) && w.TransactionDate <= DateTime.Now && w.CollectedAmount > 0).ToList();
            var nonAccumilativeRecoveryData = filteredDebtRecoveryData.Where(w => w.TransactionDate > DateTime.Now.AddMonths(-1)).ToList();

            var debtStatusList = await _PrimaryStatusRepository.GetAll();

            List<DebtRecoveryData> listOfDebt = new List<DebtRecoveryData>();

            foreach (var collectedItem in nonAccumilativeRecoveryData.GroupBy(gb => gb.ContractNo))
            {
                int? collectorId = nonAccumilativeRecoveryData.FirstOrDefault(w => w.ContractNo == collectedItem.Key).AllocatedTo;
                string collectorName = debtCollectors.FirstOrDefault(f => f.PersonnelCode == collectorId)?.NameAndSurname;
                var debtID = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == collectedItem.Key).StatusID;
                string debtStatus = debtStatusList.FirstOrDefault(f => f.Code == debtID).Description;

                DebtRecoveryData debtSummary = new DebtRecoveryData()
                {
                    LastDateAssigned = nonAccumilativeRecoveryData.OrderByDescending(ob => ob.TransactionDate).FirstOrDefault(f => f.ContractNo == collectedItem.Key).AllocatedDate,
                    ContractNo = collectedItem.Key,
                    DebtStatus = debtStatus,
                    AssignedTo = collectorName,
                    ContractStatus = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == collectedItem.Key).ContractStatus,
                    ContractLevel = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == collectedItem.Key).VipLevel,
                    ClientName = nonAccumilativeRecoveryData.FirstOrDefault(f => f.ContractNo == collectedItem.Key).ClientFullName,
                    TotalAmount = nonAccumilativeRecoveryData.Where(f => f.ContractNo == collectedItem.Key).Sum(s => s.CollectedAmount),
                    FollowUpDate = nonAccumilativeRecoveryData.OrderByDescending(ob => ob.TransactionDate).FirstOrDefault(f => f.ContractNo == collectedItem.Key).FollowUpDate,
                    PromiseToPayDate = nonAccumilativeRecoveryData.OrderByDescending(ob => ob.TransactionDate).FirstOrDefault(f => f.ContractNo == collectedItem.Key).PromiseToPayDate,
                    LastCollectedDate = nonAccumilativeRecoveryData.OrderByDescending(ob => ob.LastCollectedDate).FirstOrDefault(f => f.ContractNo == collectedItem.Key).LastCollectedDate,
                    isAccumilative = false
                };

                listOfDebt.Add(debtSummary);
            }

            foreach (var collectedItem in filteredDebtRecoveryData.GroupBy(gb => gb.ContractNo))
            {
                int? collectorId = filteredDebtRecoveryData.FirstOrDefault(w => w.ContractNo == collectedItem.Key).AllocatedTo;
                string collectorName = debtCollectors.FirstOrDefault(f => f.PersonnelCode == collectorId)?.NameAndSurname;
                var debtID = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == collectedItem.Key).StatusID;
                string debtStatus = debtStatusList.FirstOrDefault(f => f.Code == debtID).Description;

                DebtRecoveryData debtSummary = new DebtRecoveryData()
                {
                    LastDateAssigned = filteredDebtRecoveryData.OrderByDescending(ob => ob.TransactionDate).FirstOrDefault(f => f.ContractNo == collectedItem.Key).AllocatedDate,
                    ContractNo = collectedItem.Key,
                    DebtStatus = debtStatus,
                    AssignedTo = collectorName,
                    ContractStatus = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == collectedItem.Key).ContractStatus,
                    ContractLevel = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == collectedItem.Key).VipLevel,
                    ClientName = filteredDebtRecoveryData.FirstOrDefault(f => f.ContractNo == collectedItem.Key).ClientFullName,
                    TotalAmount = filteredDebtRecoveryData.Where(f => f.ContractNo == collectedItem.Key).Sum(s => s.CollectedAmount),
                    FollowUpDate = filteredDebtRecoveryData.OrderByDescending(ob => ob.TransactionDate).FirstOrDefault(f => f.ContractNo == collectedItem.Key).FollowUpDate,
                    PromiseToPayDate = filteredDebtRecoveryData.OrderByDescending(ob => ob.TransactionDate).FirstOrDefault(f => f.ContractNo == collectedItem.Key).PromiseToPayDate,
                    LastCollectedDate = filteredDebtRecoveryData.OrderByDescending(ob => ob.LastCollectedDate).FirstOrDefault(f => f.ContractNo == collectedItem.Key).LastCollectedDate,
                    isAccumilative = true
                };

                listOfDebt.Add(debtSummary);
            }

            return new OkObjectResult(new ResponseObject<DebtRecoveryData>(listOfDebt, Authorization));
        }

        [HttpGet("GetAssignedData")]
        public async Task<IActionResult> GetAssignedData(int managerID, [FromHeader] string Authorization)
        {
            var debtRecoveryData = await _DebtRecoveryDataRepository.GetAll();
            var filteredDebtRecoveryData = debtRecoveryData.Where(w => w.AllocatedBy == managerID && w.TransactionDate <= DateTime.Now).ToList();
            return new OkObjectResult(new ResponseObject<TblDebtRecoveryData>(filteredDebtRecoveryData, Authorization));
        }

        // GET: /TblDebtRecoveryData/string (Single)
        [HttpGet("GetPaymentPlan")]
        public async Task<IActionResult> GetPaymentPlan(string contractNo, [FromHeader] string Authorization)
        {
            var debtRecoveryData = await _DebtRecoveryDataRepository.GetAll();
            var filteredDebtRecoveryData = debtRecoveryData.Where(w => w.ContractNo.Contains(contractNo) && w.TransactionDate <= DateTime.Now).ToList();
            return new OkObjectResult(new ResponseObject<TblDebtRecoveryData>(filteredDebtRecoveryData, Authorization));
        }

        // POST: /TblDebtRecoveryData/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblDebtRecoveryData debtRecoveryData, [FromHeader] string Authorization)
        {
            using (var scope = new TransactionScope())
            {
                _DebtRecoveryDataRepository.Create(debtRecoveryData);
                _DebtRecoveryDataRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = debtRecoveryData.Id }, debtRecoveryData);
            };
        }

        // PUT: /TblDebtRecoveryData/int
        [HttpPut("UpdateDebt")]
        public async Task<IActionResult> UpdateDebt([FromBody] AssignDebtToCollector UpdateDebtLines, [FromHeader] string Authorization)
        {
            var debtStatus = await _PrimaryStatusRepository.GetAll();
            var debtRecoveryData = await _DebtRecoveryDataRepository.GetAll();
            int actionReqCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Action")) == null ? 4 : debtStatus.FirstOrDefault(f => f.Description.Contains("Action")).Code;            
            var filteredDebtRecoveryData = debtRecoveryData.Where(w => w.ContractNo == UpdateDebtLines.ContractNo).ToList();

            if (UpdateDebtLines != null)
            {
                if (UpdateDebtLines.AllocatedTo != null)
                {
                    foreach (var debtItem in filteredDebtRecoveryData)
                    {
                        var regionId = debtItem.Region;
                        debtItem.AllocatedTo = UpdateDebtLines.AllocatedTo;
                        debtItem.AllocatedBy = UpdateDebtLines.AllocatedBy;
                        debtItem.AllocatedDate = DateTime.Now;
                        debtItem.Region = regionId;
                        debtItem.StatusID = actionReqCode;
                        _DebtRecoveryDataRepository.Update(debtItem);
                    }
                }
                return new OkObjectResult(new ResponseObject<TblDebtRecoveryData>(debtRecoveryData, Authorization));
            }
            return new NoContentResult();
        }

        // PUT: /TblDebtRecoveryData/int
        [HttpPut("ReAssignDebt")]
        public async Task<IActionResult> ReAssignDebt([FromBody] AssignDebtToCollector UpdateDebtLines, [FromHeader] string Authorization)
        {
            var debtStatus = await _PrimaryStatusRepository.GetAll();
            int actionReqCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Action")) == null ? 4 : debtStatus.FirstOrDefault(f => f.Description.Contains("Action")).Code;
            var debtRecoveryData = await _DebtRecoveryDataRepository.GetAll();
            var filteredDebtRecoveryData = debtRecoveryData.Where(w => w.ContractNo == UpdateDebtLines.ContractNo && w.CollectedAmount == 0 && !w.Discontinued).ToList();

            if (UpdateDebtLines != null)
            {
                foreach (var debtItem in filteredDebtRecoveryData)
                {
                    debtItem.AllocatedTo = UpdateDebtLines.AllocatedTo;
                    debtItem.AllocatedBy = UpdateDebtLines.AllocatedBy;
                    debtItem.AllocatedDate = DateTime.Now;
                    debtItem.StatusID = actionReqCode;
                    _DebtRecoveryDataRepository.Update(debtItem);
                }
               return new OkObjectResult(new ResponseObject<TblDebtRecoveryData>(debtRecoveryData, Authorization));
            }
            return new NoContentResult();
        }

        // PUT: /TblDebtRecoveryData/int
        [HttpPut("")]
        public async Task<IActionResult> Put([FromBody] TblDebtRecoveryData debtRecoveryData, [FromHeader] string Authorization)
        {
            var debtStatus = await _PrimaryStatusRepository.GetAll();
            int actionReqCode = debtStatus.FirstOrDefault(f => f.Description.Contains("Action")) == null ? 4 : debtStatus.FirstOrDefault(f => f.Description.Contains("Action")).Code;
            //int actionReqCode = debtStatus.Where(f => f.Code == debtRecoveryData.StatusID).FirstOrDefault().Code;

            if (debtRecoveryData != null)
            {
                using (var scope = new TransactionScope())
                {
                    if(debtRecoveryData.AllocatedTo != null && debtRecoveryData.AllocatedDate == null)
                    {
                        debtRecoveryData.AllocatedDate = DateTime.Now;
                        debtRecoveryData.StatusID = actionReqCode;
                    }
                    _DebtRecoveryDataRepository.Update(debtRecoveryData);
                    scope.Complete();
                    return new OkObjectResult(new ResponseObject<TblDebtRecoveryData>(debtRecoveryData, Authorization));
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblDebtRecoveryData/int
        [HttpDelete("{debtRecoveryDataID}")]
        public IActionResult Delete(int debtRecoveryDataID, [FromHeader] string Authorization)
        {
            _DebtRecoveryDataRepository.Delete(debtRecoveryDataID);
            return new OkResult();
        }
    }
}
