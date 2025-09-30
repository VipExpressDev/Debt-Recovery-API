using DebtRecoveryPlatform.DBContext;
using DebtRecoveryPlatform.MiddleWare;
using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Models.NonPersistent;
using DebtRecoveryPlatform.Repository.ClassRepositories;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DebtRecoveryPlatform
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder                        
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .Build();
                    });
            });

            services.AddHealthChecks();
            services.AddControllers();

            services.AddDbContext<dr_DBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DebtRecoveryDB")));

            services.AddScoped<RepositoryInterface<TblDebtRecoveryData>, RDebtRecoveryData>();
            services.AddScoped<RepositoryInterface<TblDebtStatus>, RDebtStatus>();
            services.AddScoped<RepositoryInterface<TblBankStatus>, RBankStatus>();
            services.AddScoped<RepositoryInterface<TblPrimaryStatus>, RPrimaryStatus>();
            services.AddScoped<RepositoryInterface<TblSecondaryStatus>, RSecondaryStatus>();
            services.AddScoped<RepositoryInterface<TblStatusLinking>, RStatusLinking>();
            services.AddScoped<RepositoryInterface<TblDebtCollectors>, RDebtCollectors>();
            services.AddScoped<RepositoryInterface<TblDebtAllocationHistory>, RDebtAllocationHistory>();
            services.AddScoped<RepositoryInterface<TblActionLogger>, RActionLogger>();
            services.AddScoped<RepositoryInterface<TblLinkingStatus>, RLinkingStatus>();
            services.AddScoped<RepositoryInterface<TblRejectedMandates>, RRejectedMandates>();
            services.AddScoped<RepositoryInterface<TblNibbsData>, RNibbsData>();
            services.AddScoped<RepositoryInterface<TblClientProfileHistory>, RClientProfileHistory>();
            services.AddScoped<RepositoryInterface<TblActionedReminder>, RActionedReminder>();
            services.AddScoped<RepositoryInterface<TblBadDebtReasons>, RBadDebtReasons>();
            services.AddScoped<RepositoryInterface<TblContributingTransactions>, RContributingTransactions>();
            services.AddTransient<AllocatedDebt>();
            services.AddTransient<ClientInfoSummary>();
            services.AddTransient<ClientProfile>();
            services.AddTransient<ContractData>();
            services.AddTransient<ProvisionDebtUsers>();
            services.AddTransient<CollectorPerformanceSummary>();
            services.AddTransient<AssignableContracts>();
            services.AddTransient<AssignDebtToCollector>();
            services.AddTransient<ClientProfileHistory>();
            services.AddTransient<DebtRecoveryData>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Ensure database is up to date on startup
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<dr_DBContext>();
                db.Database.Migrate();  // <-- apply migrations automatically
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            //app.UseStaticFiles();
            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseMiddleware<TokenMiddleWare>();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                .RequireCors(MyAllowSpecificOrigins);
            });

        }
    }
}
