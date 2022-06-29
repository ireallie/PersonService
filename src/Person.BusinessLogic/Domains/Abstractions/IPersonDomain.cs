using Person.Data.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Person.BusinessLogic.Domains.Abstractions
{
    public interface IPersonDomain
    {
        public Task<long> SavePerson(Data.Entities.Person person);

        public Task<IEnumerable<Data.Entities.Person>> GetAllPeople(GetAllRequest request);
    }
}
