
using DALApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;



namespace DLXML
{
    sealed class DLXML :  IDal
    {
       
        public static readonly IDal instance = new DLXML();
        static DLXML() { }
        public static IDal Instance { get => instance; }
        private DLXML()
        {
            DataSource.Initialize();
        }

        #region DS XML Files
        XElement CustomerRoot;
        private static string DataDirectory = @"data";
        string customerPath = @"CustomerXml.xml";//Xelement
        string parcelPath = @"ParcelXml.xml";//XMLSerialzer
        string dronePath = @"DroneXml.xnml";//XMLSerialzer
        string dronchargePath = @"DroneChargeXml.xml";//XMLSerialzer
        string stationPath = @"StationXml.xml";//XMLSerialzer\
        string powerConsumptionRequest = @" PowerConsumptionRequestXml.xml";//XMLSerialzer\
        #endregion

        #region Customer

        private void CreateFiles()
        {
            CustomerRoot = new XElement("Customer");
            CustomerRoot.Save(customerPath);
        }
        private void LoadData()
        {
            try
            {
                CustomerRoot = XElement.Load(customerPath);
            }
            catch
            {
                throw new Exception("File upload problem");
            }
        }
        public void SaveCustomerList(List<Customer> CustomerList)
        {
            CustomerRoot = new XElement("Customers");

            foreach (Customer item in CustomerList)
            {
                XElement id = new XElement("id", item.Id);
                XElement name = new XElement("name", item.Name);
                XElement phone = new XElement("phone", item.Phone);
                XElement longitude = new XElement("longitude", item.Longitude);
                XElement latitude = new XElement("latitude",item.Latitude);
                XElement deleted = new XElement("deleted", item.Deleted);
                XElement customer = new XElement("customer", id, name,phone,longitude,latitude,deleted);
                CustomerRoot.Add(customer);

                addCustomer(item);
            }

             CustomerRoot.Save(customerPath);
        }
        public void SaveCustomerListLinq(List<Customer> CustomerList)
        {
            //XElement StudentRoot;

            var v = from p in CustomerList
                    select new XElement("Customer",
                                                new XElement("id", p.Id),
                                                new XElement("name",p.Name),
                                                new XElement("phone", p.Phone),
                                                new XElement("longitude", p.Longitude),
                                                new XElement("latitude", p.Latitude),
                                                new XElement("deleted", p.Deleted)
                                                );

            CustomerRoot = new XElement("Customers", v);

            CustomerRoot.Save(customerPath);
        }
        public void addCustomer(Customer item)
        {
            XElement id = new XElement("id", item.Id);
            XElement name = new XElement("name", item.Name);
            XElement phone = new XElement("phone", item.Phone);
            XElement longitude = new XElement("longitude", item.Longitude);
            XElement latitude = new XElement("latitude", item.Latitude);
            XElement deleted = new XElement("deleted", item.Deleted);
            XElement customer = new XElement("customer", id, name, phone, longitude, latitude, deleted);

            CustomerRoot.Add(customer);
            CustomerRoot.Save(customerPath);
        }
        public Customer getCustomer(int id)
        {

            LoadData();
            Customer customer;
            try
            {
                customer = (from p in CustomerRoot.Elements()
                           where Convert.ToInt32(p.Element("id").Value) == id
                           select new Customer()
                           {
                               Id = Convert.ToInt32(p.Element("id").Value),
                               Name = p.Element("name").Element("name").Value,
                               Phone= p.Element("phone").Element("phone").Value,
                               Longitude = Convert.ToDouble(p.Element("longitude").Value),
                               Latitude = Convert.ToDouble(p.Element("latitude").Value),
                               Deleted = Convert.ToBoolean(p.Element("deleted").Value)

                           }).FirstOrDefault();
            }
            catch
            {
                throw new Exception("DAL: Customer with the same id not found...");
            }
            return customer;
        }
        public IEnumerable<Customer> getCustomerList(Predicate<Customer> predicate)
        {
            LoadData();
            List<Customer> Customers;
            try
            {
                Customers = (from p in CustomerRoot.Elements()
                            select new Customer()
                            {
                                Id = Convert.ToInt32(p.Element("id").Value),
                                 Name = p.Element("name").Element("name").Value,
                                Phone = p.Element("phone").Element("phone").Value,
                                Longitude = Convert.ToDouble(p.Element("longitude").Value),
                                Latitude = Convert.ToDouble(p.Element("latitude").Value),
                                Deleted = Convert.ToBoolean(p.Element("deleted").Value)
                            }).ToList();
            }
            catch
            {
                throw new Exceptions.XMLFileLoadCreateException("List of Customers is empty");
            }
            return Customers;
        }
        public void deleteCustomer(int id)
        {
            XElement CustomerElement;
            try
            {
                CustomerElement = (from p in CustomerRoot.Elements()
                                  where Convert.ToInt32(p.Element("id").Value) == id
                                  select p).FirstOrDefault();
                CustomerElement.Remove();
                CustomerRoot.Save(customerPath);
            }
            catch
            {
                throw new IDdNotFoundExeption("DL: Drone with the same id not found...");
            }
        }
        #endregion

