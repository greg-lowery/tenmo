﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApiTests
{
    public class Account
    {
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public decimal? Balance { get; set; }
    }
}
