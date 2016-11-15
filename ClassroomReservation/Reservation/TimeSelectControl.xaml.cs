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
    public partial class TimeSelectControl : UserControl
    {
        private static List<Label> selectedViews = new List<Label>();

        private const int TOTAL = 10;

        private List<Label> buttons;
        
        private bool mouseLeftButtonDown = false;

        private SolidColorBrush selectedColor = (SolidColorBrush)Application.Current.FindResource("MicrosoftBlue");
        private SolidColorBrush hoverColor = (SolidColorBrush)Application.Current.FindResource("MicrosoftRed");
        private SolidColorBrush backgroundEven = (SolidColorBrush)Application.Current.FindResource("BackgroundOfEvenRow");
        private SolidColorBrush backgroundOdd = (SolidColorBrush)Application.Current.FindResource("BackgroundOfOddRow");

        public TimeSelectControl()
        {
            InitializeComponent();
            buttons = mainGrid.Children.OfType<Label>().ToList();
            
            foreach (Label btn in buttons)
            {
                setOriginColor(btn);

                btn.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
                btn.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
                btn.MouseMove += new MouseEventHandler(onMouseMove);
                btn.MouseEnter += new MouseEventHandler(OnMouseEnter);
                btn.MouseLeave += new MouseEventHandler(OnMouseLeave);
            }
        }

        private void OnMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Label button = sender as Label;

            foreach (Label btn in buttons)
                setOriginColor(btn);
            button.Background = selectedColor;

            selectedViews.Clear();
            selectedViews.Add(button);

            mouseLeftButtonDown = true;

            Mouse.Capture(button);
        }

        private void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            mouseLeftButtonDown = false;

            Mouse.Capture(null);
        }

        private void onMouseMove(object sender, RoutedEventArgs e)
        {
            if (mouseLeftButtonDown)
            {
                Label btn = sender as Label;
                double height = btn.ActualHeight;
                double pos = Mouse.GetPosition(btn).Y;
                int row = Grid.GetRow(btn);
                
                Console.WriteLine(height + " " + pos);

                if (-2 * height < pos && pos < -height)
                {
                    setting(row - 2, true);
                    setting(row - 1, true);
                    setting(row + 1, false);
                    setting(row + 2, false);
                }
                else if (-height < pos && pos < 0)
                {
                    setting(row - 2, false);
                    setting(row - 1, true);
                    setting(row + 1, false);
                    setting(row + 2, false);
                }
                else if (0 < pos && pos < height)
                {
                    setting(row - 2, false);
                    setting(row - 1, false);
                    setting(row + 1, false);
                    setting(row + 2, false);
                }
                else if (height < pos && pos < 2 * height)
                {
                    setting(row - 2, false);
                    setting(row - 1, false);
                    setting(row + 1, true);
                    setting(row + 2, false);
                }
                else if (2 * height < pos && pos < 3 * height)
                {
                    setting(row - 2, false);
                    setting(row - 1, false);
                    setting(row + 1, true);
                    setting(row + 2, true);
                }
            }
        }

        private void OnMouseEnter(object sender, RoutedEventArgs e)
        {
            Label btn = sender as Label;

            if (!selectedViews.Contains(btn))
                btn.Background = hoverColor;
        }

        private void OnMouseLeave(object sender, RoutedEventArgs e)
        {
            Label btn = sender as Label;

            if (!selectedViews.Contains(btn))
                setOriginColor(btn);

        }

        private void setOriginColor(Label button)
        {
            if (Grid.GetRow(button) % 2 == 0)
                button.Background = backgroundEven;
            else
                button.Background = backgroundOdd;
        }

        private void setting(int index, bool makeSelect)
        {
            if(0 <= index && index < TOTAL)
            {
                Label button = buttons[index];

                if (makeSelect)
                {
                    button.Background = selectedColor;
                    selectedViews.Add(button);
                }
                else
                {
                    setOriginColor(button);
                    selectedViews.Remove(button);
                }
            }
        }
    }
}
