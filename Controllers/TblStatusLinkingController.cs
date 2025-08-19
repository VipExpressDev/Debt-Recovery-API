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
    public class TblStatusLinkingController : ControllerBase
    {
        private readonly RepositoryInterface<TblStatusLinking> _StatusLinkingRepository;

        public TblStatusLinkingController(RepositoryInterface<TblStatusLinking> StatusLinkingRepository)
        {
            _StatusLinkingRepository = StatusLinkingRepository;
        }

        // GET: /TblStatusLinking/ (All)
        public async Task<IActionResult> Get()
        {
            var statusLinking = await _StatusLinkingRepository.GetAll();
            return new OkObjectResult(statusLinking);
        }

        // GET: /TblStatusLinking/int (Single)
        [HttpGet("{statusLinkingID}", Name = "GetStatusLinking")]
        public async Task<IActionResult> Get(int statusLinkingID, [FromHeader] string Authorization)
        {
            var statusLinking = await _StatusLinkingRepository.Get(statusLinkingID);
            return new OkObjectResult(new ResponseObject<TblStatusLinking>(statusLinking, Authorization));
        }

        // POST: /TblStatusLinking/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblStatusLinking statusLinking, [FromHeader] string Authorization)
        {
            using (var scope = new TransactionScope())
            {
                _StatusLinkingRepository.Create(statusLinking);
                _StatusLinkingRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = statusLinking.Id }, statusLinking);
            };
        }

        // PUT: /TblStatusLinking/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblStatusLinking statusLinking, [FromHeader] string Authorization)
        {
            if (statusLinking != null)
            {
                using (var scope = new TransactionScope())
                {
                    _StatusLinkingRepository.Update(statusLinking);
                    scope.Complete();
                    return new OkObjectResult(new ResponseObject<TblStatusLinking>(statusLinking, Authorization));
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblStatusLinking/int
        [HttpDelete("{statusLinkingID}")]
        public IActionResult Delete(int statusLinkingID, [FromHeader] string Authorization)
        {
            _StatusLinkingRepository.Delete(statusLinkingID);
            return new OkResult();
        }
    }
}
