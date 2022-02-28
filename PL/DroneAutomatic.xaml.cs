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
        bool automaticflag = true, stopAuto = false;
        private BLApi.IBL bl;
        Drone drone;
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
            pp.Visibility = Visibility.Hidden;



        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            pp.Visibility = Visibility.Visible;
            the_parcel.Visibility = Visibility.Visible;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Drone drone1 = bl.getDrone(e.ProgressPercentage);
            status.Content = drone1.Status;
            buttery.Content = drone1.BatteryStatus;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            while (automaticflag != true)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                { 
                    Thread.Sleep(500);
                    bl.NextState(drone.Id);
                    if (worker.WorkerReportsProgress == true)
                        worker.ReportProgress(drone.Id);
                }
            }
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            automaticflag = false;
            if (worker.IsBusy != true)
            {
                worker.RunWorkerAsync();
            }
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {

            stop.Background = Brushes.Gray;
            stopAuto = true;
            if (worker.WorkerSupportsCancellation == true)
                worker.CancelAsync();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
