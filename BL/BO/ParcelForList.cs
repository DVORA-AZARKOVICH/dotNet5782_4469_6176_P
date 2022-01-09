using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    public class ParcelForList
    {
        public int Id { get; set; }
        public String SenderName { get; set; }
        public String ReciverName { get; set; }
        public WeightCategories Weight { get; set; }
        public Priority Priority { get; set; }
        public ParcelStatus Status { get; set; }
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
