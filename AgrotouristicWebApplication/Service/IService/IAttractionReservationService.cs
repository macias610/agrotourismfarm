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
    public interface IAttractionReservationService
    {
        IList<SelectListItem> GetAvaiableAttractions();
        Attraction GetAttractionByName(string name);
        Dictionary<DateTime, List<string>> GetAttractionsInGivenWeek(string term, Dictionary<DateTime, List<string>> dictionary);
        Dictionary<DateTime, List<string>> RetreiveAttractionsInGivenWeek(string term, int id);
        int GetMaxRowsToTableAttractions(Dictionary<DateTime, List<string>> dictionary);
        Dictionary<DateTime, List<string>> InitializeDictionaryForAssignedAttractions(DateTime startDate, DateTime endDate);
        void SaveAssignedAttractions(int id, NewReservation reservation);
        void ChangeAssignedAttractions(int id, NewReservation reservation);
    }
}
