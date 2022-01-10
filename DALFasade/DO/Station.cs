using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public struct Station
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public double Longitude { set; get; }//קו אורך
        public double Latitude { set; get; }//קו רוחב
        public int Chargslot { set; get; }//עמדות הטענה
        public bool Deleted { set; get; }
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
