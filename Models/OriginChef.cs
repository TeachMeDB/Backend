using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class OriginChef
    {
        public string DishOrderId { get; set; } = null!;
        public decimal? ChefId { get; set; }

        public virtual Employee? Chef { get; set; }
        public virtual Orderlist DishOrder { get; set; } = null!;
    }
}
