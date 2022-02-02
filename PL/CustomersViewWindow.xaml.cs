﻿using System;
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
    /// Interaction logic for CustomersViewWindow.xaml
    /// </summary>
    public partial class CustomersViewWindow : Window
    {
        private Customer customer;
        private BLApi.IBL bl;

        public CustomersViewWindow(BLApi.IBL b)
        {
            InitializeComponent();
            grid1.Visibility = Visibility.Hidden;
            parcels.Visibility = Visibility.Hidden;
            addGrid.Visibility = Visibility.Visible;
        }
        public CustomersViewWindow(BLApi.IBL b,BL.BO.CustomerForList c)
        {
            InitializeComponent();
            bl = b;
            customer = b.getCustomer(c.Id);
            grid1.DataContext = customer;
            parcels.DataContext = bl;
            grid1.Visibility = Visibility.Visible;
            parcels.Visibility = Visibility.Visible;
            addGrid.Visibility = Visibility.Visible;
        }

        private void ingoingParcelsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void outgoingParcelsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (nameTextBox.Text == customer.Name && phoneNumberTextBox.Text == customer.PhoneNumber)
                {
                    throw new Exception("please enter new name or phone number to update");
                }
                else if (nameTextBox.Text != customer.Name && phoneNumberTextBox.Text == customer.PhoneNumber)
                {
                    bl.UpdateCustomerName(customer.Id, nameTextBox.Text);
                }
                else if (nameTextBox.Text == customer.Name && phoneNumberTextBox.Text != customer.PhoneNumber)
                {
                    bl.UpdateCustomerPhone(customer.Id, phoneNumberTextBox.Text);
                }
                else if (nameTextBox.Text != customer.Name && phoneNumberTextBox.Text != customer.PhoneNumber)
                {
                    bl.UpdateCustomerName(customer.Id, nameTextBox.Text);
                    bl.UpdateCustomerPhone(customer.Id, phoneNumberTextBox.Text);
                }
            }
              catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            CustomersViewWindow addCustomer = new CustomersViewWindow(bl);
            addCustomer.ShowDialog();
            this.Close();
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void addThis_Click(object sender, RoutedEventArgs e)
        {
            Customer c = new Customer();
            c.Id = Convert.ToInt32(id.Text);
            c.Name= name.Text;
            c.PhoneNumber = phone.Text;
            c.Location = new Location(Convert.ToInt32(latitude.Text), Convert.ToInt32(longitude.Text));
            bl.AddCustomer(c);
        }

    }
}
