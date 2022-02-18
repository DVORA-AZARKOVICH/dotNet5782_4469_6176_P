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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BL;
using BL.BO;
using BLApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal readonly IBL b = BLFactory.GetBL();

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new DroneListWindow(b).Show();
        }

        private void parcels_Click(object sender, RoutedEventArgs e)
        {
            new parcelListWindow().Show();
        }

        private void station_Click(object sender, RoutedEventArgs e)
        {
            new StationListWindow(b).Show();
        }

        private void customers_Click(object sender, RoutedEventArgs e)
        {
            new CustomerListWindow(b).Show();
        }
    }
}
