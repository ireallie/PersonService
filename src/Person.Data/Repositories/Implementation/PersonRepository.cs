using Microsoft.EntityFrameworkCore;
using Person.Data.Entities;
using Person.Data.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Person.Data.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly DesignTimeDbContextFactory _factory;

        public PersonRepository(DesignTimeDbContextFactory factory)
        {
            _factory = factory;
        }
        public async Task<long> Add(Entities.Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException($"{nameof(Add)} entity must not be null");
            }

            try
            {
                using (var context = _factory.CreateDbContext())
                {
                    await context.Persons.AddAsync(person);
                    await context.SaveChangesAsync();

                    return person.Id;
                }
            }
            catch (Exception)
            {
                throw new Exception($"{nameof(person)} could not be saved");
            }
                 
        }

        public async Task<IEnumerable<Entities.Person>> GetAll(GetAllRequest request)
        {
            using (var context = _factory.CreateDbContext())
            {
                var people = context.Persons.Include(p => p.Address).AsNoTracking();

                // we can actually get rid of this check here by validating inputs in our controller using built-in parameter [BindRequired]

                if (!string.IsNullOrWhiteSpace(request.FirstName))
                {
                    people = people.Where(p => p.FirstName.ToLower().Contains(request.FirstName.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(request.LastName))
                {
                    people = people.Where(p => p.LastName.ToLower().Contains(request.LastName.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(request.City))
                {
                    people = people.Where(p => p.Address.City.ToLower().Contains(request.City.ToLower()));
                }

                return await people.ToListAsync();
            }
        }

        public async Task<Entities.Person> GetById(long id)
        {
            using (var context = _factory.CreateDbContext())
            {
                Entities.Person person = await context.Persons.FirstOrDefaultAsync(p => p.Id == id);
                return person;
            }
        }

        public async Task<long> Update(Entities.Person person)
        {
            using (var context = _factory.CreateDbContext())
            {
                var personObj = await context.Persons
                    .Where(p => p.Id == person.Id)
                    .Select(p => new Entities.Person
                    {
                       Id = p.Id,
                       FirstName = p.FirstName,
                       LastName = p.LastName,
                       AddressId = p.AddressId,
                       Address = context.Addresses.Where(a => a.Id == p.AddressId).FirstOrDefault()
                    })                             
                    .FirstOrDefaultAsync();

                personObj.FirstName = person.FirstName;
                personObj.LastName = person.LastName;
                personObj.Address.AddressLine = person.Address.AddressLine;
                personObj.Address.City = person.Address.City;

                context.Update(personObj);
                await context.SaveChangesAsync();

                return personObj.Id;
            }
        }
    }
}
