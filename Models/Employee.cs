using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Assets = new HashSet<Asset>();
            Attends = new HashSet<Attend>();
            IngredientRecords = new HashSet<IngredientRecord>();
            Manages = new HashSet<Manage>();
            Payrolls = new HashSet<Payroll>();
            Prizes = new HashSet<Prize>();
            Suppliers = new HashSet<Supplier>();
            Dishes = new HashSet<Dish>();
        }

        public decimal Id { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Occupation { get; set; }
        public DateTime? Birthday { get; set; }

        public virtual Salary? OccupationNavigation { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<Attend> Attends { get; set; }
        public virtual ICollection<IngredientRecord> IngredientRecords { get; set; }
        public virtual ICollection<Manage> Manages { get; set; }
        public virtual ICollection<Payroll> Payrolls { get; set; }
        public virtual ICollection<Prize> Prizes { get; set; }
        public virtual ICollection<Supplier> Suppliers { get; set; }

        public virtual ICollection<Dish> Dishes { get; set; }
    }
}
