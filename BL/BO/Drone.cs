using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    public class Drone
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public WeightCategories Weight { get; set; }
        public double BatteryStatus { get; set; }
        public DroneStatus Status { get; set; }
        public ParcelInTransfer TheParcel { get; set; }
        public Location CurrentLocation { get; set; }
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
