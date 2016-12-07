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
using ClassroomReservation.Server;

namespace ClassroomReservation.Main
{
    /// <summary>
    /// ReservationStatusPerDay.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReservationStatusPerDay : UserControl
    {
        private static List<TextBlock> selectedViews = new List<TextBlock>();

        private DateTime date;

        private bool mouseLeftButtonDown = false;
        private int selectedButtonNum = 0;
        private int nowSelectedRow = -1;

        private Brush defaultColorOfOdd = (SolidColorBrush)Application.Current.FindResource("BackgroundOfOddRow");
        private Brush defaultColorOfEven = (SolidColorBrush)Application.Current.FindResource("BackgroundOfEvenRow");
        private Brush selectColor = Brushes.Crimson;

        private SolidColorBrush red = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private SolidColorBrush green = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private SolidColorBrush blue = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private SolidColorBrush orange = new SolidColorBrush(Color.FromRgb(255, 255, 0));
        private SolidColorBrush purple = new SolidColorBrush(Color.FromRgb(255, 0, 255));

        public ReservationStatusPerDay(DateTime date)
        {
            InitializeComponent();

            this.date = date;

            CultureInfo cultures = CultureInfo.CreateSpecificCulture("ko-KR");
            DateTextBlock.Content = date.ToString(string.Format("yyyy년 MM월 dd일 ddd요일", cultures));

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    TextBlock newBtn = new TextBlock();

                    //newBtn.Content = i + ", " + j;
                    //newBtn.Name = "Button" + i.ToString();

                    if (i % 2 == 0)
                        newBtn.Background = defaultColorOfEven;
                    else
                        newBtn.Background = defaultColorOfOdd;
                    newBtn.VerticalAlignment = VerticalAlignment.Stretch;
                    newBtn.HorizontalAlignment = HorizontalAlignment.Stretch;
                    newBtn.Name = "_" + i + "_" + j;

                    newBtn.MouseDown += new MouseButtonEventHandler(onMouseDown);
                    newBtn.MouseUp += new MouseButtonEventHandler(onMouseUp);
                    newBtn.MouseEnter += new MouseEventHandler(onMouseEnter);
                    newBtn.MouseLeave += new MouseEventHandler(onMouseLeave);
                    newBtn.MouseMove += new MouseEventHandler(onMouseMove);

                    Border myBorder1 = new Border();
                    myBorder1.BorderBrush = Brushes.Gray;
                    if (j == 0)
                        myBorder1.BorderThickness = new Thickness { Top = 0, Bottom = 0, Left = 0, Right = 1 };
                    else if (j == 9)
                        myBorder1.BorderThickness = new Thickness { Top = 0, Bottom = 0, Left = 0, Right = 0 };
                    else
                        myBorder1.BorderThickness = new Thickness { Top = 0, Bottom = 0, Left = 0, Right = 1 };

                    myBorder1.Child = newBtn;

                    Grid.SetRow(myBorder1, i + 2);
                    Grid.SetColumn(myBorder1, j);

                    wrapPanel.Children.Add(myBorder1);
                }
            }

            refresh();
        }

        public void refresh() {
            ResetBackground();

            try {
                List<ReservationItem> items = Server.ServerClient.GetDayReservation(date);
                for (int i = 0; i < items.Count; i++) {
                    int row = classroomToRow(items[i].classroom) + 2;

                    for (int column = items[i].startClass; column <= items[i].endClass; column++) {
                        TextBlock btn = (wrapPanel.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == (column - 1)) as Border).Child as TextBlock;
                        btn.Background = purple;
                    }
                }
            } catch (Exception ex) {

            }
        }

        private void onMouseDown(object sender, RoutedEventArgs e)
        {
            foreach(TextBlock selectedView in selectedViews)
            {
                if(getRow(selectedView) % 2 == 0)
                    selectedView.Background = defaultColorOfOdd;
                else
                    selectedView.Background = defaultColorOfEven;
            }

            (sender as TextBlock).Background = selectColor;
            nowSelectedRow = getRow(sender as TextBlock);
            selectedButtonNum = 0;
            selectedViews.Add(sender as TextBlock);

            mouseLeftButtonDown = true;
        }

        private void onMouseUp(object sender, RoutedEventArgs e)
        {
            mouseLeftButtonDown = false;
            //Mouse.Capture(null);
        }

        private void onMouseEnter(object sender, RoutedEventArgs e)
        {
            if (mouseLeftButtonDown && selectedButtonNum < 2 && getRow(sender as TextBlock) == nowSelectedRow)
            {
                (sender as TextBlock).Background = selectColor;
                selectedViews.Add(sender as TextBlock);
                selectedButtonNum++;
            }
        }

        private void onMouseLeave(object sender, RoutedEventArgs e)
        {
            //Mouse.Capture(sender as TextBlock);
        }

        private void onMouseMove(object sender, RoutedEventArgs e)
        {
            //if(mouseLeftButtonDown)
                //Console.WriteLine(Mouse.GetPosition(sender as TextBlock));
        }

        private int getRow (TextBlock obj)
        {
            return Int32.Parse(obj.Name.Split('_')[1]);
        }

        private int classroomToRow(string name) {
            if (name.Equals("107호"))
                return 0;
            if (name.Equals("B102호"))
                return 1;
            if (name.Equals("B103호"))
                return 2;
            if (name.Equals("B104호"))
                return 3;
            if (name.Equals("201호"))
                return 4;
            if (name.Equals("202호"))
                return 5;
            if (name.Equals("205호"))
                return 6;
            if (name.Equals("206호"))
                return 7;
            if (name.Equals("208호"))
                return 8;
            if (name.Equals("611호"))
                return 9;
            if (name.Equals("614A호"))
                return 10;
            if (name.Equals("615호"))
                return 11;
            return -1;
        }

        private void ResetBackground() {
            for (int row = 2; row < 14; row++) {
                for (int column = 0; column < 10; column++) {
                    TextBlock btn = (wrapPanel.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == (column)) as Border).Child as TextBlock;

                    if (row % 2 == 0)
                        btn.Background = defaultColorOfOdd;
                    else
                        btn.Background = defaultColorOfEven;
                }
            }
        }
    }
}
