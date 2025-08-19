using DebtRecoveryPlatform.Helpers;
using DebtRecoveryPlatform.Models.NonPersistent;
using DebtRecoveryPlatform.Models.ResponseObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Controllers.NonPersistent
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvisionDebtUsersController : Controller
    {
        private IConfiguration configuration { get; }

        public ProvisionDebtUsersController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("GetCollectorsData")]
        public IActionResult Get([FromHeader] string Authorization)
        {
            List<ProvisionDebtUsers> DebtCollectorsData = ProvisionDebtUsers.GetCollectorsData(configuration);
            return new OkObjectResult(new ResponseObject<ProvisionDebtUsers>(DebtCollectorsData, Authorization));
        }
    }
}
