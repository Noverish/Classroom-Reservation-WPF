using ClassroomReservation.Main;
using ClassroomReservation.Resource;
using ClassroomReservation.Server;
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
    public delegate void OnReservationSuccess(ReservationItem item);

    public partial class ReservationWindow : Window
    {
        public OnReservationSuccess onReservationSuccess { set; private get; }

        public ReservationWindow()
        {
            InitializeComponent();

            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
            calendar.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddDays(7), DateTime.MaxValue));

            calendar.PreviewMouseUp += new MouseButtonEventHandler((sender, e) => Mouse.Capture(null));

            OK_Button.Click += new RoutedEventHandler(Reserve);
            Cancel_Button.Click += new RoutedEventHandler((sender, e) => this.Close());

            timeSelectControl.enable(false);
            timeSelectControl.onTimeSelectChanged = OnTimeSelectChanged;

            classroomSelectControl.enable(false);
            classroomSelectControl.onClassroomSelectChanged = OnClassroomSelectChanged;

            EnableInputUserData(false);

            try {
                DateTime selectedDate = ReservationStatusPerDay.nowSelectedStatusControl.date;
                int[] selectedClasstimeRow = ReservationStatusPerDay.nowSelectedColumn;
                int selectedClassroomRow = ReservationStatusPerDay.nowSelectedRow - 2;

                calendar.SelectedDate = selectedDate;
                timeSelectControl.SetSelectedTime(selectedClasstimeRow);
                classroomSelectControl.SetSelectedClassroom(selectedClassroomRow);
                
                EnableInputUserData(true);
            } catch (Exception ex) {

            }
        }

        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            timeSelectControl.enable(true);
            timeSelectControl.ResetSelection();

            classroomSelectControl.enable(true);
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

            ReservationItem item = new ReservationItem(startDate, endDate, time[0], time[1], classroom, name, contact, content, password);

            try {
                ServerClient.getInstance().reservationAdd(item);

                onReservationSuccess?.Invoke(item);
                Close();
            } catch (ServerResult ex) {

            }
        }

        private void EnableInputUserData(bool enable) {
            if (enable) {
                overlapRectangle.Visibility = Visibility.Hidden;
            } else {
                overlapRectangle.Visibility = Visibility.Visible;
            }
        }

        private void OnTimeSelectChanged(int[] nowSelectedTime, bool isDataChanged) {
            if (isDataChanged) {
                classroomSelectControl.ResetSelection();
                classroomSelectControl.selectiveEnable(ServerClient.getInstance().checkClassroomStatusByClasstime(
                    calendar.SelectedDates[0], 
                    calendar.SelectedDates[calendar.SelectedDates.Count - 1], 
                    nowSelectedTime[0], 
                    nowSelectedTime[1]
                ));
                EnableInputUserData(false);
            } else {
                if (classroomSelectControl.HasSelectedClassroom() && timeSelectControl.HasSeletedTime())
                    EnableInputUserData(true);
            }
        }

        private void OnClassroomSelectChanged(string nowSelectedClassroom, bool isDataChanged) {
            if (isDataChanged) {
                timeSelectControl.ResetSelection();
                timeSelectControl.selectiveEnable(ServerClient.getInstance().checkClasstimeStatusByClassrom(
                    calendar.SelectedDates[0],
                    calendar.SelectedDates[calendar.SelectedDates.Count - 1],
                    nowSelectedClassroom
                ));
                EnableInputUserData(false);
            } else {
                if (classroomSelectControl.HasSelectedClassroom() && timeSelectControl.HasSeletedTime())
                    EnableInputUserData(true);
            }
        }

        //MouseLeftButtonDown="onMouseLeftBtnDown" MouseEnter="onMouseEnter" MouseLeftButtonUp="OnMouseLeftBtnUp"
    }
}
