﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public abstract class Person
    {
        [Key]
        public int Id { get; set; }
        public string Nachname { get; set; }
        public string Vorname { get; set; }
        public string Addresse { get; set; }
        public string Login { get; set; }
        public string Pw { get; set; }
        public short Berechtigung { get; set; }
    }
}

