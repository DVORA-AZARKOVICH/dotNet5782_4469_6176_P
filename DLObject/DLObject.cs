using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALApi;
using DLObject.Exceptions;
using DO;

namespace Dal
{
    public class DLObject :IDal
    {
        #region singelton
        static readonly DLObject instance = new DLObject();
        static DLObject() { DataSource.Initialize(); }
        public static DLObject Instance { get => instance; }
        #endregion

        #region Customer
        /// <summary>
        /// add a new customer
        /// </summary>
        /// <param name="customer1"></param>
        public void addCustomer(Customer item)
        {
            bool flag = false;
            foreach (var item2 in DataSource.customelist)
            {
                if (item2.Id == item.Id && item.Deleted == false)
                    flag = true;
            }
            if (!flag)
            {
                item.Deleted = false;
                DataSource.customelist.Add(item);
            }
            else
            {
                throw new Exceptions.IdExistException("Costumer is already exist");
            }
        }
        /// <summary>
        /// gives the details of a customer, based on given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Customer getCustomer(int id)
        {
 
           foreach (var item in DataSource.customelist)
            {
                if (item.Id == id && item.Deleted == false)
                    return item;
            }
            throw new Exceptions.IDdNotFoundExeption("Costumer was not found");
        }
        /// <summary>
        /// returns a list with all the existing customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> getCustomerList(Predicate<Customer> predicate)
        {
            return from item in DataSource.customelist
                   where predicate(item)
                   select item;
        }
        public void deleteCustomer(int id)
        {
            bool flag = false;
            for (int i = 0; i < DataSource.customelist.Count(); i++)
            {
                if (DataSource.customelist[i].Id == id)
                {
                    Customer c = DataSource.customelist[i];
                    c.Deleted = true;
                    DataSource.customelist.Remove(DataSource.customelist[i]);
                    DataSource.customelist.Add(c);
                    flag = true;
                    break;
                }
            }
            if (!flag)
                throw new Exceptions.IDdNotFoundExeption("Customer was not found");
        }

        #endregion

        #region Drone
        /// <summary>
        /// add a new drone to the list of existing drones
        /// </summary>
        /// <param name="drone1"></param> 
        public void addDrone(Drone drone)
        {
            bool flag = false;
            foreach (var item2 in DataSource.dronelist)
            {
                if (item2.Id == drone.Id && item2.Deleted == false)
                    flag = true;
            }
            if (!flag)
            {
                drone.Deleted = false;
                DataSource.dronelist.Add(drone);
            }
            else
            {
                throw new Exceptions.IdExistException("Drone is already exist");
            }
        }
        /// <summary>
        /// when given a drone's id,returns information about the drone.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Drone getDrone(int id)
        {
            foreach (var item in DataSource.dronelist.ToArray())
            {
                if (item.Id == id && item.Deleted == false)
                    return item;
            }

            throw new Exceptions.IDdNotFoundExeption("Drone was not found");
        }
        /// <summary>
        /// returns a list with al the existing drones.
        /// </summary>
        /// <returns></returns>

        public IEnumerable<Drone> getDroneList(Predicate<Drone> predicate)
        {

            return from item in DataSource.dronelist
                   where predicate(item)
                   select item;
        }
        /// <summary>
        /// the drone is sent to charging.
        /// </summary>
        /// <param name="numofdrone"></param>
        /// <returns></returns>

