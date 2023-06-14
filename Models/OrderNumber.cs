using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class OrderNumber
    {
        public DateTime OrderDate { get; set; }
        public string OrderNumber1 { get; set; } = null!;
        public string? UserName { get; set; }
        public string? OrderId { get; set; }

        public virtual Orderlist? Order { get; set; }
        public virtual Vip? UserNameNavigation { get; set; }
    }
}
