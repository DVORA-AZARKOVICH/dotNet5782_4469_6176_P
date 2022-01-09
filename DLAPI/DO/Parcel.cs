using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLAPI.DO
{
    public struct Parcel
    {
        public int Id { set; get; }
        public int Senderid { set; get; }
        public int Targetid { set; get; }
        public WeightCategories Weightcategory { set; get; }
        public Priorities Priority { set; get; }
        public DateTime? Requested { set; get; }//date of creating ordering
        public int Droneid { set; get; }
        public DateTime? Scheduled { set; get; }//date of connecting betwean drone and parcel
        public DateTime? Pickedup { set; get; }//picked up to send from the sender
        public DateTime? Delieverd { set; get; }//picked up by the customer
        public bool Deleted { set; get; }
        public override string ToString()
        {
            return this.ToStringProperty();
        }

    }
}
