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

        private void manager_Click(object sender, RoutedEventArgs e)
        {
            Password win=new Password();
            //ManagerWindow win=new ManagerWindow();
            win.Show();
        }

        private void customer_Click(object sender, RoutedEventArgs e)
        {
            verifyWindow win = new verifyWindow(b);//.ShowDialog();
            win.Show();
        }

        private void newCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomersViewWindow win = new CustomersViewWindow(b);
            win.Show();
        }
    }
}

