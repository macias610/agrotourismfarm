using Microsoft.AspNet.Identity.EntityFramework;
using Repository.IRepo;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repository.Repo
{
    public class ReservedAttractionsRepository : IReservedAttractionsRepository
    {
        private readonly IAgrotourismContext db;

        public ReservedAttractionsRepository(IAgrotourismContext db)
        {
            this.db = db;
        }

        public List<SelectListItem> GetWeeksForAttractions(DateTime date)
        {
            int differenceToMonday = DayOfWeek.Monday - date.DayOfWeek;
            DateTime monday = date.AddDays(differenceToMonday);
            List<string> weeks = new List<string>();
            weeks.Add("-");
            for(int i=0;i<2;i++)
            {
                weeks.Add(monday.ToShortDateString() + ";" + monday.AddDays(6).ToShortDateString());
                monday = monday.AddDays(7);
            }
            List<SelectListItem> listItem = weeks.Select(item => new SelectListItem { Text = item, Value = item, Selected = weeks.First().Equals(item)?true:false }).ToList();
            return listItem;
        }

        public List<SelectListItem> GetAvaiableDatesInWeek(string term)
        {
            List<DateTime> result = new List<DateTime>();
            for (DateTime start = DateTime.Parse(term.Split(';')[0]); start.CompareTo(DateTime.Parse(term.Split(';')[1])) <= 0; start = start.AddDays(1))
            {
                result.Add(start);
            }
            List<SelectListItem> selectList = result.Select(item => new SelectListItem { Text = item.ToShortDateString(), Value = item.ToShortDateString(), Selected = true }).ToList();
            return selectList;
        }

        public Dictionary<DateTime, List<string>> GetAttractionsInstructorsInGivenWeek(string term)
        {
            DateTime start = DateTime.Parse(term.Split(';')[0]);
            DateTime end = DateTime.Parse(term.Split(';')[1]);
            List<Attraction_Reservation> attractionsReservation = (from attrRes in db.Attractions_Reservations
                                                                   where attrRes.TermAffair.CompareTo(start) >= 0
                                                                   && attrRes.TermAffair.CompareTo(end) <= 0
                                                                   select attrRes).ToList();
            Dictionary<DateTime, List<string>> dictionary = new Dictionary<DateTime, List<string>>();
            for (DateTime st = start; st <= end; st = st.AddDays(1))
            {
                dictionary.Add(st, new List<string>());
            }
            HashSet<DateTime> dates = new HashSet<DateTime>();
            attractionsReservation.ForEach(item => dates.Add(item.TermAffair));
            foreach (DateTime date in dates)
            {
                List<string> result = new List<string>();
                attractionsReservation.Where(x => x.TermAffair.Equals(date)).ToList().ForEach(x => result.Add(x.Attraction.Name + ',' + x.QuantityParticipant + "(" + GetWorkersForReservedAttraction(x.Id) +")"));
                dictionary[date] = result;
            }
            return dictionary;
        }

        public int GetMaxRowsToTableAttractions(Dictionary<DateTime, List<string>> dictionary)
        {
            int result = 0;
            foreach (KeyValuePair<DateTime, List<string>> item in dictionary)
            {
                if (item.Value.Count > result)
                {
                    result = item.Value.Count;
                }
            }
            if (result == 0)
            {
                result++;
            }
            return result;
        }

        public string GetWorkersForReservedAttraction(int id)
        {
            Attraction_Reservation attractionReservation = db.Attractions_Reservations.Find(id);
            Dictionary<int, User> workers = new Dictionary<int, User>();
            attractionReservation.Attraction_Reservation_Worker.ToList().ForEach(x => workers.Add(x.Id,x.Worker));
            List<string> listResult = new List<string>();
            workers.ToList().ForEach(x => listResult.Add(x.Value.UserName+"|"+x.Value.Name+","+x.Value.Surname+"-"+x.Key+";"));
            string result = string.Empty;
            listResult.ForEach(x => result += x);
            return result;
        }
    }
}