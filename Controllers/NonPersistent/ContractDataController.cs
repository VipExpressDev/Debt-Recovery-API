using DebtRecoveryPlatform.Models.NonPersistent;
using DebtRecoveryPlatform.Models.ResponseObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace DebtRecoveryPlatform.Controllers.NonPersistent
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractDataController : Controller
    {
        private IConfiguration configuration { get; }

        public ContractDataController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("GetContractData")]
        public IActionResult GetContractData(string bookingRef, [FromHeader] string Authorization)
        {
            ContractData ContractData = ContractData.GetContractData(configuration, bookingRef);
            return new OkObjectResult(new ResponseObject<ContractData>(ContractData, Authorization));
        }

        [HttpGet("GetPayPlanData")]
        public IActionResult GetPayPlanData(string bookingRef, [FromHeader] string Authorization)
        {
            List<PayPlanData> PayPlanDataList = PayPlanData.GetPayPlanData(configuration, bookingRef);
            return new OkObjectResult(new ResponseObject<PayPlanData>(PayPlanDataList.OrderBy(ob => ob.DateOfPayment).ToList(), Authorization));
        }

        [HttpGet("GetPortfolioNumbers")]
        public IActionResult GetPortfolioNumbers(string bookingRef, [FromHeader] string Authorization)
        {
            List<string> PayPlanDataList = ContractData.GetPortfolioNumbers(configuration, bookingRef);
            return new OkObjectResult(new ResponseProperty(PayPlanDataList, Authorization));
        }

        [HttpGet("GetReferrals")]
        public IActionResult GetReferrals(string contractNo, [FromHeader] string Authorization)
        {
            List<ContractData> ReferralData = ContractData.GetReferalCount(configuration, contractNo);
            return new OkObjectResult(new ResponseObject<ContractData>(ReferralData, Authorization));
        }
    }
}
