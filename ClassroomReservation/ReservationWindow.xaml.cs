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
using System.Windows.Shapes;

namespace ClassroomReservation
{
    /// <summary>
    /// WpfApplication3.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReservationWindow : Window
    {
        public ReservationWindow()
        {
            InitializeComponent();

            Cancel_Button.Click += new RoutedEventHandler(Button_Click_Close);

            //int column = 0;
            //for(int row = 0;row<10;row++)
            //{
            //    Button button = (Button) mainGrid.Children
            //        .Cast<UIElement>()
            //        .First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == column);

            //    button.Click += new RoutedEventHandler(ChangeButtonColorToSelected);
            //}

        }

        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChangeButtonColorToSelected(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
