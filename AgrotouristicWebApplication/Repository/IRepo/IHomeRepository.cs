using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepo
{
    public interface IHomeRepository
    {
        List<string> GetAvaiableMeals();
        List<string> GetAvaiableHouses();
        List<string> GetAvaiableAttractions();
    }
}
