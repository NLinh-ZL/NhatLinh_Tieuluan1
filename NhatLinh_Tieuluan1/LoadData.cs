﻿using Oracle.ManagedDataAccess.Client;
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
                    string query = "SELECT * FROM nhatlinh_QLSP.NHANVIEN";
                    OracleCommand cmd = new OracleCommand(query, conn);

                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (applyEncryption)
                    {
                        EncryptDataTable(dataTable, key);
                    }

                    dataGridView1.DataSource = dataTable;
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
                            row[col] = CaesarCipherEncrypt36(originalValue, key); // Mã hóa MATKHAU
                        }
                        else if (col.ColumnName.Equals("LUONG", StringComparison.OrdinalIgnoreCase))
                        {
                            string originalValue = Convert.ToDecimal(row[col]).ToString("F2");
                            row[col] = CaesarCipherEncrypt36(originalValue, key); // Mã hóa LUONG
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
                            row[col] = CaesarCipherDecrypt36(encryptedValue, key); // Giải mã MATKHAU
                        }
                        else if (col.ColumnName.Equals("LUONG", StringComparison.OrdinalIgnoreCase))
                        {
                            string encryptedValue = row[col].ToString();
                            string decryptedValue = CaesarCipherDecrypt36(encryptedValue, key);

                            decimal originalValue;
                            if (decimal.TryParse(decryptedValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out originalValue))

                            {
                                row[col] = originalValue; // Chuyển về số thập phân
                            }
                            else
                            {
                                MessageBox.Show("Không thể chuyển '" + decryptedValue + "' về kiểu số.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            }
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
                    encrypted.Append(c); // Giữ nguyên nếu không thuộc bảng mã
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
                    decrypted.Append(c); // Giữ nguyên nếu không thuộc bảng mã
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
            if (key < 1 || key > 100)
            {
                MessageBox.Show("Khóa phải nằm trong khoảng từ 1 đến 100.", "Lỗi khóa", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (key < 1 || key > 100)
            {
                MessageBox.Show("Khóa phải nằm trong khoảng từ 1 đến 100.", "Lỗi khóa", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
