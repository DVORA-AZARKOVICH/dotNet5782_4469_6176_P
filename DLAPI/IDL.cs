using DLAPI.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLAPI
{
    public interface IDL
    {
        #region Customer
        void addCustomer(Customer item);
        Customer getCustomer(int id);
        IEnumerable<Customer> getCustomerList(Predicate<Customer> predicate);
        void deleteCustomer(int id);
        #endregion

        #region Drone
        void addDrone(Drone item);
        Drone getDrone(int id);
        IEnumerable<Drone> getDroneList(Predicate<Drone> predicate);
        void UpDatenotcharging(int numofdrone);
        void UpDateCharging(int numofdrone, int numofstation);
        double[] PowerConsumptionRequest();
        void deleteDrone(int id);
        #endregion

        #region DroneCharge
        void addDroneCharge(Dronecharge dronechatge1);
        void deleteDroneCharge(int id);
        IEnumerable<Dronecharge> getDroneChargeList(Predicate<Dronecharge> predicate);
        Dronecharge getDroneCharge(int iddrone);
        #endregion

        #region Parcel
        string addParcel(Parcel parcel1);
        Parcel getParcel(int id);
        IEnumerable<Parcel> getParcelList(Predicate<Parcel> predicate);
        void UpDateSceduled(int numofparcel, int droneId);
        void UpdatePickedUp(int numofparcel);
        void UpDateDelieverd(int numofparcel);
        IEnumerable<Parcel> getParcelsWithNoDrone();
        void deleteParcel(int id);
        string updateParcel2(Parcel parcel1);
        #endregion

        #region Station
        void addStation(Station station1);

        Station getStation(int id);

        IEnumerable<Station> getStationList(Predicate<Station> predicate);

        IEnumerable<Station> getFreeCharges();
        void deleteStation(int id);

        #endregion

        #region Sexagesimal
        string ConvertLongitude(double value);
        string ConvertLatitude(double value);
        double DistanceStation(int numofstation, double x1, double y1);
        double DistanceCustomer(int numofcustomer, double x1, double y1);
        double DistanceStationLAT(int numofstation);
        double DistanceStationLONG(int numofstation);
        double DistanceCustomerLAT(int numofcustomer);
        double DistanceCustomerLONG(int numofcustomer);
        #endregion
    }
}
