using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Promotion
    {
        public Promotion()
        {
            Hasdishes = new HashSet<Hasdish>();
        }

        public decimal PromotionId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Hasdish> Hasdishes { get; set; }
    }
}
