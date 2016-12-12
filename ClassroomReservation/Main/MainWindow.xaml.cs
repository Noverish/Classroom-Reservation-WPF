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
using ClassroomReservation.Other;

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
            
            animationTimer.Interval = new TimeSpan(120);
            animationTimer.Tick += new EventHandler(MyTimer_Tick);

            button4.Click += new RoutedEventHandler((sender, e) => {
                ReservationWindow window = new ReservationWindow();
                window.onReservationSuccess = OnReservationSuccess;
                window.ShowDialog();
            });

            button6.Click += new RoutedEventHandler((sender, e) => {
                (new PasswordForm((form, password) => {
                    if (ServerClient.DeleteReservation(nowSelectedItem.reservID, password)) {
                        OnReservationSuccess(null);
                        form.Close();
                    } else {

                    }
                })).ShowDialog();
            });
        }

        private void PasswordChange(object sender, RoutedEventArgs e)
        {
            PasswordForm signWin = new PasswordForm((window, password) => {
                LoginClient.getInstance().onChangeSuccess = (() => {
                    AlertWindow alert = new AlertWindow("성공적으로 변경 했습니다. 다시 로그인 해주세요.");
                    alert.ShowDialog();
                    changeMode(true);
                    window.Close();
                });
                LoginClient.getInstance().onChangeFailed = ((msg) => {
                    AlertWindow alert = new AlertWindow("알 수 없는 오류가 발생해서 변경에 실패 했습니다. - " + msg);
                    alert.ShowDialog();
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
                    AlertWindow alert = new AlertWindow("비밀번호가 다릅니다.");
                    alert.ShowDialog();
                });
                LoginClient.getInstance().onLoginError = ((msg) => {
                    AlertWindow alert = new AlertWindow("알 수 없는 오류가 발생 했습니다. 최초 비밀번호로 로그인 해주세요. - " + msg);
                    alert.ShowDialog();
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
                Addid.Visibility = System.Windows.Visibility.Hidden;
                AdminButtonPanel.Visibility = System.Windows.Visibility.Hidden;
            } else {
                ChangeModeButton.Content = "일반 사용자 모드로 변경";
                Addid.Visibility = System.Windows.Visibility.Visible;
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

        private void onOneSelected(StatusItem item) {
            if (item != null) {
                nowSelectedItem = item;

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
    }
}
