using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DebtRecoveryPlatform.DBContext
{
    public class provisionDBContext
    {
        public static IConfiguration Configuration { get; set; }

        public provisionDBContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private SqlConnection OpenConection(string connection)
        {
            SqlConnection con = new SqlConnection(Configuration.GetConnectionString(connection));
            con.Open();
            return con;
        }

        private void CloseConnection(SqlConnection con)
        {
            con.Close();
        }

        public void ExecuteQueries(string connection, string Query_)
        {
            SqlConnection con = OpenConection(connection);
            SqlCommand cmd = new SqlCommand(Query_, con);
            cmd.ExecuteNonQuery();
            CloseConnection(con);
        }

        public DataSet ReturnQueries(string connection, string Query_)
        {
            SqlConnection con = OpenConection(connection);
            SqlCommand cmd = new SqlCommand(Query_, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            CloseConnection(con);
            return ds;
        }
    }
}
