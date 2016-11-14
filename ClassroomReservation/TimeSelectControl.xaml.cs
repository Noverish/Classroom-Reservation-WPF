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
    /// TimeSelectControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TimeSelectControl : UserControl
    {
        private IEnumerable<Label> buttons;
        private int[] nowSelectedTime = new int[2];

        public TimeSelectControl()
        {
            InitializeComponent();
            nowSelectedTime[0] = nowSelectedTime[1] = -1;
            buttons = mainGrid.Children.OfType<Label>();

            foreach (Label btn in buttons)
            {
                btn.MouseLeftButtonDown += new MouseButtonEventHandler(OnClick);
                btn.MouseEnter += new MouseEventHandler(OnMouseEnter);
                btn.MouseLeave += new MouseEventHandler(OnMouseLeave);
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            foreach(Label btn in buttons)
            {
                btn.Background = Brushes.White;
            }

            Label button = sender as Label;
            int index = Grid.GetRow(button) + 1;

            button.Background = (SolidColorBrush)Application.Current.FindResource("MicrosoftBlue");
            nowSelectedTime[0] = nowSelectedTime[1] = index;
        }

        private void OnMouseEnter(object sender, RoutedEventArgs e)
        {
            Label button = sender as Label;
            int index = Grid.GetRow(button) + 1;
            if (nowSelectedTime[0] <= index && index <= nowSelectedTime[1])
            {
                button.Background = (SolidColorBrush)Application.Current.FindResource("MicrosoftBlue");
            }
            else
            {
                button.Background = (SolidColorBrush)Application.Current.FindResource("MicrosoftRed");
            }
        }

        private void OnMouseLeave(object sender, RoutedEventArgs e)
        {
            Label button = sender as Label;
            int index = Grid.GetRow(button) + 1;
            if (nowSelectedTime[0] <= index && index <= nowSelectedTime[1])
            {
                button.Background = (SolidColorBrush)Application.Current.FindResource("MicrosoftBlue");
            }
            else
            {
                button.Background = Brushes.White;
            }
        }
    }
}
