using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{
    public enum DroneStatus { free, inMaintence, busy }

    public enum WeightCategories { light, medium, heavy }

    public enum Priority { regular, fast, urgent }

    public enum ParcelStatus { Created, attached, pickedUp, deliverd }
}
