using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepo
{
    public interface IHomeRepository
    {
        IList<string> GetAvaiableMeals();
        IList<string> GetAvaiableHouses();
        IList<string> GetAvaiableAttractions();
    }
}
