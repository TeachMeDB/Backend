using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            Provides = new HashSet<Provide>();
        }

        public decimal SId { get; set; }
        public string? SName { get; set; }
        public string? Address { get; set; }
        public string? Contact { get; set; }
        public decimal? DirectorId { get; set; }

        public virtual Employee? Director { get; set; }
        public virtual ICollection<Provide> Provides { get; set; }
    }
}
