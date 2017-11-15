using DomainModel.Models;
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
            int numberOfWeeks = 3;
            for(int i=0;i<numberOfWeeks;i++)
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
                                                                   join reservation in db.Reservations on attrRes.ReservationId equals reservation.Id
                                                                   where !reservation.Status.Equals("oczekiwanie")
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
                attractionsReservation.Where(x => x.TermAffair.Equals(date)).ToList().ForEach(x => result.Add(x.Id+";"+ x.Attraction.Name + ',' + x.QuantityParticipant + "(" + GetInstructorsForReservedAttraction(x.Id) +")"));
                dictionary[date] = result;
            }
            return dictionary;
        }

        public Dictionary<DateTime, List<string>> GetClassesInstructorInGivenWeek(string term, string idInstructor)
        {
            DateTime start = DateTime.Parse(term.Split(';')[0]);
            DateTime end = DateTime.Parse(term.Split(';')[1]);
            List<Attraction_Reservation> instructorClassesInWeek = (from attrResWork in db.Attractions_Reservations_Workers
                                                                   where attrResWork.WorkerId.Equals(idInstructor)
                                                                   join attrRes in db.Attractions_Reservations on attrResWork.Attraction_ReservationId equals attrRes.Id
                                                                   where attrRes.TermAffair.CompareTo(start) >= 0
                                                                   && attrRes.TermAffair.CompareTo(end) <= 0
                                                                   select attrRes).ToList();
            Dictionary<DateTime, List<string>> dictionary = new Dictionary<DateTime, List<string>>();
            for (DateTime st = start; st <= end; st = st.AddDays(1))
            {
                dictionary.Add(st, new List<string>());
            }
            HashSet<DateTime> dates = new HashSet<DateTime>();
            instructorClassesInWeek.ForEach(item => dates.Add(item.TermAffair));
            foreach (DateTime date in dates)
            {
                List<string> result = new List<string>();
                instructorClassesInWeek.Where(x => x.TermAffair.Equals(date)).ToList().ForEach(x => result.Add(x.Id + ";" + x.Attraction.Name));
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

        public string GetInstructorsForReservedAttraction(int id)
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

        public string RetreiveInstructorsAssignedToAttraction(int id,string attractionName)
        {
            string result = string.Empty;
            List<User> instructors = (from attrResWork in db.Attractions_Reservations_Workers
                                      where attrResWork.Attraction_ReservationId.Equals(id)
                                      join user in db.ApplicationUsers on attrResWork.WorkerId equals user.Id
                                      where user.Profession.Equals(attractionName)
                                      select user).ToList();
            foreach(User user in instructors)
            {
                result+=user.Name + " " + user.Surname + "(" + user.UserName + ")"+Environment.NewLine;
            }
            return result;                                                           
        }

        public void RemoveAssignedInstructorAttraction(Attraction_Reservation_Worker attractionReservationWorker)
        {
            db.Attractions_Reservations_Workers.Remove(attractionReservationWorker);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public Attraction_Reservation GetAttractionReservationById(int id)
        {
            Attraction_Reservation attractionReservation = db.Attractions_Reservations.Find(id);
            return attractionReservation;
        }

        public List<SelectListItem> GetAvaiableInstructors(int id, string attractionName)
        {
            List<User> allInstructorsByProfession = (from instructor in db.ApplicationUsers
                                                     where instructor.Profession.Equals(attractionName)
                                                     select instructor).ToList();
            List<string> allIDInstructorsAlreadyAssigned = (from attrRes in db.Attractions_Reservations
                                                            where attrRes.Id.Equals(id)
                                                            join attrResWork in db.Attractions_Reservations_Workers on attrRes.Id equals attrResWork.Attraction_ReservationId
                                                            select attrResWork.WorkerId).ToList();
            List<User> result = new List<User>();
            foreach(User worker in allInstructorsByProfession)
            {
                if(!allIDInstructorsAlreadyAssigned.Contains(worker.Id))
                {
                    result.Add(worker);
                }
            }
            List<SelectListItem> selectListItem = null;

            if (result.Count > 0)
                selectListItem = result.Select(item => new SelectListItem { Text = item.Name + "," + item.Surname + "(" + item.UserName + ")", Value = item.Id, Selected = result.First().Equals(item) ? true : false }).ToList();
            else
            {
                List<string> list = new List<string>();
                list.Add("-");
                selectListItem = list.Select(item => new SelectListItem { Text = item, Value = item, Selected = true }).ToList();
            }
            return selectListItem;
        }

        public void AssignInstructorToAttraction(Attraction_Reservation_Worker attractionReservationWorker)
        {
            db.Attractions_Reservations_Workers.Add(attractionReservationWorker);
        }

        public Attraction_Reservation_Worker GetAttractionReservationWorkerById(int id)
        {
            Attraction_Reservation_Worker attractionReservationWorker = db.Attractions_Reservations_Workers.Find(id);
            return attractionReservationWorker;
        }

        public bool checkStateInstructorToAttraction(Attraction_Reservation_Worker attractionReservationWorker)
        {
            List<Attraction_Reservation_Worker> added = (from attrResWork in db.Attractions_Reservations_Workers
                                                         where attrResWork.Attraction_ReservationId.Equals(attractionReservationWorker.Attraction_ReservationId)
                                                         && attrResWork.WorkerId.Equals(attractionReservationWorker.WorkerId)
                                                         select attrResWork).ToList();
            if (added.Count >= 1)
                return true;
            else
                return false;
        }

        public User GetInstructorAssignedToAttraction(string id)
        {
            User user = (from usr in db.ApplicationUsers
                         where usr.Id.Equals(id)
                         select usr).FirstOrDefault();
            return user;
        }
    }
}