using System;
using System.Security.Cryptography;
using System.Text;

namespace NhatLinh_Tieuluan1.DAO.rsa
{
    public class RSAEncryption
    {
        public static RSAParameters publicKey;
        public static RSAParameters privateKey;

        // Tạo cặp khóa RSA (public/private)
        public static void GenerateKeys()
        {
            using (RSA rsa = RSA.Create()) // RSA.Create() tạo đối tượng RSA
            {
                rsa.KeySize = 2048; // Kích thước khóa 2048-bit
                publicKey = rsa.ExportParameters(false);  // Khóa công khai (chỉ đọc)
                privateKey = rsa.ExportParameters(true);  // Khóa riêng (đọc và ghi)
            }
        }

        // Mã hóa dữ liệu với khóa công khai
        public static string Encrypt(string plainText)
        {
            using (RSA rsa = RSA.Create()) // RSA.Create() tạo đối tượng RSA
            {
                rsa.ImportParameters(publicKey); // Nhập khóa công khai vào đối tượng RSA
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText); // Chuyển văn bản thành byte[]
                byte[] encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1); // Sử dụng PKCS#1 v1.5 padding
                return Convert.ToBase64String(encryptedData); // Chuyển dữ liệu mã hóa thành chuỗi base64
            }
        }

        public static string Decrypt(string encryptedText)
        {
            using (RSA rsa = RSA.Create()) // RSA.Create() tạo đối tượng RSA
            {
                rsa.ImportParameters(privateKey); // Nhập khóa riêng vào đối tượng RSA
                byte[] dataToDecrypt = Convert.FromBase64String(encryptedText); // Chuyển base64 thành byte[]
                byte[] decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.Pkcs1); // Sử dụng PKCS#1 v1.5 padding
                return Encoding.UTF8.GetString(decryptedData); // Chuyển byte[] thành văn bản
            }
        }
    }
}
