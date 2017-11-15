using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class PdfReservation
    {
        public Reservation Reservation { get; set; }

        public IList<decimal> HousesCosts { get; set; }

        public IList<decimal> MealsCosts { get; set; }
    }
}
