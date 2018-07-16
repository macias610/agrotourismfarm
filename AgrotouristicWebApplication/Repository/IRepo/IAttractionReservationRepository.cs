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
        void AddAttractionReservation(AttractionReservation attractionReservation);

        AttractionReservation GetAttractionReservationById(int id);

        IList<AttractionReservation> GetAttractionsReservationsByReservationId(int id);

        IList<AttractionReservation> GetAttractionsReservations();

        void RemoveAttractionReservation(AttractionReservation attractionReservation);

        void SaveChanges();

        void UpdateAttractionReservation(AttractionReservation attractionReservation, byte[] rowVersion);
    }
}
