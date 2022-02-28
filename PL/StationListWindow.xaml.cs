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

        private void Group_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                stationToListDataGrid.ItemsSource = bl.getStationList().GroupBy(x => x.FreeChargingSlots);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            stationToListDataGrid.ItemsSource = bl.getStationList();
        }

        private void stationToListDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StationToList selectedStation = (stationToListDataGrid.SelectedItem as StationToList);
            if (selectedStation != null)
            {
                StationViewWindow win = new StationViewWindow(bl, selectedStation);
                win.Closed += win_Closed;
                win.ShowDialog();
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            StationViewWindow win = new StationViewWindow(bl);
            win.Closed += win_Closed;
            win.ShowDialog();
        }

        private void win_Closed(object sender, EventArgs e)
        {
            stationToListDataGrid.ItemsSource = bl.getStationList();
        }

    }
}
