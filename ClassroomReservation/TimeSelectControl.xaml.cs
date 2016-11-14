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
        private bool mouseLeftButtonDown = false;
        private SolidColorBrush selectedColor, hoverColor;

        public TimeSelectControl()
        {
            InitializeComponent();
            nowSelectedTime[0] = nowSelectedTime[1] = -1;
            buttons = mainGrid.Children.OfType<Label>();
            selectedColor = (SolidColorBrush)Application.Current.FindResource("MicrosoftBlue");
            hoverColor = (SolidColorBrush)Application.Current.FindResource("MicrosoftRed");


            foreach (Label btn in buttons)
            {
                btn.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
                btn.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
                btn.MouseEnter += new MouseEventHandler(OnMouseEnter);
                btn.MouseLeave += new MouseEventHandler(OnMouseLeave);
            }
        }

        private void OnMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            foreach(Label btn in buttons)
            {
                btn.Background = Brushes.White;
            }

            Label button = sender as Label;
            int index = Grid.GetRow(button) + 1;

            button.Background = selectedColor;
            nowSelectedTime[0] = nowSelectedTime[1] = index;
            mouseLeftButtonDown = true;
        }

        private void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            mouseLeftButtonDown = false;
        }

        private void OnMouseEnter(object sender, RoutedEventArgs e)
        {
            Label button = sender as Label;
            int index = Grid.GetRow(button) + 1;

            if(mouseLeftButtonDown)
            {
                if(nowSelectedTime[1] - nowSelectedTime[0] < 2)
                {
                    if(index < nowSelectedTime[0])
                    {
                        nowSelectedTime[0] = index;
                        button.Background = selectedColor;
                    }
                    else if(nowSelectedTime[1] < index)
                    {
                        nowSelectedTime[1] = index;
                        button.Background = selectedColor;
                    }
                }
            }
            else
            {
                if (nowSelectedTime[0] <= index && index <= nowSelectedTime[1])
                {
                    button.Background = selectedColor;
                }
                else
                {
                    button.Background = hoverColor;
                }
            }

            
        }

        private void OnMouseLeave(object sender, RoutedEventArgs e)
        {
            Label button = sender as Label;
            int index = Grid.GetRow(button) + 1;
            if (nowSelectedTime[0] <= index && index <= nowSelectedTime[1])
            {
                button.Background = selectedColor;
            }
            else
            {
                button.Background = Brushes.White;
            }
        }
    }
}
