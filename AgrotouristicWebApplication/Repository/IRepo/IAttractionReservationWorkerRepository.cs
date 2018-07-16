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
        void AddAttractionReservationWorker(AttractionReservationWorker attractionReservationWorker);

        AttractionReservationWorker GetAttractionReservationWorkerById(int id);

        IList<AttractionReservationWorker> GetAttractionsReservationsWorkers();

        void RemoveAttractionReservationWorker(AttractionReservationWorker attractionReservationWorker);

        void SaveChanges();
    }
}
