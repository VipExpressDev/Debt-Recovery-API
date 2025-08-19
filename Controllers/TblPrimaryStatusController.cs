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
    public class TblPrimaryStatusController : ControllerBase
    {
        private readonly RepositoryInterface<TblPrimaryStatus> _PrimaryStatusRepository;

        public TblPrimaryStatusController(RepositoryInterface<TblPrimaryStatus> PrimaryStatusRepository)
        {
            _PrimaryStatusRepository = PrimaryStatusRepository;
        }

        // GET: /TblPrimaryStatus/ (All)
        public async Task<IActionResult> Get([FromHeader] string Authorization)
        {
            var primaryStatus = await _PrimaryStatusRepository.GetAll();
            return new OkObjectResult(new ResponseObject<TblPrimaryStatus>(primaryStatus, Authorization));
        }

        // GET: /TblPrimaryStatus/int (Single)
        [HttpGet("{primaryStatusID}", Name = "GetPrimaryStatus")]
        public async Task<IActionResult> Get(int primaryStatusID, [FromHeader] string Authorization)
        {
            var primaryStatus = await _PrimaryStatusRepository.GetAll();
            var status = primaryStatus.Where(w => w.Code == primaryStatusID).FirstOrDefault();
            return new OkObjectResult(new ResponseObject<TblPrimaryStatus>(status, Authorization));
        }

        // POST: /TblPrimaryStatus/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblPrimaryStatus primaryStatus, [FromHeader] string Authorization)
        {
            using (var scope = new TransactionScope())
            {
                _PrimaryStatusRepository.Create(primaryStatus);
                _PrimaryStatusRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = primaryStatus.Id }, primaryStatus);
            };
        }

        // PUT: /TblPrimaryStatus/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblPrimaryStatus primaryStatus, [FromHeader] string Authorization)
        {
            if (primaryStatus != null)
            {
                using (var scope = new TransactionScope())
                {
                    _PrimaryStatusRepository.Update(primaryStatus);
                    scope.Complete();
                    return new OkObjectResult(new ResponseObject<TblPrimaryStatus>(primaryStatus, Authorization));
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblPrimaryStatus/int
        [HttpDelete("{primaryStatusID}")]
        public IActionResult Delete(int primaryStatusID)
        {
            _PrimaryStatusRepository.Delete(primaryStatusID);
            return new OkResult();
        }
    }
}
