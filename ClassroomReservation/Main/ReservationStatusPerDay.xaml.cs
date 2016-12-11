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
using ClassroomReservation.Resource;

namespace ClassroomReservation.Main
{

    public delegate void OnOneSelected(StatusItem item);

    /// <summary>
    /// ReservationStatusPerDay.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReservationStatusPerDay : UserControl
    {
        private static int TOTAL_COLUMN = Database.getInstance().classTimeTable.Count;
        private static int TOTAL_ROW = Database.getInstance().classroomTable.Count + 2;

        public static ReservationStatusPerDay nowSelectedStatusControl { get; private set; }
        public static int[] nowSelectedColumn { get; private set; } = new int[2] { -1, -1 };
        private static int nowSelectedRow = -1;
        public static int NowSelectedRow { get { return nowSelectedRow - 2; } private set { nowSelectedRow = value; } }
        private bool mouseLeftButtonDown = false;

        public OnOneSelected onOneSelected { private get; set; }
        private CustomTextBlock[,] buttons = new CustomTextBlock[TOTAL_ROW, TOTAL_COLUMN];
        public DateTime date { get; private set; }

        private Brush defaultColorOfOdd = (SolidColorBrush)Application.Current.FindResource("BackgroundOfOddRow");
        private Brush defaultColorOfEven = (SolidColorBrush)Application.Current.FindResource("BackgroundOfEvenRow");
        private Brush selectColor = Brushes.Crimson;
        private SolidColorBrush hoverColor = (SolidColorBrush)Application.Current.FindResource("MicrosoftRed");
        
        private SolidColorBrush purple = new SolidColorBrush(Color.FromRgb(255, 0, 255));

        public ReservationStatusPerDay(DateTime date)
        {
            InitializeComponent();

            this.date = date;

            CultureInfo cultures = CultureInfo.CreateSpecificCulture("ko-KR");
            DateTextBlock.Content = date.ToString(string.Format("yyyy년 MM월 dd일 ddd요일", cultures));

            for (int row = 2; row < TOTAL_ROW; row++)
            {
                //Add RowDefinition
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);
                wrapPanel.RowDefinitions.Add(rowDef);

                for (int col = 0; col < TOTAL_COLUMN; col++)
                {
                    CustomTextBlock newBtn = new CustomTextBlock();
                    newBtn.row = row;
                    newBtn.column = col;

                    if (row % 2 == 0) {
                        newBtn.Background = defaultColorOfEven;
                        newBtn.originColor = defaultColorOfEven;
                    } else {
                        newBtn.Background = defaultColorOfOdd;
                        newBtn.originColor = defaultColorOfOdd;
                    }
                    newBtn.VerticalAlignment = VerticalAlignment.Stretch;
                    newBtn.HorizontalAlignment = HorizontalAlignment.Stretch;

                    newBtn.MouseDown += new MouseButtonEventHandler(onMouseDown);
                    newBtn.MouseUp += new MouseButtonEventHandler(onMouseUp);
                    newBtn.MouseEnter += new MouseEventHandler(onMouseEnter);
                    newBtn.MouseLeave += new MouseEventHandler(onMouseLeave);
                    newBtn.MouseMove += new MouseEventHandler(onMouseMove);

                    Border myBorder1 = new Border();
                    myBorder1.BorderBrush = Brushes.Gray;
                    if (col == 0)
                        myBorder1.BorderThickness = new Thickness { Top = 0, Bottom = 0, Left = 0, Right = 1 };
                    else if (col == 9)
                        myBorder1.BorderThickness = new Thickness { Top = 0, Bottom = 0, Left = 0, Right = 0 };
                    else
                        myBorder1.BorderThickness = new Thickness { Top = 0, Bottom = 0, Left = 0, Right = 1 };

                    myBorder1.Child = newBtn;
                    buttons[row, col] = newBtn;

                    Grid.SetRow(myBorder1, row);
                    Grid.SetColumn(myBorder1, col);

                    wrapPanel.Children.Add(myBorder1);
                }
            }

            refresh();
        }

