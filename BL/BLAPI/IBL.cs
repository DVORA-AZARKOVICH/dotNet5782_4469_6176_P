﻿using BL.BO;
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
        Customer getCustomer(int num);
        IEnumerable<CustomerForList> getCustomerList();
        IEnumerable<CustomerForList> getCustomerList(Predicate<CustomerForList> predicate);
        #endregion

        #region Drone
        void AddDrone(Drone d, int idstation);
        DroneToList UpdateInMaintence(int count, DroneToList d, IEnumerable<DO.Station> stationlist);
        DroneToList UpdateFree(DroneToList d, IEnumerable<DO.Parcel> parcellist, IEnumerable<DO.Customer> customerlist, IEnumerable<DO.Station> stationlist);
        void UpdateDrone(int numofdrone, string model);
        void UpdateDroneToCharge(int numofdrone);
        void UpdateReleseDroneFromCharge(int numofdrone, DateTime? time1);
        public void UpdateParcelToDrone(int numofdrone);
        Drone getDrone(int num);
        ParcelInTransfer convertToParcelInTransfer(DO.Parcel p);
        List<DroneToList> getdroneList();
        IEnumerable<DroneToList> getdroneList(Predicate<DroneToList> predicate);
        void NextState(int id, ref bool flagcontinue,ref int parcelid);
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
        Parcel getParcel(int num);
        IEnumerable<ParcelForList> getParcelList();
        IEnumerable<ParcelForList> getParcelList(Predicate<ParcelForList> pradicate);
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
