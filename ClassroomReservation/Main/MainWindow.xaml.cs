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
using ClassroomReservation.Client;

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

        private StatusItem nowSelectedItem;

        private SolidColorBrush backgroundEven = (SolidColorBrush)Application.Current.FindResource("BackgroundOfEvenRow");
        private SolidColorBrush backgroundOdd = (SolidColorBrush)Application.Current.FindResource("BackgroundOfOddRow");
        private SolidColorBrush labelBorderBrush = (SolidColorBrush)Application.Current.FindResource("MainColor");


        public MainWindow() {
            InitializeComponent();

            labelBorderBrush = new SolidColorBrush(labelBorderBrush.Color);
            labelBorderBrush.Opacity = 0.7;

            Hashtable classroomTable = Database.getInstance().classroomTable;
            CLASSROOM_NUM = classroomTable.Count;
            Label nowBuildingLabel = null;
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
                if (nowBuildingLabel != null && nowBuildingLabel.Content.Equals(buildingName)) {
                    Grid.SetRowSpan(nowBuildingLabel, Grid.GetRowSpan(nowBuildingLabel) + 1);
                } else {
                    Label buildingLabel = new Label();
                    buildingLabel.Content = buildingName;
                    buildingLabel.Style = Resources["LabelStyle"] as Style;
                    buildingLabel.Margin = new Thickness { Top = 1, Bottom = 0, Left = 0, Right = 0 };

                    Grid.SetRow(buildingLabel, row);
                    Grid.SetColumn(buildingLabel, 0);
                    Grid.SetRowSpan(buildingLabel, 1);

                    nowBuildingLabel = buildingLabel;
                    leftLabelsGrid.Children.Add(buildingLabel);
                }
            }

            changeMode(true);

            DateTime today = DateTime.Now;

            for (int i = 0; i < 7; i++) {
                if (today.AddDays(i).DayOfWeek != 0) {
                    ReservationStatusPerDay fileInputBox1 = new ReservationStatusPerDay(today.AddDays(i));
                    fileInputBox1.onOneSelected = onOneSelected;
                    scrollViewContentPanel.Children.Add(fileInputBox1);
                }
            }
            
            MainWindow_DatePicker.SelectedDate = today;
            Addid.Click += new RoutedEventHandler(PasswordChange);
            
            ChangeModeButton.Click += new RoutedEventHandler((s,e) => {
                if (isUserMode)
                    Login();
                else
                    changeMode();
            });
			readExcelFileButton.Click += new RoutedEventHandler(readExcelFileButton_Click);

            int year = DateTime.Today.Year;
            string halfYear = (DateTime.Today.Month <= 6) ? "상반기" : "하반기";
            int start = (DateTime.Today.Month <= 6) ? 1 : 7;
            int end = (DateTime.Today.Month <= 6) ? 6 : 12;
            halfYearDeleteButton.Content = String.Format("{0}년 {1}({2}월 ~ {3}월) DB 삭제",year, halfYear, start, end);
            halfYearDeleteButton.Click += OnHalfYearDeleteButtonClicked;

            animationTimer.Interval = new TimeSpan(120);
            animationTimer.Tick += new EventHandler(MyTimer_Tick);

            button4.Click += new RoutedEventHandler((sender, e) => {
                ReservationWindow window = new ReservationWindow();
                window.onReservationSuccess = OnReservationSuccess;
                window.ShowDialog();
            });

            button6.Click += new RoutedEventHandler((sender, e) => {
                (new PasswordForm((form, password) => {
                    if (ServerClient.reservationDeleteOne(nowSelectedItem.reservID, password)) {
                        OnReservationSuccess(null);
                        form.Close();
                    } else {

                    }
                })).ShowDialog();
            });
        }

        public void refresh() {
            try {
                int childrenNum = scrollViewContentPanel.Children.Count;

                for (int i = 0; i < childrenNum; i++) {
                    var child = scrollViewContentPanel.Children[i];
                    ReservationStatusPerDay day = child as ReservationStatusPerDay;
                    day.refresh();
                }
            } catch (Exception ex) {
                MessageBox.Show("알 수 없는 오류로 새로고침에 실패 했습니다.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PasswordChange(object sender, RoutedEventArgs e)
        {
            PasswordForm signWin = new PasswordForm((window, password) => {
                LoginClient.getInstance().onChangeSuccess = (() => {
                    MessageBox.Show("성공적으로 변경 했습니다. 다시 로그인 해주세요.", "비밀번호 변경 성공", MessageBoxButton.OK, MessageBoxImage.Information);
                    changeMode(true);
                    window.Close();
                });
                LoginClient.getInstance().onChangeFailed = ((msg) => {
                    MessageBox.Show("알 수 없는 오류가 발생해서 변경에 실패 했습니다. - " + msg, "비밀번호 변경 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                LoginClient.getInstance().ChangeAccount(password);
            });
            signWin.LoginButton.Content = "비밀번호 변경";
            signWin.ShowDialog();
        }

        private void Login()
        {
            PasswordForm loginWin = new PasswordForm((window, password) => {
                LoginClient.getInstance().onLoginSuccess = (() => {
                    changeMode(false);
                    window.Close();
                });
                LoginClient.getInstance().onPasswordWrong = (() => {
                    MessageBox.Show("비밀번호가 다릅니다.", "로그인 실패", MessageBoxButton.OK, MessageBoxImage.Warning);
                });
                LoginClient.getInstance().onLoginError = ((msg) => {
                    MessageBox.Show("알 수 없는 오류가 발생 했습니다. 최초 비밀번호로 로그인 해주세요. - " + msg, "로그인 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                LoginClient.getInstance().Login(password);
            });
            loginWin.LoginButton.Content = "로그인";
            loginWin.ShowDialog();
        }

        private void changeMode() {
            changeMode(!isUserMode);
        }

        private void changeMode(bool isUserMode)
        {
            this.isUserMode = isUserMode;

            if(isUserMode)
            {
                ChangeModeButton.Content = "관리자 모드로 변경";
                Addid.Visibility = Visibility.Hidden;
                AdminButtonPanel.Visibility = Visibility.Hidden;
                leftbottomLogoImage.Visibility = Visibility.Visible;
            } else {
                ChangeModeButton.Content = "일반 사용자 모드로 변경";
                Addid.Visibility = Visibility.Visible;
                AdminButtonPanel.Visibility = Visibility.Visible;
                leftbottomLogoImage.Visibility = Visibility.Hidden;
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
            //List<ReservationItem> items = ExcelReadClient.readExcel();
            (new LoadLectureTableWindow()).ShowDialog();
        }

        private void onOneSelected(StatusItem item) {
            if (item != null) {
                nowSelectedItem = item;

                txtbox1.Text = item.userName;
                txtbox2.Text = item.contact;
                txtbox3.Text = item.content;
            } else {
                txtbox1.Text = "";
                txtbox2.Text = "";
                txtbox3.Text = "";
            }
        }

        private void OnReservationSuccess(ReservationItem item) {
            refresh();
        }

        private void OnHalfYearDeleteButtonClicked(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("정말로 삭제 하시겠습니까?", "", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK) {
                try {
                    int year = DateTime.Today.Year;
                    DateTime startDate = (DateTime.Today.Month <= 6) ? new DateTime(year, 1, 1) : new DateTime(year, 7, 1);
                    DateTime endDate = (DateTime.Today.Month <= 6) ? new DateTime(year, 6, 30) : new DateTime(year, 12, 31);
                    ServerClient.reservationDeletePeriod(startDate, endDate, true);
                    refresh();
                    MessageBox.Show("삭제에 성공 했습니다.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                } catch (Exception re) {
                    MessageBox.Show("알 수 없는 오류로 삭제에 실패 했습니다.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
