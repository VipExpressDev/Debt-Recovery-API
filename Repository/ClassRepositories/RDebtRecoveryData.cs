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
    public class RDebtRecoveryData : RepositoryInterface<TblDebtRecoveryData>
    {
        private readonly dr_DBContext _dbContext;

        public RDebtRecoveryData(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblDebtRecoveryData debtRecoveryItem)
        {
            _dbContext.Add(debtRecoveryItem);
            Save();
        }

        public void Delete(int ID)
        {
            var debtRecoveryItem = _dbContext.TblDebtRecoveryData.Find(ID);
            _dbContext.TblDebtRecoveryData.Remove(debtRecoveryItem);
            Save();
        }

        public async Task<List<TblDebtRecoveryData>> GetAll()
        {
            return await _dbContext.TblDebtRecoveryData.ToListAsync();
        }

        public List<TblDebtRecoveryData> GetSpecific()
        {
            return _dbContext.TblDebtRecoveryData.Where(w => w.FollowUpDate <= DateTime.Now.AddDays(-7)).ToList();
        }

        public async Task<TblDebtRecoveryData> Get(int ID)
        {
            return await _dbContext.TblDebtRecoveryData.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblDebtRecoveryData debtRecoveryItem)
        {
            _dbContext.Entry(debtRecoveryItem).State = EntityState.Modified;
            Save();
        }
    }
}
