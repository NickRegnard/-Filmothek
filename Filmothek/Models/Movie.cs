﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{

    public class Movie
    {
        [Key]
        public int Id { get; set; }
        public string FilmName { get; set; }
        public string Genre { get; set; }
        public string Lenght { get; set; }
        public bool IsSeries { get; set; }
        public long Rating { get; set; }
        public long Price { get; set; }
        public string Language { get; set; }
        public string Release { get; set; }
        public bool FSK { get; set; }
        public string Inhalt { get; set; }
    }
}
