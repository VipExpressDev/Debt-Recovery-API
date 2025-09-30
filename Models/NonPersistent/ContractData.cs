using DebtRecoveryPlatform.DBContext;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class ContractData
    {
        public string ContractNo { get; set; }
        public string BookingRef { get; set; }
        public string ClientFullName { get; set; }
        public string ClientSpouseFullName { get; set; }
        public string SalesConsultant { get; set; }
        public string VIPLevel { get; set; }
        public string ContractStatus { get; set; }
        public DateTime DateOfSale { get; set; }
        public DateTime? DateDepositPaid { get; set; }
        public string ContactNo { get; set; }
        public string EmailAddress { get; set; }

        public ContractData()
        {

        }

        public ContractData(string _contractNo, string _bookingRef, string _clientFullName, string _vipLevel, string _contractStatus, DateTime _dateOfSale, DateTime? _dateDepositDate, string _contactNo, string _emailAddress, string _clientSpouseFullName, string _salesConsultant)
        {
            ContractNo = _contractNo;
            BookingRef = _bookingRef;
            ClientFullName = _clientFullName;
            VIPLevel = _vipLevel;
            ContractStatus = _contractStatus;
            DateOfSale = _dateOfSale;
            DateDepositPaid = _dateDepositDate;
            ContactNo = _contactNo;
            EmailAddress = _emailAddress;
            ClientSpouseFullName = _clientSpouseFullName;
            SalesConsultant = _salesConsultant;
        }

        public ContractData(string _contractNo, string _bookingRef, string _clientFullName, string _vipLevel, string _contractStatus)
        {
            ContractNo = _contractNo;
            BookingRef = _bookingRef;
            ClientFullName = _clientFullName;
            VIPLevel = _vipLevel;
            ContractStatus = _contractStatus;
        }

        public static ContractData GetContractData(IConfiguration Configuration, string bookingRef)
        {
            string query = "SELECT C.ContractNo,C.BookingRef, P.Title + ' ' + P.FirstNames + ' ' + P.Surname AS ClientFullName, P.SpouseTitle + ' ' + P.SpouseFirstName + ' ' + P.SpouseSurname As ClientSpouseFullName, ('[' + CAST( PS.PersonnelCode as varchar(25)) + '] - ' + PS.NameAndSurname) AS SalesConsultant, VL.Description AS VipLevel, S.LookupDisplay AS ContractStatus, C.DateOfSale, D.DateDepositPaid, B.Cellphone AS ContactNo, B.EmailAddress FROM TblContract C " +
                            "JOIN TblPortfolio P ON C.Portfolio = P.OID " +
                            "LEFT JOIN TblVipLevels VL ON C.VipAfricaLevel = VL.OID " +
                            "LEFT JOIN Status S ON C.ContractStatus = S.OID " +
                            "LEFT JOIN TblDebtor D ON D.Contract = C.OID " +
                            "LEFT JOIN TblBooking B ON C.BookingRef = B.BookingReference " +
                            "LEFT JOIN TblPersonnel PS ON PS.PersonnelCode = C.SalesAgent " +
                            "WHERE C.BookingRef LIKE '%" + bookingRef + "%'";

            provisionDBContext dbCon = new provisionDBContext(Configuration);
            DataSet ds = dbCon.ReturnQueries("ProvisionDB", query);
            DataTable dt = ds.Tables[0];

            ContractData contractData = new ContractData();

            foreach(DataRow contract in dt.Rows)
            {
                contractData = (new ContractData(contract["ContractNo"].ToString(), contract["BookingRef"].ToString(), contract["ClientFullName"].ToString(), contract["VipLevel"].ToString(), contract["ContractStatus"].ToString(), (DateTime)(contract["DateOfSale"]), contract["DateDepositPaid"] == DBNull.Value ? null : (DateTime?)(contract["DateDepositPaid"]), contract["ContactNo"].ToString(), contract["EmailAddress"].ToString(), contract["ClientSpouseFullName"].ToString(), contract["SalesConsultant"].ToString()));
            }

            return contractData;
        }

        public static List<string> GetPortfolioNumbers(IConfiguration Configuration, string bookingRef)
        {
            string query = "SELECT T.TelephoneNumber AS ContactNum FROM TblContract C " +
                            "JOIN TblPortfolio P ON C.Portfolio = P.PortfolioNumber " +
                            "LEFT JOIN TblTelephones T ON P.PortfolioNumber = T.PortfolioNumber " +
                            "WHERE C.BookingRef LIKE '%" + bookingRef + "%'";

            provisionDBContext dbCon = new provisionDBContext(Configuration);
            DataSet ds = dbCon.ReturnQueries("ProvisionDB", query);
            DataTable dt = ds.Tables[0];

            List<string> contactNumList = new List<string>();

            foreach (DataRow contract in dt.Rows)
            {
                contactNumList.Add(contract["ContactNum"].ToString());
            }

            return contactNumList;
        }

        public static List<ContractData> GetReferalCount(IConfiguration Configuration, string contractNo)
        {
            string query = "SELECT P.Title + ' ' + P.FirstNames + ' ' + P.Surname AS ReferralName, " +
                            "C.ContractNo AS ContractNo, " +
                            "C.BookingRef AS BookingRef, " +
                            "VP.Description AS ContractLevel, " +
                            "S.LookupDisplay AS ContractStatus " +
                            "FROM TblContract C " +
                            "JOIN TblPortfolio P ON C.Portfolio = P.OID " +
                            "JOIN TblVipLevels VP ON C.VipAfricaLevel = VP.OID " +
                            "JOIN Status S ON C.ContractStatus = S.OID " +
                            "WHERE C.MatrixHostContractNr LIKE '%" + contractNo + "%' " +
                            "AND C.ContractStatus IN (3,68)";

            provisionDBContext dbCon = new provisionDBContext(Configuration);
            DataSet ds = dbCon.ReturnQueries("ProvisionDB", query);
            DataTable dt = ds.Tables[0];

            List<ContractData> referralList = new List<ContractData>();

            foreach (DataRow contract in dt.Rows)
            {
                referralList.Add(new ContractData(contract["ContractNo"].ToString(), contract["BookingRef"].ToString(), contract["ReferralName"].ToString(), contract["ContractLevel"].ToString(), contract["ContractStatus"].ToString()));
            }

            return referralList;
        }
    }
}
