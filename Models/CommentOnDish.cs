using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class CommentOnDish
    {
        public string CommentId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public decimal DishId { get; set; }
        public DateTime? CommentTime { get; set; }
        public decimal? Stars { get; set; }
        public string? CommentContent { get; set; }

        public virtual Dish Dish { get; set; } = null!;
        public virtual Vip UserNameNavigation { get; set; } = null!;
    }
}
