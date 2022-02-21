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
        public UserWindow(BL.BO.Customer c)
        {
            InitializeComponent();
            customer = c;
        }


        private void ShowIncoming_Click(object sender, RoutedEventArgs e)
        {
            parcelListWindow win=new parcelListWindow(bl,customer);

        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            parcelListWindow win = new parcelListWindow(bl, customer,e);
        }

        private void ShowIngoing_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowOutgoing_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
