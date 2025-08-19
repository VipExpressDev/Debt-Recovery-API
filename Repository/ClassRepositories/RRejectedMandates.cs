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
    public class RRejectedMandates : RepositoryInterface<TblRejectedMandates>
    {
        private readonly dr_DBContext _dbContext;

        public RRejectedMandates(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblRejectedMandates rejectedMandates)
        {
            _dbContext.Add(rejectedMandates);
            Save();
        }

        public void Delete(int ID)
        {
            var rejectedMandates = _dbContext.TblRejectedMandates.Find(ID);
            _dbContext.TblRejectedMandates.Remove(rejectedMandates);
            Save();
        }

        public async Task<List<TblRejectedMandates>> GetAll()
        {
            return await _dbContext.TblRejectedMandates.ToListAsync();
        }

        public async Task<TblRejectedMandates> Get(int ID)
        {
            return await _dbContext.TblRejectedMandates.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblRejectedMandates rejectedMandates)
        {
            _dbContext.Entry(rejectedMandates).State = EntityState.Modified;
            Save();
        }
    }
}
