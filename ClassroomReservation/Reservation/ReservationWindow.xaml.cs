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
        public ReservationWindow()
        {
            InitializeComponent();

            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddDays(7), DateTime.MaxValue));

            calendar.PreviewMouseUp += new MouseButtonEventHandler((sender, e) => Mouse.Capture(null));

            OK_Button.Click += new RoutedEventHandler(Reserve);
            Cancel_Button.Click += new RoutedEventHandler((sender, e) => this.Close());

            classroomSelectControl.SetOnClassroomSelectChanged(new OnClassroomSelectChanged(OnClassroomSelectChanged));
            timeSelectControl.SetOnTimeSelectChanged(new OnTimeSelectChanged(OnTimeSelectChanged));
        }

        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            timeSelectControl.ResetSelection();
            classroomSelectControl.ResetSelection();
            EnableInputUserData(false);
        }

        private void Reserve(object sender, RoutedEventArgs e) {
            DateTime startDate = calendar.SelectedDates[0];
            DateTime endDate = calendar.SelectedDates[calendar.SelectedDates.Count - 1];
            int[] time = timeSelectControl.GetSelectedTime();
            string classroom = classroomSelectControl.GetSelectedClassroom();

            string name = nameTextBox.Text;
            string contact = numberTextBox.Text;
            string content = contentTextBox.Text;
            string password = passwordTextBox.Text;

            Server.ReservationItem item = new Server.ReservationItem(startDate, endDate, time[0], time[1], classroom, name, contact, content, password);

            Server.ServerClient.MakeReservation(item);
        }

        private void EnableInputUserData(bool enable) {
            if (enable) {
                overlapRectangle.Visibility = Visibility.Hidden;
            } else {
                overlapRectangle.Visibility = Visibility.Visible;
            }
        }

        private void OnTimeSelectChanged(int[] nowSelectedTime) {
            classroomSelectControl.ResetSelection();
        }

        private void OnClassroomSelectChanged(string nowSelectedClassroom) {
            timeSelectControl.ResetSelection();
        }
    }
}
