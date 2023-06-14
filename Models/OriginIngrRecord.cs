using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class OriginIngrRecord
    {
        public string DishOrderId { get; set; } = null!;
        public decimal RecordId { get; set; }

        public virtual Orderlist DishOrder { get; set; } = null!;
        public virtual IngredientRecord Record { get; set; } = null!;
    }
}
