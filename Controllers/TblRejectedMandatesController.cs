using DebtRecoveryPlatform.Helpers;
using DebtRecoveryPlatform.Models;
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
    public class TblRejectedMandatesController : ControllerBase
    {
        private readonly RepositoryInterface<TblRejectedMandates> _RejectedMandatesRepository;

        public TblRejectedMandatesController(RepositoryInterface<TblRejectedMandates> RejectedMandatesRepository)
        {
            _RejectedMandatesRepository = RejectedMandatesRepository;
        }

        // GET: /TblRejectedMandates/ (All)
        public IActionResult Get()
        {
            var rejectedMandates = _RejectedMandatesRepository.GetAll();
            return new OkObjectResult(rejectedMandates);
        }

        // GET: /TblRejectedMandates/int (Single)
        [HttpGet("{rejectedMandatesID}", Name = "GetRejectedMandates")]
        public IActionResult Get(int rejectedMandatesID)
        {
            var rejectedMandates = _RejectedMandatesRepository.Get(rejectedMandatesID);
            return new OkObjectResult(rejectedMandates);
        }

        // POST: /TblRejectedMandates/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblRejectedMandates rejectedMandates)
        {
            using (var scope = new TransactionScope())
            {
                _RejectedMandatesRepository.Create(rejectedMandates);
                _RejectedMandatesRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = rejectedMandates.Id }, rejectedMandates);
            };
        }

        // PUT: /TblRejectedMandates/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblRejectedMandates rejectedMandates)
        {
            if (rejectedMandates != null)
            {
                using (var scope = new TransactionScope())
                {
                    _RejectedMandatesRepository.Update(rejectedMandates);
                    scope.Complete();
                    return new OkObjectResult(rejectedMandates);
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblRejectedMandates/int
        [HttpDelete("{rejectedMandatesID}")]
        public IActionResult Delete(int rejectedMandatesID)
        {
            _RejectedMandatesRepository.Delete(rejectedMandatesID);
            return new OkResult();
        }
    }
}
