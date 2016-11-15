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
using System.Globalization;

namespace ClassroomReservation.Main
{
    /// <summary>
    /// ReservationStatusPerDay.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReservationStatusPerDay : UserControl
    {
        private static List<TextBlock> selectedViews = new List<TextBlock>();

        private const int COLUMN_NUMBER = 10;
        private const int ROW_NUMBER = 12;

        private TextBlock[,] buttons = new TextBlock[ROW_NUMBER, COLUMN_NUMBER];

        private bool mouseLeftButtonDown = false;

        private Brush backgroundOdd = (SolidColorBrush)Application.Current.FindResource("BackgroundOfOddRow");
        private Brush backgroundEven = (SolidColorBrush)Application.Current.FindResource("BackgroundOfEvenRow");
        private Brush selectColor = (SolidColorBrush)Application.Current.FindResource("Selected");
        private Brush hoverColor = (SolidColorBrush)Application.Current.FindResource("Hover");

        public ReservationStatusPerDay(DateTime date)
        {
            InitializeComponent();

            CultureInfo cultures = CultureInfo.CreateSpecificCulture("ko-KR");
            DateTextBlock.Content = date.ToString(string.Format("yyyy년 MM월 dd일 ddd요일", cultures));

            for (int row = 0; row < ROW_NUMBER; row++)
            {
                for (int column = 0; column < COLUMN_NUMBER; column++)
                {
                    TextBlock newButton = new TextBlock();


                    newButton.VerticalAlignment = VerticalAlignment.Stretch;
                    newButton.HorizontalAlignment = HorizontalAlignment.Stretch;
                    newButton.Name = "_" + row + "_" + column;

                    newButton.MouseDown += new MouseButtonEventHandler(onMouseDown);
                    newButton.MouseUp += new MouseButtonEventHandler(onMouseUp);
                    newButton.MouseMove += new MouseEventHandler(onMouseMove);
                    newButton.MouseEnter += new MouseEventHandler(onMouseEnter);
                    newButton.MouseLeave += new MouseEventHandler(OnMouseLeave);

                    setOriginColor(newButton);

                    buttons[row, column] = newButton;

                    Border myBorder1 = new Border();
                    myBorder1.BorderBrush = Brushes.Gray;
                    if (column == 0)
                        myBorder1.BorderThickness = new Thickness { Top = 0, Bottom = 0, Left = 0, Right = 1 };
                    else if (column == 9)
                        myBorder1.BorderThickness = new Thickness { Top = 0, Bottom = 0, Left = 0, Right = 0 };
                    else
                        myBorder1.BorderThickness = new Thickness { Top = 0, Bottom = 0, Left = 0, Right = 1 };
                    
                    myBorder1.Child = newButton;

                    Grid.SetRow(myBorder1, row + 2);
                    Grid.SetColumn(myBorder1, column);

                    mainGrid.Children.Add(myBorder1);
                }
            }
        }

        private void onMouseDown(object sender, RoutedEventArgs e)
        {
            TextBlock button = sender as TextBlock;

            foreach(TextBlock selectedView in selectedViews)
                setOriginColor(selectedView);
            button.Background = selectColor;

            selectedViews.Clear();
            selectedViews.Add(button);

            mouseLeftButtonDown = true;

            Mouse.Capture(button);
        }

        private void onMouseUp(object sender, RoutedEventArgs e)
        {
            mouseLeftButtonDown = false;
            Mouse.Capture(null);
        }

        private void onMouseMove(object sender, RoutedEventArgs e)
        {
            if (mouseLeftButtonDown)
            {
                TextBlock btn = sender as TextBlock;
                double width = btn.ActualWidth;
                double pos = Mouse.GetPosition(btn).X;
                int row = getRow(btn);
                int column = getColumn(btn);

                if (-2 * width < pos && pos < -width)
                {
                    setting(row, column - 2, true);
                    setting(row, column - 1, true);
                    setting(row, column + 1, false);
                    setting(row, column + 2, false);
                }
                else if (-width < pos && pos < 0)
                {
                    setting(row, column - 2, false);
                    setting(row, column - 1, true);
                    setting(row, column + 1, false);
                    setting(row, column + 2, false);
                }
                else if(0 < pos && pos < width)
                {
                    setting(row, column - 2, false);
                    setting(row, column - 1, false);
                    setting(row, column + 1, false);
                    setting(row, column + 2, false);
                }
                else if (width < pos && pos < 2 * width)
                {
                    setting(row, column - 2, false);
                    setting(row, column - 1, false);
                    setting(row, column + 1, true);
                    setting(row, column + 2, false);
                }
                else if (2 * width < pos && pos < 3 * width)
                {
                    setting(row, column - 2, false);
                    setting(row, column - 1, false);
                    setting(row, column + 1, true);
                    setting(row, column + 2, true);
                }
            }
        }

        private void onMouseEnter(object sender, RoutedEventArgs e)
        {
            TextBlock btn = sender as TextBlock;

            if (!selectedViews.Contains(btn))
                btn.Background = hoverColor;
        }

        private void OnMouseLeave(object sender, RoutedEventArgs e)
        {
            TextBlock btn = sender as TextBlock;

            if (!selectedViews.Contains(btn))
                setOriginColor(btn);
        }

        private int getRow(TextBlock obj)
        {
            return Int32.Parse(obj.Name.Split('_')[1]);
        }

        private int getColumn(TextBlock obj)
        {
            return Int32.Parse(obj.Name.Split('_')[2]);
        }

        private TextBlock getChild(int row, int column)
        {
            return buttons[row, column];
        }

        private void setOriginColor(TextBlock button)
        {
            if (getRow(button) % 2 == 0)
                button.Background = backgroundEven;
            else
                button.Background = backgroundOdd;
        }

        private void setting(int row, int column, bool makeSelect)
        {
            if (0 <= column && column < COLUMN_NUMBER)
            {
                TextBlock button = getChild(row, column);

                if (makeSelect)
                {
                    button.Background = selectColor;
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
