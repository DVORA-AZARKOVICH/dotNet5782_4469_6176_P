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

namespace PL
{
    /// <summary>
    /// Interaction logic for CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window
    {
        private BLApi.IBL bl;
        public CustomerListWindow(BLApi.IBL b)
        {
            InitializeComponent();
            bl = b;
            customerForListDataGrid.IsReadOnly = true;
            customerForListDataGrid.ItemsSource = bl.getCustomerList();
        }

        private void customerForListDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //DroneWindow dw = new DroneWindow(bl, (DroneToList)DroneListView.SelectedItem);
            ////dw.Closed += Dw_Closed;
            ////dw.Show();
            ////Customer c=;
            CustomersViewWindow win = new CustomersViewWindow(bl, (CustomerForList)customerForListDataGrid.SelectedItem);
            win.ShowDialog();
        }
    }
}
