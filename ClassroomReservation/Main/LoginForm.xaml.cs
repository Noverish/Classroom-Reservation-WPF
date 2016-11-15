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

namespace ClassroomReservation.Main
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter("a.txt");
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
            Insert_Password.Clear();
        }
    }
}
