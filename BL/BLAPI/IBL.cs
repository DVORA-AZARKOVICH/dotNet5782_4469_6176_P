using BL.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLAPI
{
    public interface IBL
    {

        #region Customer
        void AddCustomer(Customer c);
        void UpdateCustomerName(int numofcustomer, string name);
        void UpdateCustomerPhone(int numofcustomer, string phone);
        #endregion

        #region Drone
        public void AddDrone(Drone d, int idstation);
        public DroneToList UpdateInMaintence(int count, DroneToList d, IEnumerable<DLAPI.DO.Station> stationlist);
        public DroneToList UpdateFree(DroneToList d, IEnumerable<DLAPI.DO.Parcel> parcellist, IEnumerable<DLAPI.DO.Customer> customerlist, IEnumerable<DLAPI.DO.Station> stationlist);
        public void UpdateDrone(int numofdrone, string model);
        public void UpdateDroneToCharge(int numofdrone);
        public void UpdateReleseDroneFromCharge(int numofdrone, DateTime? time1);
        public void UpdateParcelToDrone(int numofdrone);
        public Drone getDrone(int num);
        public ParcelInTransfer convertToParcelInTransfer(DLAPI.DO.Parcel p);
        public List<DroneToList> getdroneList();
        public IEnumerable<DroneToList> getdroneList(Predicate<DroneToList> predicate);
        #endregion

        #region Station
        void AddStation(BL.BO.Station s);
        DLAPI.DO.Station ClosestStation(IEnumerable<DLAPI.DO.Station> stationlist, Location c);
        void UpdatStationChargeslots(int numofstation, int numofchargeslots);
        void UpdateStationName(int numofstation, string name);
        Station getStation(int num);
        IEnumerable<StationToList> getStationList();
        StationToList convertToStationList(DLAPI.DO.Station s);
        IEnumerable<StationToList> getStationWithFreeChargingSlotsList();

        #endregion

        #region Parcel
        void AddParcel(Parcel c);
        DLAPI.DO.Parcel? CheckByWeight(IEnumerable<DLAPI.DO.Parcel> parcellist, WeightCategories weight, Location c, double mindistance, DroneToList dts);
        DLAPI.DO.Parcel? CheckByDistance(IEnumerable<DLAPI.DO.Parcel> parcellist, Location c, double mindistance, DroneToList dts);
        void UpdatePickedByDrone(int numofdrone);
        void UpdateDeliverdByDrone(int numofdrone);
        #endregion

        #region CalculationAndChecks
        double MinCharge(double sumdistancemin, WeightCategories weight);
        bool ChecksPhoneNumber(string phone);
        int SumDigits(int x);
        int LastDigitID(int x);
        bool CheckIdentityNumber(int id);
        double SumCharge(DLAPI.DO.Parcel? itemparcel, DroneToList updatedrone);


        #endregion
    }
}
