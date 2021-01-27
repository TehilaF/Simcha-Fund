using Simcha_Fund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Simcha_Fund.Models
{
    public class SimchaIndexViewModel
    {
        public IEnumerable<Simcha> Simchos { get; set; }
        public int TotalContributors { get; set; }
    }
}