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
    /// Interaction logic for StationViewWindow.xaml
    /// </summary>
    public partial class StationViewWindow : Window
    {
        private BLApi.IBL bl;
        private Station station;
        
        /// <summary>
        /// initilizes the grids that are shown when viewing/updating a station's details.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="st"></param>
        public StationViewWindow(BLApi.IBL b,BL.BO.StationToList st)
        {
            InitializeComponent();
            station = bl.getStation(st.Id);
            bl = b;
        }
        /// <summary>
        /// initilizes the grids for adding a station.
        /// </summary>
        /// <param name="b"></param>
        public StationViewWindow(BLApi.IBL b)
        {
            InitializeComponent();
            this.bl = b;
            updateGrid.Visibility = Visibility.Hidden;
            drones.Visibility = Visibility.Hidden;
            addGrid.Visibility = Visibility.Visible;
            add.Visibility = Visibility.Hidden;
            update.Visibility = Visibility.Hidden;
            addThis.Visibility = Visibility.Visible;
            close.Visibility = Visibility.Visible;
        }
        private void update_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if(station.Name==nameTextBox.Text&&station.Chargslot==Convert.ToInt32(chargslotTextBox.Text))
                {
                    throw new Exception("please enter new name or number of charging slots");
                }
                else if (station.Name == nameTextBox.Text && station.Chargslot != Convert.ToInt32(chargslotTextBox.Text))
                {
                    bl.UpdatStationChargeslots(station.Id, Convert.ToInt32(chargslotTextBox.Text));
                }
                else if(station.Name != nameTextBox.Text && station.Chargslot == Convert.ToInt32(chargslotTextBox.Text))
                {
                    bl.UpdateStationName(station.Id,nameTextBox.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            StationViewWindow win = new StationViewWindow(bl);
            win.ShowDialog();
        }

        private void addThis_Click(object sender, RoutedEventArgs e)
        {
            Station s = new Station();
            s=addGrid.DataContext as Station;
            bl.AddStation(s);
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}