using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Reflection;

namespace ClassroomReservation.Main
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class LoginForm : Window
    {
        private LoginFormOnClick onClick;
        //private string secret;
        private string Id;
        private string Pw;

        public LoginForm(LoginFormOnClick onClick)
        {
            InitializeComponent();

            this.onClick = onClick;
            LoginButton.Click += new RoutedEventHandler(Button_Click);
        }

        public static string EncryptString(string InputText, string Password)
        {
            // Rihndael class를 선언하고, 초기화 합니다
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(InputText);

            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
            
            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(PlainText, 0, PlainText.Length);

            cryptoStream.FlushFinalBlock();

            byte[] CipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string EncryptedData = Convert.ToBase64String(CipherBytes);

            Console.WriteLine(DecryptString(EncryptedData, Password));

            return EncryptedData;
        }

        public static string DecryptString(string InputText, string Password)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            byte[] EncryptedData = Convert.FromBase64String(InputText);
            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

            MemoryStream memoryStream = new MemoryStream(EncryptedData);

            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

            byte[] PlainText = new byte[EncryptedData.Length];

            int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

            memoryStream.Close();
            cryptoStream.Close();

            string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);

            return DecryptedData;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Id = Insert_Id.Text;
            Pw = Insert_Password.Password;

            onClick.OnClick(this, Id, Pw);
            
            //StreamWriter sw = new StreamWriter("a.txt");
            //sw.WriteLine(encrytedData);
            /*StreamWriter sw = new StreamWriter("a.txt");
            StreamReader sr = new StreamReader("check.txt");

            while (sr.Peek() >= 0)
            {
                if (String.Equals(sr.ReadLine(), Insert_Id.Text) && String.Equals(sr.ReadLine(), Insert_Password.Password))
                {
                    sw.WriteLine("check");
                    break;
                }
            }

            sw.Close();
            sr.Close();

            Insert_Id.Clear();
            Insert_Password.Clear();*/
        }

    }
}
