using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    public class Location
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public Location(double latitude, double longitude)
        {
           
            //if (latitude < 33.7 || latitude > 36.3 || longitude < 29.3 || longitude > 33.5)
              //  throw new Exceptions.ExceptionExceptionExceedingPossibleQuantity("ERROR -The borders are out of Israel plees try again");
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