        #region Drone
        public void addDrone(Drone item)
        {
            List<DO.Drone> droneList = XMLTools.LoadListFromXMLSerialzer<DO.Drone>(dronePath);

            int index = droneList.FindIndex(t => t.Id == item.Id&&t.Deleted ==false);
            if (index == -1)
            {
                throw new IdExistException("DL: Drone with the same id already exists...");
            }
            item.Deleted = false;
            droneList.Add(item);

            XMLTools.SaveListToXNLSerialzer<DO.Drone>(droneList, dronePath);
        }
        public Drone getDrone(int id)
        {
            List<DO.Drone> droneList = XMLTools.LoadListFromXMLSerialzer<DO.Drone>(dronePath);
            int index = droneList.FindIndex(t => t.Id == id&&t.Deleted ==false);
            if (index == -1)
                throw new Exception("DAL: Drone with the same id not found...");
            return droneList.FirstOrDefault(t => t.Id == id&&t.Deleted ==false);
        }
        public IEnumerable<Drone> getDroneList(Predicate<Drone> predicate)
        {
            List<Drone> droneList = XMLTools.LoadListFromXMLSerialzer<Drone>(dronePath);
            return from drone in droneList
                   where drone.Deleted == false
                   select drone;
            
        }
        public void UpDatenotcharging(int numofdrone)
        {

            List<DO.Drone>droneList = XMLTools.LoadListFromXMLSerialzer<DO.Drone>(dronePath);
            int index = droneList.FindIndex(t => t.Id == numofdrone&& t.Deleted == false);
            if (index == -1)
            {
                throw new IDdNotFoundExeption("DL: Drone with the same id not found...");
            }
            Drone d = droneList.Find(item => item.Id == numofdrone && item.Deleted == false);
            List<DO.Dronecharge> dronechargeList = XMLTools.LoadListFromXMLSerialzer<DO.Dronecharge>(dronchargePath);
            int index2 = dronechargeList.FindIndex(t => t.Droneid == numofdrone && t.Deleted == false);
            if (index2 == -1)
            {
                throw new IDdNotFoundExeption("DL: Dronecharge with the same id not found...");
            }
            Dronecharge dc = dronechargeList.Find(item => item.Droneid == numofdrone && item.Deleted == false);
            List<DO.Station> stationList = XMLTools.LoadListFromXMLSerialzer<DO.Station>(stationPath);
            int index3 = stationList.FindIndex(t => t.Id == dc.Stationid && t.Deleted == false);
            if (index2 == -1)
            {
                throw new IDdNotFoundExeption("DL: Station with the same id not found...");
            }
             Station s = stationList.Find(item => item.Id == dc.Stationid && item.Deleted == false);
            s.Chargslot++;
            dc.Deleted = true;
            dronechargeList[index2] = dc;
            stationList[index3] = s;
            XMLTools.SaveListToXNLSerialzer<DO.Dronecharge>(dronechargeList, dronchargePath);
            XMLTools.SaveListToXNLSerialzer<DO.Station>(stationList, stationPath);
        }
        public void UpDateCharging(int numofdrone, int numofstation)
        {
            List<DO.Drone> droneList = XMLTools.LoadListFromXMLSerialzer<DO.Drone>(dronePath);
            int index = droneList.FindIndex(t => t.Id == numofdrone && t.Deleted == false);
            if (index == -1)
            {
                throw new IDdNotFoundExeption("DL: Drone with the same id not found...");
            }
            List<DO.Station> stationList = XMLTools.LoadListFromXMLSerialzer<DO.Station>(stationPath);
            int index3 = stationList.FindIndex(t => t.Id ==numofstation && t.Deleted == false);
            if (index3 == -1)
            {
                throw new IDdNotFoundExeption("DL: Station with the same id not found...");
            }
            Station s = stationList.Find(item => item.Id == numofstation&& item.Deleted == false);
            s.Chargslot--;
            stationList[index3] = s;
            XMLTools.SaveListToXNLSerialzer<DO.Station>(stationList, stationPath);
            DO.Dronecharge newdronecharge = new Dronecharge();
            newdronecharge.Droneid = numofdrone;
            newdronecharge.Stationid = numofstation;
            addDroneCharge(newdronecharge);
        }
        public double[] PowerConsumptionRequest()
        {
            double[] ob = new double[5];
            return ob;
        }
        public void deleteDrone(int id)
        {
            List<DO.Drone> droneList = XMLTools.LoadListFromXMLSerialzer<DO.Drone>(dronePath);
            int index = droneList.FindIndex(t => t.Id == id && t.Deleted == false);
            if (index == -1)
            {
                throw new IDdNotFoundExeption("DL: Drone with the same id not found...");
            }
            Drone s = droneList.Find(item => item.Id == id && item.Deleted == false);
            s.Deleted = true;
            droneList[index] = s;
            XMLTools.SaveListToXNLSerialzer<DO.Drone>(droneList, dronePath);
        }

