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

    public delegate void OnClick(Window window, string id, string password);

    public partial class LoginForm : Window
    {
        private OnClick callback;

        public LoginForm(OnClick callback)
        {
            InitializeComponent();
            this.callback = callback;

            LoginButton.Click += new RoutedEventHandler((sender, e) => {
                callback?.Invoke(this, Insert_Id.Text, Insert_Password.Password);
            });
        }
    }
}
