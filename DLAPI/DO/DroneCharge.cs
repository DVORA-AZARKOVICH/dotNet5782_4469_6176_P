using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    namespace DO
    {
        public struct DroneCharge
        {
            public int Droneid { set; get; }
            public int Stationid { set; get; }
            public bool Deleted { set; get; }
            public override string ToString()
            {
                return this.ToStringProperty();
            }
        }

    }
}
