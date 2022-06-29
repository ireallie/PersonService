using Person.BusinessLogic.Domains.Abstractions;
using Person.Data.Filters;
using Person.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Person.BusinessLogic.Domains
{
    public class PersonDomain : IPersonDomain
    {
        private readonly IPersonRepository _personRepository;

        public PersonDomain(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<IEnumerable<Data.Entities.Person>> GetAllPeople(GetAllRequest request)
        {
            return await _personRepository.GetAll(request);
        }

        public async Task<long> SavePerson(Data.Entities.Person person)
        {
            var item = await _personRepository.GetById(person.Id);

            if(item == null)
            {
                return await _personRepository.Add(person);
            }

            return await _personRepository.Update(person);
        }
    }
}
