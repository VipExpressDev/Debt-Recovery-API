using DebtRecoveryPlatform.Helpers;
using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Models.ResponseObject;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Transactions;

namespace DebtRecoveryPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TblBankStatusController : ControllerBase
    {
        private readonly RepositoryInterface<TblBankStatus> _BankStatusRepository;

        public TblBankStatusController(RepositoryInterface<TblBankStatus> BankStatusRepository)
        {
            _BankStatusRepository = BankStatusRepository;
        }

        // GET: /TblBankStatus/ (All)
        public async Task<IActionResult> Get([FromHeader] string Authorization)
        {
            var bankStatus = await _BankStatusRepository.GetAll();
            return new OkObjectResult(new ResponseObject<TblBankStatus>(bankStatus, Authorization));
        }

        // GET: /TblBankStatus/int (Single)
        [HttpGet("{bankStatusID}", Name = "GetBankStatus")]
        public async Task<IActionResult> Get(int bankStatusID, [FromHeader] string Authorization)
        {
            var bankStatus = await _BankStatusRepository.Get(bankStatusID);
            return new OkObjectResult(new ResponseObject<TblBankStatus>(bankStatus, Authorization));
        }

        // POST: /TblBankStatus/Create
        [HttpPost("Create")]
        public IActionResult Post([FromBody] TblBankStatus bankStatus, [FromHeader] string Authorization)
        {
            using (var scope = new TransactionScope())
            {
                _BankStatusRepository.Create(bankStatus);
                _BankStatusRepository.Save();
                scope.Complete();
                return CreatedAtAction(nameof(Get), new { id = bankStatus.Id }, bankStatus);
            };
        }

        // PUT: /TblBankStatus/int
        [HttpPut("")]
        public IActionResult Put([FromBody] TblBankStatus bankStatus, [FromHeader] string Authorization)
        {
            if (bankStatus != null)
            {
                using (var scope = new TransactionScope())
                {
                    _BankStatusRepository.Update(bankStatus);
                    scope.Complete();
                    return new OkObjectResult(new ResponseObject<TblBankStatus>(bankStatus, Authorization));
                }
            }
            return new NoContentResult();
        }

        // DELETE: /TblBankStatus/int
        [HttpDelete("{bankStatusID}")]
        public IActionResult Delete(int bankStatusID, [FromHeader] string Authorization)
        {
            _BankStatusRepository.Delete(bankStatusID);
            return new OkResult();
        }
    }
}
