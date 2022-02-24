using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dal
{
    internal static class DataSource
    {
        public static List<Drone> dronelist = new List<Drone>();
        public static List<Station> stationlist = new List<Station>();
        public static List<Customer> customelist = new List<Customer>();
        public static List<Parcel> parcellist = new List<Parcel>();
        public static List<Dronecharge> dronechargelist = new List<Dronecharge>();
        public static List<double> power = new List<double>();

        static XElement CustomerRoot;
        static string DataDirectory = @"data";
        static string customerPath = @"CustomerXml.xml";//Xelement
        static string parcelPath = @"ParcelXml.xml";//XMLSerialzer
        static string dronePath = @"DroneXml.xml";//XMLSerialzer
        static string dronchargePath = @"DroneChargeXml.xml";//XMLSerialzer
        static string stationPath = @"StationXml.xml";//XMLSerialzer\
        static string powerConsumptionRequest = @" PowerConsumptionRequestXml.xml";//XMLSerialzer\

        static DataSource()
        {
            Initialize();
            XMLTools.SaveListToXNLSerialzer(dronelist, dronePath);
            XMLTools.SaveListToXNLSerialzer(parcellist, parcelPath);
            XMLTools.SaveListToXNLSerialzer(customelist, customerPath);
            XMLTools.SaveListToXNLSerialzer(dronechargelist, dronchargePath);
            XMLTools.SaveListToXNLSerialzer(stationlist, stationPath);
            XMLTools.SaveListToXNLSerialzer(power, powerConsumptionRequest);
        }
        public class Config
        {
            public static int idParcel = 1000;
            //0-false,1-true
            public static double available = 0;
            public static double light = 2;
            public static double medium = 4;
            public static double heavy = 6;
            public static double chargingRate = 0.1;

        }
        internal static void Initialize()
        {


                Random rand = new Random();
                //initilizig 5 drones
                int iddrone = 1000000;
                string m = "a";
                for (int i = 0; i < 5; i++)
                {
                    Drone d = new Drone();
                    d.Id = iddrone++;
                    //d.Model = Convert.ToString(rand.Next(65, 91));
                    d.Model = m;
                    WeightCategories weight = (WeightCategories)(new Random().Next((int)WeightCategories.heavy));
                    d.Maxweight = weight;
                    d.Deleted = false;
                    dronelist.Add(d);
                }
                //initilizig 2 stations
                int idstation = 10000000;
                for (int i = 0; i < 2; i++)
                {
                    Station s = new Station();
                    s.Id = idstation++;
                    s.Name = "station" + Convert.ToString(i + 65);
                    double x, y;
                    do
                    {
                        x = rand.Next(33, 37);
                        y = rand.NextDouble();
                    }
                    while (x == 36 && y > 0.3 || x == 33 && y < 0.7);
                    s.Latitude = x + y;
                    do
                    {
                        x = rand.Next(29, 34);
                        y = rand.NextDouble();
                    }
                    while (x == 29 && y < 0.3 || x == 33 && y > 0.5);
                    s.Longitude = x + y;
                    s.Chargslot = 6;
                    s.Deleted = false;
                    stationlist.Add(s);


                }
                //intilizing 10 customers
                string phonenumber = "0533128566";
                int idcustforintilizing = 325036176;
                for (int i = 0; i < 10; i++)
                {
                    Customer c = new Customer();
                    c.Id = idcustforintilizing++;
                    c.Name = "customr: " + Convert.ToString(i + 97);
                    c.Phone = phonenumber;
                    int temp222 = Convert.ToInt32(phonenumber);
                    temp222++;
                    phonenumber = Convert.ToString(temp222);
                    double x, y;
                    do
                    {
                        x = rand.Next(33, 37);
                        y = rand.NextDouble();
                    }
                    while (x == 36 && y > 0.3 || x == 33 && y < 0.7);
                    c.Latitude = x + y;
                    do
                    {
                        x = rand.Next(29, 34);
                        y = rand.NextDouble();
                    }
                    while (x == 29 && y < 0.3 || x == 33 && y > 0.5);
                    c.Longitude = x + y;
                    c.Deleted = false;
                    customelist.Add(c);
                }
                //לבדוק את הקונברט של המספר טלפון
                int idsenderforintilizing = 325036176;
                int idtargetforintilizing = 325036177;
                iddrone = 1000000;
            for (int i = 0; i < 10; i++)
            {
                if (i == 9)
                    idtargetforintilizing = 325036176;
                Parcel p = new Parcel();
                p.Id = Config.idParcel++;
                p.Senderid = idsenderforintilizing++;
                p.Targetid = idtargetforintilizing++;
                WeightCategories weight = (WeightCategories)(new Random().Next((int)WeightCategories.heavy));
                p.Weightcategory = weight;
                Priorities prio = (Priorities)(new Random().Next((int)Priorities.urgent));
                p.Priority = prio;
                p.Requested = new DateTime();
                p.Requested = DateTime.Now;
                if (i % 2 == 0)
                {
                    p.Scheduled = new DateTime();
                    p.Scheduled = DateTime.Now;
                    p.Droneid = iddrone++;
                }
                if (i % 4 == 0)
                {
                    p.Pickedup = new DateTime();
                    p.Pickedup = DateTime.Now;
                    p.Pickedup.Value.AddMinutes(30);
                    if (i == 4 || i == 8)
                    {
                        p.Delieverd = new DateTime();
                        p.Delieverd = p.Pickedup;
                        p.Delieverd.Value.AddHours(2);
                    }
                }
                p.Deleted = false;
                parcellist.Add(p);
            }
            power.Add(0);
            power.Add(1);
            power.Add(5.5);
            power.Add(7.5);
            power.Add(10);
            power.Add(2);




        }
    }
}
