using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhatLinh_Tieuluan1
{
    public class Database
    {
        public static OracleConnection Conn;

        public static string Host;
        public static string Port;
        public static string Sid;
        public static string User;
        public static string PassWord;

        public static void Set_Database(string host, string port, string sid, string user, string pass)
        {
            Database.Host = host;
            Database.Port = port;
            Database.Sid = sid;
            Database.User = user;
            Database.PassWord = pass;
        }

        public static bool Connect()
        {
            string consys = "";
            try
            {
                if (User.ToUpper().Equals("SYS"))
                {
                    consys = ";DBA Privilege=SYSDBA;";
                }
                string connString = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = " + Host + ")(PORT = " + Port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = "
                    + Sid + "))); USER ID =" + User + "; Password = " + PassWord + consys;

                Conn = new OracleConnection();
                Conn.ConnectionString = connString;
                Conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static OracleConnection Get_Connect()
        {
            if (Conn == null)
            {
                Connect();
            }
            return Conn;
        }
    }
}
