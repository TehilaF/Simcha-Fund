using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simcha_Fund.Data;

namespace Simcha_Fund.Models
{
    public class ContributorsIndexViewModel
    {
        public IEnumerable<Contributor> Contributors { get; set; }
        public decimal Total { get; set; }
    }
}