using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ViewModel;

namespace Service.IService
{
    public interface IHouseReservationService
    {
        IList<House> GetHousesForReservation(int id);
        IList<House> GetAvaiableHousesInTerm(DateTime startDate, DateTime endDate);
        IList<SelectListItem> GetNamesAvaiableHouses(IList<House> houses);
        House GetHouseByName(string name);
        void ConfirmSelectedHouses(NewReservation reservation, IList<string> selectedHouses);

    }
}
