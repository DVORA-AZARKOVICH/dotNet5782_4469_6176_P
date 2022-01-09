using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLAPI.DO
{
    public struct Customer
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Phone { set; get; }
        public double Longitude { set; get; }//קו  רוחב
        public double Latitude { set; get; }//קו אורך
        public bool Deleted { set; get; }
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
