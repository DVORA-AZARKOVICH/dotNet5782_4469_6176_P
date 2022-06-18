using BL.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for DroneAutomatic.xaml
    /// </summary>
    public partial class DroneAutomatic : Window
    {
        BackgroundWorker worker; //field
        bool automaticflag = true, stopAuto = false, flagcontinue = true;
        private BLApi.IBL bl;
        Drone drone;
        int parcelid = 0;
        public DroneAutomatic()
        {
            InitializeComponent();
        }
        public DroneAutomatic(BLApi.IBL b,int idd)
        {

            InitializeComponent();
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            bl = b;
            drone = bl.getDrone(idd);
            id.Content = drone.Id;
            buttery.Content = drone.BatteryStatus;
            actualstatus.Content = drone.Status;
            id.Visibility = Visibility.Visible;
            buttery.Visibility = Visibility.Visible;
            actualstatus.Visibility = Visibility.Visible;
            id.Content = drone.Id;
            buttery.Content = drone.BatteryStatus;
            actualstatus.Content = drone.Status;
            the_parcel.Visibility = Visibility.Hidden;
            idparcel.Visibility = Visibility.Hidden;

            //  pp.Visibility = Visibility.Hidden;



        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Drone newd = bl.getDrone(drone.Id);
                this.Cursor = Cursors.Arrow;
                if (parcelid != 0)
                {
                    the_parcel.Visibility = Visibility.Visible;
                    idparcel.Visibility = Visibility.Visible;
                    idparcel.Content = parcelid.ToString();
                    actualstatus.Content = newd.Status;
                    buttery.Content = newd.BatteryStatus;
                }
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK); 
            }

        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                Drone drone1 = bl.getDrone(e.ProgressPercentage);
                actualstatus.Content = drone1.Status;
                buttery.Content = drone1.BatteryStatus;
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }


        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                while (flagcontinue == true)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(500);
                        bl.NextState(drone.Id, ref flagcontinue, ref parcelid);
                        if (worker.WorkerReportsProgress == true)
                            worker.ReportProgress(drone.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
            }

        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                flagcontinue = true;
                if (worker.IsBusy != true)
                {
                    worker.RunWorkerAsync();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                stop.Background = Brushes.Gray;
                stopAuto = true;
                if (worker.WorkerSupportsCancellation == true)
                    worker.CancelAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
            }
        }

        private void idparcel_Click(object sender, RoutedEventArgs e)
        {
         /*   try
            {
                int id = Convert.ToInt32(idparcel.Content);
                Parcel p = bl.getParcel(id);
                ParcelWindow p2 = new ParcelWindow(bl,p);
                p2.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
            }*/
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
