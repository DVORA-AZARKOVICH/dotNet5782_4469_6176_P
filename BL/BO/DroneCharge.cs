using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    public class DroneCharge
    {
        public int Id { get; set; }
        public double BatteryStatus { get; set; }
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
