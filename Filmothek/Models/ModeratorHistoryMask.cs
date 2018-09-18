using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmothek.Models
{
    public class ModeratorHistoryMask
    {
        public int ModeratorId { get; set; }
        public string Activity { get;set; }
        public DateTime Date { get; set; }

        public ModeratorHistoryMask(int Id, string Activity, DateTime Date)
        {
            this.ModeratorId = ModeratorId;
            this.Activity = Activity;
            this.Date = Date;
        }

        public ModeratorHistoryMask() { }
    }
}
