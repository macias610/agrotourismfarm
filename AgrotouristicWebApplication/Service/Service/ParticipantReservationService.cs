using DomainModel.Models;
using Repository.IRepo;
using Service.IService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using ViewModel;

namespace Service.Service
{
    public class ParticipantReservationService : IParticipantReservationService
    {
        private readonly IReservationHouseRepository reservationHouseRepository = null;
        private readonly IParticipantRepository participantRepository = null;
        private readonly IHouseRepository houseRepository = null;

        public ParticipantReservationService(IReservationHouseRepository reservationHouseRepository, IParticipantRepository participantRepository, IHouseRepository houseRepository)
        {
            this.reservationHouseRepository = reservationHouseRepository;
            this.participantRepository = participantRepository;
            this.houseRepository = houseRepository;
        }

        public Dictionary<string, List<Participant>> RetreiveHouseParticipants(int id)
        {
            IList<Reservation_House> reservationHouses = this.reservationHouseRepository.GetReservationHousesOfReservationId(id);
            Dictionary<string, List<Participant>> dictionary = new Dictionary<string, List<Participant>>();
            foreach (Reservation_House resHouse in reservationHouses)
            {
                string houseName = resHouse.House.HouseType.Type + '(' + resHouse.House.Name + ");";
                List<Participant> participants = resHouse.Participant.ToList();
                dictionary.Add(houseName, participants);
            }
            return dictionary;
        }

        public bool ValidateFormularParticipants(Dictionary<string, List<Participant>> dictionary)
        {
            foreach (KeyValuePair<string, List<Participant>> item in dictionary)
            {
                foreach (Participant participant in item.Value)
                {
                    var context = new ValidationContext(participant, serviceProvider: null, items: null);
                    var results = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(participant, context, results, true))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public IList<Participant> GetParticipantsHouseForReservation(int reservationId, int houseId)
        {
            IList<Reservation_House> reservationHouses = this.reservationHouseRepository
                                                                .GetReservationHousesOfReservationId(reservationId);
            int reservationHouseId = reservationHouses
                                        .Where(item => item.HouseId.Equals(houseId))
                                        .Select(item => item.Id)
                                        .SingleOrDefault();

            IList<Participant> participants = this.participantRepository
                                                    .GetParticipantsForHouseReservation(reservationHouseId);
            return participants;
        }

        public IList<SelectListItem> GetParticipantsQuantity(int quantity)
        {
            List<string> list = new List<string>();
            for (int i = 2; i <= quantity; i++)
            {
                list.Add(i.ToString());
            }
            List<SelectListItem> selectList = list.Select(item => new SelectListItem { Text = item, Value = item, Selected = true }).ToList();
            return selectList;
        }

        public void ClearParticipantsFormular(NewReservation reservation)
        {
            foreach (KeyValuePair<string, List<Participant>> item in reservation.AssignedParticipantsHouses)
            {
                foreach (Participant participant in item.Value)
                {
                    participant.Name = String.Empty;
                    participant.Surname = String.Empty;
                }
            }
        }

        public IList<Participant> CopyParticipantsData(IList<Participant> targetList, IList<Participant> actualList)
        {
            if (targetList.First().Id == 0)
            {
                return actualList;
            }
            for (int i = 0; i < targetList.Count; i++)
            {
                targetList.ElementAt(i).Name = actualList[i].Name;
                targetList.ElementAt(i).Surname = actualList[i].Surname;
            }
            return targetList;
        }

        public void ChangeAssignedParticipants(int id, NewReservation reservation)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (KeyValuePair<string, List<Participant>> item in reservation.AssignedParticipantsHouses)
                {
                    string houseName = Regex.Match(item.Key, @"\(([^)]*)\)").Groups[1].Value;
                    int houseId = this.houseRepository.GetHouseByName(houseName).Id;

                    Reservation_House reservationHouse = this.reservationHouseRepository
                                                                .GetReservationHousesOfReservationId(id)
                                                                .Where(elem => elem.HouseId.Equals(houseId))
                                                                .FirstOrDefault();
                    IList<int> participantsId = this.participantRepository.
                                                        GetParticipantsForHouseReservation(reservationHouse.Id)
                                                        .Select(elem => elem.Id)
                                                        .ToList();

                    foreach (int participantId in participantsId)
                    {
                        Participant participant = this.participantRepository.GetParticipantById(participantId);
                        Participant edited = reservation.AssignedParticipantsHouses[item.Key].FirstOrDefault(x => x.Id.Equals(participantId));
                        participant.Name = edited.Name;
                        participant.Surname = edited.Surname;
                        this.participantRepository.SaveChanges();
                    }
                }
                scope.Complete();
            }



        }
    }
}
