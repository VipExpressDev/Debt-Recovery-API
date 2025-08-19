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
    public class RSecondaryStatus : RepositoryInterface<TblSecondaryStatus>
    {
        private readonly dr_DBContext _dbContext;

        public RSecondaryStatus(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblSecondaryStatus secStatus)
        {
            _dbContext.Add(secStatus);
            Save();
        }

        public void Delete(int ID)
        {
            var secStatus = _dbContext.TblSecondaryStatus.Find(ID);
            _dbContext.TblSecondaryStatus.Remove(secStatus);
            Save();
        }

        public async Task<List<TblSecondaryStatus>> GetAll()
        {
            return await _dbContext.TblSecondaryStatus.ToListAsync();
        }

        public async Task<TblSecondaryStatus> Get(int ID)
        {
            return await _dbContext.TblSecondaryStatus.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblSecondaryStatus secStatus)
        {
            _dbContext.Entry(secStatus).State = EntityState.Modified;
            Save();
        }
    }
}
