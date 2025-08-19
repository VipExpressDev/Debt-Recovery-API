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
    public class RStatusLinking : RepositoryInterface<TblStatusLinking>
    {
        private readonly dr_DBContext _dbContext;

        public RStatusLinking(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblStatusLinking linkStatus)
        {
            _dbContext.Add(linkStatus);
            Save();
        }

        public void Delete(int ID)
        {
            var linkStatus = _dbContext.TblStatusLinking.Find(ID);
            _dbContext.TblStatusLinking.Remove(linkStatus);
            Save();
        }

        public async Task<List<TblStatusLinking>> GetAll()
        {
            return await _dbContext.TblStatusLinking.ToListAsync();
        }

        public async Task<TblStatusLinking> Get(int ID)
        {
            return await _dbContext.TblStatusLinking.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblStatusLinking linkStatus)
        {
            _dbContext.Entry(linkStatus).State = EntityState.Modified;
            Save();
        }
    }
}
