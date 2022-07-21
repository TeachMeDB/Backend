using System;
using System.Collections.Generic;

namespace youAreWhatYouEat.Models
{
    public partial class Sensor
    {
        public Sensor()
        {
            Sensorlogs = new HashSet<Sensorlog>();
        }

        public decimal SensId { get; set; }
        public string SensType { get; set; } = null!;
        public string SensModel { get; set; } = null!;
        public string SensLocation { get; set; } = null!;

        public virtual ICollection<Sensorlog> Sensorlogs { get; set; }
    }
}
