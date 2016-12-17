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

namespace ClassroomReservation.Main {
    /// <summary>
    /// LoadLectureTableWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoadLectureTableWindow : Window {
        private List<LectureItem> items;

        public LoadLectureTableWindow() {
            InitializeComponent();

            datePicker.SelectedDate = DateTime.Today;

            excelSearchButton.Click += new RoutedEventHandler(processExcel);
            processButton.Click += new RoutedEventHandler(MakeLecture);
        }

        private void processExcel(object sender, RoutedEventArgs e) {
            ExcelReadClient.onFileSelected = (fileName) => excelFileNameText.Text = fileName;
            items = ExcelReadClient.readExcel();
        }

        private void MakeLecture(object sender, RoutedEventArgs e) {
            if (datePicker.SelectedDate.HasValue) {
                foreach (LectureItem item in items) {
                    ServerClient.MakeLecture(item, datePicker.SelectedDate.Value);
                }
                Close();
            }
        }
    }
}
