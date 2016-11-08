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
    /// ReservationStatusPerDay.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReservationStatusPerDay : UserControl
    {
        private bool mouseLeftButtonDown = false;
        private int selectedButtonNum = 0;
        private int nowSelectedRow = -1;

        private Brush defaultColor = Brushes.Azure;
        private Brush selectColor = Brushes.Crimson;

        delegate void asdf(object s, RoutedEventArgs e);

        private SolidColorBrush red = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private SolidColorBrush green = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private SolidColorBrush blue = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private SolidColorBrush orange = new SolidColorBrush(Color.FromRgb(255, 255, 0));
        private SolidColorBrush purple = new SolidColorBrush(Color.FromRgb(255, 0, 255));

        List<TextBlock> buttons = new List<TextBlock>();

        public ReservationStatusPerDay()
        {
            InitializeComponent();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    TextBlock newBtn = new TextBlock();

                    //newBtn.Content = j + ", " + i;
                    //newBtn.Name = "Button" + i.ToString();

                    newBtn.Background = defaultColor;

                    newBtn.MouseDown += new MouseButtonEventHandler(onMouseDown);
                    newBtn.MouseUp += new MouseButtonEventHandler(onMouseUp);

                    newBtn.MouseEnter += new MouseEventHandler(onMouseEnter);
                    newBtn.MouseLeave += new MouseEventHandler(onMouseLeave);

                    buttons.Add(newBtn);

                    Border myBorder1 = new Border();
                    myBorder1.BorderBrush = Brushes.Black;
                    myBorder1.BorderThickness = new Thickness(1);
                    myBorder1.Child = newBtn;

                    Grid.SetRow(myBorder1, j + 2);
                    Grid.SetColumn(myBorder1, i);

                    wrapPanel.Children.Add(myBorder1);
                }
            }
        }

        private void onMouseDown(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock rec in buttons)
            {
                rec.Background = defaultColor;
            }
            (sender as TextBlock).Background = selectColor;
            nowSelectedRow = Grid.GetRow(sender as TextBlock);
            selectedButtonNum = 0;

            mouseLeftButtonDown = true;

            Console.WriteLine(Grid.GetRow(sender as UIElement));
        }

        private void onMouseUp(object sender, RoutedEventArgs e)
        {
            mouseLeftButtonDown = false;
        }

        private void onMouseEnter(object sender, RoutedEventArgs e)
        {
            if (mouseLeftButtonDown && selectedButtonNum < 2 && (Grid.GetRow(sender as TextBlock) == nowSelectedRow))
            {
                (sender as TextBlock).Background = selectColor;
                selectedButtonNum++;
            }
        }

        private void onMouseLeave(object sender, RoutedEventArgs e)
        {

        }
    }
}
