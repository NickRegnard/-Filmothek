﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public class Moderator : Person
    {
        public List<ModeratorHistory> ModeratorHistory { get; set; }
    }
}
