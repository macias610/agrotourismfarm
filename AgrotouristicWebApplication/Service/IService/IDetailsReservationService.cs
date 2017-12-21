using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewModel;

namespace Service.IService
{
    public interface IDetailsReservationService
    {
        Reservation GetReservationById(int id);
        IList<SelectListItem> GetWeeksFromSelectedTerm(DateTime startDate, DateTime endDate);
        IList<SelectListItem> GetAvaiableDatesInWeek(string term);
   
    }
}
