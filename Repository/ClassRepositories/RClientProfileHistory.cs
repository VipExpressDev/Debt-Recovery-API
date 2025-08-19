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
    public class RClientProfileHistory : RepositoryInterface<TblClientProfileHistory>
    {
        private readonly dr_DBContext _dbContext;

        public RClientProfileHistory(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblClientProfileHistory clientProfileHistory)
        {
            _dbContext.Add(clientProfileHistory);
            Save();
        }

        public void Delete(int ID)
        {
            var clientProfileHistory = _dbContext.TblClientProfileHistory.Find(ID);
            _dbContext.TblClientProfileHistory.Remove(clientProfileHistory);
            Save();
        }

        public async Task<List<TblClientProfileHistory>> GetAll()
        {
            return await _dbContext.TblClientProfileHistory.ToListAsync();
        }

        public async Task<TblClientProfileHistory> Get(int ID)
        {
            return await _dbContext.TblClientProfileHistory.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChangesAsync();
        }

        public void Update(TblClientProfileHistory clientProfileHistory)
        {
            _dbContext.Entry(clientProfileHistory).State = EntityState.Modified;
            Save();
        }
    }
}
