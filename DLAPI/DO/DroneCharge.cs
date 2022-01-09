using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLAPI.DO
{
    public struct Dronecharge
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
