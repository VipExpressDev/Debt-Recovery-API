using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.ResponseObject
{
    [NotMapped]
    public class ResponseObject<TEntity> where TEntity : class
    {
        public List<TEntity> ReturnList { get; set; }
        public TEntity ReturnObject { get; set; }
        public string Token { get; set; }

        public ResponseObject(TEntity _returnObject, string _token)
        {
            ReturnObject = _returnObject;
            Token = _token;
        }

        public ResponseObject(List<TEntity> _returnList, string _token)
        {
            ReturnList = _returnList;
            Token = _token;
        }

        public ResponseObject(string _token)
        {
            Token = _token;
        }
    }
}
