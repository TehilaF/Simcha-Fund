using Simcha_Fund.Data;
using Simcha_Fund.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Simcha_Fund.Controllers
{
    public class ContributorsController : Controller
    {
        public ActionResult Index()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            var vm = new ContributorsIndexViewModel();
            var repo = new SimchaFundRepository(Properties.Settings.Default.ConStr);
            vm.Contributors = repo.GetContributors();
            vm.Total = repo.GetTotal();
            return View(vm);
        }

        [HttpPost]
        public ActionResult New(Contributor contributor, decimal initialDeposit)
        {
            var repo = new SimchaFundRepository(Properties.Settings.Default.ConStr);
            repo.AddContributor(contributor);
            var deposit = new Deposit
            {
                Amount = initialDeposit,
                ContributorId = contributor.Id,
                Date = contributor.Date
            };
            repo.AddDeposit(deposit);
            TempData["Message"] = $"New contributor Created! ID: {contributor.Id}";
            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult Deposit(Deposit deposit)
        {
            var repo = new SimchaFundRepository(Properties.Settings.Default.ConStr);
            repo.AddDeposit(deposit);
            TempData["Message"] = $"Deposited ${deposit.Amount} for {repo.GetContributorById(deposit.ContributorId).FirstName} {repo.GetContributorById(deposit.ContributorId).LastName}";
            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult Edit(Contributor contributor)
        {
            var repo = new SimchaFundRepository(Properties.Settings.Default.ConStr);
            repo.UpdateContributor(contributor);
            TempData["Message"] = "Updated successfully";
            return RedirectToAction("index");
        }

        public ActionResult History(int contribId)
        {
            var repo = new SimchaFundRepository(Properties.Settings.Default.ConStr);
            IEnumerable<Deposit> deposits = repo.GetDepositsById(contribId);
            IEnumerable<Contribution> contributions = repo.GetContributionsById(contribId);

            IEnumerable<Transaction> transactions = deposits.Select(d => new Transaction
            {
                Action = "Deposit",
                Amount = d.Amount,
                Date = d.Date
            });
            
            if(contributions != null)
            {
                transactions.Concat(contributions.Select(c => new Transaction
                {
                    Action = "Contribution for " + repo.GetSimchaById(c.SimchaId).SimchaName,
                    Amount = c.Amount * -1,
                    Date = (DateTime)repo.GetSimchaById(c.SimchaId).SimchaDate
                }));
            }

            transactions.OrderByDescending(t => t.Date);
            var vm = new HistoryViewModel();
            vm.ContributorName = $"{repo.GetContributorById(contribId).FirstName} {repo.GetContributorById(contribId).LastName}";
            vm.ContributorBalance = repo.GetContributorBalance(contribId);
            vm.Transactions = transactions;

            return View(vm);
        }
    }
}