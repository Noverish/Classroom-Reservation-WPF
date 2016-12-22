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
    public delegate void OnSelectPeriodDelete(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SelectPeriodSelectWindow : Window {
        private OnSelectPeriodDelete onSelectPeriodDelete;

        public SelectPeriodSelectWindow(OnSelectPeriodDelete onSelectPeriodDelete) {
            InitializeComponent();

            this.onSelectPeriodDelete = onSelectPeriodDelete;

            stareDate.SelectedDate = DateTime.Now;
            endDate.SelectedDate = DateTime.Now;

            deleteButton.Click += new RoutedEventHandler((o,s) => onSelectPeriodDelete?.Invoke(stareDate.SelectedDate.Value, stareDate.SelectedDate.Value));
        }
    }
}
