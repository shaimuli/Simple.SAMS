using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Files;
using Simple.Common.Storage;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;

namespace Simple.SAMS.Competitions.Services
{
    public class CompetitionsManagerService : ICompetitionsManager
    {
        public void Create(CreateCompetitionInfo competitionCreateInfo)
        {
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            var competitionId = competitionsEngine.CreateCompetition(competitionCreateInfo);

            var players = competitionsEngine.GetCompetitionPlayers(competitionCreateInfo.PlayersFileUrl);
            
            competitionsEngine.AddPlayersToCompetition(competitionId, players);

        }

        public void UpdateCompetitionPlayers(int competitionId, string playersFileUrl)
        {
            Requires.IntArgumentPositive(competitionId, "competitionId");
            Requires.ArgumentNotNullOrEmptyString(playersFileUrl, "playersFileUrl");
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            if (!competitionsRepository.DoesCompetitionExists(competitionId))
            {
                throw new ArgumentException("Competition '{0}' could not be found.".ParseTemplate(competitionId));
            }

            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            var players = competitionsEngine.GetCompetitionPlayers(playersFileUrl);

            competitionsEngine.AddPlayersToCompetition(competitionId, players);
        }


        public void LoadCompetitions(string competitionsFileUrl)
        {
            Requires.ArgumentNotNullOrEmptyString(competitionsFileUrl, "competitionsFileUrl");
            var competitionsEngine = ServiceProvider.Get<ICompetitionsEngine>();
            var competitions = competitionsEngine.GetCompetitions(competitionsFileUrl);
            competitionsEngine.ImportCompetitions(competitions);
        }
    }
}
