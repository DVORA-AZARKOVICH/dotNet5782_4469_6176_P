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

    public partial class ParcelWindow : Window
    {
        private BLApi.IBL bl;
        private BL.BO.Parcel parcel;
        /// <summary>
        /// constractor for showing\updating a parcel
        /// </summary>
        /// <param name="b"></param>
        /// <param name="p"></param>
        public ParcelWindow(BLApi.IBL b,BL.BO.Parcel p)
        {
            InitializeComponent();
            bl = b;
            parcel = b.getParcel(p.Id);
            updateGrid.DataContext = parcel;
            AddGrid.Visibility = Visibility.Hidden;
            senderGrid.DataContext = parcel.Sender;
            recieverGrid.DataContext = parcel.Receiver;
        }
        
        public ParcelWindow(BLApi.IBL b)
        {

            InitializeComponent();
            this.bl = b;
            newParcel.Visibility = Visibility.Hidden;
            AddGrid.Visibility = Visibility.Visible;
            recieverGrid.Visibility = Visibility.Hidden;
            senderGrid.Visibility = Visibility.Hidden;
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {

        }

        private void newParcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void add_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
