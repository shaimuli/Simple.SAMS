using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Simple;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;

namespace SAMS.Controllers.Api
{
    public class CompetitionsController : ApiController
    {
        /// <summary>
        /// Get list of competitions
        /// </summary>
        /// <returns><see cref="IEnumerable[CompetitionHeaderInfo]"/></returns>
        public IEnumerable<CompetitionHeaderInfo> Get()
        {
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var result = competitionsRepository.SearchCompetitions(new CompetitionSearchQuery() { StartIndex = 0, PageSize = 1000 });
            return result.Items;
        }

        /// <summary>
        /// Get competition details with unplayed matches list
        /// </summary>
        /// <param name="referenceId">competition referenceid</param>
        /// <returns><see cref="CompetitionDetails"/></returns>
        public CompetitionDetails Get(string id)
        {
            if (id.IsNullOrEmpty())
            {
                throw new HttpException(400, "You must provide valid referenceId");
            }
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var details = competitionsRepository.GetCompetitionUnplayedMatches(id);
            if (details.IsNull())
            {
                throw new HttpException(404, "Competition '{0}' couuld not be found.".ParseTemplate(id));
            }
            return details;
        }

    }
}