        //need to change this function considered the status changes
        public void UpDateCharging(int numofdrone, int numofstation)
        {
            Dronecharge adddronecharge = new Dronecharge();
            bool flag = false;
            foreach (var item in DataSource.dronelist)
            {
                if (item.Id == numofdrone && item.Deleted == false)
                {
                    flag = true;
                    adddronecharge.Droneid = item.Id;
                }
            }
            if (!flag)
                throw new Exceptions.IDdNotFoundExeption("Drone was not found");
            foreach (var item in DataSource.stationlist)
            {
                if (item.Id == numofstation && item.Deleted == false)
                {
                    flag = true;
                    adddronecharge.Stationid = item.Id;
                }
            }
            if (!flag)
                throw new Exceptions.IDdNotFoundExeption("Station was not found");
            addDroneCharge(adddronecharge);

        }
        /// <summary>
        /// after charging, apdates the battery 
        /// </summary>
        /// <param name="numofdrone"></param>
        /// <returns></returns>
        public void UpDatenotcharging(int numofdrone)
        {
            Dronecharge removedronecharge = new Dronecharge();
            bool flag = false;
            foreach (var item in DataSource.dronelist)
            {
                if (item.Id == numofdrone)
                    flag = true;
            }
            if (!flag)
                throw new Exceptions.IDdNotFoundExeption("Drone was not found");
            flag = false;
            Station s = new Station();
            for (int i = 0; i < DataSource.dronechargelist.Count(); i++)
            {
                if (DataSource.dronechargelist[i].Droneid == numofdrone)
                {
                    flag = true;
                    removedronecharge = DataSource.dronechargelist[i];
                    break;
                }
            }
            if (!flag)
                throw new Exceptions.IDdNotFoundExeption("DroneCharge was not found");
            flag = false;
            for (int i = 0; i < DataSource.stationlist.Count(); i++)
            {
                if (DataSource.dronelist[i].Id == numofdrone)
                {
                    Station newstation = DataSource.stationlist[i];
                    //the property of status was deleted.
                    DataSource.stationlist.Remove(DataSource.stationlist[i]);
                    newstation.Chargslot++;
                    DataSource.stationlist.Add(newstation);
                    flag = true;
                }
            }
            if (!flag)
                throw new Exceptions.IDdNotFoundExeption("Station of charging was not found");
            DataSource.dronechargelist.Remove(removedronecharge);

        }
        public double[] GetElectricity()
        {
            double[] describe = new double[5];
            describe[0] = DataSource.Config.available;
            describe[1] = DataSource.Config.light;
            describe[2] = DataSource.Config.medium;
            describe[3] = DataSource.Config.heavy;
            describe[4] = DataSource.Config.chargingRate;
            return describe;
        }
        public void deleteDrone(int id)
        {
            bool flag = false;
            for (int i = 0; i < DataSource.dronelist.Count(); i++)
            {
                if (DataSource.dronelist[i].Id == id)
                {
                    Drone d = DataSource.dronelist[i];
                    d.Deleted = true;
                    DataSource.dronelist.Remove(DataSource.dronelist[i]);
                    DataSource.dronelist.Add(d);
                    flag = true;
                    break;
                }
            }
            if (!flag)
                throw new Exceptions.IDdNotFoundExeption("Drone was not found");
        }
    
    #endregion

        #region DroneCharge
    /// <summary>
    /// add a new charging point for drones
    /// </summary>
    /// <param name="dronechatge1"></param>
    public void addDroneCharge(Dronecharge dronecharge1)
    {
        dronecharge1.Deleted = false;
        DataSource.dronechargelist.Add(dronecharge1);

    }
    public void deleteDroneCharge(int iddrone)
    {
        bool flag = false;
        for (int i = 0; i < DataSource.dronechargelist.Count(); i++)
        {
            if (DataSource.dronechargelist[i].Droneid == iddrone)
            {
                Dronecharge dc = DataSource.dronechargelist[i];
                dc.Deleted = true;
                DataSource.dronechargelist.Remove(DataSource.dronechargelist[i]);
                DataSource.dronechargelist.Add(dc);
                flag = true;
                break;
            }
        }
        if (!flag)
            throw new Exceptions.IDdNotFoundExeption("DroneCharge was not found");
    }
    public IEnumerable<Dronecharge> getDroneChargeList(Predicate<Dronecharge> predicate)
    {
        return from item in DataSource.dronechargelist
               where predicate(item)
               select item;
    }
    public Dronecharge getDroneCharge(int iddrone)
    {
        foreach (var item in DataSource.dronechargelist)
        {
            if (item.Droneid == iddrone && item.Deleted == false)
                return item;
        }
        throw new Exceptions.IDdNotFoundExeption("Dronecharge was not found");
    }

        #endregion

        #region Parcel

