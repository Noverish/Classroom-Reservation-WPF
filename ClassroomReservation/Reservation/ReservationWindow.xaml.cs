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
using System.Windows.Shapes;

namespace ClassroomReservation.Reservation
{
    public partial class ReservationWindow : Window
    {
        private bool changedByUser = true;

        public ReservationWindow()
        {
            InitializeComponent();

            Cancel_Button.Click += new RoutedEventHandler(Button_Click_Close);

            //int column = 0;
            //for(int row = 0;row<10;row++)
            //{
            //    Button button = (Button) mainGrid.Children
            //        .Cast<UIElement>()
            //        .First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == column);

            //    button.Click += new RoutedEventHandler(ChangeButtonColorToSelected);
            //}

            //calendar.SelectionMode = CalendarSelectionMode.SingleDate;

            foreach (Label btn in timeSelectControl.mainGrid.Children.OfType<Label>())
            {
                btn.IsEnabled = false;
            }

            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddDays(7), DateTime.MaxValue));
        }

        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var calendar = sender as Calendar;
            
            if (calendar.SelectedDate.HasValue && calendar.SelectedDates.Count > 0) //See if a date is selected.
            {
                foreach (Label btn in timeSelectControl.mainGrid.Children.OfType<Label>())
                {
                    btn.IsEnabled = true;
                }
                // ... Display SelectedDate in Title.
                //DateTime date = calendar.SelectedDate.Value;
                //this.Title = date.ToShortDateString();
            }
        }

        //MouseLeftButtonDown="onMouseLeftBtnDown" MouseEnter="onMouseEnter" MouseLeftButtonUp="OnMouseLeftBtnUp"
    }
}
