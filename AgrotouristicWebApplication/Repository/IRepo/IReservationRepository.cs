using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepo
{
    public interface IReservationRepository
    {
        IQueryable<Reservation> GetReservations();
        IQueryable<Reservation> GetClientReservations(string id);
    }
}
