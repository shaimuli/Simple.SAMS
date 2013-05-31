﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.Reporting.WebForms;
using SAMS.Models;
using Simple;
using Simple.Common.Storage;
using Simple.Common.Web;
using Simple.ComponentModel;
using Simple.SAMS.Competitions.Services;
using Simple.SAMS.Contracts.Competitions;
using Simple.SAMS.Contracts.Players;

namespace SAMS.Controllers
{
    public class CompetitionsController : Controller
    {

        private string GetLinkTemplate(string name)
        {
            var result = ConfigurationManager.AppSettings["Links." + name];
            if (string.IsNullOrEmpty(result))
            {
                result = null;
            }
            return result;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            SystemMonitor.Error(filterContext.Exception, "Unhandled exception");

            if (HttpHelper.IsDebug)
            {
                throw filterContext.Exception;
            }
            else
            {
                base.OnException(filterContext);
            }
        }

        public ActionResult Print(int id, CompetitionSection section = CompetitionSection.Final)
        {
            var competitionEngine = ServiceProvider.Get<ICompetitionsEngine>();

            var competition = competitionEngine.GetCompetitionDetails(id);
            var generator = new TournamentBracketGenerator();
            var result = generator.Generate(competition, section);

            return Content(result);

        }

        [HttpPost]
        public ActionResult ReplaceCompetitionPlayer(int competitionId, int replacedPlayerId, int replacementPlayerId, CompetitionPlayerSource source, CompetitionPlayerStatus status, string reason)
        {
            SystemMonitor.Info("Replaceing {0} with {1}", replacedPlayerId, replacementPlayerId);
            var manager = ServiceProvider.Get<ICompetitionsManager>();
            manager.ReplacePlayer(competitionId, replacedPlayerId, replacementPlayerId, source, status, reason);
            return new HttpStatusCodeResult(200);
        }
        [HttpPost]
        public ActionResult AddCompetitionPlayer(int competitionId, int playerId, int source, int section)
        {
            SystemMonitor.Info("Adding Player {0}, source:{1}, section:{2}", playerId, source, section);
            var manager = ServiceProvider.Get<ICompetitionsManager>();
            manager.AddPlayerToCompetition(competitionId, playerId, (CompetitionPlayerSource)source, (CompetitionSection)section);
            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult RemoveCompetitionPlayer(int competitionId, int playerId, CompetitionPlayerStatus? status, string reason)
        {
            var manager = ServiceProvider.Get<ICompetitionsManager>();
            manager.RemovePlayer(competitionId, playerId, status.GetValueOrDefault(CompetitionPlayerStatus.Removed), reason);
            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult UpdateMatchResults(MatchResultUpdateModel[] updates)
        {
            var manager = ServiceProvider.Get<ICompetitionsManager>();
            UpdateStartTimes(updates, manager);
            UpdateScores(updates, manager);

            return new HttpStatusCodeResult(200);
        }

        private void UpdateStartTimes(MatchResultUpdateModel[] updates, ICompetitionsManager manager)
        {
            var dateUpdates =
            updates.Where(
                up =>
                (up.StartTimeHours.HasValue || up.StartTimeMinutes.HasValue || up.StartTimeType.HasValue) &&
                up.Date.NotNullOrEmpty());
            
            var startTimeUpdates =
            dateUpdates.Select(
                dateUpdate =>
                {
                    var startTimeUpdateInfo = new MatchStartTimeUpdateInfo();
                    startTimeUpdateInfo.MatchId = dateUpdate.Id;
                    startTimeUpdateInfo.StartTime = BuildStartTime(dateUpdate.Date, dateUpdate.StartTimeHours,
                                                                   dateUpdate.StartTimeMinutes);
                    if (dateUpdate.StartTimeType.HasValue)
                    {
                        startTimeUpdateInfo.StartTimeType = dateUpdate.StartTimeType.Value;
                    }

                    return startTimeUpdateInfo;
                });

            manager.UpdateMatchStartTime(startTimeUpdates.ToArray());
        }

        private DateTime BuildStartTime(string date, int? hours, int? minutes)
        {
            var time = new List<string>();
            if (hours.HasValue)
            {
                time.Add(hours.Value.ToString().PadLeft(2, '0'));
            }
            else
            {
                time.Add("00");
            }
            if (minutes.HasValue)
            {
                time.Add(minutes.Value.ToString().PadLeft(2, '0'));

            }
            else
            {
                time.Add("00");
            }

            date += " " + string.Join(":", time.ToArray());

            var startTime = DateTime.ParseExact(date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);

            return startTime;
        }

        private void UpdateScores(MatchResultUpdateModel[] updates, ICompetitionsManager manager)
        {
            var matchScoreUpdateInfoItems = new List<MatchScoreUpdateInfo>();
            updates.ForEach(update =>
                                {
                                    var matchScoreUpdateInfo = new MatchScoreUpdateInfo();

                                    matchScoreUpdateInfo.MatchId = update.Id;
                                    matchScoreUpdateInfo.Result = update.Result;
                                    matchScoreUpdateInfo.Winner = update.Winner;

                                    matchScoreUpdateInfoItems.Add(matchScoreUpdateInfo);
                                    if (update.SetScores.NotNullOrEmpty())
                                    {
                                        var scores =
                                            update.SetScores.Where(s => !(s.Player1Points == 0 && s.Player2Points == 0));
                                        if (scores.NotNullOrEmpty())
                                        {
                                            matchScoreUpdateInfo.SetScores = update.SetScores;
                                        }
                                    }
                                });
            if (matchScoreUpdateInfoItems.NotNullOrEmpty())
            {
                manager.UpdateMatchScore(matchScoreUpdateInfoItems.ToArray());
            }
        }

        //
        // GET: /Competitions/

        [HttpPost]
        public ActionResult PositionCompetitionPlayers(int competitionId, CompetitionSection section)
        {
            var competitionManager = ServiceProvider.Get<ICompetitionsManager>();
            competitionManager.PositionCompetitionPlayers(competitionId, section);
            return RedirectToAction("Details", new { id = competitionId });
        }

        [HttpPost]
        public ActionResult StartCompetition(int competitionId)
        {
            var competitionManager = ServiceProvider.Get<ICompetitionsManager>();
            competitionManager.StartCompetition(competitionId);
            return RedirectToAction("Details", new { id = competitionId });
        }
        [HttpPost]
        public ActionResult FinishCompetition(int competitionId)
        {
            var competitionManager = ServiceProvider.Get<ICompetitionsManager>();
            competitionManager.FinishCompetition(competitionId);
            return RedirectToAction("Details", new { id = competitionId });
        }

        public ActionResult Details(int id)
        {
            var model = GetCompetitionDetailsModel(id);

            return View(model);
        }

        public ActionResult GetSectionMatches(int id, int section)
        {
            var details = GetCompetitionDetailsModel(id);
            var matches = details.Matches.Where(m => (int) m.Section == section);
            return Json(matches, JsonRequestBehavior.AllowGet);
        }

        private CompetitionDetailsModel GetCompetitionDetailsModel(int id)
        {
            var model = new CompetitionDetailsModel();
            var competitionEngine = ServiceProvider.Get<ICompetitionsEngine>();

            var competition = competitionEngine.GetCompetitionDetails(id);
            model.Id = id;
            model.Name = competition.Name;
            model.StartTime = competition.StartTime.ToShortDateString();
            model.EndTime = competition.EndTime.HasValue ? " - " + competition.EndTime.Value.ToShortDateString() : string.Empty;
            model.LastModified = competition.LastModified.ToString();
            model.Type = competition.Type;
            model.Status = competition.Status;
            model.ReferenceId = competition.ReferenceId;
            model.MainRefereeName = competition.MainRefereeName;
            model.MainRefereePhone = competition.MainRefereePhone;
            model.Site = competition.Site;
            model.SitePhone = competition.SitePhone;
            model.Players = competition.Players;
            model.CanAddToFinal = competition.CanAddToFinal;
            model.CanAddToQualifying = competition.CanAddToQualifying;
            model.ScoreCardLink= GetLinkTemplate("ScoreCard");
            model.StatsLink= GetLinkTemplate("MatchStats");
            model.Matches =
                competition.Matches.Select(
                    m => new CompetitionMatchViewModel()
                             {
                                 Id = m.Id,
                                 Section = m.Section,
                                 StartTime = m.StartTime,
                                 StartTimeType = m.StartTimeType,
                                 Status = m.Status,
                                 Round = m.Round,
                                 RoundRelativePosition = m.RoundRelativePosition,
                                 Position = m.Position,
                                 Winner = m.Winner,
                                 Result = m.Result.HasValue ? m.Result.Value : MatchResult.Win,
                                 Player1 = m.Player1.IsNotNull() ? new MatchPlayerViewModel(m.Player1) { Rank = competition.Players.First(cp=>cp.Id == m.Player1.Id).CompetitionRank } : null,
                                 Player2 = m.Player2.IsNotNull() ? new MatchPlayerViewModel(m.Player2) { Rank = competition.Players.First(cp => cp.Id == m.Player2.Id).CompetitionRank } : null,
                                 Player3 = m.Player3.IsNotNull() ? new MatchPlayerViewModel(m.Player3) { Rank = competition.Players.First(cp => cp.Id == m.Player3.Id).CompetitionRank } : null,
                                 Player4 = m.Player4.IsNotNull() ? new MatchPlayerViewModel(m.Player4) { Rank = competition.Players.First(cp => cp.Id == m.Player4.Id).CompetitionRank } : null,
                                 SetScores = m.SetScores,
                                 StartTimeHours = new[] {new SelectListItem(),}.Concat(GetStartTimeHours(m.StartTime)),
                                 StartTimeMinutes = new[] {new SelectListItem(),}.Concat(GetStartTimeMinutes(m.StartTime)),
                                 
                             }).ToArray();
            model.PlayingStarted = model.Matches.Any(m => m.Section == CompetitionSection.Final && (int)m.Status >= (int)MatchStatus.Playing);
            model.QualifyingPlayingStarted = model.Matches.Any(m => m.Section == CompetitionSection.Qualifying && (int)m.Status >= (int)MatchStatus.Playing);
            return model;
        }

        private IEnumerable<SelectListItem> GetStartTimeMinutes(DateTime? startTime)
        {
            var items = Enumerable.Range(0, 59).Where(i => i % 15 == 0).Select(
                    i => new SelectListItem() { Value = i.ToString(), Text = i.ToString().PadLeft(2, '0') }).ToArray();
            if (startTime.HasValue)
            {
                var selected = items.FirstOrDefault(i => int.Parse(i.Value) >= startTime.Value.ToLocalTime().Minute);
                if (selected.IsNotNull())
                {
                    selected.Selected = true;
                }
            }
            return items;
        }

        private IEnumerable<SelectListItem> GetStartTimeHours(DateTime? startTime)
        {
            var items = Enumerable.Range(0, 24).Select(i => new SelectListItem() { Value = i.ToString(), Text = i.ToString().PadLeft(2, '0') }).ToArray();
            if (startTime.HasValue)
            {
                var selected = items.FirstOrDefault(i => int.Parse(i.Value) >= startTime.Value.ToLocalTime().Hour);
                if (selected.IsNotNull())
                {
                    selected.Selected = true;
                }
            }
            return items;
        }

        public ActionResult Index(int startIndex = 0, int pageSize = 50)
        {
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var result = competitionsRepository.SearchCompetitions(new CompetitionSearchQuery() { StartIndex = startIndex, PageSize = pageSize });

            return View(result.Items);
        }

        public ActionResult UpdatePlayers(int id)
        {
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var competition = competitionsRepository.GetCompetition(id);

            return View(competition);
        }

        [HttpPost]
        public ActionResult UpdatePlayers(int competitionId, HttpPostedFileBase playersFile)
        {
            if (playersFile.IsNotNull() && playersFile.ContentLength > 0)
            {
                var manager = ServiceProvider.Get<ICompetitionsManager>();
                var url = AcceptCsvFile(playersFile, "CompetitionPlayers");
                manager.UpdateCompetitionPlayers(competitionId, url.ToString());
            }

            return RedirectToAction("Details", new { id = competitionId });
        }

        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePlayersPoints(int id, UpdatePlayerPointsModel[] updates)
        {
            var repository = ServiceProvider.Get<ICompetitionRepository>();
            repository.UpdateCompetitionPlayersPoints(id, updates.Select(u=>new UpdatePlayerPointsInfo(){PlayerId = u.Id, Points = u.Points, Position = u.Position}).ToArray());
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult ImportPlayers(HttpPostedFileBase competitionsPlayersFile)
        {
            if (competitionsPlayersFile.IsNotNull() && competitionsPlayersFile.ContentLength > 0)
            {
                var url = AcceptCsvFile(competitionsPlayersFile, "CompetitionPlayers");
                var manager = ServiceProvider.Get<ICompetitionsManager>();
                try
                {
                    manager.LoadCompetitionsPlayers(url.ToString());
                }
                catch (Exception anyException)
                {
                    ModelState.AddModelError("LoadingCompetitionsFile", anyException.ToString());
                    return Import();
                }

            }
            return RedirectToAction("Index");            
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase competitionsFile)
        {
            if (competitionsFile.IsNotNull() && competitionsFile.ContentLength > 0)
            {
                var url = AcceptCsvFile(competitionsFile, "CompetitionPlayers");
                var manager = ServiceProvider.Get<ICompetitionsManager>();
                var validationResult = manager.ValidateCompetitionsFile(url.ToString());
                if (validationResult == LoadCompetitionsValidationResult.Valid)
                {
                    manager.LoadCompetitions(url.ToString());
                }
                else
                {
                    ModelState.AddModelError(validationResult.ToString(), TranslateValidationResult(validationResult));
                    return View();
                }
            }
            return RedirectToAction("Index");
        }

        private string TranslateValidationResult(LoadCompetitionsValidationResult validationResult)
        {
            return "[" + validationResult.ToString() + "]";
        }


        [HttpPost]
        public ActionResult Create(CreateCompetitionParameters parameters, HttpPostedFileBase playersFile, HttpPostedFileBase qualifyingPlayersFile)
        {
            var manager = ServiceProvider.Get<ICompetitionsManager>();
            var createCompetitionInfo = CreateCompetitionInfo(parameters);
            if (playersFile.IsNotNull())
            {
                createCompetitionInfo.PlayersFileUrl = AcceptCsvFile(playersFile, "CompetitionPlayers").ToString();
            }

            manager.Create(createCompetitionInfo);

            return RedirectToAction("Index");
        }

        private static CreateCompetitionInfo CreateCompetitionInfo(CreateCompetitionParameters parameters)
        {
            var createCompetitionInfo = new CreateCompetitionInfo();
            createCompetitionInfo.Name = parameters.Name;
            createCompetitionInfo.StartTime = parameters.StartTime;
            createCompetitionInfo.EndTime = parameters.EndTime;
            createCompetitionInfo.TypeId = parameters.Type;
            createCompetitionInfo.MainReferee = parameters.MainReferee;
            createCompetitionInfo.MainRefereePhone = parameters.MainRefereePhone;
            createCompetitionInfo.Site = parameters.Site;
            createCompetitionInfo.SitePhone = parameters.SitePhone;
            return createCompetitionInfo;
        }

        private Uri AcceptCsvFile(HttpPostedFileBase file, string folder)
        {
            var providerFactory = ServiceProvider.Get<IRemoteStorageProviderFactory>();
            var remoteStorageProvider = providerFactory.Create();

            var storageInfo = new RemoteStorageInfo()
                                  {
                                      ContentType = "text/csv",
                                      FileName =
                                          "{0}[{1}].csv".ParseTemplate(Path.GetFileNameWithoutExtension(file.FileName),
                                                                       Guid.NewGuid().ToString()),
                                      Folder = folder,
                                      Repository = "UploadedFiles"
                                  };
            var url = remoteStorageProvider.Store(file.InputStream, storageInfo);
            return url;
        }

        public ActionResult Create()
        {
            var createModel = CreateCompetitionModel();
            return View(createModel);
        }

        private static CreateCompetitionModel CreateCompetitionModel()
        {
            var createModel = new CreateCompetitionModel();
            var competitionTypeRepository = ServiceProvider.Get<ICompetitionTypeRepository>();
            var competitionTypes = competitionTypeRepository.GetCompetitionTypes();
            createModel.AvailableTypes = competitionTypes.Select(ct => new CompetitionTypeReference() { Id = ct.Id, Name = ct.Name });

            return createModel;
        }

        public ActionResult Update(int id)
        {
            return View();
        }




    }
}
