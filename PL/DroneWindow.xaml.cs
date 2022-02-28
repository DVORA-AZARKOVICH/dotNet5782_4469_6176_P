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
using BLApi;
using System.ComponentModel;
using System.Threading;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        BackgroundWorker worker; //field
        bool F;
        private BLApi.IBL bl;
        private DroneToList myDrone;
        bool automaticflag;
       // public MyData myData;

        /// <summary>
        /// initilizes the add drone window
        /// </summary>
        /// <param name="b">object of type BL</param>
         public DroneWindow(BLApi.IBL b)
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            updateGrid.Visibility = Visibility.Hidden;

            bl = b;
            //myData = new MyData();
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
            //// status_d.Visibility = Visibility.Hidden;
            // //weight_d.Visibility = Visibility.Hidden;
            // weight.Visibility = Visibility.Visible;
            // Status.Visibility = Visibility.Visible;

            // battery.Text = " ";
            // DroneId.Text = " ";
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

           /* if (e.Cancelled == true)
            {
                progBarTime.Value = 0;
                curPer.PersonalStatus = Status.SINGLE;
                MessageBox.Show("Try next time...");
            }
            else
            {
                curPer.PersonalStatus = Status.MARRIED;
                MessageBox.Show("Mazal Tov!!");
            }
            this.Cursor = Cursors.Arrow;*/
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Drone dt = new Drone();
            int idd = Convert.ToInt32(DroneId.Text);
            dt = bl.getDrone(idd);
            battery.Text = dt.BatteryStatus.ToString();
            //Status_d.Text = dt.Status.ToString();
            DroneId.Text = " ";
            /*int buttery = e.ProgressPercentage;
            progBarTime.Value = precent;
            if (precent > 50)
                curPer.PersonalStatus = Status.ENGAGED;*/
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Drone d = new Drone();
            int idd = int.Parse(e.Argument.ToString());
            d = bl.getDrone(idd);
            double buterry = d.BatteryStatus;
            while(automaticflag&& buterry<100)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    try
                    {
                        Thread.Sleep(500);
                       // bl.NextState(idd);
                        if (worker.WorkerReportsProgress == true)
                            worker.ReportProgress(idd);

                    }
                    catch (ThreadException ex)
                    {
                        worker.ReportProgress(-1, ex);
                        e.Cancel = true;
                    }


                }
                }
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int idd =1* Convert.ToInt32(Id.Text);
            // worker.RunWorkerAsync(idd);
            DroneAutomatic d = new DroneAutomatic(bl,idd);
            d.Show();

        }


        /// <summary>
        /// initilizes the drone update window
        /// </summary>
        /// <param name="b">BL object</param>
        /// <param name="selectedItem">the selelcted drone</param>
        public DroneWindow(BLApi.IBL b, DroneToList selectedItem)
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            bl = b;
            myDrone = selectedItem;
            updateGrid.DataContext = myDrone;
            addGrid.Visibility = Visibility.Hidden;

            //myDrone = selectedItem;
            //myData = new MyData() { Id = selectedItem.Id, BatteryStatus = selectedItem.BatteryStatus, Status = selectedItem.Status, Model = selectedItem.Model, Location = selectedItem.Location1, ParcelId = selectedItem.ParcelId, Weight = selectedItem.Weight };
            //this.DataContext = myData;

            add.Visibility = Visibility.Hidden;
            update.Visibility = Visibility.Visible;
            weight.Visibility = Visibility.Hidden;
            Status.Visibility = Visibility.Hidden;
            station.Visibility = Visibility.Hidden;
            //DroneId.IsReadOnly = true;
            //battery.IsReadOnly = true;
            Latitude.IsReadOnly = true;
            Longitude.IsReadOnly = true;
            Longitude.Text=myDrone.Location1.Longitude.ToString();
            Latitude.Text = myDrone.Location1.Latitude.ToString();
            //weight_d.Visibility = Visibility.Visible;
            //battery.Text = selectedItem.BatteryStatus.ToString();
            //Status_d.Text = selectedItem.Status.ToString();
            //weight_d.Text = selectedItem.Weight.ToString();
            if (myDrone.Status == DroneStatus.free)
            {
                sendToCharging.Visibility = Visibility.Visible;
                sendToParcel.Visibility = Visibility.Visible;
                pickUp.Visibility = Visibility.Hidden;
                release.Visibility = Visibility.Hidden;
                delieverd.Visibility = Visibility.Hidden;
            }
            else if (myDrone.Status == DroneStatus.inMaintence)
            {
                sendToCharging.Visibility = Visibility.Hidden;
                sendToParcel.Visibility = Visibility.Hidden;
                pickUp.Visibility = Visibility.Hidden;
                release.Visibility = Visibility.Visible;
                delieverd.Visibility = Visibility.Hidden;
            }
            else if (myDrone.Status == DroneStatus.busy)
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
            bl.UpdateDrone(myDrone.Id, Model.Text);
           // this.DataContext = myDrone;
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
            this.DataContext = myDrone;
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
            this.DataContext = myDrone;
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
            this.DataContext = myDrone;
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
            this.DataContext = myDrone;
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
            this.DataContext = myDrone;
            this.Close();
        }

  


    }
}
