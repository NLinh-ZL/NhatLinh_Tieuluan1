using System;
using System.Data;
using System.Linq;
using Oracle.ManagedDataAccess.Client;

namespace DACN.DAO
{
    public class DataProvider
    {
        private static DataProvider instance;

        private string conStr = "Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))" +
                               "(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = orcl))); " +
                               "USER ID = nhatlinh_QLSP; PASSWORD = 123";

        public static DataProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataProvider();
                }
                return instance;
            }
            private set { instance = value; }
        }

        private DataProvider() { }

        public string ConnectionString => conStr;

        /// <summary>
        /// Thực hiện câu truy vấn và trả về kết quả dưới dạng DataTable.
        /// </summary>
        public DataTable ExecuteQuery(string query, object[] parameter = null)
        {
            DataTable dt = new DataTable();
            using (OracleConnection connection = new OracleConnection(conStr))
            {
                connection.Open();
                using (OracleCommand cmd = new OracleCommand(query, connection))
                {
                    if (parameter != null)
                    {
                        string[] listPara = query.Split(' ');
                        int i = 0;
                        foreach (string item in listPara)
                        {
                            if (item.Contains(':'))
                            {
                                cmd.Parameters.Add(new OracleParameter(item, parameter[i]));
                                i++;
                            }
                        }
                    }

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// Thực hiện câu lệnh SQL không trả về dữ liệu (INSERT, UPDATE, DELETE).
        /// </summary>
        public int ExecuteNonQuery(string query, object[] parameter = null)
        {
            int data = 0;
            using (OracleConnection connection = new OracleConnection(conStr))
            {
                connection.Open();
                using (OracleCommand cmd = new OracleCommand(query, connection))
                {
                    if (parameter != null)
                    {
                        string[] listPara = query.Split(' ');
                        int i = 0;
                        foreach (string item in listPara)
                        {
                            if (item.Contains(':'))
                            {
                                cmd.Parameters.Add(new OracleParameter(item, parameter[i]));
                                i++;
                            }
                        }
                    }

                    data = cmd.ExecuteNonQuery();
                }
            }
            return data;
        }

        /// <summary>
        /// Thực hiện câu lệnh SQL và trả về giá trị đơn (Scalar).
        /// </summary>
        public object ExecuteScalar(string query, object[] parameter = null)
        {
            object data = null;
            using (OracleConnection connection = new OracleConnection(conStr))
            {
                connection.Open();
                using (OracleCommand cmd = new OracleCommand(query, connection))
                {
                    if (parameter != null)
                    {
                        string[] listPara = query.Split(' ');
                        int i = 0;
                        foreach (string item in listPara)
                        {
                            if (item.Contains(':'))
                            {
                                cmd.Parameters.Add(new OracleParameter(item, parameter[i]));
                                i++;
                            }
                        }
                    }

                    data = cmd.ExecuteScalar();
                }
            }
            return data;
        }
    }
}
