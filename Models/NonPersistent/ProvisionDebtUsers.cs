using DebtRecoveryPlatform.DBContext;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class ProvisionDebtUsers : BaseClass
    {
        public string PersonnelCode { get; set; }
        public string NameAndSurname { get; set; }
        public string Username { get; set; }
        public DateTime DateJoined { get; set; }
        public DateTime? DateResigned { get; set; }

        public ProvisionDebtUsers()
        {

        }

        public ProvisionDebtUsers(string _nameAndSurname, string _personnelCode, string _username, DateTime _dateJoined, DateTime? _dateResigned)
        {
            PersonnelCode = _personnelCode;
            NameAndSurname = _nameAndSurname;
            Username = _username;
            DateJoined = _dateJoined;
            DateResigned = _dateResigned;
        }

        public static List<ProvisionDebtUsers> GetCollectorsData(IConfiguration Configuration)
        {
            string query = "SELECT NameAndSurname, PersonnelCode, UserName, DateJoined, DateResigned FROM TblPersonnel " +
                           "WHERE IsDebtCollector = 1 " +
                           "AND GCRecord IS NULL";

            provisionDBContext dbCon = new provisionDBContext(Configuration);
            DataSet ds = dbCon.ReturnQueries("ProvisionDB", query);
            DataTable dt = ds.Tables[0];

            List<ProvisionDebtUsers> collectorData = new List<ProvisionDebtUsers>();

            foreach (DataRow debtCollector in dt.Rows)
            {
                collectorData.Add(new ProvisionDebtUsers(debtCollector["NameAndSurname"].ToString(), debtCollector["PersonnelCode"].ToString(), debtCollector["UserName"].ToString(), (DateTime)(debtCollector["DateJoined"]), debtCollector["DateResigned"] == DBNull.Value ? null : (DateTime?)(debtCollector["DateResigned"])));
            }

            return collectorData;
        }
    }
}
