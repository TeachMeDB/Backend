using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Dish
    {
        public Dish()
        {
            CommentOnDishes = new HashSet<CommentOnDish>();
            Hasdishes = new HashSet<Hasdish>();
            Chefs = new HashSet<Employee>();
            Dtags = new HashSet<Dishtag>();
            Ingrs = new HashSet<Ingredient>();
        }

        public decimal DishId { get; set; }
        public string DishName { get; set; } = null!;
        public decimal DishPrice { get; set; }
        public string DishDescription { get; set; } = null!;

        public virtual ICollection<CommentOnDish> CommentOnDishes { get; set; }
        public virtual ICollection<Hasdish> Hasdishes { get; set; }

        public virtual ICollection<Employee> Chefs { get; set; }
        public virtual ICollection<Dishtag> Dtags { get; set; }
        public virtual ICollection<Ingredient> Ingrs { get; set; }
    }
}
