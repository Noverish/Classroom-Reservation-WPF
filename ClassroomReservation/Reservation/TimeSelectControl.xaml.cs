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

namespace ClassroomReservation.Reservation
{
    public delegate void OnTimeSelectChanged(int[] nowSelectedTime);

    /// <summary>
    /// TimeSelectControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TimeSelectControl : UserControl
    {
        private OnTimeSelectChanged callback;

        private IEnumerable<Label> buttons;
        private int[] nowSelectedTime = new int[2];
        private bool mouseLeftButtonDown = false;

        private SolidColorBrush selectedColor = (SolidColorBrush)Application.Current.FindResource("MicrosoftBlue");
        private SolidColorBrush hoverColor = (SolidColorBrush)Application.Current.FindResource("MicrosoftRed");
        private SolidColorBrush backgroundEven = (SolidColorBrush)Application.Current.FindResource("BackgroundOfEvenRow");
        private SolidColorBrush backgroundOdd = (SolidColorBrush)Application.Current.FindResource("BackgroundOfOddRow");

        private Label previousButton = null;

        public TimeSelectControl()
        {
            InitializeComponent();
            nowSelectedTime[0] = nowSelectedTime[1] = -1;
            buttons = mainGrid.Children.OfType<Label>();

            ResetBackground();

            foreach (Label btn in buttons)
            {
                btn.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
                btn.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
                btn.MouseEnter += new MouseEventHandler(OnMouseEnter);
                btn.MouseLeave += new MouseEventHandler(OnMouseLeave);
            }
        }

        public void ResetSelection() {
            nowSelectedTime[0] = nowSelectedTime[1] = -1;
            ResetBackground();
        }

        public bool HasSeletedTime() {
            return (nowSelectedTime[0] >= 1 && nowSelectedTime[1] >= 1);
        }

        public int[] GetSelectedTime() {
            return nowSelectedTime;
        }

        public void SetOnTimeSelectChanged(OnTimeSelectChanged func) {
            callback = func;
        }

        private void OnMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            ResetBackground();

            Label button = sender as Label;
            int index = Grid.GetRow(button) + 1;

            button.Background = selectedColor;
            nowSelectedTime[0] = nowSelectedTime[1] = index;
            mouseLeftButtonDown = true;
        }

        private void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            mouseLeftButtonDown = false;

            callback(nowSelectedTime);
        }

        private void OnMouseEnter(object sender, RoutedEventArgs e) //when mouse on entered area
        {
            Label button = sender as Label;
            int index = Grid.GetRow(button) + 1;

            if(mouseLeftButtonDown)
            {
                if(nowSelectedTime[1] - nowSelectedTime[0] < 2)
                {
                    if(index < nowSelectedTime[0] && nowSelectedTime[0] - index < 2)
                    {
                        nowSelectedTime[0] = index;
                        button.Background = selectedColor;
                    }
                    else if(nowSelectedTime[1] < index && index - nowSelectedTime[1] < 2)
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
                    if (previousButton !=null && previousButton.Background != selectedColor && Grid.GetRow(previousButton) % 2 == 0)
                        previousButton.Background = backgroundOdd;
                    else if(previousButton != null && previousButton.Background != selectedColor && Grid.GetRow(previousButton) % 2 == 1)
                        previousButton.Background = backgroundEven;
                    button.Background = hoverColor;
                    previousButton = button;
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
                if (Grid.GetRow(button) % 2 == 0)
                    button.Background = backgroundOdd;
                else if (Grid.GetRow(button) % 2 == 1)
                    button.Background = backgroundEven;
            }
        }

        private void ResetBackground() {
            foreach (Label btn in buttons) {
                if (Grid.GetRow(btn) % 2 == 0)
                    btn.Background = backgroundOdd;
                else
                    btn.Background = backgroundEven;
            }
        }
    }
}
