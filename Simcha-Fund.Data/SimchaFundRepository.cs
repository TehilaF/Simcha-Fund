using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simcha_Fund.Data
{
    public class SimchaFundRepository
    {
        private string _connectionString;

        public SimchaFundRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Contributor> GetContributors()
        {
            List<Contributor> contributors = new List<Contributor>();
            using (var context = new ContributorDataContext(_connectionString))
            {
                contributors = context.Contributors.ToList();
            }
            foreach (Contributor c in contributors)
            {
                c.Balance = GetContributorBalance(c.Id);
            }
            return contributors.AsEnumerable();
        }

        public decimal GetTotal()
        {
            //can make code prettier by using GetContributorBalance
            //but that's two database hits per contributor - this is two hits total
            decimal contributions = 0;
            decimal deposits = 0;
            using (var context = new ContributionDataContext(_connectionString))
            {
                contributions = context.Contributions.Any() ? context.Contributions.Select(c => c.Amount).Sum() : 0;
            }
            using (var context = new DepositDataContext(_connectionString))
            {
                deposits = context.Deposits.Select(d => d.Amount).Sum();
            }
            return deposits - contributions;
        }

        public void AddContributor(Contributor contributor)
        {
            using (var context = new ContributorDataContext(_connectionString))
            {
                context.Contributors.InsertOnSubmit(contributor);
                context.SubmitChanges();
            }
        }

        public void AddDeposit(Deposit deposit)
        {
            using (var context = new DepositDataContext(_connectionString))
            {
                context.Deposits.InsertOnSubmit(deposit);
                context.SubmitChanges();
            }
        }

        public decimal GetContributorBalance(int contributorId)
        {
            decimal contributions = 0;
            decimal deposits = 0;
            using (var context = new DepositDataContext(_connectionString))
            {
                deposits = context.Deposits.Where(d => d.ContributorId == contributorId).Select(d => d.Amount).Sum();
            }
            using (var context = new ContributionDataContext(_connectionString))
            {
                contributions = context.Contributions.Any(c => c.ContributorId == contributorId) ? context.Contributions.Where(c => c.ContributorId == contributorId).Select(c => c.Amount).Sum() : 0;
            }
            return deposits - contributions;
        }

        public void UpdateContributor(Contributor contributor)
        {
            using (var context = new ContributorDataContext(_connectionString))
            {
                context.Contributors.Attach(contributor);
                context.Refresh(RefreshMode.KeepCurrentValues, contributor);
                context.SubmitChanges();
            }
        }

        public IEnumerable<Contribution> GetContributionsById(int contribId)
        {
            List<Contribution> contributions = new List<Contribution>();
            using (var context = new ContributionDataContext(_connectionString))
            {
                contributions = context.Contributions.Any(c => c.ContributorId == contribId) ? context.Contributions.Where(c => c.ContributorId == contribId).ToList() : null;
            }
            return contributions;
        }

        public IEnumerable<Deposit> GetDepositsById(int contribId)
        {
            List<Deposit> deposits = new List<Deposit>();
            using (var context = new DepositDataContext(_connectionString))
            {
                deposits = context.Deposits.Where(d => d.ContributorId == contribId).ToList();
            }
            return deposits;
        }

        public Contributor GetContributorById(int id)
        {
            using (var context = new ContributorDataContext(_connectionString))
            {
                return context.Contributors.FirstOrDefault(c => c.Id == id);
            }
        }

        //Use this when implement delete feature
        public void DeleteContributor(int id)
        {
            using (var context = new ContributionDataContext(_connectionString))
            {
                context.ExecuteCommand("DELETE FROM Contributors WHERE Id = {0}", id); //can do with context.Delete but that requires 2 database hits
            }
        }

        public IEnumerable<Simcha> GetAllSimchos()
        {
            List<Simcha> simchos = new List<Simcha>();
            using (var context = new SimchaDataContext(_connectionString))
            {
                simchos = context.Simchas.ToList();
            }
            foreach (Simcha s in simchos)
            {
                SetSimchaTotals(s);
            }
            return simchos;
        }

        private void SetSimchaTotals(Simcha simcha)
        {
            using (var context = new ContributionDataContext(_connectionString))
            {
                simcha.Total = context.Contributions.Any(c => c.SimchaId == simcha.Id) ? context.Contributions.Where(c => c.SimchaId == simcha.Id).Select(c => c.Amount).Sum() : 0;
                simcha.ContributorAmount = context.Contributions.Any(c => c.SimchaId == simcha.Id) ? context.Contributions.Count(c => c.SimchaId == simcha.Id) : 0;
            }
        }

        public void AddSimcha(Simcha simcha)
        {
            using (var context = new SimchaDataContext(_connectionString))
            {
                context.Simchas.InsertOnSubmit(simcha);
                context.SubmitChanges();
            }
        }

        public int GetContributorCount()
        {
            using (var context = new ContributorDataContext(_connectionString))
            {
                return context.Contributors.Count();
            }
        }

        public Simcha GetSimchaById(int simchaId)
        {
            using (var context = new SimchaDataContext(_connectionString))
            {
                return context.Simchas.FirstOrDefault(s => s.Id == simchaId);
            }
        }

        public IEnumerable<SimchaContributor> GetSimchaContributors(int simchaId)
        {
            List<SimchaContributor> simchaContributors = new List<SimchaContributor>();
            using (var context = new ContributionDataContext(_connectionString))
            {
                var allContributors = GetContributors();
                var currentContributions = context.Contributions.Where(c => c.SimchaId == simchaId);
                foreach (Contributor c in allContributors)
                {
                    simchaContributors.Add(new SimchaContributor
                    {
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        AlwaysInclude = c.AlwaysInclude,
                        ContributorId = c.Id,
                        Balance = GetContributorBalance(c.Id),
                        Amount = currentContributions.FirstOrDefault(contribution => contribution.ContributorId == c.Id) != null ? currentContributions.FirstOrDefault(contribution => contribution.ContributorId == c.Id).Amount : 0
                    });
                }
                return simchaContributors.AsEnumerable();
            }
        }

        public void UpdateSimchaContributions(int simchaId, List<ContributionInclusion> contributors)
        {
            using (var context = new ContributionDataContext(_connectionString))
            {
                context.ExecuteCommand("DELETE FROM Contributions WHERE SimchaId = {0}", simchaId); //can do with context.Delete but that requires 2 database hits
                foreach (var c in contributors)
                {
                    context.Contributions.InsertOnSubmit(new Contribution
                    {
                        SimchaId = simchaId,
                        ContributorId = c.ContributorId,
                        Amount = c.Amount
                    });
                }
            }
        }
    }
}
