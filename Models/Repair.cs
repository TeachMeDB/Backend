using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Repair
    {
        public string Assetsid { get; set; } = null!;
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string Longitude { get; set; } = null!;
        public string Latitude { get; set; } = null!;

        public virtual Asset Assets { get; set; } = null!;
    }
}
