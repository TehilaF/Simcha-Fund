using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Simcha_Fund.Models
{
    public class Transaction
    {
        public decimal Amount { get; set; }
        public string Action { get; set; }
        public DateTime Date { get; set; }
    }
}