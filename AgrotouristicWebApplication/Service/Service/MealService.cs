using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.IRepo;
using DomainModel.Models;

namespace Service.Service
{
    public class MealService : IMealService
    {
        private readonly IMealRepository mealRepository = null;
        private readonly IReservationHouseRepository reservationHouseRepository = null;

        public MealService(IMealRepository mealRepository, IReservationHouseRepository reservationHouseRepository)
        {
            this.mealRepository = mealRepository;
            this.reservationHouseRepository = reservationHouseRepository;
        }

        public void AddMeal(Meal meal)
        {
            this.mealRepository.AddMeal(meal);
            this.mealRepository.SaveChanges();
        }

        public int countHousesWithGivenMeal(int id)
        {
            IList<Reservation_House> reservationHouses = this.reservationHouseRepository
                                                            .GetReservationsHouses();
            int houses = reservationHouses.Where(item => item.MealId.Equals(id)).Count();
            return houses;
        }

        public Meal GetMealById(int id)
        {
            Meal meal = this.mealRepository.GetMealById(id);
            return meal;
        }

        public IList<Meal> GetMeals()
        {
            IList<Meal> meals = this.mealRepository.GetMeals();
            return meals;
        }

        public void RemoveMeal(Meal meal)
        {
            this.mealRepository.RemoveMeal(meal);
            this.mealRepository.SaveChanges();
        }

        public void UpdateMeal(Meal meal, byte[] rowVersion)
        {
            this.mealRepository.UpdateMeal(meal, rowVersion);
            this.mealRepository.SaveChanges();
        }
    }
}
