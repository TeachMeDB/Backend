using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Provide
    {
        public decimal RecordId { get; set; }
        public decimal SId { get; set; }

        public virtual IngredientRecord Record { get; set; } = null!;
        public virtual Supplier SIdNavigation { get; set; } = null!;
    }
}
