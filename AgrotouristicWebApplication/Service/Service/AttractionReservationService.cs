using DomainModel.Models;
using Repository.IRepo;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using ViewModel;

namespace Service.Service
{
    public class AttractionReservationService : IAttractionReservationService
    {
        private readonly IAttractionRepository attractionRepository = null;
        private readonly IAttractionReservationRepository attractionReservationRepository = null;
        private readonly IUserRepository userRepository = null;
        private readonly IReservationRepository reservationRepository = null;

        public AttractionReservationService(IAttractionRepository attractionRepository, IAttractionReservationRepository attractionReservationRepository, IUserRepository userRepository, IReservationRepository reservationRepository)
        {
            this.attractionRepository = attractionRepository;
            this.attractionReservationRepository = attractionReservationRepository;
            this.userRepository = userRepository;
            this.reservationRepository = reservationRepository;
        }

        public void ChangeAssignedAttractions(int id, NewReservation reservation)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                List<Attraction_Reservation> attractionsReservation = this.attractionReservationRepository
                                                                        .GetAttractionsReservationsByReservationId(id)
                                                                            .ToList();
                Dictionary<DateTime, List<string>> dictionary = new Dictionary<DateTime, List<string>>();
                for (DateTime date = reservation.AssignedAttractions.Keys.First(); date <= reservation.AssignedAttractions.Keys.Last(); date = date.AddDays(1))
                {
                    dictionary.Add(date, new List<string>());
                }
                attractionsReservation.ForEach(item => dictionary[item.TermAffair].Add(item.Attraction.Name + ',' + item.QuantityParticipant));

                List<string> attractionsToRemove = new List<string>();

                foreach (KeyValuePair<DateTime, List<string>> item in dictionary)
                {
                    List<Attraction_Reservation> attractionsReservationInDay = attractionsReservation.Where(elem => elem.TermAffair.Equals(item.Key)).ToList();
                    List<string> oldAttractions = item.Value;
                    List<string> newAttractions = reservation.AssignedAttractions[item.Key];
                    List<string> toRemove = oldAttractions.Where(elem => !(newAttractions.Contains(elem))).ToList();
                    foreach (string attractionToRemove in toRemove)
                    {
                        Attraction_Reservation attractionReservation = attractionsReservationInDay.Where(elem => elem.Attraction.Name.Equals(attractionToRemove.Split(',')[0])).FirstOrDefault();
                        attractionsReservationInDay.Remove(attractionReservation);
                        this.attractionReservationRepository.RemoveAttractionReservation(attractionReservation);
                    }
                }
                this.attractionReservationRepository.SaveChanges();
                foreach (KeyValuePair<DateTime, List<string>> item in reservation.AssignedAttractions.Where(pair => pair.Value.Any()))
                {
                    List<string> oldAttractions = dictionary[item.Key];
                    List<string> newAttractions = item.Value;
                    List<string> toAdd = newAttractions.Where(elem => !(oldAttractions.Contains(elem))).ToList();
                    foreach (string attractionToAdd in toAdd)
                    {
                        string attractionName = attractionToAdd.Split(',')[0];
                        Attraction attraction = this.attractionRepository.GetAttractionByName(attractionName);
                        Attraction_Reservation attractionReservation = new Attraction_Reservation()
                        {
                            AttractionId = attraction.Id,
                            ReservationId = id,
                            TermAffair = item.Key,
                            QuantityParticipant = Int32.Parse(attractionToAdd.Split(',')[1]),
                            OverallCost = attraction.Price * Int32.Parse(attractionToAdd.Split(',')[1])
                        };
                        this.attractionReservationRepository.AddAttractionReservation(attractionReservation);
                    }
                }
                this.attractionReservationRepository.SaveChanges();
                Reservation editedReservation = this.reservationRepository.GetReservationById(id);
                editedReservation.OverallCost = reservation.OverallCost;
                this.reservationRepository.UpdateReservation(editedReservation, editedReservation.RowVersion);
                this.reservationRepository.SaveChanges();

