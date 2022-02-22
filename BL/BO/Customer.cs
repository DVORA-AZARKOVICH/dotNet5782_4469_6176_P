using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Location Location { get; set; }
        public IEnumerable<ParcelInCustomer> OutgoingParcels { get; set; }// חבילות שנשלחו ע"י הלקוח
        public IEnumerable<ParcelInCustomer> IngoingParcels { get; set; }// חבילות שנשלחו ללקוח
        public string passward { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
