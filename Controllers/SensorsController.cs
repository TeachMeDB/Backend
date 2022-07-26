using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;

namespace youAreWhatYouEat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ModelContext _context;

        public SensorsController()
        {
            _context = new ModelContext();
        }

        public class SensorLogRecord
        {
            public DateTime time { get; set; }
            public decimal value { get; set; }

            public SensorLogRecord(Sensorlog slr)
            {
                time = slr.SlogTime;
                value = slr.SlogValue;
            }
        }
        public class SensorLogMessage
        {
            public decimal sensor_id { get; set; }
            public string sensor_type { get; set; } = null!;
            public string sensor_model { get; set; } = null!;
            public string sensor_location { get; set; } = null!;
            public List<SensorLogRecord> log { get; set; } = new List<SensorLogRecord>();
        }

        public class SensorsGetRawDataMessage
        {
            public int code { get; set; } = 200;
            public List<SensorLogMessage> data { get; set; } = new List<SensorLogMessage>();
        }

        // GET: api/Sensors/rawdata
        [HttpGet("rawdata")]
        public async Task<ActionResult<SensorsGetRawDataMessage>> GetRawData(int begin = 0, int end = 2147483647)
        {
            if (_context.Sensors == null)
            {
                return NotFound();
            }
            SensorsGetRawDataMessage ret = new SensorsGetRawDataMessage();
            try
            {
                await foreach (var s in _context.Sensors.Include(e => e.Sensorlogs).AsAsyncEnumerable())
                {
                    SensorLogMessage slm = new SensorLogMessage();
                    slm.sensor_id = s.SensId;
                    slm.sensor_model = s.SensModel;
                    slm.sensor_type = s.SensType;
                    slm.sensor_location = s.SensLocation;
                    var sl = s.Sensorlogs
                        .Where(e => e.SlogTime >= UnixTimeUtil.UnixTimeToDateTime(begin) && e.SlogTime <= UnixTimeUtil.UnixTimeToDateTime(end));
                    foreach (var slg in s.Sensorlogs)
                    {
                        slm.log.Add(new SensorLogRecord(slg));
                    }
                    ret.data.Add(slm);
                }
            }
            catch (Exception)
            {
                return StatusCode(201, ret);
            }
            return Ok(ret);
        }

        // GET: api/Sensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sensor>>> GetSensors()
        {
            if (_context.Sensors == null)
            {
                return NotFound();
            }
            return await _context.Sensors.ToListAsync();
        }

        // GET: api/Sensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sensor>> GetSensor(decimal id)
        {
            if (_context.Sensors == null)
            {
                return NotFound();
            }
            var sensor = await _context.Sensors.FindAsync(id);

            if (sensor == null)
            {
                return NotFound();
            }

            return sensor;
        }

        // PUT: api/Sensors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSensor(decimal id, Sensor sensor)
        {
            if (id != sensor.SensId)
            {
                return BadRequest();
            }

            _context.Entry(sensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SensorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Sensors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Sensor>> PostSensor(Sensor sensor)
        {
            if (_context.Sensors == null)
            {
                return Problem("Entity set 'ModelContext.Sensors'  is null.");
            }
            _context.Sensors.Add(sensor);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SensorExists(sensor.SensId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSensor", new { id = sensor.SensId }, sensor);
        }

        // DELETE: api/Sensors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSensor(decimal id)
        {
            if (_context.Sensors == null)
            {
                return NotFound();
            }
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null)
            {
                return NotFound();
            }

            _context.Sensors.Remove(sensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SensorExists(decimal id)
        {
            return (_context.Sensors?.Any(e => e.SensId == id)).GetValueOrDefault();
        }
    }
}
