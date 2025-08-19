using DebtRecoveryPlatform.DBContext;
using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Repository.ClassRepositories
{
    public class RBadDebtReasons : RepositoryInterface<TblBadDebtReasons>
    {
        private readonly dr_DBContext _dbContext;

        public RBadDebtReasons(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblBadDebtReasons badDebtReason)
        {
            _dbContext.Add(badDebtReason);
            Save();
        }

        public void Delete(int ID)
        {
            var badDebtReason = _dbContext.TblBadDebtReasons.Find(ID);
            _dbContext.TblBadDebtReasons.Remove(badDebtReason);
            Save();
        }

        public async Task<List<TblBadDebtReasons>> GetAll()
        {
            return await _dbContext.TblBadDebtReasons.ToListAsync();
        }

        public async Task<TblBadDebtReasons> Get(int ID)
        {
            return await _dbContext.TblBadDebtReasons.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblBadDebtReasons badDebtReason)
        {
            _dbContext.Entry(badDebtReason).State = EntityState.Modified;
            Save();
        }
    }
}
