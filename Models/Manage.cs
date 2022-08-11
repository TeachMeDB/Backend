using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Manage
    {
        public decimal EmployeeId { get; set; }
        public string AssetsId { get; set; } = null!;
        public string ManageType { get; set; } = null!;
        public DateTime ManageDate { get; set; }
        public string ManageReason { get; set; } = null!;
        public string ManageCost { get; set; } = null!;
        public decimal? Repair { get; set; }

        public virtual Asset Assets { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;
        public virtual Repair? RepairNavigation { get; set; }
    }
}