                scope.Complete();
            }
        }

        public Attraction GetAttractionByName(string name)
        {
            Attraction attraction = this.attractionRepository.GetAttractions()
                                            .Where(item => item.Name.Equals(name))
                                            .SingleOrDefault();
            return attraction;
        }

        public Dictionary<DateTime, List<string>> GetAttractionsInGivenWeek(string term, Dictionary<DateTime, List<string>> dictionary)
        {
            Dictionary<DateTime, List<string>> result = new Dictionary<DateTime, List<string>>();
            for (DateTime start = DateTime.Parse(term.Split(';')[0]); start.CompareTo(DateTime.Parse(term.Split(';')[1])) <= 0; start = start.AddDays(1))
            {
                result.Add(start, dictionary[start]);
            }
            return result;
        }

        public IList<SelectListItem> GetAvaiableAttractions()
        {
            IList<string> nameAttractions = this.attractionRepository.GetAttractions().Select(item => item.Name).ToList();
            List<string> userProfessions = this.userRepository.GetUsers().Select(item => item.Profession).ToList();
            IList<string> toRemoveProfessions = new List<string>(new string[] { "Administrator", "Recepcjonista" });
            userProfessions.RemoveAll(item => toRemoveProfessions.Contains(item));
            HashSet<string> professionsWithoutDuplicates = new HashSet<string>(userProfessions);

            List<string> result = nameAttractions.Where(item => professionsWithoutDuplicates.Contains(item)).ToList();
            List<SelectListItem> selectList = result.Select(item => new SelectListItem { Text = item, Value = item, Selected = true }).ToList();
            return selectList;
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

        public Dictionary<DateTime, List<string>> InitializeDictionaryForAssignedAttractions(DateTime startDate, DateTime endDate)
        {
            Dictionary<DateTime, List<string>> result = new Dictionary<DateTime, List<string>>();
            for (DateTime start = startDate; start.CompareTo(endDate) <= 0; start = start.AddDays(1))
            {
                result.Add(start, new List<string>());
            }
            return result;
        }

        public Dictionary<DateTime, List<string>> RetreiveAttractionsInGivenWeek(string term, int id)
        {
            DateTime start = DateTime.Parse(term.Split(';')[0]);
            DateTime end = DateTime.Parse(term.Split(';')[1]);

            List<Attraction_Reservation> attractionsReservation = this.attractionReservationRepository.GetAttractionsReservations().ToList();
            attractionsReservation = attractionsReservation.Where(item => item.ReservationId.Equals(id) &&
                                                                            item.TermAffair.CompareTo(start) >= 0 &&
                                                                            item.TermAffair.CompareTo(end) <= 0)
                                                           .ToList();
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
                attractionsReservation.Where(x => x.TermAffair.Equals(date)).ToList().ForEach(x => result.Add(x.Attraction.Name + ',' + x.QuantityParticipant));
                dictionary[date] = result;
            }
            return dictionary;
        }

        public void SaveAssignedAttractions(int id, NewReservation reservation)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Dictionary<DateTime, List<string>> dictionary = reservation.AssignedAttractions.Where(x => x.Value.Any()).ToDictionary(t => t.Key, t => t.Value);
                foreach (KeyValuePair<DateTime, List<string>> item in dictionary)
                {
                    foreach (string attr in item.Value)
                    {
                        string attractionName = attr.Split(',')[0];
                        int quantityParticipants = Int32.Parse(attr.Split(',')[1]);
                        Attraction attraction = this.attractionRepository.GetAttractionByName(attractionName);
                        Attraction_Reservation attractionReservation = new Attraction_Reservation()
                        {
                            AttractionId = attraction.Id,
                            ReservationId = id,
                            TermAffair = item.Key,
                            QuantityParticipant = quantityParticipants,
                            OverallCost = quantityParticipants * attraction.Price
                        };
                        this.attractionReservationRepository.AddAttractionReservation(attractionReservation);
                    }
                    this.attractionReservationRepository.SaveChanges();
                }
                scope.Complete();
            }
        }
    }
}
