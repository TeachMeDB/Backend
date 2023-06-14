using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Attend
    {
        public decimal PlanId { get; set; }
        public decimal EmployeeId { get; set; }
        public bool? Attendance { get; set; }

        public virtual Employee Employee { get; set; } = null!;
        public virtual WorkPlan Plan { get; set; } = null!;
    }
}
