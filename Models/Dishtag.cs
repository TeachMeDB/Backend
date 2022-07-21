using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Dishtag
    {
        public Dishtag()
        {
            Dishes = new HashSet<Dish>();
        }

        public decimal DtagId { get; set; }
        public string DtagName { get; set; } = null!;

        public virtual ICollection<Dish> Dishes { get; set; }
    }
}
