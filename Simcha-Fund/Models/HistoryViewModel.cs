﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Simcha_Fund.Models
{
    public class HistoryViewModel
    {
        public IEnumerable<Transaction> Transactions { get; set; }
        public string ContributorName { get; set; }
        public decimal ContributorBalance { get; set; }
    }
}