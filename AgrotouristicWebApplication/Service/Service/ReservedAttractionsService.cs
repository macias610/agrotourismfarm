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
        private readonly IAttractionReservationRepository attractionReservationRepository = null;
        private readonly IUserRepository userRepository = null;
        private readonly IAttractionReservationWorkerRepository attractionReservationWorkerRepository = null;

        public ReservedAttractionsService(IAttractionReservationRepository attractionReservationRepository, IUserRepository userRepository, IAttractionReservationWorkerRepository attractionReservationWorkerRepository)
        {
            this.attractionReservationRepository = attractionReservationRepository;
            this.userRepository = userRepository;
            this.attractionReservationWorkerRepository = attractionReservationWorkerRepository;
        }

        public void AssignInstructorToAttraction(AttractionReservationWorker attractionReservationWorker)
        {
            this.attractionReservationWorkerRepository.AddAttractionReservationWorker(attractionReservationWorker);
            this.attractionReservationWorkerRepository.SaveChanges();
        }

        public bool checkStateInstructorToAttraction(AttractionReservationWorker attractionReservationWorker)
        {
            List<AttractionReservationWorker> added = this.attractionReservationWorkerRepository
                                                            .GetAttractionsReservationsWorkers()
                                                            .ToList();
            added = added.Where(item => item.Attraction_ReservationId.Equals(attractionReservationWorker.Attraction_ReservationId))
                            .ToList();
            added = added.Where(item => item.WorkerId.Equals(attractionReservationWorker.WorkerId))
                            .ToList();

            if (added.Count >= 1)
                return true;
            else
                return false;
        }

        public AttractionReservation GetAttractionReservationById(int id)
        {
            AttractionReservation attractionReservation = this.attractionReservationRepository.GetAttractionReservationById(id);
            return attractionReservation;
        }

        public AttractionReservationWorker GetAttractionReservationWorkerById(int id)
        {
            AttractionReservationWorker attractionReservation = this.attractionReservationWorkerRepository.GetAttractionReservationWorkerById(id);
            return attractionReservation;
        }

        public Dictionary<DateTime, List<string>> GetAttractionsInstructorsInGivenWeek(string term)
        {
            DateTime start = DateTime.Parse(term.Split(';')[0]);
            DateTime end = DateTime.Parse(term.Split(';')[1]);
            List<AttractionReservation> attractionsReservation = this.attractionReservationRepository.GetAttractionsReservations().ToList();
            attractionsReservation = attractionsReservation.Where(item => item.TermAffair.CompareTo(start) >= 0
                                                                    && item.TermAffair.CompareTo(end) <= 0)
                                                            .ToList();
            attractionsReservation = attractionsReservation.Where(item => !item.Reservation.Status.Equals("oczekiwanie")).ToList();

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
                attractionsReservation.Where(x => x.TermAffair.Equals(date)).ToList().ForEach(x => result.Add(x.Id + ";" + x.Attraction.Name + ',' + x.QuantityParticipant + "(" + GetInstructorsForReservedAttraction(x.Id) + ")"));
                dictionary[date] = result;
            }
            return dictionary;
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

        public IList<SelectListItem> GetAvaiableInstructors(int id, string attractionName)
        {
            List<User> allInstructorsByProfession = this.userRepository.GetUsers().ToList();
            allInstructorsByProfession = allInstructorsByProfession
                                            .Where(item => item.Profession.Equals(attractionName))
                                            .ToList();

            IList<AttractionReservationWorker> rr = this.attractionReservationWorkerRepository
                                                            .GetAttractionsReservationsWorkers()
                                                            .ToList();
            rr = rr.Where(item => item.Attraction_ReservationId.Equals(id)).ToList();
            List<string> allIDInstructorsAlreadyAssigned = rr.Select(item => item.WorkerId).ToList();

            List<User> result = new List<User>();
            foreach (User worker in allInstructorsByProfession)
            {
                if (!allIDInstructorsAlreadyAssigned.Contains(worker.Id))
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

        public Dictionary<DateTime, List<string>> GetClassesInstructorInGivenWeek(string term, string idInstructor)
        {
            DateTime start = DateTime.Parse(term.Split(';')[0]);
            DateTime end = DateTime.Parse(term.Split(';')[1]);

            List<int> attrResIds = this.attractionReservationWorkerRepository
                                        .GetAttractionsReservationsWorkers().Where(item => item.WorkerId.Equals(idInstructor))
                                        .Select(item => item.Attraction_ReservationId)
                                        .ToList();

            List<AttractionReservation> instructorClassesInWeek = this.attractionReservationRepository
                                                                        .GetAttractionsReservations()
                                                                        .ToList();
            instructorClassesInWeek = instructorClassesInWeek
                                        .Where(item => attrResIds.Contains(item.Id))
                                        .ToList();
            instructorClassesInWeek = instructorClassesInWeek
                                        .Where(item => item.TermAffair.CompareTo(start) >= 0 && item.TermAffair.CompareTo(end) <= 0)
                                        .ToList();
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

        public User GetInstructorAssignedToAttraction(string id)
        {
            User user = this.userRepository.GetUserById(id);
            return user;
        }

        public string GetInstructorsForReservedAttraction(int id)
        {
            AttractionReservation attractionReservation = this.attractionReservationRepository.GetAttractionReservationById(id);
            Dictionary<int, User> workers = new Dictionary<int, User>();
            attractionReservation.Attraction_Reservation_Worker.ToList().ForEach(x => workers.Add(x.Id, x.Worker));
            List<string> listResult = new List<string>();
            workers.ToList().ForEach(x => listResult.Add(x.Value.UserName + "|" + x.Value.Name + "," + x.Value.Surname + "-" + x.Key + ";"));
            string result = string.Empty;
            listResult.ForEach(x => result += x);
            return result;
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

        public IList<SelectListItem> GetWeeksForAttractions(DateTime date)
        {
            int differenceToMonday = DayOfWeek.Monday - date.DayOfWeek;
            DateTime monday = date.AddDays(differenceToMonday);
            List<string> weeks = new List<string>();
            weeks.Add("-");
            int numberOfWeeks = 3;
            for (int i = 0; i < numberOfWeeks; i++)
            {
                weeks.Add(monday.ToShortDateString() + ";" + monday.AddDays(6).ToShortDateString());
                monday = monday.AddDays(7);
            }
            List<SelectListItem> listItem = weeks.Select(item => new SelectListItem { Text = item, Value = item, Selected = weeks.First().Equals(item) ? true : false }).ToList();
            return listItem;
        }

        public void RemoveAssignedInstructorAttraction(AttractionReservationWorker attractionReservationWorker)
        {
            this.attractionReservationWorkerRepository.RemoveAttractionReservationWorker(attractionReservationWorker);
            this.attractionReservationWorkerRepository.SaveChanges();
        }

        public string RetreiveInstructorsAssignedToAttraction(int id, string attractionName)
        {
            string result = string.Empty;
            List<AttractionReservationWorker> rr = this.attractionReservationWorkerRepository
                                                            .GetAttractionsReservationsWorkers()
                                                            .ToList();
            rr = rr.Where(item => item.Attraction_ReservationId.Equals(id)).ToList();

            List<User> instructors = rr.Where(item => item.Worker.Profession.Equals(attractionName))
                                        .Select(item => item.Worker)
                                        .ToList();
            foreach (User user in instructors)
            {
                result += user.Name + " " + user.Surname + "(" + user.UserName + ")" + Environment.NewLine;
            }
            return result;
        }
    }
}
