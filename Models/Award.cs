using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Award
    {
        public Award()
        {
            Prizes = new HashSet<Prize>();
        }

        public string Lv { get; set; } = null!;
        public decimal? Amount { get; set; }

        public virtual ICollection<Prize> Prizes { get; set; }
    }
}
