using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.SAMS.Contracts.Players;

namespace Simple.SAMS.Contracts.Competitions
{
    public interface IPlayersRepository: IEntityRepository<Player>
    {
        Dictionary<string, int> MatchPlayerByIdNumber(Player[] players);
        int? GetPlayerIdByIdNumber(string idNumber);
    }
}
