using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace youAreWhatYouEat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ScheduleController : ControllerBase
    {
        private readonly ModelContext _context;

        public ScheduleController()
        {
            _context = new ModelContext();
        }

        public class PeopleInfo
        {
            public string id { get; set; }
            public string? name { get; set; }
            public string? gender { get; set; }
            public bool? attendance { get; set; }
        }

        public class ScheduleInfo
        {
            public string plan_id { get; set; }
            public string? time_start { get; set; }
            public string? time_end { get; set; }
            public string? place { get; set; }
            public string? occupation { get; set; }
            public List<PeopleInfo>? peoples { get; set; }
        }

        public class ScheduleMessage
        {
            public List<ScheduleInfo> data { get; set; } = new List<ScheduleInfo>();
        }

        // GET 获取指定排班信息
        [HttpGet("GetScheduleInfo")]
        public async Task<ActionResult<ScheduleMessage>> GetScheduleInfo(string? start, string? end, string? id, string? place, string? occupation)
        {
            var scheduleInfo = await _context.WorkPlans
                .Include(w => w.Attends)
                    .ThenInclude(a => a.Employee)
                .ToListAsync();

            DateTime? start_time = null, end_time = null;
            if (start != null) start_time = Convert.ToDateTime(start);
            if (end != null) end_time = Convert.ToDateTime(end);

            ScheduleMessage message = new ScheduleMessage();
            List<ScheduleInfo> infos = new List<ScheduleInfo>();
            foreach (WorkPlan schedule in scheduleInfo)
            {
                if ((start_time == null || start_time <= schedule.TimeStart) && (end_time == null || end_time >= 
                    schedule.TimeEnd) && (place == null || schedule.Place == place) && (occupation == null || 
                    schedule.Occupation == occupation))
                {
                    bool tag = true;
                    if (id != null)
                    {
                        tag = false;
                        foreach(Attend attend in schedule.Attends)
                        {
                            if (attend.Employee.Id.ToString() == id)
                            {
                                tag = true;
                                break;
                            }
                        }
                    }
                    if (!tag) continue;

                    ScheduleInfo info = new ScheduleInfo();
                    info.plan_id = schedule.Id.ToString();
                    info.time_start = ((DateTime)schedule.TimeStart).ToString("yyyy-MM-dd hh:mm:ss");
                    info.time_end = ((DateTime)schedule.TimeEnd).ToString("yyyy-MM-dd hh:mm:ss");
                    info.place = schedule.Place;
                    info.occupation = schedule.Occupation;

                    List<PeopleInfo> peoples = new List<PeopleInfo>();
                    foreach (Attend attend in schedule.Attends)
                    {
                        PeopleInfo peop = new PeopleInfo();
                        peop.id = attend.Employee.Id.ToString();
                        peop.name = attend.Employee.Name;
                        peop.gender = attend.Employee.Gender;
                        peop.attendance = attend.Attendance;
                        peoples.Add(peop);
                    }
                    info.peoples = peoples;
                    infos.Add(info);
                }
            }

            if (infos.Count == 0) return NotFound();
            message.data = infos;
            return Ok(message);
        }

        // GET 获取可排班人员
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST 增加一条排班记录
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // DELETE 删除一条排班记录
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
