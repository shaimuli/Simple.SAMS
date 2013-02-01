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

        public ActionResult GetPlayerByIdNumber(string idNumber)
        {
            var playersRepository = ServiceProvider.Get<IPlayersRepository>();
            var playerId = playersRepository.GetPlayerIdByIdNumber(idNumber);
            var result = default(Player);
            if (playerId.HasValue)
            {
                result = playersRepository.Get(playerId.Value);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(int? competitionId, int? replacedPlayerId, Player player, CompetitionPlayerSource competitionPlayerSource, CompetitionSection competitionSection)
        {
            var playersRepository = ServiceProvider.Get<IPlayersRepository>();
            var newPlayerId = playersRepository.Add(player);

            if (competitionId.HasValue)
            {
                var manager = ServiceProvider.Get<ICompetitionsManager>();
                if (replacedPlayerId.HasValue)
                {
                    manager.ReplacePlayer(competitionId.Value, replacedPlayerId.Value, newPlayerId, competitionPlayerSource, competitionSection);
                }
                else
                {
                    manager.AddPlayerToCompetition(competitionId.Value, newPlayerId, competitionPlayerSource, competitionSection);
                }
                return RedirectToAction("Details", "Competitions", new { id = competitionId.Value });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Create(int? competitionId, int? replacePlayerId, string idNumber, CompetitionPlayerSource source, CompetitionSection section)
        {
            var player = new Player();
            var model = new CreatePlayerModel { Player = player };

            if (competitionId.HasValue)
            {
                var competitionRepository = ServiceProvider.Get<ICompetitionRepository>();
                model.Competition = competitionRepository.GetCompetition(competitionId.Value);
                model.Source = source;
                model.Section = section;

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
