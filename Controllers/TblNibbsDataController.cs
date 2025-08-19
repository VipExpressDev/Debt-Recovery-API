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
    public class TblNibbsDataController : ControllerBase
    {
        private readonly RepositoryInterface<TblNibbsData> _NibbsDataRepository;

        public TblNibbsDataController(RepositoryInterface<TblNibbsData> NibbsDataRepository)
        {
            _NibbsDataRepository = NibbsDataRepository;
        }

        // GET: /TblNibbsData/ (All)
        public IActionResult Get()
        {
            var nibbsData = _NibbsDataRepository.GetAll();
            return new OkObjectResult(nibbsData);
        }

        // GET: /TblNibbsData/int (Single)
        [HttpGet("{nibbsDataID}", Name = "GetNibbsData")]
        public IActionResult Get(int nibbsDataID)
        {
            var nibbsData = _NibbsDataRepository.Get(nibbsDataID);
            return new OkObjectResult(nibbsData);
        }

        // POST: /TblNibbsData/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblNibbsData nibbsData)
        {
            using (var scope = new TransactionScope())
            {
                nibbsData.CreatedDate = DateTime.Now;
                _NibbsDataRepository.Create(nibbsData);
                _NibbsDataRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = nibbsData.Id }, nibbsData);
            };
        }

        // PUT: /TblNibbsData/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblNibbsData nibbsData)
        {
            if (nibbsData != null)
            {
                using (var scope = new TransactionScope())
                {
                    _NibbsDataRepository.Update(nibbsData);
                    scope.Complete();
                    return new OkObjectResult(nibbsData);
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblNibbsData/int
        [HttpDelete("{nibbsDataID}")]
        public IActionResult Delete(int nibbsDataID)
        {
            _NibbsDataRepository.Delete(nibbsDataID);
            return new OkResult();
        }
    }
}
