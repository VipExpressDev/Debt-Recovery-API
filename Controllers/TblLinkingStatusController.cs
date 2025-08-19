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
    public class TblLinkingStatusController : ControllerBase
    {
        private readonly RepositoryInterface<TblLinkingStatus> _LinkingStatusRepository;

        public TblLinkingStatusController(RepositoryInterface<TblLinkingStatus> LinkingStatusRepository)
        {
            _LinkingStatusRepository = LinkingStatusRepository;
        }

        // GET: /TblLinkingStatus/ (All)
        public IActionResult Get()
        {
            var linkingStatus = _LinkingStatusRepository.GetAll();
            return new OkObjectResult(linkingStatus);
        }

        // GET: /TblLinkingStatus/int (Single)
        [HttpGet("{linkingStatusID}", Name = "GetLinkingStatus")]
        public IActionResult Get(int linkingStatusID)
        {
            var linkingStatus = _LinkingStatusRepository.Get(linkingStatusID);
            return new OkObjectResult(linkingStatus);
        }

        // POST: /TblLinkingStatus/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblLinkingStatus linkingStatus)
        {
            using (var scope = new TransactionScope())
            {
                linkingStatus.CreatedDate = DateTime.Now;
                _LinkingStatusRepository.Create(linkingStatus);
                _LinkingStatusRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = linkingStatus.Id }, linkingStatus);
            };
        }

        // PUT: /TblLinkingStatus/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblLinkingStatus linkingStatus)
        {
            if (linkingStatus != null)
            {
                using (var scope = new TransactionScope())
                {
                    _LinkingStatusRepository.Update(linkingStatus);
                    scope.Complete();
                    return new OkObjectResult(linkingStatus);
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblLinkingStatus/int
        [HttpDelete("{linkingStatusID}")]
        public IActionResult Delete(int linkingStatusID)
        {
            _LinkingStatusRepository.Delete(linkingStatusID);
            return new OkResult();
        }
    }
}
