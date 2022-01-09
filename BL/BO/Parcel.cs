using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    public class Parcel
    {
        public int Id { get; set; }
        public CustomerInParcel Sender { get; set; }//שולח החבילה
        public CustomerInParcel Receiver { get; set; }// הלקוח שמקבל את החבילה
        public WeightCategories Weight { get; set; }
        public Priority Priority { get; set; }
        public DroneInParcel Droneinparcel { get; set; }
        public DateTime? Created { get; set; }//זמן יצירת המשלוח
        public DateTime? Scheduled { get; set; }//זמן השיוך לרחפן
        public DateTime? PickedUp { get; set; }
        public DateTime? Delivered { get; set; }
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
