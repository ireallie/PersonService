using Person.Data.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Person.Data.Repositories
{
    public interface IPersonRepository
    {
        public Task<long> Add(Entities.Person person);

        public Task<long> Update(Entities.Person person);

        public Task<Entities.Person> GetById(long id);

        public Task<IEnumerable<Entities.Person>> GetAll(GetAllRequest request);
    }
}
