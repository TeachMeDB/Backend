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

        public class EmployeeInfo2
        {
            public string id { get; set; }
            public string? name { get; set; }
            public string? gender { get; set; }
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
        [HttpGet("GetFreeEmployee")]
        public async Task<ActionResult<List<EmployeeInfo2>>> GetFreeEmployee(string? start, string? end, string? place, string? occupation)
        {
            DateTime start_date, end_date;
            if (start != null) start_date = Convert.ToDateTime(start);
            else start_date = DateTime.MinValue;
            if (end != null) end_date = Convert.ToDateTime(end);
            else end_date = DateTime.MaxValue;

            var workInfo = await _context.WorkPlans
                .Include(e => e.Attends)
                .Where(w => (w.TimeStart <= end_date && w.TimeEnd >= start_date))
                .ToListAsync();
            List<decimal> busyIds = new List<decimal>();
            foreach(var work in workInfo)
            {
                foreach(var attend in work.Attends)
                {
                    if (!busyIds.Exists(b => b == attend.EmployeeId))
                        busyIds.Add(attend.EmployeeId);
                }
            }

            var employees = await _context.Employees.ToListAsync();
            List<EmployeeInfo2> info = new List<EmployeeInfo2>();

            foreach(var employee in employees)
            {
                if ((occupation == null || employee.Occupation == occupation) && !busyIds.Exists(b => b == employee.Id))
                {
                    EmployeeInfo2 emp = new EmployeeInfo2();
                    emp.id = employee.Id.ToString();
                    emp.name = employee.Name;
                    emp.gender = employee.Gender;
                    info.Add(emp);
                }
            }

            if (info.Count == 0) return NotFound();
            return Ok(info);
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
