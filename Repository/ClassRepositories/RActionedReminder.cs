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
    public class RActionedReminder : RepositoryInterface<TblActionedReminder>
    {
        private readonly dr_DBContext _dbContext;

        public RActionedReminder(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblActionedReminder actionedReminder)
        {
            _dbContext.Add(actionedReminder);
            Save();
        }

        public void Delete(int ID)
        {
            var actionedReminder = _dbContext.TblActionedReminder.Find(ID);
            _dbContext.TblActionedReminder.Remove(actionedReminder);
            Save();
        }

        public async Task<List<TblActionedReminder>> GetAll()
        {
            return await _dbContext.TblActionedReminder.ToListAsync();
        }

        public async Task<TblActionedReminder> Get(int ID)
        {
            return await _dbContext.TblActionedReminder.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblActionedReminder actionedReminder)
        {
            _dbContext.Entry(actionedReminder).State = EntityState.Modified;
            Save();
        }
    }
}
