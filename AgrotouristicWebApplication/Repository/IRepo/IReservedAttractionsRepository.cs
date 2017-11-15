using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Repository.IRepo
{
    public interface IReservedAttractionsRepository
    {
        List<SelectListItem> GetWeeksForAttractions(DateTime date);
        List<SelectListItem> GetAvaiableDatesInWeek(string term);
        Dictionary<DateTime, List<string>> GetAttractionsInstructorsInGivenWeek(string term);
        Dictionary<DateTime, List<string>> GetClassesInstructorInGivenWeek(string term,string idInstructor);
        List<SelectListItem> GetAvaiableInstructors(int id, string attractionName);
        Attraction_Reservation GetAttractionReservationById(int id);
        Attraction_Reservation_Worker GetAttractionReservationWorkerById(int id);
        int GetMaxRowsToTableAttractions(Dictionary<DateTime, List<string>> dictionary);
        bool checkStateInstructorToAttraction(Attraction_Reservation_Worker attractionReservationWorker);
        User GetInstructorAssignedToAttraction(string id);
        string GetInstructorsForReservedAttraction(int id);
        string RetreiveInstructorsAssignedToAttraction(int id,string attractionName);
        void RemoveAssignedInstructorAttraction(Attraction_Reservation_Worker attractionReservationWorker);
        void AssignInstructorToAttraction(Attraction_Reservation_Worker attractionReservationWorker);
        void SaveChanges();

    }
}
