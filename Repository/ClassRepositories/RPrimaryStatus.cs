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
    public class RPrimaryStatus : RepositoryInterface<TblPrimaryStatus>
    {
        private readonly dr_DBContext _dbContext;

        public RPrimaryStatus(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblPrimaryStatus primStatus)
        {
            _dbContext.Add(primStatus);
            Save();
        }

        public void Delete(int ID)
        {
            var primStatus = _dbContext.TblPrimaryStatus.Find(ID);
            _dbContext.TblPrimaryStatus.Remove(primStatus);
            Save();
        }

        public async Task<List<TblPrimaryStatus>> GetAll()
        {
            return await _dbContext.TblPrimaryStatus.ToListAsync();
        }

        public async Task<TblPrimaryStatus> Get(int ID)
        {
            return await _dbContext.TblPrimaryStatus.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblPrimaryStatus primStatus)
        {
            _dbContext.Entry(primStatus).State = EntityState.Modified;
            Save();
        }
    }
}
