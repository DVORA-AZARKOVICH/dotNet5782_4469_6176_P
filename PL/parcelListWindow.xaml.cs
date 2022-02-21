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

namespace PL
{
    /// <summary>
    /// Interaction logic for parcelListWindow.xaml
    /// </summary>
    public partial class parcelListWindow : Window
    {
        private BLApi.IBL bl;
        public parcelListWindow()
        {
            InitializeComponent();
            parcelForListDataGrid.IsReadOnly = true;  
        }
        public parcelListWindow(BLApi.IBL b, Customer c)
        {
            InitializeComponent();
            bl = b;
            parcelForListDataGrid.IsReadOnly = true;
            
           IEnumerable<ParcelForList> parcels=b.getParcelList(item=>item.ReciverName==c.Name);
        }
        public parcelListWindow(BLApi.IBL b, Customer c,EventArgs e)
        {
            InitializeComponent();
            bl = b;
            parcelForListDataGrid.IsReadOnly = true;

            IEnumerable<ParcelForList> parcels = b.getParcelList(item => item.SenderName == c.Name);
        }
    }
}
