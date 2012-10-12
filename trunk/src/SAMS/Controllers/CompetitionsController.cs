using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAMS.Models;
using Simple;
using Simple.Common.Storage;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;

namespace SAMS.Controllers
{
    public class CompetitionsController : Controller
    {
        //
        // GET: /Competitions/

        public ActionResult Details(int id)
        {
            var model = new CompetitionDetailsModel();
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            
            var competition = competitionsRepository.GetCompetitionDetails(id);
            model.Id = id;
            model.Name = competition.Name;
            model.StartTime = competition.StartTime;

            model.Matches =
                competition.Matches.Select(
                    m => new CompetitionMatchViewModel()
                             {
                                 Id = m.Id,
                                 StartTime = m.StartTime,
                                 Status = m.Status,
                                 Round=m.Round,
                                 Position = m.Position
                             }).ToArray();
            return View(model);
        }

        public ActionResult Index(int startIndex = 0, int pageSize = 50)
        {
            var competitionsRepository = ServiceProvider.Get<ICompetitionRepository>();
            var result = competitionsRepository.SearchCompetitions(new CompetitionSearchQuery() { StartIndex = startIndex, PageSize = pageSize});

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
            return RedirectToAction("Index");
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
            var createCompetitionInfo = new CreateCompetitionInfo();
            createCompetitionInfo.Name = parameters.Name;
            createCompetitionInfo.StartTime = parameters.StartTime;
            createCompetitionInfo.TypeId = parameters.Type;

            var url = AcceptCsvFile(playersFile, "CompetitionPlayers");

            createCompetitionInfo.PlayersFileUrl = url.ToString();
            manager.Create(createCompetitionInfo);

            return RedirectToAction("Index");
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
            createModel.AvailableTypes = competitionTypes.Select(ct => new CompetitionTypeReference() { Id= ct.Id, Name = ct.Name });
                
            return createModel;
        }

        public ActionResult Update(int id)
        {
            return View();
        }
        



    }
}
