using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Models.NonPersistent;
using DebtRecoveryPlatform.Models.ResponseObject;
using DebtRecoveryPlatform.Models.Temp;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    [ApiController]
    public class QuartzJobsController : ControllerBase
    {
        private readonly RepositoryInterface<TblDebtRecoveryData> _DebtRecoveryDataRepository;
        private readonly RepositoryInterface<TblActionedReminder> _ActionedReminderRepository;
        private readonly RepositoryInterface<TblPrimaryStatus> _PrimaryStatusRepository;
        static HttpClient client = new HttpClient();

        public QuartzJobsController(RepositoryInterface<TblDebtRecoveryData> DebtRecoveryDataRepository, RepositoryInterface<TblActionedReminder> ActionedReminderRepository, RepositoryInterface<TblPrimaryStatus> PrimaryStatusRepository)
        {
            _DebtRecoveryDataRepository = DebtRecoveryDataRepository;
            _ActionedReminderRepository = ActionedReminderRepository;
            _PrimaryStatusRepository = PrimaryStatusRepository;
        }

        public int GetWorkingDays(DateTime from, DateTime to)
        {
            var dayDifference = (int)to.Subtract(from).TotalDays;

            if (dayDifference > 0)
            {
                return Enumerable
                    .Range(1, dayDifference)
                    .Select(x => from.AddDays(x))
                    .Count(x => x.DayOfWeek != DayOfWeek.Saturday && x.DayOfWeek != DayOfWeek.Sunday);
            }
            else
            {
                return 0;
            }
        }

        // PUT: /TblActionedReminder/int
        [HttpGet("")]
        public async Task<IActionResult> UpdateDormantDebtStatus()
        {
            List<TblDebtRecoveryData> FullDebtRecovery = await _DebtRecoveryDataRepository.GetAll();
            List<TblDebtRecoveryData> FullDebtRecoveryDataList = FullDebtRecovery.Where(w => w.AllocatedDate != null).ToList();
            List<TblActionedReminder> FullActionedReminderList = await _ActionedReminderRepository.GetAll();
            List<TblPrimaryStatus> FullPrimaryStatusList = await _PrimaryStatusRepository.GetAll();

            List<DormantDebt> DormantDebtList = new List<DormantDebt>();

            int ActionRequiredID = FullPrimaryStatusList.SingleOrDefault(f => f.Description.Contains("Action Req")).Code;
            int FollowUpID = FullPrimaryStatusList.SingleOrDefault(f => f.Description.Contains("Follow")).Code;
            int PTPID = FullPrimaryStatusList.SingleOrDefault(f => f.Description.Contains("Promise")).Code;

            foreach (TblDebtRecoveryData debtItem in FullDebtRecoveryDataList.Where(w => w.StatusID == ActionRequiredID))
            {
                int workingDaysCount = GetWorkingDays((DateTime)debtItem.AllocatedDate, DateTime.Now);

                if (workingDaysCount >= 2)
                {
                    // Email logic.
                    if (!DormantDebtList.Any(a => a.ContractNo == debtItem.ContractNo))
                    {
                        DormantDebt dormantDebt = new DormantDebt()
                        {
                            ContractNo = debtItem.ContractNo,
                            LastActionedDate = (DateTime)debtItem.AllocatedDate
                        };
                        DormantDebtList.Add(dormantDebt);
                    }
                }
            }

            foreach (TblActionedReminder reminderItem in FullActionedReminderList)
            {
                int workingDaysCount = GetWorkingDays(reminderItem.ReminderDate, DateTime.Now);

                if (reminderItem.ReminderTypeID == FollowUpID && workingDaysCount >= 2)
                {
                    // Email logic.
                    if (!DormantDebtList.Any(a => a.ContractNo == reminderItem.ContractNo))
                    {
                        DormantDebt dormantDebt = new DormantDebt()
                        {
                            ContractNo = reminderItem.ContractNo,
                            LastActionedDate = (DateTime)reminderItem.ReminderDate
                        };
                        DormantDebtList.Add(dormantDebt);
                    }
                }
                else if (reminderItem.ReminderTypeID == PTPID && workingDaysCount >= 3) // 1 Day ekstra for PTP (1 grace day)
                {
                    // Email logic.
                    if (!DormantDebtList.Any(a => a.ContractNo == reminderItem.ContractNo))
                    {
                        DormantDebt dormantDebt = new DormantDebt()
                        {
                            ContractNo = reminderItem.ContractNo,
                            LastActionedDate = (DateTime)reminderItem.ReminderDate
                        };
                        DormantDebtList.Add(dormantDebt);
                    }
                }
            }

            StringBuilder NotificationMessage = new StringBuilder();
            NotificationMessage.AppendLine("<html>");
            NotificationMessage.AppendLine("<body>");
            NotificationMessage.AppendLine("<p>Good Day, </p></br>");
            NotificationMessage.AppendLine("<p>Please note, the following contracts have remained unactioned in the debt collection system: </p></br>");
            NotificationMessage.AppendLine("<p>Contract:&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp; Last action date: </p>");

            foreach (var dormantDebt in DormantDebtList)
            {
                NotificationMessage.AppendLine("<p>" + dormantDebt.ContractNo + "&emsp;&emsp;&emsp;&emsp;- " + dormantDebt.LastActionedDate.ToString("yyyy/MM/dd") + "</p>");
            }

            NotificationMessage.AppendLine("<p>Please take a look as to why these contracts are no longer being actioned. </p>");
            NotificationMessage.AppendLine("</body>");
            NotificationMessage.AppendLine("</html>");

            Mailer mailToSend = new Mailer()
            {
                receipientAddress = "dev3@simsgroup.co.za",
                senderAddress = "provision@oaks.co.za",
                senderName = "Debt Recovery Platform",
                subject = "Dormant Debt",
                body = NotificationMessage.ToString(),
                sourceSystemId = "c743af4a-fdca-4e51-85b5-37e83e0dc015"
            };

            //var byteContent = new ByteArrayContent(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mailToSend)));
            //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var data = new StringContent(JsonConvert.SerializeObject(mailToSend), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://localhost:82/Mailer", data);

            return new OkResult();
        }
    }

    public class Mailer
    {
        public string receipientAddress { get; set; }
        public string senderAddress { get; set; }
        public string senderName { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string sourceSystemId { get; set; }
    }
}