
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DLXML
{
    sealed class DLXML : IDL
    {
        static readonly IDL instance = new DLXML();
        static DLXML() { }
        DLXML() { }
        public static IDL Instance { get => instance; }
        #region DS XML Files
        string customerPath = @"CustomerXml.xml";//Xelement
        string parcelPath = @"ParcelXml.xml";//XMLSerialzer
        string dronePath = @"DroneXml.xnml";//XMLSerialzer
        string dronchargePath = @"DroneChargeXml.xml";//XMLSerialzer
        string stationPath = @"StationXml.xml";//XMLSerialzer\
        #endregion
        void addStation(Station station1)
        {

        }
        Station getStation(int id)
        {

        }
        public IEnumerable <DLAPI.DO.Station> getStationList()
        {
            List<Station> ListStations = XMLTools.LoadListFromXMLSerialzer<Station>(stationPath);
            return from station in ListStations
                   select station;
        }
        IEnumerable<Station> getFreeCharges();
        void deleteStation(int id);
    }
}
