﻿using Repository.Models;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Repository.IRepo
{
    public interface IReservationDetailsRepository
    {
        List<House> GetHousesForReservation(int id);
        Meal GetHouseMealForReservation(int reservationId, int houseId);
        List<Participant> GetParticipantsHouseForReservation(int reservationId, int houseId);
        List<House> GetAvaiableHousesInTerm(DateTime startDate, DateTime endDate);
        List<SelectListItem> GetNamesAvaiableHouses(List<House> houses);
        House GetHouseByName(string name);
        List<SelectListItem> GetNamesAvaiableMeals();
        List<Participant> CopyParticipantsData(List<Participant> tagetList, List<Participant> actualList);
        List<SelectListItem> GetSelectedHousesMeals(Dictionary<string, int> dictionary, bool longVersion);

        void SaveSelectedHouses(NewReservation reservation, List<string> selectedHouses);
        void SaveAssignedMealsToHouses(NewReservation reservation, List<string> selectedMeals);
        void ClearParticipantsFormular(NewReservation reservation);
        bool ValidateFormularParticipants(Dictionary<string, List<Participant>> dictionary);
    }
}