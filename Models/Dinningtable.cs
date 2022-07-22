using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Dinningtable
    {
        public Dinningtable()
        {
            Orderlists = new HashSet<Orderlist>();
        }

        public decimal TableId { get; set; }
        public decimal? CustomerNumber { get; set; }
        public decimal? TableCapacity { get; set; }
        public string? Occupied { get; set; }

        public virtual ICollection<Orderlist> Orderlists { get; set; }
    }
}
