using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Audit
    {
        public DateTime LogTime { get; set; }
        public string Log { get; set; } = null!;
    }
}
