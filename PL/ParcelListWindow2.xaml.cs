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
    /// Interaction logic for ParcelListWindow2.xaml
    /// </summary>
    public partial class ParcelListWindow2 : Window
    {
        public BLApi.IBL bl;
        public Customer customer;
        public ParcelListWindow2(BLApi.IBL b)
        {
            InitializeComponent();
            bl = b;
            sort.ItemsSource = Enum.GetValues(typeof(ParcelStatus));
            parcelForListDataGrid.ItemsSource = bl.getParcelList();
        }
        public ParcelListWindow2(BLApi.IBL b, Customer c)
        {
            InitializeComponent();
            bl = b;
           // parcelForListDataGrid.IsReadOnly = true;
            parcelForListDataGrid.ItemsSource = b.getParcelList(item => item.ReciverName == c.Name);
           // sort.ItemsSource = Enum.GetValues(typeof(ParcelStatus));
            sort.Visibility = Visibility.Hidden;
            reciver.Visibility = Visibility.Hidden;
            sender.Visibility = Visibility.Hidden;

        }
        public ParcelListWindow2(BLApi.IBL b, Customer c, EventArgs e)
        {
            InitializeComponent();
            bl = b;
            customer = c;

            parcelForListDataGrid.ItemsSource = bl.getParcelList(item => item.SenderName == c.Name);
            // sort.ItemsSource = Enum.GetValues(typeof(ParcelStatus));
            sort.Visibility = Visibility.Hidden;
            reciver.Visibility = Visibility.Hidden;
            sender.Visibility = Visibility.Hidden;
        }

        private void sender_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var t = bl.getParcelList().GroupBy(x => x.SenderName);
                parcelForListDataGrid.ItemsSource = t;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void reciver_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var t = bl.getParcelList().GroupBy(x => x.ReciverName);
                parcelForListDataGrid.ItemsSource = t;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((ParcelStatus)sort.SelectedItem == ParcelStatus.attached)
                {
                    var t = bl.getParcelList(item => item.Status == ParcelStatus.attached);
                    parcelForListDataGrid.ItemsSource = t;
                    if (!t.Any())
                        throw new Exception("there are no parcels of this type");
                }
                if ((ParcelStatus)sort.SelectedItem == ParcelStatus.Created)
                {
                    var t = bl.getParcelList(item => item.Status == ParcelStatus.Created);
                    parcelForListDataGrid.ItemsSource = t;
                    if (!t.Any())
                        throw new Exception("there are no parcels of this type");
                }
                if ((ParcelStatus)sort.SelectedItem == ParcelStatus.deliverd)
                {
                    var t = bl.getParcelList(item => item.Status == ParcelStatus.deliverd);
                    parcelForListDataGrid.ItemsSource = t;
                    if (!t.Any())
                        throw new Exception("there are no parcels of this type");
                }
                if ((ParcelStatus)sort.SelectedItem == ParcelStatus.pickedUp)
                {
                    var t = bl.getParcelList(item => item.Status == ParcelStatus.pickedUp);
                    parcelForListDataGrid.ItemsSource = t;
                    if (!t.Any())
                        throw new Exception("there are no parcels of this type");
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message,
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

    }
}
