using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAMS.Models;
using Simple;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;

namespace SAMS.Controllers
{
    public class PlayersController : Controller
    {
        //
        // GET: /Players/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int? competitionId, int? replacedPlayerId, Player player, CompetitionPlayerSource source)
        {
            var playersRepository = ServiceProvider.Get<IPlayersRepository>();
            var newPlayerId = playersRepository.Add(player);

            if (competitionId.HasValue)
            {
                var manager = ServiceProvider.Get<ICompetitionsManager>();
                if (replacedPlayerId.HasValue)
                {
                    manager.ReplacePlayer(competitionId.Value, replacedPlayerId.Value, newPlayerId, source);
                }
                else
                {
                    manager.AddPlayerToCompetition(competitionId.Value, newPlayerId, source);
                }
                return RedirectToAction("Details", "Competitions", new { id = competitionId.Value });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Create(int? competitionId, int? replacePlayerId, string idNumber)
        {
            var player = new Player();
            var model = new CreatePlayerModel { Player = player };

            if (competitionId.HasValue)
            {
                var competitionRepository = ServiceProvider.Get<ICompetitionRepository>();
                model.Competition = competitionRepository.GetCompetition(competitionId.Value);

                if (replacePlayerId.HasValue)
                {
                    var playersRepository = ServiceProvider.Get<IPlayersRepository>();
                    model.ReplacedPlayer = playersRepository.Get(replacePlayerId.Value);
                }
            }

            if (idNumber.NotNullOrEmpty())
            {
                model.Player.IdNumber = idNumber;
            }
            return View(model);
        }

        public ActionResult GetPlayerId(string idNumber)
        {
            var playersRepository = ServiceProvider.Get<IPlayersRepository>();
            var result = playersRepository.GetPlayerIdByIdNumber(idNumber);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
