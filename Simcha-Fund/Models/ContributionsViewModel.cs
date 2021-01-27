using Simcha_Fund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Simcha_Fund.Models
{
    public class ContributionsViewModel
    {
        public Simcha Simcha { get; set; }
        public IEnumerable<SimchaContributor> Contributors { get; set; }
    }
}