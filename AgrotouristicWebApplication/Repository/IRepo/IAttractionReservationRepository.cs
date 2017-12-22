using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepo
{
    public interface IAttractionReservationRepository
    {
        void AddAttractionReservation(Attraction_Reservation attractionReservation);

        Attraction_Reservation GetAttractionReservationById(int id);

        IList<Attraction_Reservation> GetAttractionsReservationsByReservationId(int id);

        IList<Attraction_Reservation> GetAttractionsReservations();

        void RemoveAttractionReservation(Attraction_Reservation attractionReservation);

        void SaveChanges();

        void UpdateAttractionReservation(Attraction_Reservation attractionReservation, byte[] rowVersion);
    }
}
