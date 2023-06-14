using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Salary
    {
        public Salary()
        {
            Employees = new HashSet<Employee>();
            WorkPlans = new HashSet<WorkPlan>();
        }

        public string Occupation { get; set; } = null!;
        public decimal? Amount { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<WorkPlan> WorkPlans { get; set; }
    }
}
