using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Asset
    {
        public Asset()
        {
            Manages = new HashSet<Manage>();
        }

        public string AssetsId { get; set; } = null!;
        public string AssetsType { get; set; } = null!;
        public decimal EmployeeId { get; set; }
        public decimal AssetsStatus { get; set; }

        public virtual Employee Employee { get; set; } = null!;
        public virtual ICollection<Manage> Manages { get; set; }
    }
}
