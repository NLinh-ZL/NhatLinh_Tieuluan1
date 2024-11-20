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
    public partial class LoadData : Form
    {
        public LoadData()
        {
            InitializeComponent();
        }

        private void LoadData_Load(object sender, EventArgs e)
        {
            txtKey.Value = 3;
            LoadDataToGrid(applyEncryption: true);
            btnEncrypt.Enabled = false;
            btnDecrypt.Enabled = true;
            txtKey.Enabled = false;

        }

        private void LoadDataToGrid(bool applyEncryption = false, int key = 3)
        {
            try
            {
                if (!Database.Connect())
                {
                    MessageBox.Show("Kết nối đến cơ sở dữ liệu không thành công.");
                    return;
                }

                using (OracleConnection conn = Database.Get_Connect())
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }

                    OracleCommand cmd = new OracleCommand("BEGIN :nhanVienCursor := nhatlinh_QLSP.F_GetNhanVienData; END;", conn);
                    cmd.CommandType = CommandType.Text;

                    OracleParameter outParam = new OracleParameter("nhanVienCursor", OracleDbType.RefCursor);
                    outParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outParam);

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        if (applyEncryption)
                        {
                            EncryptDataTable(dataTable, key);
                        }

                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }



        private void EncryptDataTable(DataTable dataTable, int key)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (row[col] != DBNull.Value)
                    {
                        if (col.ColumnName.Equals("MATKHAU", StringComparison.OrdinalIgnoreCase))
                        {
                            string originalValue = row[col].ToString();
                            row[col] = CaesarCipherEncrypt36(originalValue, key);
                        }
                    }
                }
            }
        }


        private void DecryptDataTable(DataTable dataTable, int key)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (row[col] != DBNull.Value)
                    {
                        if (col.ColumnName.Equals("MATKHAU", StringComparison.OrdinalIgnoreCase))
                        {
                            string encryptedValue = row[col].ToString();
                            row[col] = CaesarCipherDecrypt36(encryptedValue, key);
                        }
                    }
                }
            }
        }


        private string CaesarCipherEncrypt36(string input, int key)
        {
            const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder encrypted = new StringBuilder();

            foreach (char c in input.ToUpper())
            {
                int index = Alphabet.IndexOf(c);
                if (index != -1)
                {
                    encrypted.Append(Alphabet[(index + key) % 36]);
                }
                else
                {
                    encrypted.Append(c);
                }
            }

            return encrypted.ToString();
        }

        private string CaesarCipherDecrypt36(string input, int key)
        {
            const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder decrypted = new StringBuilder();

            foreach (char c in input.ToUpper())
            {
                int index = Alphabet.IndexOf(c);
                if (index != -1)
                {
                    int newIndex = (index - key + 36) % 36;
                    decrypted.Append(Alphabet[newIndex]);
                }
                else
                {
                    decrypted.Append(c);
                }
            }

            return decrypted.ToString();
        }

        private void btn_logout_Click(object sender, EventArgs e)
        {
            if (Database.Get_Connect() != null && Database.Get_Connect().State == ConnectionState.Open)
            {
                Database.Get_Connect().Close();
            }

            DangNhap loginForm = new DangNhap();
            loginForm.Show();

            this.Close();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            int key = (int)txtKey.Value;
            if (key < 1 || key > 36)
            {
                MessageBox.Show("Khóa phải nằm trong khoảng từ 1 đến 36.", "Lỗi khóa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dataGridView1.DataSource != null && dataGridView1.DataSource is DataTable)
            {
                DataTable dataTable = (DataTable)dataGridView1.DataSource;
                EncryptDataTable(dataTable, key); 
                btnEncrypt.Enabled = false;
                btnDecrypt.Enabled = true;
                txtKey.Enabled = false;
            }

        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            int key = (int)txtKey.Value;
            if (key < 1 || key > 36)
            {
                MessageBox.Show("Khóa phải nằm trong khoảng từ 1 đến 36.", "Lỗi khóa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dataGridView1.DataSource != null && dataGridView1.DataSource is DataTable)
            {
                DataTable dataTable = (DataTable)dataGridView1.DataSource;
                DecryptDataTable(dataTable, key); 
                btnEncrypt.Enabled = true;
                btnDecrypt.Enabled = false;
                txtKey.Enabled = true;
            }
        }

        private void txt_mhnhan_Click(object sender, EventArgs e)
        {
            NV_MHNhan mhNhan = new NV_MHNhan();
            mhNhan.Show();
            this.Hide();
        }
    }
}