        /// <summary>
        /// a new parcel that have been ordered.
        /// </summary>
        /// <param name="parcel1"></param>
        /// <returns></returns>
        public string addParcel(Parcel parcel1)
        {
            parcel1.Id = DataSource.Config.idParcel++;
            parcel1.Deleted = false;
            DataSource.parcellist.Add(parcel1);
            return Convert.ToString(parcel1.Id);
        }
        public void updateParcel2(Parcel parcel1)
        {
            parcel1.Id = parcel1.Id;
            parcel1.Deleted = false;
            DataSource.parcellist.Add(parcel1);
        }
        /// <summary>
        /// when given parcel id, returns the details of a parcel.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Parcel getParcel(int id)
        {
            for(int i = 0;i<DataSource.parcellist.Count();i++)
           // foreach (var item in DataSource.parcellist)
            {
                if (DataSource.parcellist[i].Id == id && DataSource.parcellist[i].Deleted == false)
                    return DataSource.parcellist[i];
            }
            throw new Exceptions.IDdNotFoundExeption("Parcel was not found");
        }
        /// <summary>
        /// return the list with all of the parcels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> getParcelList(Predicate<Parcel> predicate)
        {
            return from item in DataSource.parcellist
                   where predicate(item)
                   select item;
        }
        /// <summary>
        /// returns the parcel to main to update the schedule date 
        /// </summary>
        /// <param name="numofparcel"></param>
        /// <returns></returns>
        public void UpDateSceduled(int numofparcel, int droneId)
        {
            bool flag1 = false;
            bool flag2 = false;
            for (int i = 0; i < DataSource.dronelist.Count(); i++)
            {
                if (DataSource.dronelist[i].Id == droneId)
                {
                    flag2 = true;
                    i = DataSource.dronelist.Count();
                }
            }
            if (!flag2)
            {
                throw new Exceptions.IDdNotFoundExeption("Drone was not found");
            }
            for (int i = 0; i < DataSource.parcellist.Count(); i++)
            {
                if (DataSource.parcellist[i].Id == numofparcel)
                {
                    Parcel newparcel = DataSource.parcellist[i];
                    newparcel.Scheduled = DateTime.Now;
                    newparcel.Droneid = droneId;
                    DataSource.parcellist.Remove(DataSource.parcellist[i]);
                    DataSource.parcellist.Add(newparcel);
                    i = DataSource.parcellist.Count();
                    flag1 = true;
                }
            }
            if (!flag1)
            {
                throw new Exceptions.IDdNotFoundExeption("Parcel was not found");
            }
        }
        /// <summary>
        /// update when the parcel has been picked up.
        /// </summary>
        /// <param name="numofparcel"></param>
        /// <returns></returns>
        public void UpdatePickedUp(int numofparcel)
        {
            bool flag = false;
            for (int i = 0; i < DataSource.parcellist.Count(); i++)
            {
                if (DataSource.parcellist[i].Id == numofparcel)
                {
                    Parcel newparcel = DataSource.parcellist[i];
                    newparcel.Pickedup = DateTime.Now;
                    DataSource.parcellist.Remove(DataSource.parcellist[i]);
                    DataSource.parcellist.Add(newparcel);
                    i = DataSource.parcellist.Count();
                    flag = true;
                }
            }
            if (!flag)
            {
                throw new Exceptions.IDdNotFoundExeption("Parcel was not found");
            }
        }
        /// <summary>
        /// after the parcel is delievered, updates day of delievery.
        /// </summary>
        /// <param name="numofparcel"></param>
        /// <returns></returns>
        public void UpDateDelieverd(int numofparcel)
        {
            bool flag = false;
            for (int i = 0; i < DataSource.parcellist.Count(); i++)
            {
                if (DataSource.parcellist[i].Id == numofparcel)
                {
                    Parcel newparcel = DataSource.parcellist[i];
                    newparcel.Delieverd = DateTime.Now;
                    DataSource.parcellist.Remove(DataSource.parcellist[i]);
                    DataSource.parcellist.Add(newparcel);
                    i = DataSource.parcellist.Count();
                    flag = true;
                }
            }
            if (!flag)
            {
                throw new Exceptions.IDdNotFoundExeption("Parcel was not found");
            }
        }
        /// <summary>
        /// create a list with all of the parcels that don't have a drone.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> getParcelsWithNoDrone()
        {
            List<Parcel> parcelsWithNoDrone = new List<Parcel>();
            foreach (var item in DataSource.parcellist)
            {
                if (item.Droneid == 0)
                    parcelsWithNoDrone.Add(item);
            }
            return parcelsWithNoDrone;
        }
        public void deleteParcel(int id)
        {
            bool flag = false;
            for (int i = 0; i < DataSource.parcellist.Count(); i++)
            {
                if (DataSource.parcellist[i].Id == id)
                {
                    Parcel p = DataSource.parcellist[i];
                    p.Deleted = true;
                    DataSource.parcellist.Remove(DataSource.parcellist[i]);
                    DataSource.parcellist.Add(p);
                    flag = true;
                    break;
                }
            }
            if (!flag)
                throw new Exceptions.IDdNotFoundExeption("Parcel was not found");
        }
        #endregion

