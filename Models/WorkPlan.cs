using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class WorkPlan
    {
        public WorkPlan()
        {
            Attends = new HashSet<Attend>();
        }

        public decimal Id { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public string? Place { get; set; }
        public string? Occupation { get; set; }
        public decimal? No { get; set; }

        public virtual Salary? OccupationNavigation { get; set; }
        public virtual ICollection<Attend> Attends { get; set; }
    }
}
