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

namespace PL
{
    /// <summary>
    /// Interaction logic for verifyWindow.xaml
    /// </summary>
    public partial class verifyWindow : Window
    {
        internal readonly BLApi.IBL bl;
        public BL.BO.Customer customer;
        public verifyWindow(BLApi.IBL b)
        {
            InitializeComponent();
            bl = b;
        }

        private void cEnter_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    BL.BO.Customer c = bl.getCustomer(Convert.ToInt32(UserID.Text));

            //    if (UserName.Text == c.Name)
            //    {
            //        UserWindow win = new UserWindow(customer);
            //        this.Close();
            //    }
            //    else throw new Exception("ID or user name is incorrect");
            //}
            //catch (Exception ex)
            //{

            //    MessageBox.Show(ex.Message,
            //        "ERROR",
            //        MessageBoxButton.OK,
            //        MessageBoxImage.Error);
            //}
        }
    }
}
