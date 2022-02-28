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
            add.Visibility = Visibility.Hidden;
            
        }

        public ParcelWindow(BLApi.IBL b)
        {

            InitializeComponent();
            this.bl = b;
            //newParcel.Visibility = Visibility.Hidden;
            AddGrid.Visibility = Visibility.Visible;
            recieverGrid.Visibility = Visibility.Hidden;
            senderGrid.Visibility = Visibility.Hidden;
           // update.Visibility = Visibility.Hidden;
            updateGrid.Visibility = Visibility.Hidden;
            Sender.ItemsSource = from item in bl.getCustomerList()
                                 select item.Id;
            Reciver.ItemsSource = from item in bl.getCustomerList()
                                  select item.Id;
            priority.ItemsSource=Enum.GetValues(typeof(Priority));
            weight.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            
        }
        public ParcelWindow(BLApi.IBL b,int id)
        {

            InitializeComponent();
            this.bl = b;
            Parcel p = bl.getParcel(id);
            //newParcel.Visibility = Visibility.Hidden;
            AddGrid.Visibility = Visibility.Visible;
            recieverGrid.Visibility = Visibility.Hidden;
            senderGrid.Visibility = Visibility.Hidden;
            // update.Visibility = Visibility.Hidden;
            updateGrid.Visibility = Visibility.Hidden;
            Sender.ItemsSource = from item in bl.getCustomerList()
                                 select item.Id;
            Reciver.ItemsSource = from item in bl.getCustomerList()
                                  select item.Id;
            priority.ItemsSource = Enum.GetValues(typeof(Priority));
            weight.ItemsSource = Enum.GetValues(typeof(WeightCategories));

        }


        public ParcelWindow(BLApi.IBL b,BL.BO.Customer c ,object e)
        {

            InitializeComponent();
            this.bl = b;
            
            //newParcel.Visibility = Visibility.Hidden;
            AddGrid.Visibility = Visibility.Visible;
            recieverGrid.Visibility = Visibility.Hidden;
            senderGrid.Visibility = Visibility.Hidden;
            // update.Visibility = Visibility.Hidden;
            updateGrid.Visibility = Visibility.Hidden;
            Sender.ItemsSource = from item in bl.getCustomerList()
                                 where item.Id==c.Id
                                 select item.Id;
            Reciver.ItemsSource = from item in bl.getCustomerList()
                                  select item.Id;
            priority.ItemsSource = Enum.GetValues(typeof(Priority));
            weight.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BL.BO.Parcel p= new BL.BO.Parcel();
                p.Weight = (WeightCategories)weight.SelectedItem;
                p.Priority = (Priority)priority.SelectedItem;
                int id = Convert.ToInt32(Sender.SelectedItem);
                p.Sender = new CustomerInParcel(id, bl.getCustomer(id).Name);
                id = Convert.ToInt32(Reciver.SelectedItem);
                p.Receiver=new CustomerInParcel(id,bl.getCustomer(id).Name);
                bl.AddParcel(p);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"ERROR",MessageBoxButton.OK);
            }
            MessageBox.Show("New parcel was added seccessfully!", "added!", MessageBoxButton.OK, MessageBoxImage.None);
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