        #endregion

        #region Station
        public void addStation(Station station1)
        {
            List<DO.Station> stationList = XMLTools.LoadListFromXMLSerialzer<DO.Station>(stationPath);

            int index = stationList.FindIndex(t => t.Id == station1.Id&&t.Deleted ==false);
            if (index != -1)
            {
                throw new IdExistException("DL: Station with the same id already exists...");
            }
            station1.Deleted = false;
            stationList.Add(station1);

            XMLTools.SaveListToXNLSerialzer<DO.Station>(stationList, stationPath);
        }
        public Station getStation(int id)
        {
            List<DO.Station> stationList = XMLTools.LoadListFromXMLSerialzer<DO.Station>(stationPath);
            int index = stationList.FindIndex(t => t.Id == id && t.Deleted == false);
            if (index == -1)
                throw new Exception("DAL: Station with the same id not found...");
            return stationList.FirstOrDefault(t => t.Id== id&&t.Deleted ==false);
        }
        public IEnumerable <DO.Station> getStationList()
        {
            List<Station> ListStations = XMLTools.LoadListFromXMLSerialzer<Station>(stationPath);
            return from station in ListStations
                   where station.Deleted ==false
                   select station;
        }
        public IEnumerable<Station> getFreeCharges()
        {
            List<DO.Station> stationList = XMLTools.LoadListFromXMLSerialzer<DO.Station>(stationPath);
            return  from item in stationList
                    where item.Chargslot!=1&&item.Deleted ==false
                    select item; //item.Clone();
        }
        public void deleteStation(int id)
        {
            List<DO.Station> stationList = XMLTools.LoadListFromXMLSerialzer<DO.Station>(stationPath);
            int index = stationList.FindIndex(t => t.Id == id && t.Deleted == false);
            if (index ==-1)
            {
                throw new IDdNotFoundExeption("DL: Station with the same id not found...");
            }
            Station s = stationList.Find(item => item.Id == id && item.Deleted == false);
            s.Deleted = true;
            stationList[index] = s;
            XMLTools.SaveListToXNLSerialzer<DO.Station>(stationList, stationPath);
        }
        #endregion

        #region DroneCharg
        public void addDroneCharge(Dronecharge dronechatge1)
        {
            List<DO.Dronecharge> dronechargeList = XMLTools.LoadListFromXMLSerialzer<DO.Dronecharge>(dronchargePath);

            int index = dronechargeList.FindIndex(t => t.Droneid == dronechatge1.Droneid && t.Deleted == false);
            if (index != -1)
            {
                throw new IdExistException("DL: DroneCharge with the same id already exists...");
            }
            dronechatge1.Deleted = false;
            dronechargeList.Add(dronechatge1);

            XMLTools.SaveListToXNLSerialzer<DO.Dronecharge>(dronechargeList, dronchargePath);
        }
        public void deleteDroneCharge(int id)
        {
            List<DO.Dronecharge> dronechargeList = XMLTools.LoadListFromXMLSerialzer<DO.Dronecharge>(dronchargePath);
            int index = dronechargeList.FindIndex(t => t.Droneid == id && t.Deleted == false);
            if (index == -1)
            {
                throw new IDdNotFoundExeption("DL: DroneCharge with the same id not found...");
            }
            Dronecharge dc = dronechargeList.Find(item => item.Droneid== id && item.Deleted == false);
            dc.Deleted = true;
            dronechargeList[index] = dc;
            XMLTools.SaveListToXNLSerialzer<DO.Dronecharge>(dronechargeList, dronchargePath);
        }
        public IEnumerable<Dronecharge> getDroneChargeList(Predicate<Dronecharge> predicate)
        {
            List<Dronecharge> dronechargeList = XMLTools.LoadListFromXMLSerialzer<Dronecharge>(dronchargePath);
            return from dronecharge in dronechargeList
                   where dronecharge.Deleted == false
                   select dronecharge;
        }
        public Dronecharge getDroneCharge(int iddrone)
        {
            List<DO.Dronecharge> dronechargeList = XMLTools.LoadListFromXMLSerialzer<DO.Dronecharge>(dronchargePath);
            int index = dronechargeList.FindIndex(t => t.Droneid == iddrone && t.Deleted == false);
            if (index == -1)
                throw new Exception("DAL: DroneCharge with the same id not found...");
            return dronechargeList.FirstOrDefault(t => t.Droneid == iddrone && t.Deleted == false);
        }
        #endregion

