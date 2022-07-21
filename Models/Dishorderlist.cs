using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Dishorderlist
    {
        public string DishOrderId { get; set; } = null!;
        public string OrderId { get; set; } = null!;
        public string DishId { get; set; } = null!;
        public decimal FinalPayment { get; set; }
        public string DishStatus { get; set; } = null!;
    }
}
