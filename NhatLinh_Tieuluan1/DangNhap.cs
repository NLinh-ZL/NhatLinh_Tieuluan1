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
    public partial class DangNhap : Form
    {
        public DangNhap()
        {
            InitializeComponent();
        }

        bool Check_TextBox(string user, string pass)
        {
            if(user == "")
            {
                MessageBox.Show("Chưa điền thông tin vào User");
                txt_user.Focus();
                return false;
            }
            else if (pass == "")
            {
                MessageBox.Show("Chưa điền thông tin vào Pass");
                txt_pass.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            string host = "localhost";
            string port = "1521";
            string sid = "orcl";
            string user = txt_user.Text;
            string pass = txt_pass.Text;
            if (Check_TextBox(user, pass))
            {
                Database.Set_Database(host, port, sid, user, pass);
                if (Database.Connect())
                {
                    OracleConnection c = Database.Get_Connect();
                    MessageBox.Show("Đăng nhập thành công " + c.ServerVersion);

                    LoadData loadDataForm = new LoadData();
                    loadDataForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Đăng nhập thất bại");
                }
            }
        }
    }
}
