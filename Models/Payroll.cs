using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Payroll
    {
        public DateTime PayDatetime { get; set; }
        public decimal EmployeeId { get; set; }

        public virtual Employee Employee { get; set; } = null!;
    }
}
