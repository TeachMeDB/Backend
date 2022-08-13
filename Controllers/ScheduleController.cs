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
            public string? avatar { get; set; }
        }

        public class ScheduleInfo2
        {
            public string time_start { get; set; }
            public string time_end { get; set; }
            public string occupation { get; set; }
            public string place { get; set; }
            public List<string> employee_ids { get; set; }
        }

        // GET 获取指定排班信息
        [HttpGet("GetScheduleInfo")]
        public async Task<ActionResult<List<ScheduleInfo>>> GetScheduleInfo(string? start, string? end, string? id, string? place, string? occupation)
        {
            var scheduleInfo = await _context.WorkPlans
                .Include(w => w.Attends)
                    .ThenInclude(a => a.Employee)
                .ToListAsync();

            DateTime? start_time = null, end_time = null;
            if (start != null) start_time = Convert.ToDateTime(start);
            if (end != null) end_time = Convert.ToDateTime(end);

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
                        foreach (Attend attend in schedule.Attends)
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
                    info.time_start = ((DateTime)schedule.TimeStart).ToString("yyyy-MM-dd HH:mm:ss");
                    info.time_end = ((DateTime)schedule.TimeEnd).ToString("yyyy-MM-dd HH:mm:ss");
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

            if (infos.Count == 0) return NoContent();
            return Ok(infos);
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
            foreach (var work in workInfo)
            {
                foreach (var attend in work.Attends)
                {
                    if (!busyIds.Exists(b => b == attend.EmployeeId))
                        busyIds.Add(attend.EmployeeId);
                }
            }

            var employees = await _context.Employees.ToListAsync();
            List<EmployeeInfo2> info = new List<EmployeeInfo2>();

            foreach (var employee in employees)
            {
                if ((occupation == null || employee.Occupation == occupation) && !busyIds.Exists(b => b == employee.Id))
                {
                    EmployeeInfo2 emp = new EmployeeInfo2();
                    emp.id = employee.Id.ToString();
                    emp.name = employee.Name;
                    emp.gender = employee.Gender;
                    emp.avatar = System.Configuration.ConfigurationManager.AppSettings["ImagesUrl"] + "employees/employee_" + employee.Id.ToString() + ".png";
                    info.Add(emp);
                }
            }

            if (info.Count == 0) return NoContent();
            return Ok(info);
        }

        // POST 增加一条排班记录
        [HttpPost("PostScheduleInfo")]
        public async Task<ActionResult<bool>> PostScheduleInfo(ScheduleInfo2 p)
        {
            try
            {
                DateTime start = Convert.ToDateTime(p.time_start);
                DateTime end = Convert.ToDateTime(p.time_end);
                string occupation = p.occupation;
                string place = p.place;
                var id = await _context.WorkPlans
                    .MaxAsync(b => b.Id) + 1;

                WorkPlan workPlan = new WorkPlan();
                workPlan.TimeStart = start;
                workPlan.TimeEnd = end;
                workPlan.Occupation = occupation;
                workPlan.Place = place;
                workPlan.Id = id;
                workPlan.No = p.employee_ids.Count;

                _context.WorkPlans.Add(workPlan);
                for (int i = 0; i < p.employee_ids.Count; i++)
                {
                    Attend attend = new Attend();
                    attend.Attendance = false;
                    attend.PlanId = id;
                    attend.EmployeeId = Convert.ToDecimal(p.employee_ids[i]);
                    _context.Attends.Add(attend);
                }
                await _context.SaveChangesAsync();
                return Ok(true);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE 删除一条排班记录
        [HttpDelete("DeleteScheduleInfo")]
        public async Task<ActionResult<bool>> Delete(string? id)
        {
            decimal? del_id = Convert.ToDecimal(id);
            if (del_id == null) return BadRequest();

            var work_plan = await _context.WorkPlans.FindAsync(del_id);
            if (work_plan == null) return NoContent();

            try
            {
                _context.WorkPlans.Remove(work_plan);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("attend")]
        public async Task<ActionResult<string>> HardwareAttend(string id)
        {
            decimal eid = Convert.ToDecimal(id);
            var employee = await _context.Employees.FindAsync(eid);
            if (employee == null)
            {
                return NoContent();
            }
            try
            {
                /*Console.WriteLine(DateTime.Now.ToShortTimeString() + employee.Name);*/
                DateTime now = DateTime.Now;
                TimeSpan ts = TimeSpan.FromMinutes(10);
                var atts = await _context.Attends.Include(e => e.Plan).Where(e => e.EmployeeId == eid).ToArrayAsync();
                var att = atts.Where(e => (e.Plan.TimeStart - now) > TimeSpan.Zero && (e.Plan.TimeStart - now) < ts).ToArray();
                if (att.Length > 0)
                    att.First().Attendance = true;
                else
                    return BadRequest("ERR");
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest("ERR");
            }
            return Ok(employee.Name);
        }
    }
}
