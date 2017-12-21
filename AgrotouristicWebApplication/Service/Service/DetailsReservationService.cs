using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.Models;
using System.Web;
using System.Web.Mvc;
using ViewModel;
using Repository.IRepo;
using System.ComponentModel.DataAnnotations;

namespace Service.Service
{
    public class DetailsReservationService : IDetailsReservationService
    {
        private readonly IReservationRepository reservationRepository = null;

        public DetailsReservationService(IReservationRepository reservationRepository)
        {
            this.reservationRepository = reservationRepository;
        }

        public IList<SelectListItem> GetAvaiableDatesInWeek(string term)
        {
            List<DateTime> result = new List<DateTime>();
            for (DateTime start = DateTime.Parse(term.Split(';')[0]); start.CompareTo(DateTime.Parse(term.Split(';')[1])) <= 0; start = start.AddDays(1))
            {
                result.Add(start);
            }
            List<SelectListItem> selectList = result.Select(item => new SelectListItem { Text = item.ToShortDateString(), Value = item.ToShortDateString(), Selected = true }).ToList();
            return selectList;
        }

        public Reservation GetReservationById(int id)
        {
            Reservation reservation = this.reservationRepository.GetReservationById(id);
            return reservation;
        }

        public IList<SelectListItem> GetWeeksFromSelectedTerm(DateTime startDate, DateTime endDate)
        {
            IList<string> list = new List<string>(new string[] { "-" });
            int counter = 0;
            int daysToDisplay = 6;
            for (DateTime start = startDate, tmp = startDate; tmp.CompareTo(endDate) <= 0; tmp = tmp.AddDays(1), counter++)
            {
                if (counter == daysToDisplay || tmp.CompareTo(endDate) == 0)
                {
                    counter = 0;
                    list.Add(start.ToShortDateString() + ";" + tmp.ToShortDateString());
                    start = tmp.AddDays(1);
                }
            }
            List<SelectListItem> selectList = list.Select(item => new SelectListItem { Text = item, Value = item, Selected = item.Equals("-") ? true : false }).ToList();
            return selectList;
        }

    }
}
