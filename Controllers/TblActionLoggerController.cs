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
    public class TblActionLoggerController : ControllerBase
    {
        private readonly RepositoryInterface<TblActionLogger> _ActionLoggerRepository;
        private readonly RepositoryInterface<TblDebtCollectors> _DebtCollectorsRepository;

        public TblActionLoggerController(RepositoryInterface<TblActionLogger> ActionLoggerRepository, RepositoryInterface<TblDebtCollectors> DebtCollectorsRepository)
        {
            _ActionLoggerRepository = ActionLoggerRepository;
            _DebtCollectorsRepository = DebtCollectorsRepository;
        }

        // GET: /TblActionLogger/ (All)
        public async Task<IActionResult> Get([FromHeader] string Authorization)
        {
            var actionLogger = await _ActionLoggerRepository.GetAll();
            var debtCollectors = await      _DebtCollectorsRepository.GetAll();

            List<TblActionLogger> actionLoggerList = actionLogger.ToList();
            List<ActionLogger> allActionsLogged = new List<ActionLogger>();

            foreach (var action in actionLoggerList)
            {
                string collectorName = debtCollectors.Where(w => w.PersonnelCode == action.CreatedBy).FirstOrDefault().NameAndSurname;

                ActionLogger actionLine = new ActionLogger()
                {
                    Action = action.Action,
                    View = action.View,
                    ActionedBy = collectorName
                };

                allActionsLogged.Add(actionLine);
            }
            return new OkObjectResult(new ResponseObject<ActionLogger>(allActionsLogged, Authorization));
        }

        // GET: /TblActionLogger/int (Single)
        [HttpGet("GetUserActions")]
        public async Task<IActionResult> GetUserActions(int selectedUserID, [FromHeader] string Authorization)
        {
            var actionLogger = await _ActionLoggerRepository.GetAll();
            var debtCollectors = await _DebtCollectorsRepository.GetAll();

            List<TblActionLogger> actionLoggerList = actionLogger.Where(w => w.CreatedBy == selectedUserID).ToList();
            List<ActionLogger> allActionsLogged = new List<ActionLogger>();

            foreach (var action in actionLoggerList)
            {
                string collectorName = debtCollectors.Where(w => w.PersonnelCode == action.CreatedBy).FirstOrDefault().NameAndSurname;

                ActionLogger actionLine = new ActionLogger()
                {
                    Action = action.Action,
                    View = action.View,
                    ActionedBy = collectorName
                };

                allActionsLogged.Add(actionLine);
            }
            return new OkObjectResult(new ResponseObject<ActionLogger>(allActionsLogged, Authorization));
        }

        // POST: /TblActionLogger/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblActionLogger actionLogger, [FromHeader] string Authorization)
        {
            using (var scope = new TransactionScope())
            {
                actionLogger.CreatedDate = DateTime.Now;
                _ActionLoggerRepository.Create(actionLogger);
                _ActionLoggerRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = actionLogger.Id }, actionLogger);
            };
        }

        // PUT: /TblActionLogger/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblActionLogger actionLogger, [FromHeader] string Authorization)
        {
            if (actionLogger != null)
            {
                using (var scope = new TransactionScope())
                {
                    _ActionLoggerRepository.Update(actionLogger);
                    scope.Complete();
                    return new OkObjectResult(new ResponseObject<TblActionLogger>(actionLogger, Authorization));
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblActionLogger/int
        [HttpDelete("{actionLoggerID}")]
        public IActionResult Delete(int actionLoggerID, [FromHeader] string Authorization)
        {
            _ActionLoggerRepository.Delete(actionLoggerID);
            return new OkResult();
        }
    }
}
