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
        public int? Length { get; set; }
        public bool IsSeries { get; set; }
        public float? Rating { get; set; }
        public float? Price { get; set; }
        public string LanguageDub { get; set; }
        public string LanguageSub { get; set; }
        public DateTime Release { get; set; }
        public int? FSK { get; set; }
        public string Content { get; set; }

        public List<CustomerHistory> CustomerHistory { get; set; }
    }
}
