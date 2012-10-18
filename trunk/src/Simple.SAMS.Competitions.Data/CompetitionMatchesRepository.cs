using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Competitions;
using Simple.Utilities;

namespace Simple.SAMS.Competitions.Data
{
    public class CompetitionMatchesRepository : DataRepositoryBase<CompetitionsDataContext>, ICompetitionMatchesRepository
    {
        public void AddCompetitionMatches(int competitionId, MatchHeaderInfo[] matches)
        {
            UseDataContext(
                dataContext=>
                    {
                        var competition = dataContext.Competitions.FirstOrDefault(c => c.Id == competitionId);
                        if (competition.IsNull())
                        {
                            throw new ArgumentException(
                                "Competition '{0}' could not be found.".ParseTemplate(competitionId));
                        }

                        competition.Status = (int) CompetitionStatus.MatchesCreated;

                        matches.ForEach(
                            match=>
                                {
                                    var dataMatch = new Match();
                                    dataMatch.CompetitionId = competitionId;
                                    dataMatch.Created = dataMatch.Updated = DateTime.UtcNow;
                                    dataMatch.RowStatus = 0;

                                    MapMatchToDataMatch(match, dataMatch);

                                    dataContext.Matches.InsertOnSubmit(dataMatch);
                                });
                        
                        dataContext.SubmitChanges();
                    });
        }

        private void MapMatchToDataMatch(MatchHeaderInfo match, Match dataMatch)
        {
            dataMatch.SectionId = (int) match.Section;
            dataMatch.Position = match.Position;
            dataMatch.Round = match.Round;
            dataMatch.Status = (int) match.Status;
            dataMatch.StartTimeType = (int) match.StartTimeType;
        }
    }
}
