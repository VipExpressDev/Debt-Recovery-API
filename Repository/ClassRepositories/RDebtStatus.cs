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
    public class RDebtStatus : RepositoryInterface<TblDebtStatus>
    {
        private readonly dr_DBContext _dbContext;

        public RDebtStatus(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblDebtStatus debtStatus)
        {
            _dbContext.Add(debtStatus);
            Save();
        }

        public void Delete(int ID)
        {
            var debtStatus = _dbContext.TblDebtStatus.Find(ID);
            _dbContext.TblDebtStatus.Remove(debtStatus);
            Save();
        }

        public async Task<List<TblDebtStatus>> GetAll()
        {
            return await _dbContext.TblDebtStatus.ToListAsync();
        }

        public async Task<TblDebtStatus> Get(int ID)
        {
            return await _dbContext.TblDebtStatus.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblDebtStatus debtStatus)
        {
            _dbContext.Entry(debtStatus).State = EntityState.Modified;
            Save();
        }
    }
}
