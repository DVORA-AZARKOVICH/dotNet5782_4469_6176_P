
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public struct Drone
    {
        public int Id { get; set; }
        public string Model { get; set; }//דגם רחפן
        public WeightCategories Maxweight { get; set; }
        public bool Deleted { set; get; }
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
