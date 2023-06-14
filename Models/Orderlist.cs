using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Orderlist
    {
        public Orderlist()
        {
            Dishorderlists = new HashSet<Dishorderlist>();
            OrderNumbers = new HashSet<OrderNumber>();
        }

        public string OrderId { get; set; } = null!;
        public DateTime CreationTime { get; set; }
        public decimal TableId { get; set; }
        public string OrderStatus { get; set; } = null!;

        public virtual Dinningtable Table { get; set; } = null!;
        public virtual ICollection<Dishorderlist> Dishorderlists { get; set; }
        public virtual ICollection<OrderNumber> OrderNumbers { get; set; }
    }
}
