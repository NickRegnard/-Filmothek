﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public class CustomerHistory
    {
        [Key]
        public int Id { get; set; }

        public int MovieId {get; set;}
        public Movie Movie { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string Note { get; set; }
        public bool isBorrowing { get; set; }
    }
}
