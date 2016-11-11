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

namespace ClassroomReservation
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isUserMode = true;

        public MainWindow()
        {
            InitializeComponent();

            DateTime today = DateTime.Now;

            ReservationStatusPerDay fileInputBox1 = new ReservationStatusPerDay(today);
            ReservationStatusPerDay fileInputBox2 = new ReservationStatusPerDay(today.AddDays(1));
            ReservationStatusPerDay fileInputBox3 = new ReservationStatusPerDay(today.AddDays(2));
            ReservationStatusPerDay fileInputBox4 = new ReservationStatusPerDay(today.AddDays(3));
            ReservationStatusPerDay fileInputBox5 = new ReservationStatusPerDay(today.AddDays(4));
            ReservationStatusPerDay fileInputBox6 = new ReservationStatusPerDay(today.AddDays(5));

            Content.Children.Add(fileInputBox1);
            Content.Children.Add(fileInputBox2);
            Content.Children.Add(fileInputBox3);
            Content.Children.Add(fileInputBox4);
            Content.Children.Add(fileInputBox5);
            Content.Children.Add(fileInputBox6);

            ChangeModeButton.Click += new RoutedEventHandler(changeMode);
            readExcelFileButton.Click += new RoutedEventHandler(readExcelFile);

            AdminButtonPanel.Visibility = System.Windows.Visibility.Hidden;
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

        public void readExcelFile(object sender, RoutedEventArgs e)
        {
            //var fileName = string.Format("{0}\\fileNameHere", Directory.GetCurrentDirectory());
            //var connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", fileName);

            //var adapter = new OleDbDataAdapter("SELECT * FROM [workSheetNameHere$]", connectionString);
            //var ds = new DataSet();

            //adapter.Fill(ds, "anyNameHere");

            //DataTable data = ds.Tables["anyNameHere"];

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                //fileOpenTextBox.Text = dlg.FileName;
            }
        }
    }
}
