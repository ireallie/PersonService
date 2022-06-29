using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Person.BusinessLogic.Domains.Abstractions;
using Person.Data.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace Person.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonDomain _personDomain;

        public PersonController(IPersonDomain personDomain)
        {
            _personDomain = personDomain;
        }

        /// <summary>
        /// API endpoint to store person from string into database
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>

        [HttpPost("savePerson")]
        public async Task<long> Save(string json)
        {
            var person = Serializer.DeSerialize(json, new Data.Entities.Person());

            return await _personDomain.SavePerson(person);
        }

        /// <summary>
        /// API endpoint to query persons from database
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpGet("getPersons")] 
        public async Task<string> GetAll([FromQuery] GetAllRequest request)
        {
            var people = await _personDomain.GetAllPeople(request);
            
            return Serializer.Serialize(people);
        }
    }
}