        #region Parcel
        public string addParcel(Parcel parcel1)
        {
            List<DO.Parcel> parcelList = XMLTools.LoadListFromXMLSerialzer<DO.Parcel>(parcelPath);
            //parcel1.Id = config.numofparcel;
            parcel1.Deleted = false;
            parcel1.Droneid = 0;
            parcelList.Add(parcel1);
            XMLTools.SaveListToXNLSerialzer<DO.Parcel>(parcelList, parcelPath);
            return Convert.ToString( parcel1.Id);
        }
        public Parcel getParcel(int id)
        {
            List<DO.Parcel> parcelList = XMLTools.LoadListFromXMLSerialzer<DO.Parcel>(parcelPath);
            int index = parcelList.FindIndex(t => t.Id == id && t.Deleted == false);
            if (index == -1)
                throw new Exception("DAL: Parcel with the same id not found...");
            return parcelList.FirstOrDefault(t => t.Id == id && t.Deleted == false);
        }
        public IEnumerable<Parcel> getParcelList(Predicate<Parcel> predicate)
        {
            List<Parcel> parcelList = XMLTools.LoadListFromXMLSerialzer<Parcel>(parcelPath);
            return from parcel in parcelList
                   where parcel.Deleted == false
                   select parcel;
        }
        public void UpDateSceduled(int numofparcel, int droneId)
        {
            List<DO.Parcel> parcelList = XMLTools.LoadListFromXMLSerialzer<DO.Parcel>(parcelPath);
            int index = parcelList.FindIndex(t => t.Id == numofparcel && t.Deleted == false);
            if (index == -1)
            {
                throw new IDdNotFoundExeption("DL: Parcel with the same id not found...");
            }
            Parcel p = parcelList.Find(item => item.Id == numofparcel && item.Deleted == false);
            List<DO.Drone> droneList = XMLTools.LoadListFromXMLSerialzer<DO.Drone>(dronePath);
            int index2 = droneList.FindIndex(t => t.Id == droneId && t.Deleted == false);
            if (index == -1)
            {
                throw new IDdNotFoundExeption("DL: Drone with the same id not found...");
            }
            p.Droneid = droneId;
            p.Scheduled = DateTime.Now;
            parcelList[index] = p;
            XMLTools.SaveListToXNLSerialzer<DO.Parcel>(parcelList, parcelPath);
        }
        public void UpdatePickedUp(int numofparcel)
        {
            List<DO.Parcel> parcelList = XMLTools.LoadListFromXMLSerialzer<DO.Parcel>(parcelPath);
            int index = parcelList.FindIndex(t => t.Id == numofparcel && t.Deleted == false);
            if (index == -1)
            {
                throw new IDdNotFoundExeption("DL: Parcel with the same id not found...");
            }
            Parcel p = parcelList.Find(item => item.Id == numofparcel && item.Deleted == false);
            p.Pickedup = DateTime.Now;
            parcelList[index] = p;
            XMLTools.SaveListToXNLSerialzer<DO.Parcel>(parcelList, dronePath);
        }
        public void UpDateDelieverd(int numofparcel)
        {
            List<DO.Parcel> parcelList = XMLTools.LoadListFromXMLSerialzer<DO.Parcel>(parcelPath);
            int index = parcelList.FindIndex(t => t.Id == numofparcel && t.Deleted == false);
            if (index == -1)
            {
                throw new IDdNotFoundExeption("DL: Parcel with the same id not found...");
            }
            Parcel p = parcelList.Find(item => item.Id == numofparcel && item.Deleted == false);
            p.Delieverd = DateTime.Now;
            parcelList[index] = p;
            XMLTools.SaveListToXNLSerialzer<DO.Parcel>(parcelList, dronePath);
        }
        public IEnumerable<Parcel> getParcelsWithNoDrone()
        {
            List<Parcel> parcelList = XMLTools.LoadListFromXMLSerialzer<Parcel>(parcelPath);
            return from parcel in parcelList
                   where parcel.Deleted == false&&parcel.Droneid ==0
                   select parcel;
        }
        public void deleteParcel(int id)
        {
            List<DO.Parcel> parcelList = XMLTools.LoadListFromXMLSerialzer<DO.Parcel>(parcelPath);
            int index = parcelList.FindIndex(t => t.Id == id && t.Deleted == false);
            if (index == -1)
            {
                throw new IDdNotFoundExeption("DL: Parcel with the same id not found...");
            }
            Parcel p = parcelList.Find(item => item.Id == id && item.Deleted == false);
            p.Deleted = true;
            parcelList[index] = p;
            XMLTools.SaveListToXNLSerialzer<DO.Parcel>(parcelList, dronePath);
        }
        public void updateParcel2(Parcel parcel1)
        {
            List<DO.Parcel> parcelList = XMLTools.LoadListFromXMLSerialzer<DO.Parcel>(parcelPath);
            parcel1.Deleted = false;
            parcelList.Add(parcel1);
            XMLTools.SaveListToXNLSerialzer<DO.Parcel>(parcelList, dronePath);

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
        public string ConvertLongitude(double value)
        {
            string str = Convert_To_Sexagesimal(value);
            return str + "S";

        }
        public string ConvertLatitude(double value)
        {
            string str = Convert_To_Sexagesimal(value);
            return str + "E";

        }
        public double DistanceStation(int numofstation, double x1, double y1)
        {
            double x2 = DistanceStationLAT(numofstation);
            double y2 = DistanceStationLONG(numofstation);
            double distance = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            return distance;
        }
        public double DistanceCustomer(int numofcustomer, double x1, double y1)
        {
            double x2 = DistanceCustomerLAT(numofcustomer);
            double y2 = DistanceCustomerLONG(numofcustomer);
            double distance = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            return distance;
        }
        public double DistanceStationLAT(int numofstation)
        {

            List<DO.Station> stationList = XMLTools.LoadListFromXMLSerialzer<DO.Station>(stationPath);
            int index = stationList.FindIndex(t => t.Id == id && t.Deleted == false);
            if (index == -1)
                throw new Exception("DAL: Station with the same id not found...");
            Station s= stationList.FirstOrDefault(t => t.Id == id && t.Deleted == false);
            return s.Latitude;
        }
        public double DistanceStationLONG(int numofstation)
        {
            List<DO.Station> stationList = XMLTools.LoadListFromXMLSerialzer<DO.Station>(stationPath);
            int index = stationList.FindIndex(t => t.Id == id && t.Deleted == false);
            if (index == -1)
                throw new Exception("DAL: Station with the same id not found...");
            Station s = stationList.FirstOrDefault(t => t.Id == id && t.Deleted == false);
            return s.Longitude;

        }
        //to use customer's
        public double DistanceCustomerLAT(int numofcustomer)
        {
            List<DO.Station> stationList = XMLTools.LoadListFromXMLSerialzer<DO.Station>(stationPath);
            int index = stationList.FindIndex(t => t.Id == id && t.Deleted == false);
            if (index == -1)
                throw new Exception("DAL: Station with the same id not found...");
            return stationList.FirstOrDefault(t => t.Id == id && t.Deleted == false);
        }
        public double DistanceCustomerLONG(int numofcustomer)
        {
            List<DO.Station> stationList = XMLTools.LoadListFromXMLSerialzer<DO.Station>(stationPath);
            int index = stationList.FindIndex(t => t.Id == id && t.Deleted == false);
            if (index == -1)
                throw new Exception("DAL: Station with the same id not found...");
            return stationList.FirstOrDefault(t => t.Id == id && t.Deleted == false);
        }

        public IEnumerable<Station> getStationList(Predicate<Station> predicate)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region PowerConsumptionRequest
        public double[] GetElectricity()
        {
            var temp1 = XMLTools.LoadListFromXmlElement(powerConsumptionRequest);
            var temp2 = temp1.Element("electricityRates").Elements();
            var temp3 = temp2.Select(e => Convert.ToDouble(e.Value)).ToArray();
            return temp3;

        }
        #endregion

    }
}
