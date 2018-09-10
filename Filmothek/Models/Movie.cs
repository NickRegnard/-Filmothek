using System;
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
        public string MovieName { get; set; }
        public string Genre { get; set; }
        public string Lenght { get; set; }
        public bool IsSeries { get; set; }
        public long Rating { get; set; }
        public long Prise { get; set; }
        public string Language { get; set; }
        public string Release { get; set; }
        public int FSK { get; set; }
        public string Content { get; set; }
    }
}
