using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepo
{
    public interface IAttractionReservationWorkerRepository
    {
        void AddAttractionReservationWorker(Attraction_Reservation_Worker attractionReservationWorker);

        Attraction_Reservation_Worker GetAttractionReservationWorkerById(int id);

        IList<Attraction_Reservation_Worker> GetAttractionsReservationsWorkers();

        void RemoveAttractionReservationWorker(Attraction_Reservation_Worker attractionReservationWorker);

        void SaveChanges();
    }
}
