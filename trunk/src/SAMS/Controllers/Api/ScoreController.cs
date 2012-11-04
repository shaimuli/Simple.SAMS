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
    public class ScoreController : ApiController
    {
        //// GET api/score
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/score/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/score
        public void Post([FromBody]MatchScoreUpdateInfo matchInfo)
        {
            var manager = ServiceProvider.Get<ICompetitionsManager>();
            manager.UpdateMatchScore(new[]{matchInfo});
        }

        //// PUT api/score/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/score/5
        //public void Delete(int id)
        //{
        //}
    }
}
