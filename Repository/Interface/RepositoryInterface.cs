using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Repository.Interface
{
    public interface RepositoryInterface<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetAll();
        Task<TEntity> Get(int item);
        void Create(TEntity item);
        void Delete(int item);
        void Update(TEntity item);
        void Save();

        #region To-Do
        //void CreateMultiple(IEnumerable<TEntity> items);
        //void DeleteMultiple(IEnumerable<TEntity> items);
        #endregion
    }
}
