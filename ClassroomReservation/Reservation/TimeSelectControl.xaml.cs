using ClassroomReservation.Resource;
using System;
using System.Collections;
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
    public delegate void OnTimeSelectChanged(int[] nowSelectedTime, bool isDataChanged);

    /// <summary>
    /// TimeSelectControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TimeSelectControl : UserControl
    {
        private const int CLASS_NUM = 10;

        public OnTimeSelectChanged onTimeSelectChanged { set; private get; }

        private IEnumerable<Label> buttons;
        private int[] beforeSelectedTime = new int[2];
        private int[] nowSelectedTime = new int[2];
        private bool mouseLeftButtonDown = false;

        private SolidColorBrush selectedColor = (SolidColorBrush)Application.Current.FindResource("SelectedColor");
        private SolidColorBrush hoverColor = (SolidColorBrush)Application.Current.FindResource("HoverColor");
        private SolidColorBrush backgroundEven = (SolidColorBrush)Application.Current.FindResource("BackgroundOfEvenRow");
        private SolidColorBrush backgroundOdd = (SolidColorBrush)Application.Current.FindResource("BackgroundOfOddRow");
        
        public TimeSelectControl()
        {
            InitializeComponent();

            Hashtable classTimeTable = Database.getInstance().classTimeTable;
            for (int time = 1; time <= classTimeTable.Count; time++) {
                Label label = new Label();
                label.Content = classTimeTable[time];

                Grid.SetRow(label, time - 1);
                Grid.SetColumn(label, 0);
                
                mainGrid.Children.Add(label);
            }

            beforeSelectedTime[0] = beforeSelectedTime[1] = -2;
            nowSelectedTime[0] = nowSelectedTime[1] = -2;
            buttons = mainGrid.Children.OfType<Label>();

            ResetBackground();

            foreach (Label btn in buttons)
            {
                btn.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
                btn.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
                btn.MouseEnter += new MouseEventHandler(OnMouseEnter);
                btn.MouseLeave += new MouseEventHandler(OnMouseLeave);
                btn.MouseMove += new MouseEventHandler(OnMouseMove);
            }
        }


        public void enable(bool enable) {
            if (enable)
                overlapRectangle.Visibility = Visibility.Hidden;
            else
                overlapRectangle.Visibility = Visibility.Visible;
        }

        public void ResetSelection() {
            beforeSelectedTime[0] = beforeSelectedTime[1] = -2;
            nowSelectedTime[0] = nowSelectedTime[1] = -2;
            ResetBackground();
        }

        public bool HasSeletedTime() {
            return (nowSelectedTime[0] >= 0 && nowSelectedTime[1] >= 0);
        }

        public int[] GetSelectedTime() {
            int[] returnArray = (int[])nowSelectedTime.Clone();
            returnArray[0]++;
            returnArray[1]++;

            return returnArray;
        }

        public void SetSelectedTime(int[] selectedTimeRow) {
            enable(true);
            nowSelectedTime = (int[])selectedTimeRow.Clone();
            SetSelectByRow(nowSelectedTime[0], nowSelectedTime[1]);
        }

        private void OnMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            ResetBackground();

            Label button = sender as Label;
            int index = Grid.GetRow(button);

            button.Background = selectedColor;
            mouseLeftButtonDown = true;

            Mouse.Capture(button);
        }

        private void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            mouseLeftButtonDown = false;

            Mouse.Capture(null);

            onTimeSelectChanged?.Invoke(GetSelectedTime(), (beforeSelectedTime[0] >= 0 && beforeSelectedTime[1] >= 0) && !beforeSelectedTime.SequenceEqual(nowSelectedTime));

            beforeSelectedTime = (int[])nowSelectedTime.Clone();
        }

        private void OnMouseEnter(object sender, RoutedEventArgs e) //when mouse on entered area
        {

            Label button = sender as Label;
            int index = Grid.GetRow(button);

            if (!mouseLeftButtonDown) {
                button.Background = hoverColor;
            }
        }

        private void OnMouseLeave(object sender, RoutedEventArgs e)
        {
            Label button = sender as Label;
            int index = Grid.GetRow(button);

            if (nowSelectedTime[0] <= index && index <= nowSelectedTime[1])
                button.Background = selectedColor;
            else {
                if (index % 2 == 0)
                    button.Background = backgroundOdd;
                else
                    button.Background = backgroundEven;
            }
        }

        private void OnMouseMove(object sender, RoutedEventArgs e) {
            if (mouseLeftButtonDown) {
                Label button = sender as Label;
                int row = Grid.GetRow(button);
                double height = button.ActualHeight;
                double y = Mouse.GetPosition(button).Y;

                if (-2 * height < y && y < -height)
                    SetSelectByRow(row - 2, row);
                else if (-height < y && y < 0)
                    SetSelectByRow(row - 1, row);
                else if (0 < y && y < height)
                    SetSelectByRow(row, row);
                else if (height < y && y < 2 * height)
                    SetSelectByRow(row, row + 1);
                else if (2 * height < y && y < 3 * height)
                    SetSelectByRow(row, row + 2);
            }
        }

        private void SetSelectByRow(int fromRow, int toRow) {
            if (fromRow < 0)
                fromRow = 0;

            if (toRow > CLASS_NUM - 1)
                toRow = CLASS_NUM - 1;

            nowSelectedTime[0] = fromRow;
            nowSelectedTime[1] = toRow;

            ResetBackground();

            for (int i = fromRow; i <= toRow; i++) {
                Label item = mainGrid.Children.Cast<Label>().First(e => Grid.GetRow(e) == i && Grid.GetColumn(e) == 0);
                item.Background = selectedColor;
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
