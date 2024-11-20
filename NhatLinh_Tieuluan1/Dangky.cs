using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NhatLinh_Tieuluan1
{
    public partial class Dangky : Form
    {
        public Dangky()
        {
            InitializeComponent();
        }

        private void btn_dk_Click(object sender, EventArgs e)
        {
            string newUser = txt_user.Text;
            string newPassword = txt_pass.Text;
            string dataOwner = "nhatlinh_QLSP";
            string tableName = "NHANVIEN";

            if (string.IsNullOrEmpty(newUser) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return;
            }

            try
            {
                Database.Set_Database("localhost", "1521", "orcl", "admin_user", "123");

                if (!Database.Connect())
                {
                    MessageBox.Show("Kết nối với tài khoản admin_user thất bại.");
                    return;
                }

                OracleCommand cmd = Database.Get_Connect().CreateCommand();
                cmd.CommandText = "BEGIN admin_user.Pr_create_user(:p_new_user, :p_new_password, :p_data_owner, :p_table_name); END;";

                cmd.Parameters.Add("p_new_user", OracleDbType.Varchar2).Value = newUser;
                cmd.Parameters.Add("p_new_password", OracleDbType.Varchar2).Value = newPassword;
                cmd.Parameters.Add("p_data_owner", OracleDbType.Varchar2).Value = dataOwner;
                cmd.Parameters.Add("p_table_name", OracleDbType.Varchar2).Value = tableName;

                cmd.ExecuteNonQuery();

                MessageBox.Show("Tài khoản được tạo thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo tài khoản: {ex.Message}");
            }
            finally
            {
                if (Database.Conn != null && Database.Conn.State != System.Data.ConnectionState.Closed)
                {
                    Database.Conn.Close();
                }
            }
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            DangNhap dn = new DangNhap();
            dn.Show();
            this.Hide();
        }
    }
}
