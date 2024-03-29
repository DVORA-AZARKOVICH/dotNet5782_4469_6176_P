﻿using BL.BO;
using BL.Exceptions;
using BLApi;
using DALApi;
using System;
using System.Collections.Generic;
using System.Linq;
//using DALApi;


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

            double available = 0;
            double light = 15.5;
            double medium = 25.5;
            double heavy = 37.5;
            double chargingRate = 2;
            /*  ob = Dalob.GetElectricity();
              double available = ob[0];
              double light = ob[1];
              double medium = ob[2];
              double heavy = ob[3];
              double chargingRate = ob[4];*/
            //converting the DAL drone list to BL list
            IEnumerable<DroneToList> temp;
            /*temp = from item in Dalob.getDroneList(item => item.Deleted == false)
                   select new BO.DroneToList()
                   {
                       Id = item.Id,
                       Model = item.Model,
                       Weight = (BO.WeightCategories)item.Maxweight


                   };*/

            // dronetolistBL = temp.ToList();
            var parcellist = Dalob.getParcelList(item => item.Deleted == false).ToList();
            var stationlist = Dalob.getStationList(item => item.Deleted == false);
            var customerlist = Dalob.getCustomerList(item => item.Deleted == false);
            var temp2 = Dalob.getDroneList(item => item.Deleted == false);
            var tem3 = from item in Dalob.getDroneList(item => item.Deleted == false)
                       select new BO.DroneToList()
                       {
                           Id = item.Id,
                           Model = item.Model,
                           Weight = (BO.WeightCategories)item.Maxweight
                       };
            dronetolistBL = tem3.ToList();
            //Goes through all the packages and updates data accordingly
            for (int i = 0; i < parcellist.Count(); i++)

            //   foreach (var itemparcel in parcellist)
            {

                // checking if the customer - sender and customer target of parcel exist case not throw exception
                bool flag1 = true, flag2 = true;
                try
                {
                    DO.Customer c = Dalob.getCustomer(parcellist[i].Targetid);
                }
                catch (Exception ex)
                {
                    flag1 = false;
                    throw new Exceptions.myExceptions.itemNotFoundExceptions(ex.Message, ex);
                }
                try
                {
                    DO.Customer c = Dalob.getCustomer(parcellist[i].Senderid);
                }
                catch (Exception ex)
                {
                    flag2 = false;
                    throw new myExceptions.itemNotFoundExceptions(ex.Message, ex);
                }
                if (flag1 && flag2)
                {
                    //In case the parcel was conect to a drone byut has not yet been delivered to the customer
                    if (parcellist[i].Droneid != 0 && parcellist[i].Delieverd == null)
                    {
                        for (int j = 0; j < dronetolistBL.Count(); j++)
                        //foreach (var updatedrone in dronetolistBL)
                        {
                            //finding the drone in the list and updating
                            if (dronetolistBL[j].Id == parcellist[i].Droneid)
                            {
                                dronetolistBL[j].Status = DroneStatus.busy;
                                int idsender = parcellist[i].Senderid;//the id of the sender
                                                                      //searching for the sender to know its location for the calculation.
                                try
                                {
                                    DO.Customer sender = Dalob.getCustomer(parcellist[i].Senderid);
                                    Location location = new Location(sender.Latitude, sender.Longitude);
                                    //case the parcel wasnt picked up yet
                                    if (parcellist[i].Pickedup == null)
                                    {
                                        DO.Station? s = ClosestStation(stationlist, location);
                                        if (s != null)
                                        {
                                            Location l = new Location(s.Value.Latitude, s.Value.Longitude);
                                            dronetolistBL[j].Location1 = l;

                                        }
                                        else
                                            throw new myExceptions.itemNotFoundExceptions("Match station was not found");//hapens when the list of station is empty

                                    }
                                    //case the parcel did picked up but hasnt got to the target yet
                                    else if (parcellist[i].Pickedup != null && parcellist[i].Delieverd == null)
                                    {
                                        Location droneLocation = new Location(sender.Latitude, sender.Longitude);
                                        dronetolistBL[j].Location1 = droneLocation;
                                        //updatedrone.Location.Latitude = sender.Latitude;
                                        //updatedrone.Location.Longitude = sender.Latitude;
                                    }
                                    //distance betwean drone and sender
                                    DO.Parcel? p = parcellist[i];
                                    double minrateofcharge = SumCharge(p, dronetolistBL[j]);
                                    if (minrateofcharge < 100)
                                    {
                                        dronetolistBL[j].BatteryStatus = minrateofcharge + rand.NextDouble() * 100;
                                    }
                                    else if (minrateofcharge == 100)
                                    {
                                        dronetolistBL[j].BatteryStatus = 100;
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
                    t.Location1 = new Location(item.Location1.Latitude, item.Location1.Longitude);
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
                    t.Location1 = new Location(34, 31);
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
                    // foreach (var item in dronetolistBL)
                    for (int k = 0; k < dronetolistBL.Count(); k++)
                    {
                        if (dronetolistBL[k].Id == tt[i].Droneid)
                        {
                            if (dronetolistBL[k].Status == DroneStatus.busy)
                                dronetolistBL[k].ParcelId = tt[i].Id;
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
                          select new ParcelInCustomer { Id = parcel.Id, Priority = (Priority)parcel.Priority, Weight = (BO.WeightCategories)parcel.Weightcategory };
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
                           select new ParcelInCustomer { Id = parcel.Id, Priority = (Priority)parcel.Priority, Weight = (BO.WeightCategories)parcel.Weightcategory };
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
            BO.Customer customer = new BO.Customer()
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
        public IEnumerable<CustomerForList> getCustomerList()
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

        public IEnumerable<CustomerForList> getCustomerList(Predicate<CustomerForList> predicate)
        {
            {
                var customers = from item in getCustomerList()
                                where predicate(item)
                                select item;
                return customers;
            }
        }
        #endregion

        #region Drone
        public void AddDrone(BO.Drone d, int idstation)
        {

            d.BatteryStatus = 20 + rand.NextDouble() * 20;
            d.Status = DroneStatus.inMaintence;
            try
            {
                DO.Station s = Dalob.getStation(idstation);
                d.CurrentLocation = new Location(s.Latitude, s.Longitude);
                DO.Drone newdrone = new DO.Drone();
                newdrone.Id = d.Id;
                newdrone.Model = d.Model;
                newdrone.Maxweight = (DO.WeightCategories)d.Weight;
                try
                {
                    //adding the drone also to the list of dronetolistBL
                    Dalob.addDrone(newdrone);
                    BO.DroneToList newdronetolist = new BO.DroneToList();
                    newdronetolist.Id = d.Id;
                    newdronetolist.Model = d.Model;
                    newdronetolist.Status = d.Status;
                    newdronetolist.Weight = d.Weight;
                    newdronetolist.Location1 = d.CurrentLocation;
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
            DO.Station s = Dalob.getStation(t[index].Id);
            s.Chargslot--;
            Dalob.deleteStation(s.Id);
            Dalob.addStation(s);
            //updation the drone's location
            dronetolistBL.Remove(d);
            d.Location1 = new Location(t[index].Latitude, t[index].Longitude);
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
                DO.Customer? c = Dalob.getCustomer(idcustomer);
                d.Location1.Longitude = c.Value.Longitude;
                d.Location1.Latitude = c.Value.Latitude;
            }
            catch (Exception ex)
            {
                throw new myExceptions.itemNotFoundExceptions(ex.Message, ex);
            }

            double mindistance = 0;
            DO.Station? s = ClosestStation(stationlist, d.Location1);
            if (s != null)
            {
                mindistance = dalob.DistanceStation(s.Value.Id, d.Location1.Latitude, d.Location1.Longitude);
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
                DO.Drone updatedrone = Dalob.getDrone(numofdrone);
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
                DO.Drone updatedrone = Dalob.getDrone(numofdrone);
                BO.DroneToList dronetolist = null;
                dronetolist = dronetolistBL.Find(item => item.Id == numofdrone && item.Status == DroneStatus.free);
                if (dronetolist is null)
                    throw new Exception("the Drone is not free to deliver to charge");//הרחפן לא פנוי אז אי אפשר לשלוח אותו לטעינה
                IEnumerable<DO.Station> stationlist = Dalob.getStationList(item => item.Deleted == false);
                List<DO.Station> t = stationlist.ToList();
                Location c = new Location(dronetolist.Location1.Latitude, dronetolist.Location1.Longitude);
                bool succeed = false;
                double mindistance = 0;
                DO.Station s;
                if (t is not null && c is not null)
                {
                    do
                    {
                        s = ClosestStation(stationlist, c);
                        if (s.Chargslot > 0)
                        {
                            mindistance = dalob.DistanceStation(s.Id, dronetolist.Location1.Latitude, dronetolist.Location1.Longitude);
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
                        dronetolist.Location1.Latitude = s.Latitude;
                        dronetolist.Location1.Longitude = s.Longitude;
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
                    DO.Dronecharge dronecharge = Dalob.getDroneCharge(numofdrone);
                    Dalob.deleteDroneCharge(dronecharge.Droneid);
                    try
                    {
                        //updates the station
                        DO.Station stations = Dalob.getStation(dronecharge.Stationid);
                        Dalob.deleteStation(dronecharge.Stationid);
                        DO.Station s = new DO.Station();
                        s = stations;
                        s.Chargslot++;
                        Dalob.addStation(s);
                        //Dalob.deleteDroneCharge(numofdrone);

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
                DO.Drone d = Dalob.getDrone(numofdrone);
                var t = dronetolistBL.ToList();
                DroneToList dts = t.Find(item => item.Id == numofdrone);
                // DroneToList dts = dronetolistBL.Find(item => item.Id == numofdrone);
                if (dts != null)
                {
                    if (dts.Status == DroneStatus.free)
                    {
                        //updating the distance the drone can fly according to its battery
                        double maxdistance = 0;
                        if (dts.Weight == BO.WeightCategories.light)
                            maxdistance = dts.BatteryStatus / ob[1];
                        if (dts.Weight == BO.WeightCategories.medium)
                            maxdistance = dts.BatteryStatus / ob[2];
                        if (dts.Weight == BO.WeightCategories.heavy)
                            maxdistance = dts.BatteryStatus / ob[3];
                        var parcell = Dalob.getParcelList(item => item.Deleted == false).Where(item => item.Scheduled == null && SumCharge(item, dts) <= dts.BatteryStatus);
                        //seperating the list DAL parcels to differnt lists according to their priority
                        var parcellisturgent = parcell.Where(item => (Priority)item.Priority == Priority.urgent);
                        var parcellistfast = parcell.Where(item => (Priority)item.Priority == Priority.fast);
                        var parcellistregular = parcell.Where(item => (Priority)item.Priority == Priority.regular);
                        //the location of this drone
                        Location c = new Location(dts.Location1.Latitude, dts.Location1.Longitude);
                        DO.Parcel? p = CheckByWeight(parcellisturgent, dts.Weight, c, maxdistance, dts);
                        if (p is null)
                            p = CheckByWeight(parcellistfast, dts.Weight, c, maxdistance, dts);
                        if (p is null)
                            p = CheckByWeight(parcellistregular, dts.Weight, c, maxdistance, dts);
                        if (p is null)
                            throw new Exceptions.ExceptionExceptionExceedingPossibleQuantity("there is no match parcel");
                        //אם זה הפניה לא צריך להסיר?
                        dronetolistBL.Remove(dts);
                        dts.ParcelId = p.Value.Id;
                        dts.Status = DroneStatus.busy;
                        DO.Parcel pp = Dalob.getParcel(p.Value.Id);
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
            BO.Drone d = new BO.Drone();
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
                    d.CurrentLocation = item.Location1;
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
                Weight = (BO.WeightCategories)p.Weightcategory,
                Destination = new Location(Dalob.getCustomer(p.Targetid).Latitude, Dalob.getCustomer(p.Targetid).Longitude),
                Pickup = new Location(Dalob.getCustomer(p.Senderid).Latitude, Dalob.getCustomer(p.Senderid).Longitude),
            };
            parcel.Distance = Math.Sqrt(Math.Pow(parcel.Pickup.Latitude - parcel.Destination.Latitude, 2) + Math.Pow(parcel.Pickup.Longitude - parcel.Destination.Longitude, 2));
            return parcel;
        }
        public List<DroneToList> getdroneList()
        {
            var drones = from drone in dalob.getDroneList(station => station.Deleted == false)
                         select convertTodroneInParcel(drone);
            if (drones.Any())
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
        public void NextState(int id ,ref bool flagcontinue,ref int parcelid)
        {
            bool flag = false;
            BO.Drone d = getDrone(id);
            switch (d.Status)
            {
                case BO.DroneStatus.free:
                    {
                        flagcontinue = false;
                        try
                        {
                            UpdateParcelToDrone(id);
                            parcelid = d.TheParcel.Id;
                            d.Status = DroneStatus.busy;
                            d.BatteryStatus -= 6.2;
                        }
                        catch (Exception)
                        {
                            UpdateDroneToCharge(id);
                        }
                    }
                    break;
                case BO.DroneStatus.inMaintence:
                    {
                        if (d.BatteryStatus == 100)
                        {
                            UpdateReleseDroneFromCharge(id, DateTime.Now);
                            UpdateParcelToDrone(id);
                            DroneToList dts = dronetolistBL.Find(item => item.Id == id);
                            if(dts!=null)

                            flagcontinue = false;
                            parcelid = dts.ParcelId;
                            if (parcelid != 0)
                                d.Status = DroneStatus.busy;
                            else
                                d.Status = DroneStatus.free;
                            d.BatteryStatus -= 6.2;
                        }
                        else
                        {
                            flag = true;
                        }

                    }
                    break;
                case BO.DroneStatus.busy:
                    {
                        flagcontinue = false;
                        switch (d.TheParcel.Status)
                        {
                            case BO.ParcelInTransf.intransfer:
                                {
                                    UpdateDeliverdByDrone(id);
                                    parcelid = d.TheParcel.Id;
                                    d.BatteryStatus -= 6.2;
                                }
                                break;
                            case BO.ParcelInTransf.wautToCollect:
                                {
                                    UpdatePickedByDrone(id);
                                    parcelid = 0;
                                    d.Status = DroneStatus.free;
                                    d.BatteryStatus -= 6.2;

                                }
                                break;

                        }

                    }
                    break;
            }
                    if (flag)
                    {
                        d.BatteryStatus = (int)(Math.Min(5 + d.BatteryStatus, 100) * 100) / 100.0;
                    }
                    var dtt = dronetolistBL.Find(item => item.Id == d.Id);
                    dronetolistBL.Remove(dtt);
                    dtt.BatteryStatus = d.BatteryStatus;
                    dtt.Location1 = new Location(d.CurrentLocation.Longitude, d.CurrentLocation.Latitude);
                    dtt.ParcelId = (d.TheParcel == null) ? 0 : d.TheParcel.Id;
                    dtt.Status = d.Status;
                    dronetolistBL.Add(dtt);
                    /* BO.DroneToList dtl = new BO.DroneToList()
                     {
                         Id = d.Id,
                         Model = d.Model,
                         Weight = d.Weight,
                         BatteryStatus = d.BatteryStatus,
                         Status = d.Status,
                         Location1 = new Location(d.CurrentLocation.Longitude, d.CurrentLocation.Latitude),
                         ParcelId = (d.TheParcel==null)?0:d.TheParcel.Id

                     };
                     dronetolistBL.Add(dtl);*/
            


        }


        #endregion

        #region Station
        public void AddStation(BO.Station s)
        {
            s.DronesInCharging = null;
            DO.Station newstation = new DO.Station();
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
            DO.Station s = new DO.Station();
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
                DO.Station updatestation = new DO.Station();
                updatestation = Dalob.getStation(numofstation);
                int x = dronetolistBL.Where(item => (item.Status == DroneStatus.inMaintence) && (item.Location1.Longitude == updatestation.Longitude) && (item.Location1.Latitude == updatestation.Latitude)).Count();
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
                DO.Station updatestation = new DO.Station();
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
            DO.Station st = Dalob.getStation(num);
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
            BO.Station s = new BO.Station()
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
        public IEnumerable<StationToList> getStationsList(Predicate<StationToList> predicate)
        {
            var stations = from item in getStationList()
                           where predicate(item)
                           select item;
            return stations;
        }
        public StationToList convertToStationList(DO.Station s)
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
        public IEnumerable<StationToList> getStationList(Predicate<StationToList> predicate)
        {
            {
                var stations = from item in getStationList()
                               where predicate(item)
                               select item;
                return stations;
            }
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
            DO.Parcel newparcel = new DO.Parcel();
            newparcel.Id = c.Id;
            newparcel.Senderid = c.Sender.Id;
            newparcel.Targetid = c.Receiver.Id;
            newparcel.Priority = (DO.Priorities)c.Priority;
            newparcel.Weightcategory = (DO.WeightCategories)c.Weight;
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
        public DO.Parcel? CheckByWeight(IEnumerable<DO.Parcel> parcellist, BO.WeightCategories weight, Location c, double mindistance, DroneToList dts)
        {
            if (weight == BO.WeightCategories.heavy)
            {
                var parcellistheavy = parcellist.Where(item => (BO.WeightCategories)item.Weightcategory == BO.WeightCategories.heavy);
                if (parcellistheavy != null)
                {
                    DO.Parcel? p = CheckByDistance(parcellistheavy, c, mindistance, dts);
                    if (p is not null)
                        return (DO.Parcel)p;
                }
                weight = BO.WeightCategories.medium;

            }
            if (weight == BO.WeightCategories.medium)
            {
                var parcellistmedium = parcellist.Where(item => (BO.WeightCategories)item.Weightcategory == BO.WeightCategories.medium);
                if (parcellistmedium != null)
                {
                    DO.Parcel? p = CheckByDistance(parcellistmedium, c, mindistance, dts);
                    if (p is not null)
                        return (DO.Parcel)p;
                }
                weight = BO.WeightCategories.light;
            }
            if (weight == BO.WeightCategories.light)
            {
                var parcellistlight = parcellist.Where(item => (BO.WeightCategories)item.Weightcategory == BO.WeightCategories.light);
                if (parcellistlight != null)
                {
                    DO.Parcel? p = CheckByDistance(parcellistlight, c, mindistance, dts);
                    if (p is not null)
                        return (DO.Parcel)p;
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
            DO.Parcel? p = null;
            var t = parcellist.ToList();

            //checking for the closest parcel to the drone the and the buttery of the drone needs to feet the distance
            if (t != null)
            {
                do
                {
                    if (flag)
                    {
                        t.Remove((DO.Parcel)p);
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
                    return (DO.Parcel)p;
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
                    DO.Parcel p = (DO.Parcel)Dalob.getParcel((int)dts.ParcelId);
                    if (p.Delieverd == null && p.Scheduled != null)
                    {
                        double sum1 = dalob.DistanceCustomer(p.Senderid, dts.Location1.Latitude, dts.Location1.Longitude);
                        double sumcharge = MinCharge(sum1, dts.Weight);
                        DO.Customer? c = Dalob.getCustomer(p.Senderid);
                        dts.BatteryStatus = dts.BatteryStatus - sumcharge;
                        dts.Location1 = new Location(c.Value.Latitude, c.Value.Longitude);
                        Dalob.deleteParcel(p.Id);
                        p.Delieverd = DateTime.Now;
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
                DO.Parcel p = (DO.Parcel)Dalob.getParcel((int)dts.ParcelId);
                if (dts.ParcelId != 0)
                {

                    if (p.Pickedup != null && p.Delieverd == null)
                    {
                        double sum2 = dalob.DistanceCustomer(p.Targetid, dts.Location1.Latitude, dts.Location1.Longitude);
                        double sumcharge = MinCharge(sum2, dts.Weight);
                        DO.Customer? c1 = Dalob.getCustomer(p.Targetid);
                        dts.BatteryStatus = dts.BatteryStatus - sumcharge;
                        dts.Location1 = new Location(c1.Value.Latitude, c1.Value.Longitude);
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
                DO.Customer? c = Dalob.getCustomer(p.Targetid);
            }
            catch (Exception ex)
            {
                throw new Exceptions.myExceptions.itemNotFoundExceptions(ex.Message, ex);
            }

        }
        public BO.Parcel getParcel(int num)
        {
            DO.Parcel p = (DO.Parcel)Dalob.getParcel(num);
            BO.Parcel parcel = new BO.Parcel()
            {
                Id = p.Id,
                Sender = new CustomerInParcel(Dalob.getCustomer(p.Senderid).Id, Dalob.getCustomer(p.Senderid).Name),
                Receiver = new CustomerInParcel(Dalob.getCustomer(p.Targetid).Id, Dalob.getCustomer(p.Targetid).Name),
                Weight = (BO.WeightCategories)p.Weightcategory,
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
                    drone.CurrentLocation = item.Location1;
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
                                 Weight = (BO.WeightCategories)parcel.Weightcategory,
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
            DO.Customer c = Dalob.getCustomer(num);
            CustomerInParcel customerInParcel = new CustomerInParcel(c.Id, c.Name);
            return customerInParcel;
        }
        public IEnumerable<ParcelForList> getParcelList(Predicate<ParcelForList> predicate)
        {
            var parcels = from item in getParcelList()
                          where predicate(item)
                          select item;
            return parcels;
            //throw new Exceptions.emptyListException("there are no drones of this type");
        }

        #endregion

        #region CalculationAndChecks
        public double MinCharge(double sumdistancemin, BO.WeightCategories weight)
        {
            double x = 0;
            if (weight == BO.WeightCategories.light)
                return sumdistancemin * ob[1];
            if (weight == BO.WeightCategories.medium)
                return sumdistancemin * ob[2];
            if (weight == BO.WeightCategories.heavy)
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
            double distaceDroneToSender = dalob.DistanceCustomer(itemparcel.Value.Senderid, updatedrone.Location1.Latitude, updatedrone.Location1.Longitude);
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