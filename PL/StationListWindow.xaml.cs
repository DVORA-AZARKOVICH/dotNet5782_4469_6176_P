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
using BLApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for StationListWindow.xaml
    /// </summary>
    public partial class StationListWindow : Window
    {
        private IBL bl;
        public StationListWindow(IBL b)
        {

            InitializeComponent();
            bl = b;
            stationToListDataGrid.DataContext = bl.getStationList();
            
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            int num = Convert.ToInt32(Selector.Text);

            if (Selector.Text.Length == 0)
            {
                stationToListDataGrid.ItemsSource = bl.getStationList();
            }
            else
            {
    //            stationToListDataGrid.ItemsSource = bl.getStationList(item => item.FreeChargingSlots == num);
            }
        
        }
    }
}
