using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }
        public int Chargslot { get; set; }
        public IEnumerable<DroneCharge> DronesInCharging { get; set; }
        public override string ToString()
        {
            string str = " ";
            foreach (var drone in DronesInCharging)
            {
                str += '\n' + drone.ToString();
            }
            string str2 = '\n' + "Id:" + Id + '\n' + "Name:" + Name + '\n' + "Location:" + Location + '\n' + "Amount of charging slots:" + Chargslot + "Drones in charging" + str;
            return str2;
        }
    }
}
