using DebtRecoveryPlatform.DBContext;
using DebtRecoveryPlatform.Models;
using DebtRecoveryPlatform.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Repository.ClassRepositories
{
    public class RLinkingStatus : RepositoryInterface<TblLinkingStatus>
    {
        private readonly dr_DBContext _dbContext;

        public RLinkingStatus(dr_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TblLinkingStatus linkingStatus)
        {
            _dbContext.Add(linkingStatus);
            Save();
        }

        public void Delete(int ID)
        {
            var linkingStatus = _dbContext.TblLinkingStatus.Find(ID);
            _dbContext.TblLinkingStatus.Remove(linkingStatus);
            Save();
        }

        public async Task<List<TblLinkingStatus>> GetAll()
        {
            return await _dbContext.TblLinkingStatus.ToListAsync();
        }

        public async Task<TblLinkingStatus> Get(int ID)
        {
            return await _dbContext.TblLinkingStatus.FindAsync(ID);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(TblLinkingStatus linkingStatus)
        {
            _dbContext.Entry(linkingStatus).State = EntityState.Modified;
            Save();
        }
    }
}
