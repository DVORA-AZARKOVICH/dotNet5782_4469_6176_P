﻿using BL.BO;
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
using BLApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public class MyData
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public WeightCategories Weight { get; set; }
        public double BatteryStatus { get; set; }
        public DroneStatus Status { get; set; }
        public Location Location { get; set; }
        public int ParcelId { get; set; }

    }
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        private BLApi.IBL bl;
        private DroneToList myDrone;
        public MyData myData;

        /// <summary>
        /// initilizes the add drone window
        /// </summary>
        /// <param name="b">object of type BL</param>
        public DroneWindow(BLApi.IBL b)
        {
            InitializeComponent();
            bl = b;
            myData = new MyData();
            Status.ItemsSource = Enum.GetValues(typeof(DroneStatus));
            weight.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            var t = from item in bl.getStationList()
                    select item.Id;
            station.ItemsSource = t;
            update.Visibility = Visibility.Hidden;
            sendToCharging.Visibility = Visibility.Hidden;
            sendToParcel.Visibility = Visibility.Hidden;
            pickUp.Visibility = Visibility.Hidden;
            release.Visibility = Visibility.Hidden;
            delieverd.Visibility = Visibility.Hidden;
            status_d.Visibility = Visibility.Hidden;
            weight_d.Visibility = Visibility.Hidden;
            weight.Visibility = Visibility.Visible;
            Status.Visibility = Visibility.Visible;
            battery.Text = " ";
            DroneId.Text = " ";
        }

        /// <summary>
        /// initilizes the drone update window
        /// </summary>
        /// <param name="b">BL object</param>
        /// <param name="selectedItem">the selelcted drone</param>
        public DroneWindow(BLApi.IBL b, DroneToList selectedItem)
        {
            InitializeComponent();
            bl = b;
            myDrone = selectedItem;
            myData = new MyData() { Id = selectedItem.Id, BatteryStatus = selectedItem.BatteryStatus, Status = selectedItem.Status, Model = selectedItem.Model, Location = selectedItem.Location, ParcelId = selectedItem.ParcelId, Weight = selectedItem.Weight };
            this.DataContext = myData;
            add.Visibility = Visibility.Hidden;
            update.Visibility = Visibility.Visible;
            status_d.Visibility = Visibility.Visible;
            weight_d.Visibility = Visibility.Visible;
            weight.Visibility = Visibility.Hidden;
            Status.Visibility = Visibility.Hidden;
            station.Visibility = Visibility.Hidden;
            DroneId.IsReadOnly = true;
            battery.IsReadOnly = true;
            latitude.IsReadOnly = true;
            longitude.IsReadOnly = true;
            if (selectedItem.Status == DroneStatus.free)
            {
                sendToCharging.Visibility = Visibility.Visible;
                sendToParcel.Visibility = Visibility.Visible;
                pickUp.Visibility = Visibility.Hidden;
                release.Visibility = Visibility.Hidden;
                delieverd.Visibility = Visibility.Hidden;
            }
            else if (selectedItem.Status == DroneStatus.inMaintence)
            {
                sendToCharging.Visibility = Visibility.Hidden;
                sendToParcel.Visibility = Visibility.Hidden;
                pickUp.Visibility = Visibility.Hidden;
                release.Visibility = Visibility.Visible;
                delieverd.Visibility = Visibility.Hidden;
            }
            else if (selectedItem.Status == DroneStatus.busy)
            {
                sendToCharging.Visibility = Visibility.Hidden;
                sendToParcel.Visibility = Visibility.Hidden;
                pickUp.Visibility = Visibility.Visible;
                release.Visibility = Visibility.Hidden;
                delieverd.Visibility = Visibility.Visible;
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            Station st = bl.getStation(Convert.ToInt32(station.SelectedItem));
            Drone d = new Drone()
            {
                Id = Convert.ToInt32(DroneId.Text),
                BatteryStatus = Convert.ToInt32(battery.Text),
                Model = model.Text,
                Weight = (WeightCategories)weight.SelectedItem,
                Status = (DroneStatus)Status.SelectedItem,
                CurrentLocation = new Location(st.Location.Latitude, st.Location.Longitude),
                //CurrentLocation = new Location(Convert.ToDouble(latitude), Convert.ToDouble(longitude)),
            };
            try
            {
                bl.AddDrone(d, st.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            this.Close();

        }

        private void DroneId_TextChanged(object sender, TextChangedEventArgs e)
        {
            int check;
            if (int.TryParse(DroneId.Text, out check))
            {
                DroneId.Background = System.Windows.Media.Brushes.White;
            }
            else
                DroneId.BorderBrush = System.Windows.Media.Brushes.Red;
        }

        private void battery_TextChanged(object sender, TextChangedEventArgs e)
        {
            int check;
            if (int.TryParse(battery.Text, out check))
            {
                battery.BorderBrush = System.Windows.Media.Brushes.White;
            }
            else
                battery.BorderBrush = System.Windows.Media.Brushes.Red;
        }

        private void model_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (model.Text.Length == 0)
                battery.BorderBrush = System.Windows.Media.Brushes.Red;
            else
                battery.BorderBrush = System.Windows.Media.Brushes.White;
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            bl.UpdateDrone(myDrone.Id, model.Text);
            this.DataContext = myData;
            this.Close();
        }

        private void sendToParcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.UpdateParcelToDrone(myDrone.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                     "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            this.DataContext = myData;
            this.Close();
        }

        private void sendToCharging_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.UpdateDroneToCharge(myDrone.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                     "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            this.DataContext = myData;
            this.Close();
        }

        private void release_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.UpdateReleseDroneFromCharge(myDrone.Id, DateTime.Now);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                     "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            this.DataContext = myData;
            this.Close();
        }

        private void delieverd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.UpdateDeliverdByDrone(myDrone.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                     "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            this.DataContext = myData;
            this.Close();
        }

        private void pickUp_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                bl.UpdatePickedByDrone(myDrone.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                     "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            this.DataContext = myData;
            this.Close();
        }


    }
}