        #region Station

        /// <summary>
        /// add a new station
        /// </summary>
        /// <param name="station1"></the station to add>
        public void addStation(Station station1)
        {
            foreach (var item in DataSource.stationlist)
            {
                if (item.Id == station1.Id && item.Deleted == false)
                    throw new Exceptions.IdExistException("Station already exists");
            }
            station1.Deleted = false;
            DataSource.stationlist.Add(station1);
        }
        /// <summary>
        /// gets an id number of a station and gives the details about the station.
        /// </summary>
        /// <param name="id"></the id of the station>
        /// <returns></returns>
        public Station getStation(int id)
        {
            foreach (var item in DataSource.stationlist)
            {
                if (item.Id == id && item.Deleted == false)
                    return item;
            }
            throw new Exceptions.IDdNotFoundExeption("Station was not found");
        }
        /// <summary>
        /// returns the station list.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Station> getStationList(Predicate<Station> predicate)
        {
            return from item in DataSource.stationlist
                   where predicate(item)
                   select item;
        }
        /// <summary>
        /// returns a list with al the station that have free charging slots.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Station> getFreeCharges()
        {
            List<Station> freeCharges = new List<Station>();
            foreach (var item in DataSource.stationlist)
            {
                if (item.Chargslot != 1)
                    freeCharges.Add(item);
            }
            return freeCharges;
        }
        public void deleteStation(int id)
        {
            bool flag = false;
            for (int i = 0; i < DataSource.stationlist.Count(); i++)
            {
                if (DataSource.stationlist[i].Id == id && DataSource.stationlist[i].Deleted == false)
                {
                    Station s = DataSource.stationlist[i];
                    s.Deleted = true;
                    DataSource.stationlist.Remove(DataSource.stationlist[i]);
                    DataSource.stationlist.Add(s);
                    flag = true;
                    break;
                }
            }
            if (!flag)
                throw new Exceptions.IDdNotFoundExeption("Station was not found");
        }
        #endregion

        #region Sexagesimal
        private string Convert_To_Sexagesimal(double value)
        {
            int degris = (int)value;//the int part
            value = value - degris;//seperating the double part
            int minutes = (int)(value * 60);//mult it by 60 and taking the integer
            double seconds = value * 60 - minutes;//seperating the double part
            seconds = seconds * 60;//multiple the last part in 60 - can remane in decimal.
            string str = Convert.ToString(degris) + '°' + Convert.ToString(minutes) + "'" + Convert.ToString(seconds) + "''";
            return str;

        }
        public  string ConvertLongitude(double value)
        {
            string str = Convert_To_Sexagesimal(value);
            return str + "S";

        }
        public  string ConvertLatitude(double value)
        {
            string str = Convert_To_Sexagesimal(value);
            return str + "E";

        }
        public  double DistanceStation(int numofstation, double x1, double y1)
        {
            double x2 = DistanceStationLAT(numofstation);
            double y2 = DistanceStationLONG(numofstation);
            double distance = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            return distance;
        }
        public  double DistanceCustomer(int numofcustomer, double x1, double y1)
        {
            double x2 = DistanceCustomerLAT(numofcustomer);
            double y2 = DistanceCustomerLONG(numofcustomer);
            double distance = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            return distance;
        }
        public  double DistanceStationLAT(int numofstation)
        {
            foreach (var item in DataSource.stationlist)
            {
                if (item.Id == numofstation)
                {
                    return item.Latitude;
                }
            }
            throw new Exceptions.IDdNotFoundExeption("Station was not found");

        }
        public  double DistanceStationLONG(int numofstation)
        {
            foreach (var item in DataSource.stationlist)
            {
                if (item.Id == numofstation)
                {
                    return item.Longitude;
                }
            }
            throw new Exceptions.IDdNotFoundExeption("Station was not found");
        }
        public  double DistanceCustomerLAT(int numofcustomer)
        {
            foreach (var item in DataSource.customelist)
            {
                if (item.Id == numofcustomer)
                {
                    return item.Latitude;
                }
            }
            throw new Exceptions.IDdNotFoundExeption("Customer was not found");
        }
        public  double DistanceCustomerLONG(int numofcustomer)
        {
            foreach (var item in DataSource.customelist)
            {
                if (item.Id == numofcustomer)
                {
                    return item.Longitude;
                }
            }
            throw new Exceptions.IDdNotFoundExeption("Customer was not found");
        }
        #endregion


    }
}
