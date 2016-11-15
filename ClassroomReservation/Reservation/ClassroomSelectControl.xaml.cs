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
    /// <summary>
    /// TimeSelectControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClassroomSelectControl : UserControl
    {
        private IEnumerable<Label> buttons;

        private bool mouseLeftButtonDown = false;
        private SolidColorBrush selectedColor, hoverColor;

        private SolidColorBrush backgroundEven = (SolidColorBrush)Application.Current.FindResource("BackgroundOfEvenRow");
        private SolidColorBrush backgroundOdd = (SolidColorBrush)Application.Current.FindResource("BackgroundOfOddRow");

        private Label[] labelNames;
        private int previousColor = -1;

        public ClassroomSelectControl()
        {
            InitializeComponent();

            labelNames = new Label[]{ time1, time2, time3, time4, time5, time6, time7, time8, time9, time10, time11, time12};

            for (int i=0; i<12; i++)
            {
                if(i%2==0)
                {
                    labelNames[i].Background = backgroundOdd;
                }
                else
                {
                    labelNames[i].Background = backgroundEven;
                }
            }

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
            for (int i = 0; i < 12; i++)
            {
                if (i % 2 == 0)
                {
                    labelNames[i].Background = backgroundOdd;
                }
                else
                {
                    labelNames[i].Background = backgroundEven;
                }
            }

            (sender as Label).Background = selectedColor;
            previousColor = -1;
            mouseLeftButtonDown = true;
        }

        private void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            mouseLeftButtonDown = false;
        }

        private void OnMouseEnter(object sender, RoutedEventArgs e)
        {        
            if(mouseLeftButtonDown){}

            else
            {
                if(previousColor>=0 && previousColor % 2 == 0)
                {
                    labelNames[previousColor].Background = backgroundOdd;
                }
                else if(previousColor >= 0 && previousColor % 2 ==1)
                {
                    labelNames[previousColor].Background = backgroundEven;
                }
                previousColor = Grid.GetRow(sender as Label);
                labelNames[previousColor].Background = hoverColor;
            }
        }

        private void OnMouseLeave(object sender, RoutedEventArgs e)
        {
            //getrow
        }
    }
}
