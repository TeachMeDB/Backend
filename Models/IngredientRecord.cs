using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class IngredientRecord
    {
        public decimal RecordId { get; set; }
        public decimal? IngrId { get; set; }
        public DateTime? PurchasingDate { get; set; }
        public decimal? Surplus { get; set; }
        public decimal? Purchases { get; set; }
        public string? MeasureUnit { get; set; }
        public decimal? ShelfLife { get; set; }
        public DateTime? ProducedDate { get; set; }
        public decimal? Price { get; set; }
        public decimal? DirectorId { get; set; }

        public virtual Employee? Director { get; set; }
        public virtual Ingredient? Ingr { get; set; }
        public virtual Provide Provide { get; set; } = null!;
    }
}
