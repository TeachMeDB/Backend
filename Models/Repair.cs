using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Repair
    {
        public Repair()
        {
            Manages = new HashSet<Manage>();
        }

        public decimal RepairId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }

        public virtual ICollection<Manage> Manages { get; set; }
    }
}
