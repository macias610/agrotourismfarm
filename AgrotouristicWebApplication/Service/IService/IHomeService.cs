using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IHomeService
    {
        IList<string> GetAvaiableMeals();
        IList<string> GetAvaiableHouses();
        IList<string> GetAvaiableAttractions();
    }
}
