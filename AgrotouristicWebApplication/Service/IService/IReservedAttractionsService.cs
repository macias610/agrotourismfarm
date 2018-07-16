using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Service.IService
{
    public interface IReservedAttractionsService
    {
        IList<SelectListItem> GetWeeksForAttractions(DateTime date);
        IList<SelectListItem> GetAvaiableDatesInWeek(string term);
        Dictionary<DateTime, List<string>> GetAttractionsInstructorsInGivenWeek(string term);
        Dictionary<DateTime, List<string>> GetClassesInstructorInGivenWeek(string term, string idInstructor);
        IList<SelectListItem> GetAvaiableInstructors(int id, string attractionName);
        AttractionReservation GetAttractionReservationById(int id);
        AttractionReservationWorker GetAttractionReservationWorkerById(int id);
        int GetMaxRowsToTableAttractions(Dictionary<DateTime, List<string>> dictionary);
        bool checkStateInstructorToAttraction(AttractionReservationWorker attractionReservationWorker);
        User GetInstructorAssignedToAttraction(string id);
        string GetInstructorsForReservedAttraction(int id);
        string RetreiveInstructorsAssignedToAttraction(int id, string attractionName);
        void RemoveAssignedInstructorAttraction(AttractionReservationWorker attractionReservationWorker);
        void AssignInstructorToAttraction(AttractionReservationWorker attractionReservationWorker);
    }
}
