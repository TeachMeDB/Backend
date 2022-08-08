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

        public class SensorUsedRecord
        {
            public string? type { get; set; }
            public decimal consumption { get; set; }

            public SensorUsedRecord(string t, decimal c)
            {
                type = t;
                consumption = c;
            }
        }

        public class SensorsGetUsedResourcesMessage
        {
            public int code { get; set; } = 200;
            public DateTime begin { get; set; } = new DateTime();
            public DateTime end { get; set; } = new DateTime();
            public List<SensorUsedRecord> data { get; set; } = new List<SensorUsedRecord>();
        }

        // GET: api/Sensors/rawdata
        [HttpGet("rawdata")]
        public async Task<ActionResult<SensorsGetRawDataMessage>> GetRawData(string? begin, string? end)
        {
            if (_context.Sensors == null)
            {
                return NoContent();
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

                    DateTime start_time, end_time;
                    if (begin == null) start_time = DateTime.MinValue;
                    else start_time = Convert.ToDateTime(begin);
                    if (end == null) end_time = DateTime.MaxValue;
                    else end_time = Convert.ToDateTime(end);

                    var sl = s.Sensorlogs
                        .Where(e => e.SlogTime >= start_time && e.SlogTime <= end_time);
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

        // GET: api/Sensors/used
        [HttpGet("used")]
        public async Task<ActionResult<SensorsGetUsedResourcesMessage>> GetUsedResource(string? begin, string? end)
        {
            if (_context.Sensors == null)
            {
                return NoContent();
            }
            SensorsGetUsedResourcesMessage ret = new SensorsGetUsedResourcesMessage();

            DateTime start_time, end_time;
            if (begin == null) start_time = DateTime.MinValue;
            else start_time = Convert.ToDateTime(begin);
            if (end == null) end_time = DateTime.MaxValue;
            else end_time = Convert.ToDateTime(end);
            ret.begin = start_time;
            ret.end = end_time;
            try
            {
                List<Sensor> lll = await _context.Sensors.Include(e => e.Sensorlogs).ToListAsync();
                foreach (var s in lll.GroupBy(e => e.SensType))
                {
                    var k = s.Key;
                    decimal v = 0.0M;
                    foreach (var slg in s.AsEnumerable())
                    {
                        var l1 = slg.Sensorlogs
                            .Where(e => e.SlogTime >= start_time && e.SlogTime <= end_time)
                            .MaxBy(e => e.SlogTime);
                        var l2 = slg.Sensorlogs
                            .Where(e => e.SlogTime >= start_time && e.SlogTime <= end_time)
                            .MinBy(e => e.SlogTime);
                        if (l1 == null || l2 == null)
                        {
                            v += 0.0M;
                        }
                        else
                        {
                            v += l1.SlogValue - l2.SlogValue;
                        }
                    }
                    ret.data.Add(new SensorUsedRecord(k, v));
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
                return NoContent();
            }
            return await _context.Sensors.ToListAsync();
        }

        // GET: api/Sensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sensor>> GetSensor(decimal id)
        {
            if (_context.Sensors == null)
            {
                return NoContent();
            }
            var sensor = await _context.Sensors.FindAsync(id);

            if (sensor == null)
            {
                return NoContent();
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
                    return NoContent();
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
                return NoContent();
            }
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null)
            {
                return NoContent();
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
