using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Simple.ComponentModel;
using Simple.SAMS.Contracts.Competitions;

namespace SAMS.Controllers
{
    public class CompetitionTypesController : Controller
    {
        //
        // GET: /CompetitionTypes/

        public ActionResult Index()
        {
            var competitionTypesRepository = ServiceProvider.Get<ICompetitionTypeRepository>();
            var types = competitionTypesRepository.GetCompetitionTypes();

            return View(types);
        }
        
        [HttpPost]
        public ActionResult Edit(CompetitionType competitionType)
        {
            var competitionTypesRepository = ServiceProvider.Get<ICompetitionTypeRepository>();
            competitionTypesRepository.Update(competitionType);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Create(CompetitionType competitionType)
        {
            var competitionTypesRepository = ServiceProvider.Get<ICompetitionTypeRepository>();
            competitionTypesRepository.Add(competitionType);
            return RedirectToAction("Index");
        }        
        public ActionResult Create()
        {
            var competitionType = new CompetitionType();
            return View(competitionType);
        }

        public ActionResult BriefDetails(int id)
        {
            var competitionTypesRepository = ServiceProvider.Get<ICompetitionTypeRepository>();
            var competitionType = competitionTypesRepository.Get(id);

            return View(competitionType);
        }

        public ActionResult Edit(int id)
        {
            var competitionTypesRepository = ServiceProvider.Get<ICompetitionTypeRepository>();
            var competitionType = competitionTypesRepository.Get(id);
            return View(competitionType);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var competitionTypesRepository = ServiceProvider.Get<ICompetitionTypeRepository>();
            competitionTypesRepository.Remove(id);
            return new HttpStatusCodeResult(200);
        }
    }
}
