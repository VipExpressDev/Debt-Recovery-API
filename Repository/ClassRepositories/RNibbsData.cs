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
    public class RNibbsData : RepositoryInterface<TblNibbsData>
    {
        private readonly dr_DBContext _dbContext;

        public RNibbsData(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblNibbsData nibbsData)
        {
            _dbContext.Add(nibbsData);
            Save();
        }

        public void Delete(int ID)
        {
            var nibbsData = _dbContext.TblNibbsData.Find(ID);
            _dbContext.TblNibbsData.Remove(nibbsData);
            Save();
        }

        public async Task<List<TblNibbsData>> GetAll()
        {
            return await _dbContext.TblNibbsData.ToListAsync();
        }

        public async Task<TblNibbsData> Get(int ID)
        {
            return await _dbContext.TblNibbsData.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblNibbsData nibbsData)
        {
            _dbContext.Entry(nibbsData).State = EntityState.Modified;
            Save();
        }
    }
}
