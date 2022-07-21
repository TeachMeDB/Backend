using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Orderlist
    {
        public Orderlist()
        {
            OrderNumbers = new HashSet<OrderNumber>();
        }

        public string OrderId { get; set; } = null!;
        public DateTime CreationTime { get; set; }
        public string TableId { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;

        public virtual ICollection<OrderNumber> OrderNumbers { get; set; }
    }
}
