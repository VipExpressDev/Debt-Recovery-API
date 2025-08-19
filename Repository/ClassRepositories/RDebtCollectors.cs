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
    public class RDebtCollectors : RepositoryInterface<TblDebtCollectors>
    {
        private readonly dr_DBContext _dbContext;

        public RDebtCollectors(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblDebtCollectors debtCollectors)
        {
            _dbContext.Add(debtCollectors);
            Save();
        }

        public void Delete(int ID)
        {
            var debtCollectors = _dbContext.TblDebtCollectors.Find(ID);
            _dbContext.TblDebtCollectors.Remove(debtCollectors);
            Save();
        }

        public async Task<List<TblDebtCollectors>> GetAll()
        {
            return await _dbContext.TblDebtCollectors.ToListAsync();
        }

        public async Task<TblDebtCollectors> Get(int ID)
        {
            return await _dbContext.TblDebtCollectors.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblDebtCollectors debtCollectors)
        {
            _dbContext.Entry(debtCollectors).State = EntityState.Modified;
            Save();
        }
    }
}
