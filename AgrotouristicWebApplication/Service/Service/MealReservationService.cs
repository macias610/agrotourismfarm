using DomainModel.Models;
using Repository.IRepo;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModel;

namespace Service.Service
{
    public class MealReservationService :IMealReservationService
    {
        private readonly IMealRepository mealRepository = null;
        private readonly IReservationHouseRepository reservationHouseRepository = null;

        public MealReservationService(IMealRepository mealRepository, IReservationHouseRepository reservationHouseRepository)
        {
            this.mealRepository = mealRepository;
            this.reservationHouseRepository = reservationHouseRepository;
        }

        public IList<SelectListItem> GetSelectedHousesMeals(Dictionary<string, int> dictionary, bool longVersion)
        {
            List<string> result = new List<string>();
            foreach (KeyValuePair<string, int> item in dictionary)
            {
                Meal meal = this.mealRepository.GetMealById(item.Value);
                string mealFullName = meal.Type + "(" + meal.Price + "[zł]-os./dzień)";
                result.Add(item.Key + mealFullName);
            }
            List<SelectListItem> list = result.Select(item => new SelectListItem { Text = longVersion ? item : item.Split(';')[0] + ';', Value = longVersion ? item : item.Split(';')[0] + ';', Selected = true }).ToList();
            return list;
        }

        public void ConfirmAssignedMealsToHouses(NewReservation reservation, IList<string> selectedMeals)
        {
            foreach (string houseMeal in selectedMeals)
            {
                string houseName = houseMeal.Split(';')[0] + ';';
                string mealType = houseMeal.Split(';')[1].Split('(')[0];
                int mealId = this.mealRepository.GetMealByType(mealType).Id;
                reservation.AssignedHousesMeals[houseName] = mealId;
            }
        }

        public IList<SelectListItem> GetNamesAvaiableMeals()
        {
            IList<Meal> meals = this.mealRepository.GetMeals();
            IList<SelectListItem> selectList = meals.Select(meal => new SelectListItem { Value = meal.Type + "(" + meal.Price + "[zł]-os./dzień" + ")", Text = meal.Type + "(" + meal.Price + "[zł]-os./dzień" + ")" })
                                                .ToList();
            return selectList;
        }

        public Meal GetHouseMealForReservation(int reservationId, int houseId)
        {
            IList<Reservation_House> reservationHouses = this.reservationHouseRepository.GetReservationHousesOfReservationId(reservationId);
            Meal meal = reservationHouses
                            .Where(item => item.HouseId.Equals(houseId))
                            .Select(item => item.Meal).SingleOrDefault();
            return meal;
        }
    }
}
