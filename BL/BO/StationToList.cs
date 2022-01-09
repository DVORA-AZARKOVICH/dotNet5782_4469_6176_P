using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    public class StationToList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FreeChargingSlots { get; set; }
        public int BusyChargingSlots { get; set; }
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
