using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Ingredient
    {
        public Ingredient()
        {
            IngredientRecords = new HashSet<IngredientRecord>();
            Dishes = new HashSet<Dish>();
        }

        public decimal IngrId { get; set; }
        public string IngrName { get; set; } = null!;
        public string IngrDescription { get; set; } = null!;
        public string? IngrType { get; set; }

        public virtual ICollection<IngredientRecord> IngredientRecords { get; set; }

        public virtual ICollection<Dish> Dishes { get; set; }
    }
}
