using Simcha_Fund.Data;
using Simcha_Fund.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Simcha_Fund.Controllers
{
    public class SimchosController : Controller
    {
        public ActionResult Index()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            var vm = new SimchaIndexViewModel();
            var repo = new SimchaFundRepository(Properties.Settings.Default.ConStr);
            vm.Simchos = repo.GetAllSimchos();
            vm.TotalContributors = repo.GetContributorCount();
            return View(vm);
        }

        [HttpPost]
        public ActionResult New(string name, DateTime date)
        {
            var repo = new SimchaFundRepository(Properties.Settings.Default.ConStr);
            var simcha = new Simcha() { SimchaName = name, SimchaDate = date };
            repo.AddSimcha(simcha);

            TempData["Message"] = $"Mazel Tov on: {simcha.SimchaName}!";
            return RedirectToAction("index");
        }

        public ActionResult Contributions(int simchaId)
        {
            var repo = new SimchaFundRepository(Properties.Settings.Default.ConStr);
            Simcha simcha = repo.GetSimchaById(simchaId);
            IEnumerable<SimchaContributor> contributors = repo.GetSimchaContributors(simchaId);

            var viewModel = new ContributionsViewModel
            {
                Contributors = contributors,
                Simcha = simcha
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UpdateContributions(List<ContributionInclusion> contributors, int simchaId)
        {
            var repo = new SimchaFundRepository(Properties.Settings.Default.ConStr);
            repo.UpdateSimchaContributions(simchaId, contributors);
            TempData["Message"] = "Simcha updated successfully";
            return RedirectToAction("index");
        }
    }
}