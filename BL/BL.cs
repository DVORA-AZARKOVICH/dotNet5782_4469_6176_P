using BL.BO;
using BL.Exceptions;
using BLApi;
using DALApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALApi;
using DO;
using DLObject;
using DALFasade;
using BL.BO;

namespace BL
    {
        sealed class BL : IBL
        {
            #region Constructor

            static readonly BLApi.IBL instance = new BL();
            public static IBL Instance { get => instance; }

            internal IDal dalob = DALFasade.DALApi.DalFactory.GetDal();
            BL()
            {
                this.Initilize();
            }
            //    private IDL dalob = new DL.DLObjectt();
            public Random rand = new Random();
            public List<DroneToList> dronetolistBL = new List<DroneToList>();
            public double[] ob = new double[5];
        

            public IDal Dalob { get => dalob; set => dalob = value; }
            public void Initilize()
            {
                ob = Dalob.PowerConsumptionRequest();
                double available = ob[0];
                double light = ob[1];
                double medium = ob[2];
                double heavy = ob[3];
                double chargingRate = ob[4];
                //converting the DAL drone list to BL list
                IEnumerable<DroneToList> temp;
                temp = from item in Dalob.getDroneList(item => item.Deleted == false)
                       select new BO.DroneToList()
                       {
                           Id = item.Id,
                           Model = item.Model,
                           Weight = (BO.WeightCategories)item.Maxweight


                       };

                dronetolistBL = temp.ToList();
                var parcellist = Dalob.getParcelList(item => item.Deleted == false);
                var stationlist = Dalob.getStationList(item => item.Deleted == false);
                var customerlist = Dalob.getCustomerList(item => item.Deleted == false);
                //Goes through all the packages and updates data accordingly
                foreach (var itemparcel in parcellist)
                {

                    //checking if the customer - sender and customer target of parcel exist case not throw exception
                    bool flag1 = true, flag2 = true;
                    try
                    {
                        DLAPI.DO.Customer c = Dalob.getCustomer(itemparcel.Targetid);
                    }
                    catch (Exception ex)
                    {
                        flag1 = false;
                        throw new Exceptions.myExceptions.itemNotFoundExceptions(ex.Message, ex);
                    }
                    try
                    {
                        DLAPI.DO.Customer c = Dalob.getCustomer(itemparcel.Senderid);
                    }
                    catch (Exception ex)
                    {
                        flag2 = false;
                        throw new myExceptions.itemNotFoundExceptions(ex.Message, ex);
                    }
                    if (flag1 && flag2)
                    {
                        //In case the parcel was conect to a drone byut has not yet been delivered to the customer
                        if (itemparcel.Droneid != 0 && itemparcel.Delieverd == null)
                        {
                            foreach (var updatedrone in dronetolistBL)
                            {
                                //finding the drone in the list and updating
                                if (updatedrone.Id == itemparcel.Droneid)
                                {
                                    updatedrone.Status = DroneStatus.busy;
                                    int idsender = itemparcel.Senderid;//the id of the sender
                                                                       //searching for the sender to know its location for the calculation.
                                    try
                                    {
                                        DO.Customer sender = Dalob.getCustomer(itemparcel.Senderid);
                                        Location location = new Location(sender.Latitude, sender.Longitude);
                                        //case the parcel wasnt picked up yet
                                        if (itemparcel.Pickedup == null)
                                        {
                                            DO.Station? s = ClosestStation(stationlist, location);
                                            if (s != null)
                                            {
                                                Location l = new Location(s.Value.Latitude, s.Value.Longitude);
                                                updatedrone.Location = l;

                                            }
                                            else
                                                throw new myExceptions.itemNotFoundExceptions("Match station was not found");//hapens when the list of station is empty

                                        }
                                        //case the parcel did picked up but hasnt got to the target yet
                                        else if (itemparcel.Pickedup != null && itemparcel.Delieverd == null)
                                        {
                                            Location droneLocation = new Location(sender.Latitude, sender.Longitude);
                                            updatedrone.Location = droneLocation;
                                            //updatedrone.Location.Latitude = sender.Latitude;
                                            //updatedrone.Location.Longitude = sender.Latitude;
                                        }
                                        //distance betwean drone and sender
                                        DO.Parcel? p = itemparcel;
                                        double minrateofcharge = SumCharge(p, updatedrone);
                                        if (minrateofcharge < 100)
                                        {
                                            updatedrone.BatteryStatus = minrateofcharge + rand.NextDouble() * 100;
                                        }
                                        else if (minrateofcharge == 100)
                                        {
                                            updatedrone.BatteryStatus = 100;
                                        }
                                        else
                                        {
                                            throw new Exceptions.ExceptionExceptionExceedingPossibleQuantity("The drone can stand the codt of the buttery for this shift");

                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        throw new myExceptions.itemNotFoundExceptions(ex.Message);
                                    }



                                }

                            }

                        }
                    }


                }
                foreach (var item in dronetolistBL.ToArray())
                {
                    if (item.Status != DroneStatus.busy)
                    {
                        item.Status = (DroneStatus)new Random().Next((int)DroneStatus.inMaintence);
                    }
                }
                //               dronetolistBL[4].Status = DroneStatus.inMaintence;
                foreach (var item in dronetolistBL.ToArray())
                {
                    if (item.Status == DroneStatus.inMaintence)
                    {
                        DroneToList t = new DroneToList();
                        t = item;
                        t.Location = new Location(item.Location.Latitude, item.Location.Longitude);
                        t = UpdateInMaintence(stationlist.Count(), t, stationlist);

                    }
                }

                // dronetolistBL.Remove(item);
                // dronetolistBL.Add(t);
                var v = parcellist.Where(item => item.Delieverd != null);
                foreach (var item in dronetolistBL.ToArray())
                {
                    if (item.Status == DroneStatus.free)
                    {
                        DroneToList t = new DroneToList();
                        t = item;
                        t.Location = new Location(34, 31);
                        t = UpdateFree(t, v, customerlist, stationlist);
                        dronetolistBL.Remove(item);
                        dronetolistBL.Add(t);
                    }

                }
                var tt = parcellist.ToList();
                for (int i = 0; i < tt.Count(); i++)
                {
                    if (tt[i].Droneid != 0)
                    {
                        foreach (var item in dronetolistBL)
                            if (item.Id == tt[i].Droneid)
                            {
                                if (item.Status == DroneStatus.busy)
                                    item.ParcelId = tt[i].Id;
                                else
                                {
                                    DO.Parcel p = Dalob.getParcel(tt[i].Id);
                                    p.Droneid = 0;
                                    Dalob.deleteParcel(p.Id);
                                    Dalob.addParcel(p);
                                }
                            }

                    }
                }

            }

            #endregion

            #region Customer
            public void AddCustomer(BO.Customer c)
            {
                DO.Customer newcustomer = new DO.Customer();
                if (CheckIdentityNumber(c.Id))
                {
                    newcustomer.Id = c.Id;
                    newcustomer.Name = c.Name;
                    try
                    {
                        Location p = new Location(c.Location.Latitude, c.Location.Longitude);
                        newcustomer.Latitude = c.Location.Latitude;
                        newcustomer.Longitude = c.Location.Longitude;
                        newcustomer.Phone = c.PhoneNumber;
                        try
                        {
                            Dalob.addCustomer(newcustomer);
                        }
                        catch (Exception ex)
                        {
                            throw new Exceptions.myExceptions.itemAllreadyExistException(ex.Message, ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exceptions.ExceptionExceptionExceedingPossibleQuantity(ex.Message, ex);
                    }
                }
                else
                    throw new Exceptions.ExceptionExceptionExceedingPossibleQuantity("This certificate number is invalid");

            }
            public void UpdateCustomerName(int numofcustomer, string name)
            {
                try
                {
                    DO.Customer updatecustomer = new DO.Customer();
                    updatecustomer = Dalob.getCustomer(numofcustomer);
                    Dalob.deleteCustomer(numofcustomer);
                    updatecustomer.Name = name;
                    Dalob.addCustomer(updatecustomer);
                }
                catch (Exception ex)
                {
                    throw new Exceptions.myExceptions.itemNotFoundExceptions(ex.Message, ex);
                }
            }
            public void UpdateCustomerPhone(int numofcustomer, string phone)
            {
                if (!ChecksPhoneNumber(phone))
                    throw new Exception("Phone number is incorrect");
                try
                {
                    DO.Customer updatecustomer = new DO.Customer();
                    updatecustomer = Dalob.getCustomer(numofcustomer);
                    Dalob.deleteCustomer(numofcustomer);
                    updatecustomer.Phone = phone;
                    Dalob.addCustomer(updatecustomer);
                }
                catch (Exception ex)
                {
                    throw new Exceptions.myExceptions.itemNotFoundExceptions(ex.Message, ex);
                }
            }
            public BO.Customer getCustomer(int num)
            {

                DO.Customer c = Dalob.getCustomer(num);
                var Ingoing = from parcel in Dalob.getParcelList(item => item.Senderid == c.Id && item.Deleted == false)
                                  //where parcel.Senderid == c.Id
                              select new ParcelInCustomer { Id = parcel.Id, Priority = (Priority)parcel.Priority, Weight = (WeightCategories)parcel.Weightcategory };
                foreach (var item in Ingoing)
                {
                    // var indexParcel = dalob.getParcelList(c => c.Id == item.Id);
                    var indexParcel = Dalob.getParcelList(item => item.Deleted == false).ToList().FindIndex(c => c.Id == item.Id);
                    var par = Dalob.getParcelList(item => item.Deleted == false).ToList().ElementAt(indexParcel);
                    if (par.Delieverd != null)
                        item.Status = ParcelStatus.deliverd;
                    else if (par.Pickedup != null)
                        item.Status = ParcelStatus.pickedUp;
                    else if (par.Scheduled != null)
                        item.Status = ParcelStatus.attached;
                    else if (par.Requested != null)
                        item.Status = ParcelStatus.Created;
                }
                var outgoing = from parcel in Dalob.getParcelList(item => item.Senderid == c.Id)
                                   //where parcel.Senderid == c.Id
                               select new ParcelInCustomer { Id = parcel.Id, Priority = (Priority)parcel.Priority, Weight = (WeightCategories)parcel.Weightcategory };
                foreach (var item in outgoing)
                {
                    var indexParcel = Dalob.getParcelList(item => item.Deleted == false).ToList().FindIndex(c => c.Id == item.Id);
                    var par = Dalob.getParcelList(item => item.Deleted == false).ToList().ElementAt(indexParcel);
                    DateTime date = new DateTime();
                    if (par.Delieverd != null)
                        item.Status = ParcelStatus.deliverd;
                    else if (par.Pickedup != null)
                        item.Status = ParcelStatus.pickedUp;
                    else if (par.Scheduled != null)
                        item.Status = ParcelStatus.attached;
                    else if (par.Requested != null)
                        item.Status = ParcelStatus.Created;
                }
                Customer customer = new Customer()
                {
                    Id = c.Id,
                    Name = c.Name,
                    PhoneNumber = c.Phone,
                    Location = new Location(c.Latitude, c.Longitude),
                    IngoingParcels = Ingoing,
                    OutgoingParcels = outgoing
                };
                return customer;
            }
            public IEnumerable<CustomerForList> getCustomersList()
            {
                var customers = from customer in Dalob.getCustomerList(item => item.Deleted == false)
                                select new CustomerForList()
                                {
                                    Id = customer.Id,
                                    Name = customer.Name,
                                    PhoneNumber = customer.Phone,
                                    SentParcels = 0,
                                    Delivered = 0,
                                    Arrived = 0,
                                    Ordered = 0,
                                };
                DateTime date = new DateTime();
                foreach (var customer in customers)
                {
                    foreach (var parcel in Dalob.getParcelList(item => item.Deleted = false))
                    {
                        if (parcel.Senderid == customer.Id)
                        {
                            if (parcel.Delieverd != date)
                                customer.Delivered++;
                            else if (parcel.Scheduled != date)
                                customer.SentParcels++;
                        }
                        else
                            if (parcel.Targetid == customer.Id)
                        {
                            if (parcel.Delieverd != date)
                                customer.Arrived++;
                            else if (parcel.Scheduled != date)
                                customer.Ordered++;
                        }
                    }
                }
                if (customers.Any())
                    return customers;
                throw new Exceptions.emptyListException("this list is empty");
            }
            #endregion

            #region Drone
            public void AddDrone(BO.Drone d, int idstation)
            {

                d.BatteryStatus = 20 + rand.NextDouble() * 20;
                d.Status = DroneStatus.inMaintence;
                try
                {
                    DLAPI.DO.Station s = Dalob.getStation(idstation);
                    d.CurrentLocation = new Location(s.Latitude, s.Longitude);
                    DLAPI.DO.Drone newdrone = new DLAPI.DO.Drone();
                    newdrone.Id = d.Id;
                    newdrone.Model = d.Model;
                    newdrone.Maxweight = (DLAPI.DO.WeightCategories)d.Weight;
                    try
                    {
                        //adding the drone also to the list of dronetolistBL
                        Dalob.addDrone(newdrone);
                        BO.DroneToList newdronetolist = new BO.DroneToList();
                        newdronetolist.Id = d.Id;
                        newdronetolist.Model = d.Model;
                        newdronetolist.Status = d.Status;
                        newdronetolist.Weight = d.Weight;
                        newdronetolist.Location = d.CurrentLocation;
                        newdronetolist.BatteryStatus = d.BatteryStatus;
                        newdronetolist.ParcelId = 0;
                        dronetolistBL.Add(newdronetolist);
                        Dalob.UpDateCharging(d.Id, s.Id);

                    }
                    //case the drone already exist
                    catch (Exception ex)
                    {
                        throw new Exceptions.myExceptions.itemAllreadyExistException(ex.Message, ex);
                    }
                }
                //case the station is not exist
                catch (Exception ex)
                {
                    throw new myExceptions.itemNotFoundExceptions(ex.Message, ex);
                }



            }
            public BO.DroneToList UpdateInMaintence(int count, BO.DroneToList d, IEnumerable<DO.Station> stationlist)
            //public void UpdateInMaintence(int count, BO.DroneToList d, IEnumerable<IDAL.DO.Station> stationlist)
            {
                //random a station with an free chargslot
                var t = stationlist.ToList();
                int index = 0;
                bool flag = false;
                do
                {
                    index = rand.Next(0, count - 1);
                    if (t[index].Chargslot > 0)
                        flag = true;
                }
                while (!flag);
                //updating the statios number of free charge slots
                DLAPI.DO.Station s = Dalob.getStation(t[index].Id);
                s.Chargslot--;
                Dalob.deleteStation(s.Id);
                Dalob.addStation(s);
                //updation the drone's location
                dronetolistBL.Remove(d);
                d.Location = new Location(t[index].Latitude, t[index].Longitude);
                //adding a dronecharge variable
                Dalob.UpDateCharging(d.Id, t[index].Id);
                //random the battery amount of the drone
                d.BatteryStatus = 0 + rand.NextDouble() * 20;
                dronetolistBL.Add(d);
                return d;
            }
            public BO.DroneToList UpdateFree(BO.DroneToList d, IEnumerable<DO.Parcel> parcellist, IEnumerable<DO.Customer> customerlist, IEnumerable<DO.Station> stationlist)
            {
                Random rand = new Random();
                int index = rand.Next(0, parcellist.Count() - 1);
                var t = parcellist.ToList();
                int idcustomer = t[index].Targetid;
                try
                {
                    DLAPI.DO.Customer? c = Dalob.getCustomer(idcustomer);
                    d.Location.Longitude = c.Value.Longitude;
                    d.Location.Latitude = c.Value.Latitude;
                }
                catch (Exception ex)
                {
                    throw new myExceptions.itemNotFoundExceptions(ex.Message, ex);
                }

                double mindistance = 0;
                DLAPI.DO.Station? s = ClosestStation(stationlist, d.Location);
                if (s != null)
                {
                    mindistance = dalob.DistanceStation(s.Value.Id, d.Location.Latitude, d.Location.Longitude);
                    double mincharge1 = MinCharge(mindistance, d.Weight);
                    if (mincharge1 > 100)
                        throw new Exceptions.ExceptionExceptionExceedingPossibleQuantity("There is not inough battery for this flight");
                    if (mincharge1 == 100)
                        d.BatteryStatus = 100;
                    else
                    {
                        double battury = rand.Next((int)mincharge1, 100);
                        d.BatteryStatus = battury;
                    }
                }
                else
                {
                    throw new Exceptions.emptyListException("Empty list of Stations");
                }
                return d;
            }
            public void UpdateDrone(int numofdrone, string model)
            {
                try
                {
                    DLAPI.DO.Drone updatedrone = Dalob.getDrone(numofdrone);
                    Dalob.deleteDrone(numofdrone);
                    updatedrone.Model = model;
                    Dalob.addDrone(updatedrone);

                }
                catch (Exception ex)
                {
                    throw new myExceptions.itemNotFoundExceptions(ex.Message, ex);
                }
                var t = dronetolistBL.Find(item => item.Id == numofdrone);
                if (t != null)
                {
                    dronetolistBL.Remove(t);
                    t.Model = model;
                    dronetolistBL.Add(t);
                }
                else
                    throw new myExceptions.itemNotFoundExceptions("DroneToLIST was not found");
                //   BO.DroneToList updatedronetolist = (DroneToList)dronetolistBL.Where(item => item.Id == numofdrone).Select(item => item.Model = model);
                //   if(updatedronetolist is null)
                //      throw new myExceptions.itemNotFound
                //
                //      Exceptions("DroneToLIST was not found");

            }
            public void UpdateDroneToCharge(int numofdrone)

            {
                try
                {
                    DLAPI.DO.Drone updatedrone = Dalob.getDrone(numofdrone);
                    BO.DroneToList dronetolist = null;
                    dronetolist = dronetolistBL.Find(item => item.Id == numofdrone && item.Status == DroneStatus.free);
                    if (dronetolist is null)
                        throw new Exception("the Done is not free to deliver to charge");//הרחפן לא פנוי אז אי אפשר לשלוח אותו לטעינה
                    IEnumerable<DLAPI.DO.Station> stationlist = Dalob.getStationList(item => item.Deleted == false);
                    List<DLAPI.DO.Station> t = stationlist.ToList();
                    Location c = new Location(dronetolist.Location.Latitude, dronetolist.Location.Longitude);
                    bool succeed = false;
                    double mindistance = 0;
                    DLAPI.DO.Station s;
                    if (t is not null && c is not null)
                    {
                        do
                        {
                            s = ClosestStation(stationlist, c);
                            if (s.Chargslot > 0)
                            {
                                mindistance = dalob.DistanceStation(s.Id, dronetolist.Location.Latitude, dronetolist.Location.Longitude);
                                double min = MinCharge(mindistance, dronetolist.Weight);
                                if (min > dronetolist.BatteryStatus)
                                    throw new Exceptions.ExceptionExceptionExceedingPossibleQuantity("There is not inough battery for moving to the closest station");
                                else
                                    succeed = true;

                            }
                            else
                            {
                                t.Remove(s);
                            }
                        }
                        while (!succeed && t is not null);
                        if (!succeed)
                            throw new Exception("There were no free charge slots at any station");
                        else
                        {

                            dronetolistBL.Remove(dronetolist);
                            //updates the BL drone
                            dronetolist.BatteryStatus = MinCharge(mindistance, dronetolist.Weight);
                            dronetolist.Location.Latitude = s.Latitude;
                            dronetolist.Location.Longitude = s.Longitude;
                            dronetolist.Status = DroneStatus.inMaintence;
                            dronetolistBL.Add(dronetolist);
                            //updates the DAL station in less one free charge slots
                            s.Chargslot--;
                            Dalob.deleteStation(s.Id);
                            s.Deleted = false;
                            Dalob.addStation(s);
                            //IDAL.DO.Dronecharge newdronecharge = new IDAL.DO.Dronecharge();
                            //newdronecharge.Droneid = dronetolist.Id;
                            //newdronecharge.Stationid = s.Id;
                            Dalob.UpDateCharging(numofdrone, s.Id);


                        }
                    }
                    else
                    {
                        throw new Exceptions.emptyListException("The list of station is empty");
                    }


                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }



            }
            public void UpdateReleseDroneFromCharge(int numofdrone, DateTime? time1)
            {
                var t = this.dronetolistBL.Find(item => item.Id == numofdrone && item.Status == DroneStatus.inMaintence);
                if (t is not null && time1 is not null)
                {
                    DroneToList d = t;
                    int sum = time1.Value.Hour * 60 * 60 + time1.Value.Minute * 60 + time1.Value.Second;
                    d.BatteryStatus = sum * ob[4];//לבדוק את זה
                    if (d.BatteryStatus > 100)
                        d.BatteryStatus = 100;
                    d.Status = DroneStatus.free;
                    try
                    {
                        //deletes the drone charge node in database
                        DLAPI.DO.Dronecharge dronecharge = Dalob.getDroneCharge(numofdrone);
                        Dalob.deleteDroneCharge(dronecharge.Droneid);
                        try
                        {
                            //updates the station
                            DLAPI.DO.Station stations = Dalob.getStation(dronecharge.Stationid);
                            Dalob.deleteStation(dronecharge.Stationid);
                            DLAPI.DO.Station s = new DLAPI.DO.Station();
                            s = stations;
                            s.Chargslot++;
                            Dalob.addStation(s);
                            Dalob.deleteDroneCharge(numofdrone);

                        }
                        catch (Exception ex)
                        {
                            throw new myExceptions.itemNotFoundExceptions(ex.Message, ex);
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new myExceptions.itemNotFoundExceptions(ex.Message, ex);
                    }
                }
                else
                    throw new myExceptions.itemNotFoundExceptions("Drone was not found in maintence");
            }
            public void UpdateParcelToDrone(int numofdrone)
            {
                try
                {
                    DLAPI.DO.Drone d = Dalob.getDrone(numofdrone);
                    var t = dronetolistBL.ToList();
                    DroneToList dts = t.Find(item => item.Id == numofdrone);
                    // DroneToList dts = dronetolistBL.Find(item => item.Id == numofdrone);
                    if (dts != null)
                    {
                        if (dts.Status == DroneStatus.free)
                        {
                            //updating the distance the drone can fly according to its battery
                            double maxdistance = 0;
                            if (dts.Weight == WeightCategories.light)
                                maxdistance = dts.BatteryStatus / ob[1];
                            if (dts.Weight == WeightCategories.medium)
                                maxdistance = dts.BatteryStatus / ob[2];
                            if (dts.Weight == WeightCategories.heavy)
                                maxdistance = dts.BatteryStatus / ob[3];
                            var parcell = Dalob.getParcelList(item => item.Deleted == false).Where(item => item.Scheduled == null && SumCharge(item, dts) <= dts.BatteryStatus);
                            //seperating the list DAL parcels to differnt lists according to their priority
                            var parcellisturgent = parcell.Where(item => (Priority)item.Priority == Priority.urgent);
                            var parcellistfast = parcell.Where(item => (Priority)item.Priority == Priority.fast);
                            var parcellistregular = parcell.Where(item => (Priority)item.Priority == Priority.regular);
                            //the location of this drone
                            Location c = new Location(dts.Location.Latitude, dts.Location.Longitude);
                            DLAPI.DO.Parcel? p = CheckByWeight(parcellisturgent, dts.Weight, c, maxdistance, dts);
                            if (p is null)
                                p = CheckByWeight(parcellistfast, dts.Weight, c, maxdistance, dts);
                            if (p is null)
                                p = CheckByWeight(parcellistregular, dts.Weight, c, maxdistance, dts);
                            if (p is null)
                                throw new Exceptions.ExceptionExceptionExceedingPossibleQuantity("there is no match parcel");
                            //אם זה הפניה לא צריך להסיר?
                            dronetolistBL.Remove(dts);
                            dts.ParcelId = p.Value.Id;
                            DLAPI.DO.Parcel pp = Dalob.getParcel(p.Value.Id);
                            pp.Scheduled = DateTime.Now;
                            Dalob.deleteParcel(pp.Id);
                            Dalob.updateParcel2(pp);
                            dronetolistBL.Add(dts);
                        }
                    }
                    else
                        throw new myExceptions.itemNotFoundExceptions("DroneToList was not found");
                }
                catch (Exception ex)
                {
                    throw new myExceptions.itemNotFoundExceptions(ex.Message, ex);
                }

            }
            public BO.Drone getDrone(int num)
            {
                Drone d = new();
                foreach (var item in dronetolistBL)
                {
                    if (item.Id == num)
                    {
                        d.Id = num;
                        d.Model = item.Model;
                        d.Weight = item.Weight;
                        d.BatteryStatus = item.BatteryStatus;
                        d.Status = item.Status;
                        if (item.ParcelId != 0)
                            d.TheParcel = convertToParcelInTransfer(Dalob.getParcel(item.ParcelId));
                        d.CurrentLocation = item.Location;
                        break;
                    }
                }
                return d;
            }
            public ParcelInTransfer convertToParcelInTransfer(DO.Parcel p)
            {

                ParcelInTransfer parcel = new()
                {
                    Id = p.Id,
                    Sender = new CustomerInParcel(Dalob.getCustomer(p.Senderid).Id, Dalob.getCustomer(p.Senderid).Name),
                    Reciver = new CustomerInParcel(Dalob.getCustomer(p.Targetid).Id, Dalob.getCustomer(p.Targetid).Name),
                    Weight = (WeightCategories)p.Weightcategory,
                    Destination = new Location(Dalob.getCustomer(p.Targetid).Latitude, Dalob.getCustomer(p.Targetid).Longitude),
                    Pickup = new Location(Dalob.getCustomer(p.Senderid).Latitude, Dalob.getCustomer(p.Senderid).Longitude),
                };
                parcel.Distance = Math.Sqrt(Math.Pow(parcel.Pickup.Latitude - parcel.Destination.Latitude, 2) + Math.Pow(parcel.Pickup.Longitude - parcel.Destination.Longitude, 2));
                return parcel;
            }
            public List<DroneToList> getdroneList()
            {
                // var drones = from drone in dalob.getDroneList(station => station.Deleted == false)
                //               select convertTodroneInParcel(drone);
                if (this.dronetolistBL.Any())
                {
                    var t = dronetolistBL.ToList();
                    return t;
                }
                throw new Exceptions.emptyListException("Drone list is empty");
            }
            public IEnumerable<DroneToList> getdroneList(Predicate<DroneToList> predicate)
            {
                var drones = from item in dronetolistBL
                             where predicate(item)
                             select item;
                if (drones.Any())
                    return drones;
                return drones;
                //throw new Exceptions.emptyListException("there are no drones of this type");
            }
            #endregion

            #region Station
            public void AddStation(BO.Station s)
            {
                s.DronesInCharging = null;
                DLAPI.DO.Station newstation = new DLAPI.DO.Station();
                newstation.Id = s.Id;
                newstation.Name = s.Name;
                newstation.Latitude = s.Location.Latitude;
                newstation.Longitude = s.Location.Longitude;
                newstation.Chargslot = s.Chargslot;
                try
                {
                    Dalob.addStation(newstation);
                }
                catch (Exception ex)
                {
                    throw new Exceptions.myExceptions.itemAllreadyExistException(ex.Message, ex);
                }

            }
            public DO.Station ClosestStation(IEnumerable<DO.Station> stationlist, Location c)
            {
                double mindistance = 2000000, temp = 0;
                DLAPI.DO.Station s = new DLAPI.DO.Station();
                foreach (var item in stationlist)
                {
                    temp = dalob.DistanceStation(item.Id, c.Latitude, c.Longitude);
                    if (temp < mindistance)
                    {
                        mindistance = temp;
                        s = item;
                    }
                }
                return s;
            }
            public void UpdatStationChargeslots(int numofstation, int numofchargeslots)
            {
                try
                {
                    DLAPI.DO.Station updatestation = new DLAPI.DO.Station();
                    updatestation = Dalob.getStation(numofstation);
                    int x = dronetolistBL.Where(item => (item.Status == DroneStatus.inMaintence) && (item.Location.Longitude == updatestation.Longitude) && (item.Location.Latitude == updatestation.Latitude)).Count();
                    numofchargeslots = numofchargeslots - x;
                    if (numofchargeslots < 0)
                        throw new Exception("not enough charge slots - extending amount");//אם ישנו את מספר עמדות הטעינה זה לא תקין כי הפנויות יהיו מינוס והמינימום הוא0
                    else
                    {
                        Dalob.deleteStation(numofstation);
                        updatestation.Chargslot = numofchargeslots;
                        Dalob.addStation(updatestation);

                    }
                }
                catch (Exception ex)
                {
                    throw new Exceptions.myExceptions.itemNotFoundExceptions(ex.Message, ex);
                }

            }
            public void UpdateStationName(int numofstation, string name)
            {
                try
                {
                    DLAPI.DO.Station updatestation = new DLAPI.DO.Station();
                    updatestation = Dalob.getStation(numofstation);
                    Dalob.deleteStation(numofstation);
                    updatestation.Name = name;
                    Dalob.addStation(updatestation);
                }
                catch (Exception ex)
                {
                    throw new Exceptions.myExceptions.itemNotFoundExceptions(ex.Message, ex);
                }


            }
            public BO.Station getStation(int num)
            {
                DLAPI.DO.Station st = Dalob.getStation(num);
                var Drones = from drone in Dalob.getDroneChargeList(drone => drone.Stationid == st.Id)
                                 //where drone.Stationid == st.Id
                             select new DroneCharge() { Id = drone.Droneid };
                foreach (var item in Drones)
                {
                    foreach (var drone in dronetolistBL)
                        if (item.Id == drone.Id)
                        {
                            item.BatteryStatus = drone.BatteryStatus;
                            break;
                        }
                }
                Station s = new Station()
                {
                    Id = st.Id,
                    Name = st.Name,
                    Location = new Location(st.Latitude, st.Longitude),
                    Chargslot = st.Chargslot,
                    DronesInCharging = Drones,
                };
                return s;
            }
            public IEnumerable<StationToList> getStationList()
            {
                var stations = from station in Dalob.getStationList(station => station.Deleted == false)
                               select convertToStationList(station);
                if (stations.Any())
                    return stations;
                throw new Exceptions.emptyListException("Station list is empty");
            }
            public StationToList convertToStationList(<DO.Station s)
            {
                StationToList station = new StationToList
                {
                    Id = s.Id,
                    Name = s.Name,
                    FreeChargingSlots = s.Chargslot,
                };
                int busySlots = 0;
                foreach (var item in Dalob.getDroneChargeList(item => item.Stationid == station.Id))
                {
                    busySlots++;
                }
                station.BusyChargingSlots = busySlots;
                return station;
            }
            public IEnumerable<StationToList> getStationWithFreeChargingSlotsList()
            {
                var list = from item in getStationList()
                           where item.FreeChargingSlots != 0
                           select item;
                if (list.Any())
                {
                    return list;
                }
                throw new Exceptions.emptyListException("there are no station with free charging slots");
            }



            #endregion

            #region Parcel
            public void AddParcel(BO.Parcel c)
            {
                c.Created = DateTime.Now;
                c.Delivered = null;
                c.Scheduled = null;
                c.PickedUp = null;
                c.Droneinparcel = null;
                DLAPI.DO.Parcel newparcel = new DLAPI.DO.Parcel();
                newparcel.Id = c.Id;
                newparcel.Senderid = c.Sender.Id;
                newparcel.Targetid = c.Receiver.Id;
                newparcel.Priority = (DLAPI.DO.Priorities)c.Priority;
                newparcel.Weightcategory = (DLAPI.DO.WeightCategories)c.Weight;
                newparcel.Droneid = 0;
                newparcel.Requested = DateTime.Now;
                newparcel.Delieverd = null;
                newparcel.Scheduled = null;
                newparcel.Pickedup = null;
                try
                {
                    Dalob.addParcel(newparcel);

                }
                catch (Exception ex)
                {
                    throw new Exceptions.myExceptions.itemAllreadyExistException(ex.Message, ex);
                }
            }
            public DO.Parcel? CheckByWeight(IEnumerable<DO.Parcel> parcellist, WeightCategories weight, Location c, double mindistance, DroneToList dts)
            {
                if (weight == WeightCategories.heavy)
                {
                    var parcellistheavy = parcellist.Where(item => (WeightCategories)item.Weightcategory == WeightCategories.heavy);
                    if (parcellistheavy != null)
                    {
                        DLAPI.DO.Parcel? p = CheckByDistance(parcellistheavy, c, mindistance, dts);
                        if (p is not null)
                            return (DLAPI.DO.Parcel)p;
                    }
                    weight = WeightCategories.medium;

                }
                if (weight == WeightCategories.medium)
                {
                    var parcellistmedium = parcellist.Where(item => (WeightCategories)item.Weightcategory == WeightCategories.medium);
                    if (parcellistmedium != null)
                    {
                        DLAPI.DO.Parcel? p = CheckByDistance(parcellistmedium, c, mindistance, dts);
                        if (p is not null)
                            return (DLAPI.DO.Parcel)p;
                    }
                    weight = WeightCategories.light;
                }
                if (weight == WeightCategories.light)
                {
                    var parcellistlight = parcellist.Where(item => (WeightCategories)item.Weightcategory == WeightCategories.light);
                    if (parcellistlight != null)
                    {
                        DLAPI.DO.Parcel? p = CheckByDistance(parcellistlight, c, mindistance, dts);
                        if (p is not null)
                            return (DLAPI.DO.Parcel)p;
                    }
                }
                return null;

            }
            public DO.Parcel? CheckByDistance(IEnumerable<DO.Parcel> parcellist, Location c, double mindistance, DroneToList dts)
            {
                var ttt = parcellist.ToList();
                if (ttt.Count() == 0)
                    return null;
                double mindi = 100000000000000, temp = 0;
                bool flag = false;
                DLAPI.DO.Parcel? p = null;
                var t = parcellist.ToList();

                //checking for the closest parcel to the drone the and the buttery of the drone needs to feet the distance
                if (t != null)
                {
                    do
                    {
                        if (flag)
                        {
                            t.Remove((DLAPI.DO.Parcel)p);
                            //parcellist = t;

                        }
                        foreach (var item in t)
                        {
                            temp = dalob.DistanceCustomer(item.Senderid, c.Latitude, c.Longitude);
                            if (temp <= mindi)
                            {
                                mindi = temp;
                                p = item;
                                flag = true;
                            }
                        }
                    }
                    while (t.Count() > 0);
                    if (p != null)
                        return (DLAPI.DO.Parcel)p;
                    else
                        return null;
                }

                return null;
            }
            public void UpdatePickedByDrone(int numofdrone)
            {
                var t = dronetolistBL.ToList();
                DroneToList dts = t.Find(item => item.Id == numofdrone);
                if (dts.ParcelId != 0)
                {
                    try
                    {
                        DLAPI.DO.Parcel p = (DLAPI.DO.Parcel)Dalob.getParcel((int)dts.ParcelId);
                        if (p.Pickedup == null && p.Scheduled != null)
                        {
                            double sum1 = dalob.DistanceCustomer(p.Senderid, dts.Location.Latitude, dts.Location.Longitude);
                            double sumcharge = MinCharge(sum1, dts.Weight);
                            DLAPI.DO.Customer? c = Dalob.getCustomer(p.Senderid);
                            dts.BatteryStatus = dts.BatteryStatus - sumcharge;
                            dts.Location = new Location(c.Value.Latitude, c.Value.Longitude);
                            Dalob.deleteParcel(p.Id);
                            p.Pickedup = DateTime.Now;
                            Dalob.updateParcel2(p);

                        }
                        else
                        {
                            throw new Exception("Improper sequence of activities");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exceptions.myExceptions.itemNotFoundExceptions(ex.Message, ex);
                    }
                }
            }
            public void UpdateDeliverdByDrone(int numofdrone)
            {
                try
                {
                    var t = dronetolistBL.ToList();
                    DroneToList dts = t.Find(item => item.Id == numofdrone);
                    //DroneToList dts = dronetolistBL.Find(item => item.Id == numofdrone);
                    DLAPI.DO.Parcel p = (DLAPI.DO.Parcel)Dalob.getParcel((int)dts.ParcelId);
                    if (dts.ParcelId != 0)
                    {

                        if (p.Pickedup != null && p.Delieverd == null)
                        {
                            double sum2 = dalob.DistanceCustomer(p.Targetid, dts.Location.Latitude, dts.Location.Longitude);
                            double sumcharge = MinCharge(sum2, dts.Weight);
                            DLAPI.DO.Customer? c1 = Dalob.getCustomer(p.Targetid);
                            dts.BatteryStatus = dts.BatteryStatus - sumcharge;
                            dts.Location = new Location(c1.Value.Latitude, c1.Value.Longitude);
                            dts.Status = DroneStatus.free;
                            Dalob.deleteParcel(p.Id);
                            p.Delieverd = DateTime.Now;
                            Dalob.updateParcel2(p);
                        }
                        else
                        {
                            throw new Exception("Improper sequence of activities");
                        }
                    }
                    DLAPI.DO.Customer? c = Dalob.getCustomer(p.Targetid);
                }
                catch (Exception ex)
                {
                    throw new Exceptions.myExceptions.itemNotFoundExceptions(ex.Message, ex);
                }

            }
            public BO.Parcel getParcel(int num)
            {
                DLAPI.DO.Parcel p = (DLAPI.DO.Parcel)Dalob.getParcel(num);
                Parcel parcel = new Parcel()
                {
                    Id = p.Id,
                    Sender = new CustomerInParcel(Dalob.getCustomer(p.Senderid).Id, Dalob.getCustomer(p.Senderid).Name),
                    Receiver = new CustomerInParcel(Dalob.getCustomer(p.Targetid).Id, Dalob.getCustomer(p.Targetid).Name),
                    Weight = (WeightCategories)p.Weightcategory,
                    Priority = (Priority)p.Priority,
                    Droneinparcel = convertTodroneInParcel(Dalob.getDrone(p.Droneid)),
                    Created = p.Requested,
                    PickedUp = p.Pickedup,
                    Delivered = p.Delieverd,
                    Scheduled = p.Scheduled,
                };
                return parcel;
            }
            public DroneInParcel convertTodroneInParcel(DO.Drone d)
            {
                DroneInParcel drone = new DroneInParcel(d.Id);
                foreach (var item in dronetolistBL)
                {
                    if (item.Id == d.Id)
                    {
                        drone.BatteryStatus = item.BatteryStatus;
                        drone.CurrentLocation = item.Location;
                        break;
                    }
                }
                return drone;
            }
            public IEnumerable<ParcelForList> getParcelList()
            {
                var parcelList = from parcel in Dalob.getParcelList(item => item.Deleted == false)
                                 select new ParcelForList()
                                 {
                                     Id = parcel.Id,
                                     ReciverName = Dalob.getCustomer(parcel.Targetid).Name,
                                     SenderName = Dalob.getCustomer(parcel.Senderid).Name,
                                     Weight = (WeightCategories)parcel.Weightcategory,
                                     Priority = (Priority)parcel.Priority,
                                 };
                foreach (var item in parcelList)
                {
                    var indexParcel = Dalob.getParcelList(item => item.Deleted == false).ToList().FindIndex(c => c.Id == item.Id);
                    var par = Dalob.getParcelList(item => item.Deleted == false).ToList().ElementAt(indexParcel);
                    //DateTime? date = new DateTime?();
                    if (par.Delieverd != null)
                        item.Status = ParcelStatus.deliverd;
                    else if (par.Pickedup != null)
                        item.Status = ParcelStatus.pickedUp;
                    else if (par.Scheduled != null)
                        item.Status = ParcelStatus.attached;
                    else if (par.Requested != null)
                        item.Status = ParcelStatus.Created;
                }
                if (parcelList.Any())
                    return parcelList;
                throw new Exceptions.emptyListException();
            }
            public IEnumerable<ParcelForList> getParcelsWithoutaDrone()
            {
                var list = from parcel in getParcelList()
                           where parcel.Status == ParcelStatus.Created
                           select parcel;
                if (list.Any())
                    return list;
                throw new Exceptions.emptyListException("all parcels have been attached to drone");
            }
            public CustomerInParcel getCustomerInParcel(int num)
            {
                DLAPI.DO.Customer c = Dalob.getCustomer(num);
                CustomerInParcel customerInParcel = new CustomerInParcel(c.Id, c.Name);
                return customerInParcel;
            }

            #endregion

            #region CalculationAndChecks
            public double MinCharge(double sumdistancemin, WeightCategories weight)
            {
                double x = 0;
                if (weight == WeightCategories.light)
                    return sumdistancemin * ob[1];
                if (weight == WeightCategories.medium)
                    return sumdistancemin * ob[2];
                if (weight == WeightCategories.heavy)
                    return sumdistancemin * ob[3];
                return x;
            }
            public bool ChecksPhoneNumber(string phone)
            {
                if (phone.Length == 10)
                {
                    if (phone[0] == '0')
                    {
                        if (phone[1] == '5')
                        {
                            if (phone[2] == '0' || phone[2] == '2' || phone[2] == '3' || phone[2] == '4' || phone[2] == '5' || phone[2] == '8')
                                return true;
                        }
                    }
                }

                return false;
            }
            public int SumDigits(int x)

            {//The function gets a number.

                int sum = 0;

                while (x > 0)

                {

                    sum += x % 10;

                    x /= 10;

                }

                return sum;

            }//The function returns the sum of its digits.
            public int LastDigitID(int x)

            {//The function receives an ID number.

                int num, sum = 0;

                for (int i = 8; i >= 1; i--)

                {

                    num = x % 10;

                    if (i % 2 == 0)

                    {

                        num *= 2;

                    }

                    x /= 10;

                    sum += SumDigits(num);

                }

                sum = sum % 10;

                return 10 - sum;

            }//The function returns the audit digit of the ID card.
            public bool CheckIdentityNumber(int id)
            {
                int x = id % 10;
                if (x == LastDigitID(id / 10))
                    return true;
                return false;
            }
            public double SumCharge(DO.Parcel? itemparcel, DroneToList updatedrone)
            {
                double distaceDroneToSender = dalob.DistanceCustomer(itemparcel.Value.Senderid, updatedrone.Location.Latitude, updatedrone.Location.Longitude);
                //distance betwean sender to target
                double targetlat = dalob.DistanceCustomerLAT(itemparcel.Value.Targetid);
                double targetlong = dalob.DistanceCustomerLONG(itemparcel.Value.Targetid);
                double distanceSenderToTarget = dalob.DistanceCustomer(itemparcel.Value.Senderid, targetlat, targetlong);
                //distance betwean target to the closest station
                double mindistance1 = 0, newdistance1 = 0, newlong1 = 0, newlat1 = 0;
                foreach (var itemstation in Dalob.getStationList(item => item.Deleted == false))
                {
                    newdistance1 = dalob.DistanceStation(itemstation.Id, targetlat, targetlong);
                    if (newdistance1 < mindistance1)
                    {
                        newlong1 = itemstation.Longitude;
                        newlat1 = itemstation.Latitude;
                        mindistance1 = newdistance1;
                    }
                }
                double sumdistancemin = distaceDroneToSender + distanceSenderToTarget + mindistance1;
                double minrateofcharge = MinCharge(sumdistancemin, updatedrone.Weight);
                return minrateofcharge;
            }
            #endregion
        }
    }

