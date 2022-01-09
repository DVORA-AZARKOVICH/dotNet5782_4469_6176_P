using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    public class DroneInParcel
    {
        public int Id { get; set; }
        public double BatteryStatus { get; set; }
        public Location CurrentLocation { get; set; }
        public DroneInParcel(int id)
        {
            this.Id = id;
        }
        public DroneInParcel()
        {
        }
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
