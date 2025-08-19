using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.MiddleWare
{
    public class TokenMiddleWare
    {
        private readonly RequestDelegate _next;
        static HttpClient client = new HttpClient();
        public TokenMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);
            return;
            var gg = context.Request.Headers["Authorization"];

            if (gg.Count() != 0)
            {
                HttpResponseMessage responseMessage = await client.GetAsync("https://vip-portfolio.com:98/User/ValidateToken/?token=" + gg.ToString());
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    context.Request.Headers["Authorization"] = responseMessage.Content.ReadAsStringAsync().Result;
                    await _next(context);
                }
                else
                {
                    // Hierdie is jaun se baby.
                }
            }
            else
            {
                if(context.Request.Path.ToString().Contains("Quartz"))
                {
                    await _next(context);
                }
            }
        }
    }
}
