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
    public class TblDebtAllocationHistoryController : Controller
    {
        private readonly RepositoryInterface<TblDebtAllocationHistory> _DebtAllocationHistoryRepository;
        private readonly RepositoryInterface<TblDebtCollectors> _DebtCollectorsRepository;
        private readonly RepositoryInterface<TblDebtRecoveryData> _DebtRecoveryDataRepository;

        public TblDebtAllocationHistoryController(RepositoryInterface<TblDebtRecoveryData> DebtRecoveryDataRepository, RepositoryInterface<TblDebtAllocationHistory> DebtAllocationHistoryRepository, RepositoryInterface<TblDebtCollectors> DebtCollectorsRepository)
        {
            _DebtRecoveryDataRepository = DebtRecoveryDataRepository;
            _DebtAllocationHistoryRepository = DebtAllocationHistoryRepository;
            _DebtCollectorsRepository = DebtCollectorsRepository;
        }

        // GET: /TblDebtAllocationHistory/ (All)
        public async Task<IActionResult> Get([FromHeader] string Authorization)
        {
            var debtAllocationHistory = await _DebtAllocationHistoryRepository.GetAll();
            var debtCollectors = await _DebtCollectorsRepository.GetAll();
            var debtRecoveryData =  await  _DebtRecoveryDataRepository.GetAll();

            List<TblDebtAllocationHistory> debtHistoryList = debtAllocationHistory.ToList();
            List<AllocationHistory> HistoricalAllocationsList = new List<AllocationHistory>();

            foreach (var historyItem in debtHistoryList)
            {
                string collectorName = debtCollectors.Where(w => w.PersonnelCode == historyItem.AllocatedTo).FirstOrDefault().NameAndSurname;
                string managerName = debtCollectors.Where(w => w.PersonnelCode == historyItem.AllocatedBy).FirstOrDefault().NameAndSurname;
                string contractNo = debtRecoveryData.Where(w => w.Id == historyItem.DebtItemID).FirstOrDefault().ContractNo;
                string BookingRef = debtRecoveryData.Where(w => w.Id == historyItem.DebtItemID).FirstOrDefault().BookingRef;

                AllocationHistory HistoryLine = new AllocationHistory()
                {
                    ContractNo = contractNo,
                    BookingRef = BookingRef,
                    AllocatedBy = managerName,
                    AllocatedTo = collectorName,
                    DateAllocated = historyItem.DateAllocated
                };

                HistoricalAllocationsList.Add(HistoryLine);
            }

            return new OkObjectResult(new ResponseObject<AllocationHistory>(HistoricalAllocationsList, Authorization));
        }

        // GET: /TblDebtAllocationHistory/int (Single)
        [HttpGet("GetDebtAllocationHistory")]
        public async Task<IActionResult> Get(int debtManagerID, [FromHeader] string Authorization, string bookingRef)
        {
            var debtAllocationHistory = await _DebtAllocationHistoryRepository.GetAll();
            var debtAllocationHistoryList = debtAllocationHistory.Where(w => w.AllocatedBy == debtManagerID).ToList();
            var debtCollectors = await _DebtCollectorsRepository.GetAll();
            var debtRecoveryData = await _DebtRecoveryDataRepository.GetAll();

            List<AllocationHistory> HistoricalAllocationsList = new List<AllocationHistory>();

            foreach (var historyItem in debtAllocationHistoryList)
            {
                string collectorName = debtCollectors.Where(w => w.PersonnelCode == historyItem.AllocatedTo).FirstOrDefault()?.NameAndSurname;
                string managerName = debtCollectors.Where(w => w.PersonnelCode == historyItem.AllocatedBy).FirstOrDefault()?.NameAndSurname;
                string contractNo = debtRecoveryData.Where(w => w.Id == historyItem.DebtItemID).FirstOrDefault()?.ContractNo;
                bookingRef = debtRecoveryData.Where(w => w.Id == historyItem.DebtItemID).FirstOrDefault()?.BookingRef;

            if (bookingRef == null)
                {
                    continue;
                }

                AllocationHistory HistoryLine = new AllocationHistory()
                {
                    ContractNo = contractNo,
                    BookingRef = bookingRef,
                    AllocatedBy = managerName,
                    AllocatedTo = collectorName,
                    DateAllocated = historyItem.DateAllocated
                };

                HistoricalAllocationsList.Add(HistoryLine);
            }

            return new OkObjectResult(new ResponseObject<AllocationHistory>(HistoricalAllocationsList, Authorization));
        }

        // POST: /TblDebtAllocationHistory/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblDebtAllocationHistory debtAllocationHistory, [FromHeader] string Authorization)
        {
            using (var scope = new TransactionScope())
            {
                debtAllocationHistory.DateAllocated = DateTime.Now;
                _DebtAllocationHistoryRepository.Create(debtAllocationHistory);
                _DebtAllocationHistoryRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = debtAllocationHistory.Id }, debtAllocationHistory);
            };
        }

        // PUT: /TblDebtAllocationHistory/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblDebtAllocationHistory debtAllocationHistory, [FromHeader] string Authorization)
        {
            if (debtAllocationHistory != null)
            {
                using (var scope = new TransactionScope())
                {
                    _DebtAllocationHistoryRepository.Update(debtAllocationHistory);
                    scope.Complete();
                    return new OkObjectResult(new ResponseObject<TblDebtAllocationHistory>(debtAllocationHistory, Authorization));
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblDebtAllocationHistory/int
        [HttpDelete("{debtAllocationHistoryID}")]
        public IActionResult Delete(int debtAllocationHistoryID, [FromHeader] string Authorization)
        {
            _DebtAllocationHistoryRepository.Delete(debtAllocationHistoryID);
            return new OkResult();
        }
    }
}
