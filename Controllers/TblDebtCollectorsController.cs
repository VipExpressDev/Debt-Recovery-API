using DebtRecoveryPlatform.Helpers;
using DebtRecoveryPlatform.Models;
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
    public class TblDebtCollectorsController : ControllerBase
    {
        private readonly RepositoryInterface<TblDebtCollectors> _DebtCollectorsRepository;

        public TblDebtCollectorsController(RepositoryInterface<TblDebtCollectors> DebtCollectorsRepository)
        {
            _DebtCollectorsRepository = DebtCollectorsRepository;
        }

        // GET: /TblDebtCollectors/ (All)
        public async Task<IActionResult> Get([FromHeader] string Authorization)
        {
            var debtCollectors = await _DebtCollectorsRepository.GetAll();
            return new OkObjectResult(new ResponseObject<TblDebtCollectors>(debtCollectors, Authorization));
        }

        // GET: /TblDebtCollectors/ (All)
        [HttpGet("RegionalCollectors")]
        public async Task<IActionResult> RegionalCollectors(int managerRegionID, [FromHeader] string Authorization)
        {
            var debtCollectors = await _DebtCollectorsRepository.GetAll();
            var nonManagers = debtCollectors.Where(w => !w.isManager && w.RegionID == managerRegionID).ToList();
            return new OkObjectResult(new ResponseObject<TblDebtCollectors>(nonManagers, Authorization));
        }

        // GET: /TblDebtCollectors/int (Single)
        [HttpGet("{debtCollectorsID}", Name = "GetDebtCollectors")]
        public async Task<IActionResult> Get(int debtCollectorsID, [FromHeader] string Authorization)
        {
            var debtCollectors = await _DebtCollectorsRepository.GetAll();
            var activeCollector = debtCollectors.Where(w => w.PersonnelCode == debtCollectorsID).FirstOrDefault();
            return new OkObjectResult(new ResponseObject<TblDebtCollectors>(activeCollector, Authorization));
        }

        // POST: /TblDebtCollectors/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblDebtCollectors debtCollectors, [FromHeader] string Authorization)
        {
            using (var scope = new TransactionScope())
            {
                _DebtCollectorsRepository.Create(debtCollectors);
                _DebtCollectorsRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = debtCollectors.Id }, debtCollectors);
            };
        }

        // PUT: /TblDebtCollectors/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblDebtCollectors debtCollectors, [FromHeader] string Authorization)
        {
            if (debtCollectors != null)
            {
                using (var scope = new TransactionScope())
                {
                    _DebtCollectorsRepository.Update(debtCollectors);
                    scope.Complete();
                    return new OkObjectResult(new ResponseObject<TblDebtCollectors>(debtCollectors, Authorization));
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblDebtCollectors/int
        [HttpDelete("{debtCollectorsID}")]
        public IActionResult Delete(int debtCollectorsID)
        {
            _DebtCollectorsRepository.Delete(debtCollectorsID);
            return new OkResult();
        }
    }
}
