using challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Services
{
    public interface ICompensationService
    {
        Compensation GetById(String id);
        Compensation Create(Compensation employee);
        Compensation Replace(Compensation originalCompensation, Compensation newCompensation);
        List<Compensation> GetAllCompensations(); 
    }
}
