using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using DebtRecoveryPlatform.Helpers;
using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Models.ResponseObject;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DebtRecoveryPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TblDebtStatusController : ControllerBase
    {
        private readonly RepositoryInterface<TblDebtStatus> _DebtStatusRepository;

        public TblDebtStatusController(RepositoryInterface<TblDebtStatus> DebtStatusRepository)
        {
            _DebtStatusRepository = DebtStatusRepository;
        }

        // GET: /TblDebtStatus/ (All)
        public async Task<IActionResult> Get([FromHeader] string Authorization)
        {
            var debtStatus = await _DebtStatusRepository.GetAll();
            return new OkObjectResult(new ResponseObject<TblDebtStatus>(debtStatus, Authorization));
        }

        // GET: /TblDebtStatus/int (Single)
        [HttpGet("{debtStatusID}", Name = "GetDebtStatus")]
        public async Task<IActionResult> Get(int debtStatusID, [FromHeader] string Authorization)
        {
            var debtStatus = await _DebtStatusRepository.Get(debtStatusID);
            return new OkObjectResult(new ResponseObject<TblDebtStatus>(debtStatus, Authorization));
        }

        // POST: /TblDebtStatus/Create
        [HttpPost("Create")]
        public IActionResult Create([FromBody] TblDebtStatus debtStatus, [FromHeader] string Authorization)
        {
            using (var scope = new TransactionScope())
            {
                _DebtStatusRepository.Create(debtStatus);
                _DebtStatusRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = debtStatus.Id }, debtStatus);
            };
        }

        // PUT: /TblDebtStatus/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblDebtStatus debtStatus, [FromHeader] string Authorization)
        {
            if (debtStatus != null)
            {
                using (var scope = new TransactionScope())
                {
                    _DebtStatusRepository.Update(debtStatus);
                    scope.Complete();
                    return new OkObjectResult(new ResponseObject<TblDebtStatus>(debtStatus, Authorization));
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblDebtStatus/int
        [HttpDelete("{debtStatusID}")]
        public IActionResult Delete(int debtStatusID)
        {
            _DebtStatusRepository.Delete(debtStatusID);
            return new OkResult();
        }
    }
}
