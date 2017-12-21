using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModel;

namespace Service.IService
{
    public interface IMealReservationService
    {
        Meal GetHouseMealForReservation(int reservationId, int houseId);
        IList<SelectListItem> GetNamesAvaiableMeals();
        void ConfirmAssignedMealsToHouses(NewReservation reservation, IList<string> selectedMeals);
        IList<SelectListItem> GetSelectedHousesMeals(Dictionary<string, int> dictionary, bool longVersion);
    }
}
