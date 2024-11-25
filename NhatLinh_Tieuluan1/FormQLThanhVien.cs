using NhatLinh_Tieuluan1.DAO;
using NhatLinh_Tieuluan1.DAO.rsa;
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
    public partial class FormQLThanhVien : Form
    {
        public FormQLThanhVien()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                // Tạo đối tượng DAO mới
                ThanhVienDAO dao = new ThanhVienDAO();

                // Gọi phương thức GetThanhVienData để lấy dữ liệu từ thủ tục
                DataTable dt = dao.GetThanhVienData();

                // Hiển thị dữ liệu lên DataGridView
                dgv_ThanhVien.DataSource = dt;
                RSAEncryption.GenerateKeys();
                EncryptPhoneNumberColumn(dgv_ThanhVien);
            }
            catch (Exception ex)
            {
                Console.Write("Lỗi khi lấy dữ liệu: " + ex.Message);
            }
        }
        private void EncryptPhoneNumberColumn(DataGridView dataGridView)
        {
            // Lặp qua từng dòng trong DataGridView và mã hóa số điện thoại
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["SDT"].Value != null)
                {
                    string phoneNumber = row.Cells["SDT"].Value.ToString();

                    // Mã hóa số điện thoại
                    string encryptedPhoneNumber = RSAEncryption.Encrypt(phoneNumber);

                    // Gán giá trị mã hóa vào cột "Số điện thoại"
                    row.Cells["SDT"].Value = encryptedPhoneNumber;
                }
            }
        }
        public static void DecryptPhoneNumberColumn(DataGridView dataGridView)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["SDT"].Value != null)
                {
                    string encryptedPhoneNumber = row.Cells["SDT"].Value.ToString();

                    // Giải mã số điện thoại
                    string decryptedPhoneNumber = RSAEncryption.Decrypt(encryptedPhoneNumber);

                    // Gán giá trị giải mã vào cột "Số điện thoại"
                    row.Cells["SDT"].Value = decryptedPhoneNumber;
                }
            }
        }

        private void btn_GiaiMa_Click(object sender, EventArgs e)
        {
            DecryptPhoneNumberColumn(dgv_ThanhVien);
        }

        private void btn_MaHoa_Click(object sender, EventArgs e)
        {
            EncryptPhoneNumberColumn(dgv_ThanhVien);
        }
    }
}
