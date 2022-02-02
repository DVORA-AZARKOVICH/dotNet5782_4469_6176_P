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
            customerForListDataGrid.ItemsSource = b.getCustomerList();
        }

        private void viewCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerForList selected = (customerForListDataGrid.SelectedItem as CustomerForList);
            if (selected != null)
            {
                CustomersViewWindow win = new CustomersViewWindow(bl,selected);
                win.ShowDialog();
            }
        }
    }
}
