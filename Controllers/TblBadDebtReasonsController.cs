using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using DebtRecoveryPlatform.Helpers;
using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Models.ResponseObject;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DebtRecoveryPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TblBadDebtReasonsController : ControllerBase
    {
        private readonly RepositoryInterface<TblBadDebtReasons> _BadDebtReasonsRepository;

        public TblBadDebtReasonsController(RepositoryInterface<TblBadDebtReasons> PrimaryStatusRepository)
        {
            _BadDebtReasonsRepository = PrimaryStatusRepository;
        }

        // GET: /TblBadDebtReasons/ (All)
        public async Task<IActionResult> Get([FromHeader] string Authorization)
        {
            var badDebtReasons = await _BadDebtReasonsRepository.GetAll();
            return new OkObjectResult(new ResponseObject<TblBadDebtReasons>(badDebtReasons, Authorization));
        }

        // GET: /TblBadDebtReasons/int (Single)
        [HttpGet("GetBadDebtReasons")]
        public async Task<IActionResult> Get(int badDebtReasonsID, [FromHeader] string Authorization)
        {
            var badDebtReasons = await _BadDebtReasonsRepository.GetAll();
            var status = badDebtReasons.Where(w => w.Code == badDebtReasonsID).FirstOrDefault();
            return new OkObjectResult(new ResponseObject<TblBadDebtReasons>(status, Authorization));
        }

        // POST: /TblBadDebtReasons/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblBadDebtReasons badDebtReasons, [FromHeader] string Authorization)
        {
            using (var scope = new TransactionScope())
            {
                _BadDebtReasonsRepository.Create(badDebtReasons);
                _BadDebtReasonsRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = badDebtReasons.Id }, badDebtReasons);
            };
        }

        // PUT: /TblBadDebtReasons/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblBadDebtReasons badDebtReasons, [FromHeader] string Authorization)
        {
            if (badDebtReasons != null)
            {
                using (var scope = new TransactionScope())
                {
                    _BadDebtReasonsRepository.Update(badDebtReasons);
                    scope.Complete();
                    return new OkObjectResult(new ResponseObject<TblBadDebtReasons>(badDebtReasons, Authorization));
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblBadDebtReasons/int
        [HttpDelete("{badDebtReasonsID}")]
        public IActionResult Delete(int badDebtReasonsID)
        {
            _BadDebtReasonsRepository.Delete(badDebtReasonsID);
            return new OkResult();
        }
    }
}