        public void refresh() {
            try {
                nowSelectedStatusControl = null;
                nowSelectedRow = -1;
                nowSelectedColumn[0] = nowSelectedColumn[1] = -1;

                List<StatusItem> items = Server.ServerClient.GetDayReservation(date);

                foreach(CustomTextBlock btn in buttons) {
                    if (btn != null) {
                        btn.originColor = (btn.row % 2 == 0) ? defaultColorOfOdd : defaultColorOfEven;
                        btn.Background = btn.originColor;
                        btn.item = null;
                    }
                }

                for (int i = 0; i < items.Count; i++) {
                    //Console.WriteLine(items[i].classroom);

                    int row = Database.getInstance().GetRowByClassroom(items[i].classroom) + 2;
                    int column = items[i].classtime - 1;

                    CustomTextBlock btn = buttons[row, column];
                    btn.originColor = purple;
                    btn.item = items[i];
                }

                ResetBackground();
            } catch (Exception ex) {
                //Console.WriteLine(ex.StackTrace);
            }
        }

        private void onMouseDown(object sender, RoutedEventArgs e)
        {
            nowSelectedStatusControl?.ResetBackground();

            CustomTextBlock button = sender as CustomTextBlock;
            Border border = button.Parent as Border;
            int row = Grid.GetRow(border);
            int column = Grid.GetColumn(border);

            nowSelectedStatusControl = this;
            nowSelectedColumn[0] = nowSelectedColumn[1] = column;
            nowSelectedRow = row;

            Mouse.Capture(button);
            mouseLeftButtonDown = true;

            SetSelection(column, column, column, row);
            
            onOneSelected?.Invoke(button.item);
        }

        private void onMouseUp(object sender, RoutedEventArgs e)
        {
            mouseLeftButtonDown = false;
            Mouse.Capture(null);
        }

        private void onMouseEnter(object sender, RoutedEventArgs e)
        {
            TextBlock button = sender as TextBlock;
            button.Background = hoverColor;
        }

        private void onMouseLeave(object sender, RoutedEventArgs ee)
        {
            ResetBackground();
            if (Equals(nowSelectedStatusControl)) {
                if (nowSelectedColumn[0] >= 0 && nowSelectedColumn[1] < TOTAL_COLUMN) {
                    for (int column = nowSelectedColumn[0]; column <= nowSelectedColumn[1]; column++) {
                        (((wrapPanel.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == nowSelectedRow && Grid.GetColumn(e) == column)) as Border).Child as TextBlock).Background = selectColor;
                    }
                }
            }
        }

        private void onMouseMove(object sender, RoutedEventArgs e)
        {
            if (mouseLeftButtonDown) {
                TextBlock button = sender as TextBlock;
                Border border = button.Parent as Border;
                int row = Grid.GetRow(border);
                int column = Grid.GetColumn(border);
                double width = button.ActualWidth;
                double x = Mouse.GetPosition(button).X;

                if (-2 * width < x && x < -width)
                    SetSelection(column - 2, column, column, row);
                else if (-width < x && x < 0)
                    SetSelection(column - 1, column, column, row);
                else if (0 < x && x < width)
                    SetSelection(column, column, column, row);
                else if (width < x && x < 2 * width)
                    SetSelection(column, column + 1, column, row);
                else if (2 * width < x && x < 3 * width)
                    SetSelection(column, column + 2, column, row);
            }
        }

        private void SetSelection(int startColumn, int endColumn, int pivotColumn, int row) {
            if (startColumn < 0)
                startColumn = 0;

            if (endColumn > TOTAL_COLUMN - 1)
                endColumn = TOTAL_COLUMN - 1;

            for (int column = pivotColumn - 2; column <= pivotColumn + 2; column++) {
                if (column < 0 || column > TOTAL_COLUMN - 1)
                    continue;

                CustomTextBlock btn = buttons[row, column];

                if (startColumn <= column && column <= endColumn) {
                    btn.Background = selectColor;
                } else {
                    btn.Background = btn.originColor;
                }
            }

            nowSelectedColumn[0] = startColumn;
            nowSelectedColumn[1] = endColumn;
        }

        private void ResetBackground() {
            foreach (CustomTextBlock btn in buttons) {
                if (btn != null)
                    btn.Background = btn.originColor;
            }
        }

        public class CustomTextBlock : TextBlock {
            public int row { get; set; }
            public int column { get; set; }
            public Brush originColor { get; set; }
            public StatusItem item { get; set; }
        }
    }
}
