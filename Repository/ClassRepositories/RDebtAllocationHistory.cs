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
    public class RDebtAllocationHistory : RepositoryInterface<TblDebtAllocationHistory>
    {
        private readonly dr_DBContext _dbContext;

        public RDebtAllocationHistory(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblDebtAllocationHistory debtAllocation)
        {
            _dbContext.Add(debtAllocation);
            Save();
        }

        public void Delete(int ID)
        {
            var debtAllocation = _dbContext.TblDebtAllocationHistory.Find(ID);
            _dbContext.TblDebtAllocationHistory.Remove(debtAllocation);
            Save();
        }

        public async Task<List<TblDebtAllocationHistory>> GetAll()
        {
            return await _dbContext.TblDebtAllocationHistory.ToListAsync();
        }

        public async Task<TblDebtAllocationHistory> Get(int ID)
        {
            return await _dbContext.TblDebtAllocationHistory.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblDebtAllocationHistory debtAllocation)
        {
            _dbContext.Entry(debtAllocation).State = EntityState.Modified;
            Save();
        }
    }
}
