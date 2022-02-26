using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BL.BO
{
    public class ParcelInTransfer
    {
        public int Id { get; set; }
        public ParcelInTransf Status { get; set; }//מחכה למשלוח או בדרך ללקוח
        public WeightCategories Weight { get; set; }
        public CustomerInParcel Sender { get; set; }
        public CustomerInParcel Reciver { get; set; }
        public Location Pickup { get; set; }
        public Location Destination { get; set; }
        public double Distance { get; set; }//מרחק ההובלה
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
