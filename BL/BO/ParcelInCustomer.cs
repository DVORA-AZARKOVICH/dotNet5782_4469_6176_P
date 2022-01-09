using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    public class ParcelInCustomer
    {
        public int Id { get; set; }
        public WeightCategories Weight { get; set; }
        public Priority Priority { get; set; }
        public ParcelStatus Status { get; set; }
        public CustomerInParcel OtherCustomer { get; set; }//המקבל עבור השולח והשולח עבור המקבל
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
