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
    public class RBankStatus : RepositoryInterface<TblBankStatus>
    {
        private readonly dr_DBContext _dbContext;

        public RBankStatus(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblBankStatus bankStatus)
        {
            _dbContext.Add(bankStatus);
            Save();
        }

        public void Delete(int ID)
        {
            var bankStatus = _dbContext.TblBankStatus.Find(ID);
            _dbContext.TblBankStatus.Remove(bankStatus);
            Save();
        }

        public async Task<List<TblBankStatus>> GetAll()
        {
            return await _dbContext.TblBankStatus.ToListAsync();
        }

        public async Task<TblBankStatus> Get(int ID)
        {
            return await _dbContext.TblBankStatus.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblBankStatus bankStatus)
        {
            _dbContext.Entry(bankStatus).State = EntityState.Modified;
            Save();
        }
    }
}
