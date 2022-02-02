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
using BL.BO;
using BLApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for StationListWindow.xaml
    /// </summary>
    public partial class StationListWindow : Window
    {
        private IBL bl;
        public StationListWindow(IBL b)
        {

            InitializeComponent();
            bl = b;
            stationToListDataGrid.IsReadOnly = true;
            stationToListDataGrid.ItemsSource = bl.getStationList();
            
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var t = bl.getStationList(item => item.FreeChargingSlots == Convert.ToInt32(Selector.Text));
                stationToListDataGrid.ItemsSource = t;
                if (!t.Any())
                    throw new Exception("there are no station with this amount of slots");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            stationToListDataGrid.ItemsSource = bl.getStationList();
        }

        private void showStation_Click(object sender, RoutedEventArgs e)
        {
            StationToList selectedStation = (stationToListDataGrid.SelectedItem as StationToList);
            if (selectedStation != null)
            {
                StationViewWindow win = new StationViewWindow(bl,selectedStation);
                win.ShowDialog();
            }
        }
    }
}
