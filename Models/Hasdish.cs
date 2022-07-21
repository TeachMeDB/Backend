using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Hasdish
    {
        public decimal PromotionId { get; set; }
        public decimal DishId { get; set; }
        public decimal? Discount { get; set; }

        public virtual Dish Dish { get; set; } = null!;
        public virtual Promotion Promotion { get; set; } = null!;
    }
}
