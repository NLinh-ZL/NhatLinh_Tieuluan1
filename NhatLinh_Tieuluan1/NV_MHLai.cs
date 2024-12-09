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
    public partial class NV_MHLai : Form
    {
        public NV_MHLai()
        {
            InitializeComponent();
        }

        private void LoadDataToGrid(bool applyEncryption = false, int key = 5)
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



        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            // Kiểm tra nguồn dữ liệu từ DataGridView
            if (dataGridView1.DataSource != null && dataGridView1.DataSource is DataTable dataTable)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    if (!row.IsNull("MATKHAU"))
                    {
                        string plainText = row["MATKHAU"].ToString();
                        string encryptedText = EncryptAddressInOracle(plainText); // Mã hóa MATKHAU
                        row["MATKHAU"] = encryptedText;
                    }
                }

                MessageBox.Show("Mã hóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnEncrypt.Enabled = false;
                btnDecrypt.Enabled = true;
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            // Kiểm tra nguồn dữ liệu từ DataGridView
            if (dataGridView1.DataSource != null && dataGridView1.DataSource is DataTable dataTable)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    if (!row.IsNull("MATKHAU"))
                    {
                        string encryptedText = row["MATKHAU"].ToString();
                        string decryptedText = DecryptAddressInOracle(encryptedText); // Giải mã MATKHAU
                        row["MATKHAU"] = decryptedText;
                    }
                }

                MessageBox.Show("Giải mã thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnEncrypt.Enabled = true;
                btnDecrypt.Enabled = false;
            }
        }

        private string EncryptAddressInOracle(string plainText)
        {
            using (OracleConnection conn = Database.Get_Connect()) // Sử dụng kết nối hiện tại
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("BEGIN :result := Crypto_Pkg.EncryptData(:plainText); END;", conn))
                {
                    cmd.Parameters.Add("plainText", OracleDbType.Varchar2).Value = plainText;
                    cmd.Parameters.Add("result", OracleDbType.Raw, ParameterDirection.ReturnValue);

                    cmd.ExecuteNonQuery();
                    return BitConverter.ToString((byte[])cmd.Parameters["result"].Value).Replace("-", "");
                }
            }
        }

        private string DecryptAddressInOracle(string encryptedText)
        {
            using (OracleConnection conn = Database.Get_Connect()) // Sử dụng kết nối hiện tại
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("BEGIN :result := Crypto_Pkg.DecryptData(:encryptedText); END;", conn))
                {
                    cmd.Parameters.Add("encryptedText", OracleDbType.Raw).Value = StringToByteArray(encryptedText);
                    cmd.Parameters.Add("result", OracleDbType.Varchar2, ParameterDirection.ReturnValue);

                    cmd.ExecuteNonQuery();
                    return cmd.Parameters["result"].Value.ToString();
                }
            }
        }


        private byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length / 2)
                             .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))
                             .ToArray();
        }



        private void txt_mhlai_Click(object sender, EventArgs e)
        {
            LoadData loadDataForm = new LoadData();
            loadDataForm.Show();
            this.Hide();
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

        private void NV_MHLai_Load(object sender, EventArgs e)
        {
            txtKey.Value = 3;
            LoadDataToGrid(applyEncryption: true);
            btnEncrypt.Enabled = false;
            btnDecrypt.Enabled = true;
            txtKey.Enabled = false;
        }
    }
}