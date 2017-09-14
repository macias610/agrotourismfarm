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
        int GetMaxRowsToTableAttractions(Dictionary<DateTime, List<string>> dictionary);
        string GetWorkersForReservedAttraction(int id);
    }
}
