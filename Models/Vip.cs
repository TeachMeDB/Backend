using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Vip
    {
        public Vip()
        {
            CommentOnDishes = new HashSet<CommentOnDish>();
            CommentOnServices = new HashSet<CommentOnService>();
            OrderNumbers = new HashSet<OrderNumber>();
        }

        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime? Birthday { get; set; }
        public string? Gender { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Credit { get; set; }
        public string? IsDefault { get; set; }

        public virtual ICollection<CommentOnDish> CommentOnDishes { get; set; }
        public virtual ICollection<CommentOnService> CommentOnServices { get; set; }
        public virtual ICollection<OrderNumber> OrderNumbers { get; set; }
    }
}
