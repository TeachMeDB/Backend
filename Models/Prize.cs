using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Prize
    {
        public DateTime PrizeDatetime { get; set; }
        public decimal EmployeeId { get; set; }
        public string Lv { get; set; } = null!;

        public virtual Employee Employee { get; set; } = null!;
        public virtual Award LvNavigation { get; set; } = null!;
    }
}
