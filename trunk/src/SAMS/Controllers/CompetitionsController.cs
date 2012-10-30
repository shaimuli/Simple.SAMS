using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SAMS.Models;
using Simple;
using Simple.Common.Storage;
using Simple.ComponentModel;
using Simple.Data;
using Simple.SAMS.Contracts.Competitions;

namespace SAMS.Controllers
{
    public class CompetitionsController : Controller
    {
        [HttpPost]
        public ActionResult UpdateMatchResults(FormCollection values)
        {
            SystemMonitor.Info("Queued...{0}", string.Join(",", values.AllKeys));
            return new HttpStatusCodeResult(200);
        }

        //
        // GET: /Competitions/
        
        [HttpPost]
        public ActionResult StartCompetition(int competitionId)
        {
            var competitionManager = ServiceProvider.Get<ICompetitionsManager>();
            competitionManager.StartCompetition(competitionId);
            return RedirectToAction("Details", new {id = competitionId});
        }
        [HttpPost]
        public ActionResult FinishCompetition(int competitionId)
        {
            var competitionManager = ServiceProvider.Get<ICompetitionsManager>();
            competitionManager.FinishCompetition(competitionId);
            return RedirectToAction("Details", new {id = competitionId});
        }
        
        public ActionResult Details(int id)
        {
            var model = new CompetitionDetailsModel();
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();

            var competition = competitionsRepository.GetCompetitionDetails(id);
            model.Id = id;
            model.Name = competition.Name;
            model.StartTime = competition.StartTime.ToShortDateString();
            model.EndTime = competition.EndTime.HasValue ? " - " + competition.EndTime.Value.ToShortDateString() : string.Empty;
            model.LastModified = competition.LastModified.ToString();
            model.Type = competition.Type;
            model.Status = competition.Status;
            model.ReferenceId = competition.ReferenceId;
            model.Players = competition.Players;

            model.Matches =
                competition.Matches.Select(
                    m => new CompetitionMatchViewModel()
                             {
                                 Id = m.Id,
                                 Section = m.Section,
                                 StartTime = m.StartTime,
                                 Status = m.Status,
                                 Round = m.Round,
                                 Position = m.Position,
                                 Player1 = m.Player1.IsNotNull() ? new MatchPlayerViewModel(m.Player1) : null,
                                 Player2 = m.Player2.IsNotNull() ? new MatchPlayerViewModel(m.Player2) : null,
                                 Player3 = m.Player3.IsNotNull() ? new MatchPlayerViewModel(m.Player3) : null,
                                 Player4 = m.Player4.IsNotNull() ? new MatchPlayerViewModel(m.Player4) : null,
                                 SetScores = m.SetScores
                             }).ToArray();
            
            return View(model);
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
            var manager = ServiceProvider.Get<ICompetitionsManager>();
            var url = AcceptCsvFile(playersFile, "CompetitionPlayers");
            manager.UpdateCompetitionPlayers(competitionId, url.ToString());
            return RedirectToAction("Details", new { id = competitionId });
        }

        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase competitionsFile)
        {
            var url = AcceptCsvFile(competitionsFile, "CompetitionPlayers");
            var manager = ServiceProvider.Get<ICompetitionsManager>();
            manager.LoadCompetitions(url.ToString());

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Create(CreateCompetitionParameters parameters, HttpPostedFileBase playersFile)
        {
            var manager = ServiceProvider.Get<ICompetitionsManager>();
            var createCompetitionInfo = CreateCompetitionInfo(parameters);
            var url = default(string);
            if (playersFile.IsNotNull())
            {
                url = AcceptCsvFile(playersFile, "CompetitionPlayers").ToString();
            }

            manager.Create(createCompetitionInfo, url);

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
