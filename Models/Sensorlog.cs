using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Sensorlog
    {
        public decimal SlogId { get; set; }
        public decimal? SensId { get; set; }
        public DateTime SlogTime { get; set; }
        public decimal SlogValue { get; set; }

        public virtual Sensor? Sens { get; set; }
    }
}
