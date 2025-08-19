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
    public class TblActionedReminderController : ControllerBase
    {
        private readonly RepositoryInterface<TblActionedReminder> _ActionedReminderRepository;
        private readonly RepositoryInterface<TblDebtCollectors> _DebtCollectorsRepository;
        private readonly RepositoryInterface<TblPrimaryStatus> _PrimaryStatusRepository;

        public TblActionedReminderController(RepositoryInterface<TblActionedReminder> ActionedReminderRepository, RepositoryInterface<TblDebtCollectors> DebtCollectorsRepository, RepositoryInterface<TblPrimaryStatus> PrimaryStatusRepository)
        {
            _ActionedReminderRepository = ActionedReminderRepository;
            _DebtCollectorsRepository = DebtCollectorsRepository;
            _PrimaryStatusRepository = PrimaryStatusRepository;
        }

        // GET: /TblActionedReminder/ (All)
        public async Task<IActionResult> Get([FromHeader] string Authorization)
        {
            var actionedReminder = await _ActionedReminderRepository.GetAll();
            return new OkObjectResult(new ResponseObject<TblActionedReminder>(actionedReminder, Authorization));
        }

        // GET: /TblActionedReminder/int (Single)
        [HttpGet("{userID}", Name = "GetActionedReminder")]
        public async Task<IActionResult> Get(int userID, [FromHeader] string Authorization)
        {
            var debtStatusses = await _PrimaryStatusRepository.GetAll();
            var reminderList = await _ActionedReminderRepository.GetAll();
            var applicableReminderList = reminderList.Where(w => w.ActionedByID == userID || w.ManagerID == userID).ToList();

            int actionReqCode = debtStatusses.FirstOrDefault(f => f.Description.Contains("Action")) == null ? 0 : debtStatusses.FirstOrDefault(f => f.Description.Contains("Action")).Code;
            int followUpCode = debtStatusses.FirstOrDefault(f => f.Description.Contains("Follow")) == null ? 0 : debtStatusses.FirstOrDefault(f => f.Description.Contains("Follow")).Code;
            int ptpCode = debtStatusses.FirstOrDefault(f => f.Description.Contains("Promise")) == null ? 0 : debtStatusses.FirstOrDefault(f => f.Description.Contains("Promise")).Code;

            var debtCollectors =await  _DebtCollectorsRepository.GetAll();
            List<DebtReminder> DebtReminderList = new List<DebtReminder>();

            foreach (var reminderItem in applicableReminderList)
            {
                string collectorName = debtCollectors.Where(w => w.PersonnelCode == reminderItem.ActionedByID).FirstOrDefault()?.NameAndSurname;
                string managerName = debtCollectors.Where(w => w.PersonnelCode == reminderItem.ManagerID).FirstOrDefault()?.NameAndSurname;

                if(reminderItem.ReminderTypeID == followUpCode && reminderItem.ReminderDate <= DateTime.Now.Date)
                {
                    DebtReminder HistoryLine = new DebtReminder()
                    {
                        ContractNo = reminderItem.ContractNo,
                        ManagerName = managerName,
                        CollectorName = collectorName,
                        ReminderDate = reminderItem.ReminderDate,
                        ReminderType = debtStatusses.Single(s => s.Code == reminderItem.ReminderTypeID).Description
                    };

                    DebtReminderList.Add(HistoryLine);
                }
                else if(reminderItem.ReminderTypeID == ptpCode && reminderItem.ReminderDate < DateTime.Now.Date)
                {
                    DebtReminder HistoryLine = new DebtReminder()
                    {
                        ContractNo = reminderItem.ContractNo,
                        ManagerName = managerName,
                        CollectorName = collectorName,
                        ReminderDate = reminderItem.ReminderDate,
                        ReminderType = debtStatusses.Single(s => s.Code == reminderItem.ReminderTypeID).Description
                    };

                    DebtReminderList.Add(HistoryLine);
                }
            }

            return new OkObjectResult(new ResponseObject<DebtReminder>(DebtReminderList, Authorization));
        }

        // POST: /TblActionedReminder/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Post([FromBody] TblActionedReminder actionedReminder, [FromHeader] string Authorization)
        {
            var reminderList = await _ActionedReminderRepository.GetAll();
            var updateRemindersList = reminderList.Where(w => w.ContractNo == actionedReminder.ContractNo).ToList();

            if(updateRemindersList.Count() > 0)
            {
                foreach(var reminder in updateRemindersList)
                {
                    reminder.ReminderDate = actionedReminder.ReminderDate;
                    reminder.ReminderTypeID = actionedReminder.ReminderTypeID;

                    using (var scope = new TransactionScope())
                    {
                        _ActionedReminderRepository.Update(reminder);
                        scope.Complete();
                    }
                }

                return new OkObjectResult(new ResponseObject<TblActionedReminder>(actionedReminder, Authorization));
            }
            else
            {
                using (var scope = new TransactionScope())
                {
                    actionedReminder.CreatedDate = DateTime.Now;
                    _ActionedReminderRepository.Create(actionedReminder);
                    _ActionedReminderRepository.Save();
                    scope.Complete();
                    return CreatedAtAction(nameof(Get), new { id = actionedReminder.Id }, actionedReminder);
                };
            }
        }

        // PUT: /TblActionedReminder/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblActionedReminder actionedReminder, [FromHeader] string Authorization)
        {
            if (actionedReminder != null)
            {
                using (var scope = new TransactionScope())
                {
                    _ActionedReminderRepository.Update(actionedReminder);
                    scope.Complete();
                    return new OkObjectResult(new ResponseObject<TblActionedReminder>(actionedReminder, Authorization));
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblActionedReminder/int
        [HttpDelete("{actionedReminderID}")]
        public IActionResult Delete(int actionedReminderID, [FromHeader] string Authorization)
        {
            _ActionedReminderRepository.Delete(actionedReminderID);
            return new OkResult();
        }
    }
}
