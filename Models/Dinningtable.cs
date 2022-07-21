using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Dinningtable
    {
        public decimal TableId { get; set; }
        public decimal? CustomerNumber { get; set; }
        public decimal? TableCapacity { get; set; }
        public string? Occupied { get; set; }
    }
}
