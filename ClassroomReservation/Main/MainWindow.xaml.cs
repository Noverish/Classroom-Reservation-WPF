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
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ClassroomReservation.Reservation;
using ClassroomReservation.Server;
using System.Collections;
using ClassroomReservation.Resource;

namespace ClassroomReservation.Main
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isUserMode = true;

        private int CLASSROOM_NUM;

		DispatcherTimer animationTimer = new DispatcherTimer();
		private double reservationStatusPerDayWidth;
		double delta = 0;
		int deltaDirection = 1;
		double startPos;
        static Hashtable ht;


        private SolidColorBrush backgroundEven = (SolidColorBrush)Application.Current.FindResource("BackgroundOfEvenRow");
        private SolidColorBrush backgroundOdd = (SolidColorBrush)Application.Current.FindResource("BackgroundOfOddRow");

        public MainWindow() {
            InitializeComponent();

            Hashtable classroomTable = Database.getInstance().classroomTable;
            CLASSROOM_NUM = classroomTable.Count;
            Border nowBuildingLabelBorder = null;
            for (int row = 0; row < CLASSROOM_NUM; row++) {

                //Add RowDefinition
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);
                leftLabelsGrid.RowDefinitions.Add(rowDef);

                //Get building name and classroom name
                string classroomName = (classroomTable[row] as ClassroomItem).classroom;
                string buildingName = (classroomTable[row] as ClassroomItem).building;

                //Add label to Grid
                Label classroomLabel = new Label();
                classroomLabel.Content = classroomName;
                classroomLabel.Background = (row % 2 == 0) ? backgroundEven : backgroundOdd;

                Grid.SetRow(classroomLabel, row);
                Grid.SetColumn(classroomLabel, 1);

                leftLabelsGrid.Children.Add(classroomLabel);

                //Adjust building label
                if (nowBuildingLabelBorder != null && (nowBuildingLabelBorder.Child as Label).Content.Equals(buildingName)) {
                    Grid.SetRowSpan(nowBuildingLabelBorder, Grid.GetRowSpan(nowBuildingLabelBorder) + 1);
                } else {
                    Label buildingLabel = new Label();
                    buildingLabel.Content = buildingName;
                    buildingLabel.Style = Resources["LabelStyle"] as Style;

                    Border border = new Border();
                    border.Style = Resources["BorderStyle"] as Style;
                    border.Child = buildingLabel;

                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, 0);
                    Grid.SetRowSpan(border, 1);

                    nowBuildingLabelBorder = border;
                    leftLabelsGrid.Children.Add(border);
                }
            }


            DateTime today = DateTime.Now;

            for (int i = 0; i < 7; i++) {
                if (today.AddDays(i).DayOfWeek != 0) {
                    ReservationStatusPerDay fileInputBox1 = new ReservationStatusPerDay(today.AddDays(i));
                    fileInputBox1.onOneSelected = onOneSelected;
                    scrollViewContentPanel.Children.Add(fileInputBox1);
                }
            }
            
            MainWindow_DatePicker.SelectedDate = today;
            Addid.Click += new RoutedEventHandler(ShowSignUp);

			ChangeModeButton.Click += new RoutedEventHandler(changeMode);
            ChangeModeButton.Click += new RoutedEventHandler(ShowLogin);
			readExcelFileButton.Click += new RoutedEventHandler(readExcelFileButton_Click);

            AdminButtonPanel.Visibility = System.Windows.Visibility.Hidden;
            
            animationTimer.Interval = new TimeSpan(120);
            animationTimer.Tick += new EventHandler(MyTimer_Tick);

            button4.Click += new RoutedEventHandler((sender, e) => {
                ReservationWindow window = new ReservationWindow();
                window.onReservationSuccess = OnReservationSuccess;
                window.ShowDialog();
            });

            ht = new Hashtable();
        }

        public void ShowSignUp(object sender, RoutedEventArgs e)
        {
            LoginForm signWin = new LoginForm(new RegisterOnClick());
            signWin.LoginButton.Content = "회원가입";
            signWin.ShowDialog();
        }

        public void ShowLogin(object sender, RoutedEventArgs e)
        {
            LoginForm loginWin = new LoginForm(new LoginOnClick());
            loginWin.ShowDialog();
        }

        public void changeMode(object sender, RoutedEventArgs e)
        {
            isUserMode = !isUserMode;

            if(isUserMode)
            {
                AdminButtonPanel.Visibility = System.Windows.Visibility.Hidden;
            } else
            {
                AdminButtonPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public static DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.RegisterAttached("HorizontalOffset",
                                                typeof(double),
                                                typeof(MainWindow),
                                                new UIPropertyMetadata(0.0, OnHorizontalOffsetChanged));

        private static void OnHorizontalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = target as ScrollViewer;

            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
            }
        }

		void MyTimer_Tick(object sender, EventArgs e)
		{
			if(Math.Abs(delta) > reservationStatusPerDayWidth)
			{
				animationTimer.Stop();
			}
			else
			{
				delta += deltaDirection;
				ScrollViewer.ScrollToHorizontalOffset(startPos + delta);
			}
		}

		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
			ReservationStatusPerDay child = scrollViewContentPanel.Children.OfType<ReservationStatusPerDay>().FirstOrDefault();
			reservationStatusPerDayWidth = child.ActualWidth;

			animationTimer.Start();
			deltaDirection = (e.Delta < 0) ? 2 : -2;
			delta = 0;
			startPos = ScrollViewer.HorizontalOffset;

			e.Handled = true;
        }

        private void readExcelFileButton_Click(object sender, RoutedEventArgs e)
        {
            List<ReservationItem> items = ExcelReadClient.readExcel();
        }

        private void onOneSelected(ReservationItem item) {
            if (item != null) {
                infoPanel.Visibility = Visibility.Visible;

                txtbox1.Text = item.userName;
                txtbox2.Text = item.contact;
                txtbox3.Text = item.content;
            } else {
                infoPanel.Visibility = Visibility.Hidden;
            }
        }

        private void OnReservationSuccess(ReservationItem item) {
            int childrenNum = scrollViewContentPanel.Children.Count;

            for (int i = 0; i < childrenNum; i++) {
                var child = scrollViewContentPanel.Children[i];
                ReservationStatusPerDay day = child as ReservationStatusPerDay;
                day.refresh();
            }
        }

        static System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binformatter;
        private class LoginOnClick : LoginFormOnClick
        {
            void LoginFormOnClick.OnClick(LoginForm form, string Id, string password)
            {
                Hashtable vectorDeserialized = null;

                using (var fs = File.Open("c:\\temp\\vector.bin", FileMode.Open))
                {
                    vectorDeserialized = (Hashtable)binformatter.Deserialize(fs);
                }

                foreach (DictionaryEntry entry in vectorDeserialized)
                {
                    if(Id.Equals(entry.Key))
                    {
                        if(Id.Equals(LoginForm.DecryptString((string)entry.Value, password)))
                        {
                            form.Close();
                        }
                    }
                }
            }
        }

        private class RegisterOnClick : LoginFormOnClick
        {
            void LoginFormOnClick.OnClick(LoginForm form, string Id, string password)
            {
                ht.Add(Id, LoginForm.EncryptString(Id, password));

                binformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                using (var fs = File.Create("c:\\temp\\vector.bin"))
                {
                    binformatter.Serialize(fs, ht);
                }

                form.Close();
            }
        }
    }
}
