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

namespace ClassroomReservation.Reservation {
    public delegate void OnClassroomSelectChanged(string nowSelectedClassroom, bool isDataChanged);

    /// <summary>
    /// TimeSelectControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClassroomSelectControl : UserControl {
        public OnClassroomSelectChanged onClassroomSelectChanged { set; private get; }

        private int TOTAL_NUM;

        private ClassroomLabel[] buttons;
        
        private SolidColorBrush selectedColor = (SolidColorBrush)Application.Current.FindResource("MicrosoftBlue");
        private SolidColorBrush hoverColor = (SolidColorBrush)Application.Current.FindResource("MicrosoftRed");
        private SolidColorBrush backgroundEven = (SolidColorBrush)Application.Current.FindResource("BackgroundOfEvenRow");
        private SolidColorBrush backgroundOdd = (SolidColorBrush)Application.Current.FindResource("BackgroundOfOddRow");

        private Label beforeSelected;
        private ClassroomLabel nowSelected;
        private bool mouseLeftButtonDown = false;
        private int previousColor = -1;

        public ClassroomSelectControl() {
            InitializeComponent();

            Hashtable classroomTable = Database.getInstance().classroomTable;
            TOTAL_NUM = classroomTable.Count;
            buttons = new ClassroomLabel[TOTAL_NUM];
            Label nowBuildingLabel = null;
            for (int row = 0; row < TOTAL_NUM; row++) {

                //Add RowDefinition
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(1, GridUnitType.Star);
                mainGrid.RowDefinitions.Add(rowDef);

                //Get building name and classroom name
                string classroomName = (classroomTable[row] as ClassroomItem).classroom;
                string buildingName = (classroomTable[row] as ClassroomItem).building;

                //Add label to Grid
                ClassroomLabel classroomLabel = new ClassroomLabel(buildingName, classroomName);
                classroomLabel.Content = classroomName;
                classroomLabel.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
                classroomLabel.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
                classroomLabel.MouseEnter += new MouseEventHandler(OnMouseEnter);
                classroomLabel.MouseLeave += new MouseEventHandler(OnMouseLeave);

                Grid.SetRow(classroomLabel, row);
                Grid.SetColumn(classroomLabel, 1);

                buttons[row] = classroomLabel;
                mainGrid.Children.Add(classroomLabel);

                //Adjust building label
                if (nowBuildingLabel != null && nowBuildingLabel.Content.Equals(buildingName)) {
                    Grid.SetRowSpan(nowBuildingLabel, Grid.GetRowSpan(nowBuildingLabel) + 1);
                } else {
                    Label buildingLabel = new Label();
                    buildingLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    buildingLabel.VerticalAlignment = VerticalAlignment.Center;
                    buildingLabel.Content = buildingName;

                    Grid.SetRow(buildingLabel, row);
                    Grid.SetColumn(buildingLabel, 0);
                    Grid.SetRowSpan(buildingLabel, 1);

                    nowBuildingLabel = buildingLabel;
                    mainGrid.Children.Add(buildingLabel);
                }
            }

            ResetBackground();
        }


        public void enable(bool enable) {
            if (enable)
                overlapRectangle.Visibility = Visibility.Hidden;
            else
                overlapRectangle.Visibility = Visibility.Visible;
        }

        public void ResetSelection() {
            beforeSelected = null;
            nowSelected = null;
            ResetBackground();
        }

        public bool HasSelectedClassroom() {
            return nowSelected != null;
        }

        public string GetSelectedClassroom() {
            return nowSelected.GetFullName();
        }


        private void OnMouseLeftButtonDown(object sender, RoutedEventArgs e) {
            nowSelected = sender as ClassroomLabel;

            ResetBackground();

            nowSelected.Background = selectedColor;
            previousColor = -1;
            mouseLeftButtonDown = true;
        }

        private void OnMouseLeftButtonUp(object sender, RoutedEventArgs e) {
            mouseLeftButtonDown = false;

            onClassroomSelectChanged?.Invoke(nowSelected.GetFullName(), beforeSelected != null && beforeSelected != nowSelected);
            beforeSelected = nowSelected;
        }

        private void OnMouseEnter(object sender, RoutedEventArgs e) {
            if (mouseLeftButtonDown) { } else {
                if (previousColor >= 0 && previousColor % 2 == 0 && buttons[previousColor].Background != selectedColor) {
                    buttons[previousColor].Background = backgroundOdd;
                } else if (previousColor >= 0 && previousColor % 2 == 1 && buttons[previousColor].Background != selectedColor) {
                    buttons[previousColor].Background = backgroundEven;
                }

                previousColor = Grid.GetRow(sender as Label);
                if (buttons[previousColor].Background != selectedColor)
                    buttons[previousColor].Background = hoverColor;
            }
        }

        private void OnMouseLeave(object sender, RoutedEventArgs e) {
            for (int i = 0; i < TOTAL_NUM; i++) {
                if (buttons[i].Background == selectedColor)
                    buttons[i].Background = selectedColor;
                else if (i % 2 == 0)
                    buttons[i].Background = backgroundOdd;
                else if (i % 2 == 1)
                    buttons[i].Background = backgroundEven;
            }
        }

        private void ResetBackground() {
            for (int i = 0; i < TOTAL_NUM; i++) {
                if (i % 2 == 0) {
                    buttons[i].Background = backgroundOdd;
                } else {
                    buttons[i].Background = backgroundEven;
                }
            }
        }
    }

    class ClassroomLabel : Label {
        public string buildingName { get; private set; }
        public string classroomName { get; private set; }

        public ClassroomLabel(string buildingName, string classroomName) {
            this.buildingName = buildingName;
            this.classroomName = classroomName;
        }

        public string GetFullName() {
            return buildingName + ":" + classroomName;
        }
    }
}
