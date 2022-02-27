using BL.BO;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>

    public partial class UserWindow : Window
    {
        private BLApi.IBL bl;
        private Customer customer;
        public UserWindow(BLApi.IBL b, BL.BO.Customer c)
        {
            InitializeComponent();
            customer = c;
            bl = b;
        }


        private void ShowIncoming_Click(object sender, RoutedEventArgs e)
        {
            ParcelListWindow2 win = new ParcelListWindow2(bl, customer);
            win.Show();

        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ParcelWindow win = new ParcelWindow(bl);
            win.Show();
        }


        private void ShowOutgoing_Click(object sender, RoutedEventArgs e)
        {
            
            ParcelListWindow2 win = new ParcelListWindow2(bl, customer, e);
            win.Show();
        }
    }
}

