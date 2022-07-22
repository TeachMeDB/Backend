using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Dishorderlist
    {
        public string DishOrderId { get; set; } = null!;
        public string OrderId { get; set; } = null!;
        public decimal DishId { get; set; }
        public decimal FinalPayment { get; set; }
        public string DishStatus { get; set; } = null!;

        public virtual Dish Dish { get; set; } = null!;
        public virtual Orderlist Order { get; set; } = null!;
    }
}
