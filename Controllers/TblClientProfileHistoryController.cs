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
    public class TblClientProfileHistoryController : ControllerBase
    {
        private readonly RepositoryInterface<TblClientProfileHistory> _ClientProfileHistoryRepository;
        private readonly RepositoryInterface<TblDebtCollectors> _DebtCollectorsRepository;
        private readonly RepositoryInterface<TblPrimaryStatus> _PrimaryStatusRepository;

        public TblClientProfileHistoryController(RepositoryInterface<TblClientProfileHistory> ClientProfileHistoryRepository, RepositoryInterface<TblDebtCollectors> DebtCollectorsRepository, RepositoryInterface<TblPrimaryStatus> PrimaryStatusRepository)
        {
            _ClientProfileHistoryRepository = ClientProfileHistoryRepository;
            _DebtCollectorsRepository = DebtCollectorsRepository;
            _PrimaryStatusRepository = PrimaryStatusRepository;
        }

        // GET: /TblClientProfileHistory/ (All)
        public async Task<IActionResult> Get([FromHeader] string Authorization)
        {
            var clientProfileHistory = await _ClientProfileHistoryRepository.GetAll();
            return new OkObjectResult(new ResponseObject<TblClientProfileHistory>(clientProfileHistory, Authorization));
        }

        // GET: /TblClientProfileHistory/int (Single)
        [HttpGet(Name = "GetClientProfileHistory")]
        public async Task<IActionResult> Get(string bookingRef, [FromHeader] string Authorization)
        {
            var allDebtCollectors = await _DebtCollectorsRepository.GetAll();
            var debtStatusses = await _PrimaryStatusRepository.GetAll();
            var clientProfileHistory = await _ClientProfileHistoryRepository.GetAll();
            var collectorHistory = clientProfileHistory.Where(w => w.BookingRef.Contains(bookingRef)).OrderByDescending(obd => obd.CreatedDate).ToList();

            List<ClientProfileHistory> debtPerformanceList = new List<ClientProfileHistory>();

            foreach (var history in collectorHistory)
            {
                string collectorName = allDebtCollectors.First(f => f.PersonnelCode == history.ActionedByID).NameAndSurname;
                string status = debtStatusses.First(f => f.Code == history.StatusID).Description;

                ClientProfileHistory historyItem = new ClientProfileHistory()
                {
                    ContractNo = history.ContractNo,
                    BookingRef = history.BookingRef,
                    ActionedBy = collectorName,
                    Status = status,
                    DateActioned = history.DateActioned,
                    StatusDate = history.StatusDate,
                    Comment = history.Comment
                };

                debtPerformanceList.Add(historyItem);
            }
            return new OkObjectResult(new ResponseObject<ClientProfileHistory>(debtPerformanceList, Authorization));
        }

        // POST: /TblClientProfileHistory/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblClientProfileHistory clientProfileHistory, [FromHeader] string Authorization)
        {
            using (var scope = new TransactionScope())
            {
                clientProfileHistory.CreatedDate = DateTime.Now;
                clientProfileHistory.DateActioned = DateTime.Now;
                    _ClientProfileHistoryRepository.Create(clientProfileHistory);
                _ClientProfileHistoryRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = clientProfileHistory.Id }, clientProfileHistory);
            };
        }

        // PUT: /TblClientProfileHistory/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblClientProfileHistory clientProfileHistory, [FromHeader] string Authorization)
        {
            if (clientProfileHistory != null)
            {
                using (var scope = new TransactionScope())
                {
                    _ClientProfileHistoryRepository.Update(clientProfileHistory);
                    scope.Complete();
                    return new OkObjectResult(new ResponseObject<TblClientProfileHistory>(clientProfileHistory, Authorization));
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblClientProfileHistory/int
        [HttpDelete("{clientProfileHistoryID}")]
        public IActionResult Delete(int clientProfileHistoryID)
        {
            _ClientProfileHistoryRepository.Delete(clientProfileHistoryID);
            return new OkResult();
        }
    }
}
