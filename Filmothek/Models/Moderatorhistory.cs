using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public class ModeratorHistory
    {
        [Key]
        public int Id { get; set; }

        public int ModeratorId { get; set; }
        public Moderator Moderator { get; set; }

        public string Activity { get; set; }
        public DateTime Date { get; set; }
    }
}
