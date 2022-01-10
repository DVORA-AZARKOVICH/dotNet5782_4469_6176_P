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
using BLApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>

    public partial class DroneListWindow : Window
    {
        private BLApi.IBL bl;
        public DroneListWindow(IBL b)
        {
            InitializeComponent();
            bl = b;
            DroneListView.ItemsSource = bl.getdroneList();
            StatusSelector.ItemsSource = Enum.GetValues(typeof(DroneStatus));
            WeightSelector.ItemsSource = Enum.GetValues(typeof(WeightCategories));
        }

        private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((DroneStatus)StatusSelector.SelectedItem == DroneStatus.busy)
                {
                    var t = bl.getdroneList(item => item.Status == DroneStatus.busy);
                    DroneListView.ItemsSource = t;
                    if (t.Any())
                    {
                        DroneListView.ItemsSource = t;
                    }
                    else
                        throw new Exception("there are no drones of this type");
                }
                else if ((DroneStatus)StatusSelector.SelectedItem == DroneStatus.inMaintence)
                {
                    var t = bl.getdroneList(item => item.Status == DroneStatus.inMaintence);
                    DroneListView.ItemsSource = t;
                    if (t.Any())
                    {
                        DroneListView.ItemsSource = t;
                    }
                    else
                    {
                        DroneListView.ItemsSource = t;
                        throw new Exception("there are no drones of this type");
                    }
                }
                else if ((DroneStatus)StatusSelector.SelectedItem == DroneStatus.free)
                {
                    var t = bl.getdroneList(item => item.Status == DroneStatus.free);
                    if (t.Any())
                    {
                        DroneListView.ItemsSource = t;
                    }
                    else
                        throw new Exception("there are no drones of this type");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message,
                    "ERROR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void WeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((WeightCategories)WeightSelector.SelectedItem == WeightCategories.light)
                {
                    var t = bl.getdroneList(item => item.Weight == WeightCategories.light);
                    DroneListView.ItemsSource = t;
                    if (t.Any())
                    {
                        DroneListView.ItemsSource = t;
                    }
                    else
                        throw new Exception("there are no drones of this type");
                }
                if ((WeightCategories)WeightSelector.SelectedItem == WeightCategories.medium)
                {
                    var t = bl.getdroneList(item => item.Weight == WeightCategories.medium);
                    DroneListView.ItemsSource = t;
                    if (t.Any())
                    {
                        DroneListView.ItemsSource = t;
                    }
                    else
                        throw new Exception("there are no drones of this type");
                }
                if ((WeightCategories)WeightSelector.SelectedItem == WeightCategories.heavy)
                {
                    var t = bl.getdroneList(item => item.Weight == WeightCategories.heavy);
                    DroneListView.ItemsSource = t;
                    if (t.Any())
                    {
                        DroneListView.ItemsSource = t;
                    }
                    else
                        throw new Exception("there are no drones of this type");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "ERROR",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl).Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void DroneListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DroneWindow dw = new DroneWindow(bl, (DroneToList)DroneListView.SelectedItem);
            dw.Closed += Dw_Closed;
            dw.Show();
        }
        private void Dw_Closed(object sender, EventArgs e)
        {
            DroneListView.ItemsSource = bl.getdroneList();
        }

        private void AddDrone_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl).Show();
        }

        private void ExitDrone_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
