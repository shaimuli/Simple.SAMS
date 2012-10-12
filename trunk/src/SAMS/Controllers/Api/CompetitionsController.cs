using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;

namespace SAMS.Controllers.Api
{
    public class CompetitionsController : ApiController
    {
        // GET api/competitions
        public IEnumerable<CompetitionHeaderInfo> Get()
        {
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var result = competitionsRepository.SearchCompetitions(new CompetitionSearchQuery() { StartIndex = 0, PageSize = 1000 });
            return result.Items;
        }

        // GET api/competitions/5
        public CompetitionDetails Get(int id)
        {
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            return competitionsRepository.GetCompetitionDetails(id);
        }

        // POST api/competitions
        public void Post([FromBody]string value)
        {
        }

        // PUT api/competitions/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/competitions/5
        public void Delete(int id)
        {
        }
    }
}
