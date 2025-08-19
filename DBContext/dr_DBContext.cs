using DebtRecoveryPlatform.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DebtRecoveryPlatform.DBContext
{
    public class dr_DBContext : DbContext
    {
        public dr_DBContext(DbContextOptions<dr_DBContext> options) : base(options)
        {

        }

        public dr_DBContext()
        {

        }

        public static dr_DBContext Create()
        {
            return new dr_DBContext();
        }

        public DbSet<TblDebtRecoveryData> TblDebtRecoveryData { get; set; }
        public DbSet<TblDebtStatus> TblDebtStatus { get; set; }
        public DbSet<TblBankStatus> TblBankStatus { get; set; }
        public DbSet<TblPrimaryStatus> TblPrimaryStatus { get; set; }
        public DbSet<TblSecondaryStatus> TblSecondaryStatus { get; set; }
        public DbSet<TblStatusLinking> TblStatusLinking { get; set; }
        public DbSet<TblDebtCollectors> TblDebtCollectors { get; set; }
        public DbSet<TblDebtAllocationHistory> TblDebtAllocationHistory { get; set; }
        public DbSet<TblActionLogger> TblActionLogger { get; set; }
        public DbSet<TblLinkingStatus> TblLinkingStatus { get; set; }
        public DbSet<TblRejectedMandates> TblRejectedMandates { get; set; }
        public DbSet<TblNibbsData> TblNibbsData { get; set; }
        public DbSet<TblClientProfileHistory> TblClientProfileHistory { get; set; }
        public DbSet<TblActionedReminder> TblActionedReminder { get; set; }
        public DbSet<TblBadDebtReasons> TblBadDebtReasons { get; set; }
        public DbSet<TblContributingTransactions> TblContributingTransactions { get; set; }

        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }

        private void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues["GCRecord"] = null;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["GCRecord"] = "[" + DateTime.Now.Ticks + "] - " + entry.CurrentValues["Id"];
                        break;
                }
            }
        }
            
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TblDebtRecoveryData>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblDebtStatus>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblBankStatus>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblPrimaryStatus>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblSecondaryStatus>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblStatusLinking>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblDebtCollectors>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblDebtAllocationHistory>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblActionLogger>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblLinkingStatus>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblRejectedMandates>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblNibbsData>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblClientProfileHistory>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblActionedReminder>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
            builder.Entity<TblBadDebtReasons>().HasQueryFilter(m => EF.Property<int?>(m, "GCRecord") == null);
        }
    }
}
