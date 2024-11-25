using DACN.DAO;
using NhatLinh_Tieuluan1.DAO.rsa;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NhatLinh_Tieuluan1.DAO
{
    public class ThanhVienDAO
    {
        public DataTable GetThanhVienData()
        {
            DataTable dt = new DataTable();

            // Câu lệnh gọi thủ tục GetThanhVienData
            string query = "BEGIN GetThanhVienData(:p_cursor); END;";

            // Sử dụng DataProvider để thực thi câu truy vấn và lấy dữ liệu
            using (OracleConnection conn = new OracleConnection(DataProvider.Instance.ConnectionString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    OracleParameter outCursor = new OracleParameter(":p_cursor", OracleDbType.RefCursor);
                    outCursor.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outCursor);

                    // Sử dụng OracleDataAdapter để điền dữ liệu vào DataTable
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }
        
    }
}
