using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClassroomReservation
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ReservationStatusPerDay fileInputBox1 = new ReservationStatusPerDay();
            ReservationStatusPerDay fileInputBox2 = new ReservationStatusPerDay();
            ReservationStatusPerDay fileInputBox3 = new ReservationStatusPerDay();
            ReservationStatusPerDay fileInputBox4 = new ReservationStatusPerDay();
            ReservationStatusPerDay fileInputBox5 = new ReservationStatusPerDay();
            ReservationStatusPerDay fileInputBox6 = new ReservationStatusPerDay();

            Content.Children.Add(fileInputBox1);
            Content.Children.Add(fileInputBox2);
            Content.Children.Add(fileInputBox3);
            Content.Children.Add(fileInputBox4);
            Content.Children.Add(fileInputBox5);
            Content.Children.Add(fileInputBox6);
        }
    }
}
