using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using DebtRecoveryPlatform.Helpers;
using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DebtRecoveryPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TblSecondaryStatusController : ControllerBase
    {
        private readonly RepositoryInterface<TblSecondaryStatus> _SecondaryStatusRepository;

        public TblSecondaryStatusController(RepositoryInterface<TblSecondaryStatus> SecondaryStatusRepository)
        {
            _SecondaryStatusRepository = SecondaryStatusRepository;
        }

        // GET: /TblSecondaryStatus/ (All)
        public IActionResult Get()
        {
            var secondaryStatus = _SecondaryStatusRepository.GetAll();
            return new OkObjectResult(secondaryStatus);
        }

        // GET: /TblSecondaryStatus/int (Single)
        [HttpGet("{secondaryStatusID}", Name = "GetSecondaryStatus")]
        public IActionResult Get(int secondaryStatusID)
        {
            var secondaryStatus = _SecondaryStatusRepository.Get(secondaryStatusID);
            return new OkObjectResult(secondaryStatus);
        }

        // POST: /TblSecondaryStatus/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblSecondaryStatus secondaryStatus)
        {
            using (var scope = new TransactionScope())
            {
                _SecondaryStatusRepository.Create(secondaryStatus);
                _SecondaryStatusRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = secondaryStatus.Id }, secondaryStatus);
            };
        }

        // PUT: /TblSecondaryStatus/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblSecondaryStatus secondaryStatus)
        {
            if (secondaryStatus != null)
            {
                using (var scope = new TransactionScope())
                {
                    _SecondaryStatusRepository.Update(secondaryStatus);
                    scope.Complete();
                    return new OkObjectResult(secondaryStatus);
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblSecondaryStatus/int
        [HttpDelete("{secondaryStatusID}")]
        public IActionResult Delete(int secondaryStatusID)
        {
            _SecondaryStatusRepository.Delete(secondaryStatusID);
            return new OkResult();
        }
    }
}
