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
    public class RActionLogger : RepositoryInterface<TblActionLogger>
    {
        private readonly dr_DBContext _dbContext;

        public RActionLogger(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblActionLogger actionLogger)
        {
            _dbContext.Add(actionLogger);
            Save();
        }

        public void Delete(int ID)
        {
            var actionLogger = _dbContext.TblActionLogger.Find(ID);
            _dbContext.TblActionLogger.Remove(actionLogger);
            Save();
        }

        public async Task<List<TblActionLogger>> GetAll()
        {
            return await _dbContext.TblActionLogger.ToListAsync();
        }

        public async Task<TblActionLogger> Get(int ID)
        {
            return await _dbContext.TblActionLogger.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblActionLogger actionLogger)
        {
            _dbContext.Entry(actionLogger).State = EntityState.Modified;
            Save();
        }
    }
}
