using BL.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLApi
{
    public interface IBL
    {

        #region Customer
        void AddCustomer(Customer c);
        void UpdateCustomerName(int numofcustomer, string name);
        void UpdateCustomerPhone(int numofcustomer, string phone);
        public Customer getCustomer(int num);
        public List<CustomerForList> getCustomerList();
        public IEnumerable<CustomerForList> getCustomerList(Predicate<DroneToList> predicate);
        #endregion

        #region Drone
        public void AddDrone(Drone d, int idstation);
        public DroneToList UpdateInMaintence(int count, DroneToList d, IEnumerable<DO.Station> stationlist);
        public DroneToList UpdateFree(DroneToList d, IEnumerable<DO.Parcel> parcellist, IEnumerable<DO.Customer> customerlist, IEnumerable<DO.Station> stationlist);
        public void UpdateDrone(int numofdrone, string model);
        public void UpdateDroneToCharge(int numofdrone);
        public void UpdateReleseDroneFromCharge(int numofdrone, DateTime? time1);
        public void UpdateParcelToDrone(int numofdrone);
        public Drone getDrone(int num);
        public ParcelInTransfer convertToParcelInTransfer(DO.Parcel p);
        public List<DroneToList> getdroneList();
        public IEnumerable<DroneToList> getdroneList(Predicate<DroneToList> predicate);
        #endregion

        #region Station
        void AddStation(BL.BO.Station s);
        DO.Station ClosestStation(IEnumerable<DO.Station> stationlist, Location c);
        void UpdatStationChargeslots(int numofstation, int numofchargeslots);
        void UpdateStationName(int numofstation, string name);
        Station getStation(int num);
        IEnumerable<StationToList> getStationList();
        IEnumerable<StationToList> getStationList(Predicate<StationToList> predicate);
        StationToList convertToStationList(DO.Station s);
        IEnumerable<StationToList> getStationWithFreeChargingSlotsList();

        #endregion

        #region Parcel
        void AddParcel(Parcel c);
        DO.Parcel? CheckByWeight(IEnumerable<DO.Parcel> parcellist, WeightCategories weight, Location c, double mindistance, DroneToList dts);
        DO.Parcel? CheckByDistance(IEnumerable<DO.Parcel> parcellist, Location c, double mindistance, DroneToList dts);
        void UpdatePickedByDrone(int numofdrone);
        void UpdateDeliverdByDrone(int numofdrone);
        IEnumerable<ParcelForList> getParcelList();
        #endregion

        #region CalculationAndChecks
        double MinCharge(double sumdistancemin, WeightCategories weight);
        bool ChecksPhoneNumber(string phone);
        int SumDigits(int x);
        int LastDigitID(int x);
        bool CheckIdentityNumber(int id);
        double SumCharge(DO.Parcel? itemparcel, DroneToList updatedrone);


        #endregion
    }
}
