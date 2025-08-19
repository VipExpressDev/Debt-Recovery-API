using DebtRecoveryPlatform.DBContext;
using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Repository.ClassRepositories
{
    public class RContributingTransactions : RepositoryInterface<TblContributingTransactions>
    {
        private readonly dr_DBContext _dbContext;

        public RContributingTransactions(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblContributingTransactions item)
        {
            _dbContext.Add(item);
            Save();
        }

        public void Delete(int ID)
        {
            var actionLogger = _dbContext.TblContributingTransactions.Find(ID);
            _dbContext.TblContributingTransactions.Remove(actionLogger);
            Save();
        }

        public async Task<List<TblContributingTransactions>> GetAll()
        {
            return await _dbContext.TblContributingTransactions.ToListAsync();
        }

        public async Task<TblContributingTransactions> Get(int ID)
        {
            return await _dbContext.TblContributingTransactions.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblContributingTransactions item)
        {
            _dbContext.Entry(item).State = EntityState.Modified;
            Save();
        }
    }
}
