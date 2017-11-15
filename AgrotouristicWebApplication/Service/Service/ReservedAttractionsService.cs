using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Models;
using System.Web.Mvc;
using Repository.IRepo;

namespace Service.Service
{
    public class ReservedAttractionsService : IReservedAttractionsService
    {
        private readonly IReservedAttractionsRepository reservedAttractionsRepository = null;

        public ReservedAttractionsService(IReservedAttractionsRepository reservedAttractionsRepository)
        {
            this.reservedAttractionsRepository = reservedAttractionsRepository;
        }

        public void AssignInstructorToAttraction(Attraction_Reservation_Worker attractionReservationWorker)
        {
            this.reservedAttractionsRepository.AssignInstructorToAttraction(attractionReservationWorker);
            this.reservedAttractionsRepository.SaveChanges();
        }

        public bool checkStateInstructorToAttraction(Attraction_Reservation_Worker attractionReservationWorker)
        {
            bool result = this.reservedAttractionsRepository.checkStateInstructorToAttraction(attractionReservationWorker);
            return result;
        }

        public Attraction_Reservation GetAttractionReservationById(int id)
        {
            Attraction_Reservation attractionReservation = this.reservedAttractionsRepository.GetAttractionReservationById(id);
            return attractionReservation;
        }

        public Attraction_Reservation_Worker GetAttractionReservationWorkerById(int id)
        {
            Attraction_Reservation_Worker attractionReservation = this.reservedAttractionsRepository.GetAttractionReservationWorkerById(id);
            return attractionReservation;
        }

        public Dictionary<DateTime, List<string>> GetAttractionsInstructorsInGivenWeek(string term)
        {
            Dictionary<DateTime, List<string>> dictionary = this.reservedAttractionsRepository.GetAttractionsInstructorsInGivenWeek(term);
            return dictionary;
        }

        public IList<SelectListItem> GetAvaiableDatesInWeek(string term)
        {
            IList<SelectListItem> dates = this.reservedAttractionsRepository.GetAvaiableDatesInWeek(term);
            return dates;
        }

        public IList<SelectListItem> GetAvaiableInstructors(int id, string attractionName)
        {
            IList<SelectListItem> instructors = this.reservedAttractionsRepository.GetAvaiableInstructors(id, attractionName);
            return instructors;
        }

        public Dictionary<DateTime, List<string>> GetClassesInstructorInGivenWeek(string term, string idInstructor)
        {
            Dictionary<DateTime, List<string>> dictionary = this.reservedAttractionsRepository.GetClassesInstructorInGivenWeek(term, idInstructor);
            return dictionary;
        }

        public User GetInstructorAssignedToAttraction(string id)
        {
            User user = this.reservedAttractionsRepository.GetInstructorAssignedToAttraction(id);
            return user;
        }

        public string GetInstructorsForReservedAttraction(int id)
        {
            string instructors = this.reservedAttractionsRepository.GetInstructorsForReservedAttraction(id);
            return instructors;
        }

        public int GetMaxRowsToTableAttractions(Dictionary<DateTime, List<string>> dictionary)
        {
            int quantity = this.reservedAttractionsRepository.GetMaxRowsToTableAttractions(dictionary);
            return quantity;
        }

        public IList<SelectListItem> GetWeeksForAttractions(DateTime date)
        {
            IList<SelectListItem> weeks = this.reservedAttractionsRepository.GetWeeksForAttractions(date);
            return weeks;
        }

        public void RemoveAssignedInstructorAttraction(Attraction_Reservation_Worker attractionReservationWorker)
        {
            this.reservedAttractionsRepository.RemoveAssignedInstructorAttraction(attractionReservationWorker);
            this.reservedAttractionsRepository.SaveChanges();
        }

        public string RetreiveInstructorsAssignedToAttraction(int id, string attractionName)
        {
            string instructors = this.reservedAttractionsRepository.RetreiveInstructorsAssignedToAttraction(id, attractionName);
            return instructors;
        }
    }
}
