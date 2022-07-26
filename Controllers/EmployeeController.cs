using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace youAreWhatYouEat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ModelContext _context;

        public EmployeeController()
        {
            _context = new ModelContext();
        }

        public class EmployeeInfo
        {
            public string Id { get; set; }
            public string? Name { get; set; }
            public string? Gender { get; set; }
            public string? Occupation { get; set; }
            public string? Birthday { get; set; }
            public decimal? Attendance_rate { get; set; }
            public int? Award_times { get; set; }
            public string? avatar { get; set; }
        }

        public class AttendInfo
        {
            public string plan_id { get; set; }
            public string? time_start { get; set; }
            public string? time_end { get; set; }
            public string? place { get; set; }
            public bool? attendence { get; set; }
        }
        public class PayrollInfo
        {
            public string pay_datetime { get; set; }
            public decimal? amount { get; set; }
        }
        public class PrizeInfo
        {
            public string prize_datetime { get; set; }
            public string? level { get; set; }
            public decimal? amount { get; set; }
        }

        public class EmployeeMessage
        {
            public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();
            public EmployeeMessage()
            {
                data.Add("id", null);
                data.Add("name", null);
                data.Add("gender", null);
                data.Add("occupation", null);
                data.Add("birthday", null);
                data.Add("avatar", null);
                data.Add("cover", null);
            }
        }
        public class ModifyMessage
        {
            public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();
            public ModifyMessage()
            {
                data.Add("success", false);
            }
        }

        // GET 获取所有员工的信息
        [HttpGet("GetAllEmployeeInfo")]
        public async Task<ActionResult<List<EmployeeInfo>>> GetAllEmployeeInfo()
        {
            var employeeInfo = await _context.Employees
                .Include(e => e.Attends)
                .Include(e => e.Prizes)
                .ToListAsync();
            List<EmployeeInfo> info = new List<EmployeeInfo>();

            foreach (Employee employee in employeeInfo)
            {
                EmployeeInfo tem = new EmployeeInfo();
                tem.Id = employee.Id.ToString();
                tem.Name = employee.Name;
                tem.Gender = employee.Gender;
                tem.Occupation = employee.Occupation;
                tem.Birthday = ((DateTime)employee.Birthday).ToString("yyyy-MM-dd");
                tem.avatar = System.Configuration.ConfigurationManager.AppSettings["ImagesUrl"] + "employees/employee_" + tem.Id.ToString() + ".jpg";

                decimal tot = 0, participant = 0;
                foreach (Attend item in employee.Attends)
                {
                    tot++;
                    if (item.Attendance == true) participant++;
                }
                if (tot != 0) tem.Attendance_rate = participant / tot * 100;
                else tem.Attendance_rate = 0;
                tem.Award_times = employee.Prizes.Count;

                info.Add(tem);
            }

            info.Sort((x, y) => { return x.Id.CompareTo(y.Id); });
            return Ok(info);
        }

        // GET 获取一位员工的信息
        [HttpGet("GetOneEmployeeInfo/{id}")]
        public async Task<ActionResult<EmployeeMessage>> GetOneEmployeeInfo(decimal? id)
        {
            if (id == null) return BadRequest();
            EmployeeMessage message = new EmployeeMessage();

            var employee = await _context.Employees
                .Include(e => e.Attends)
                    .ThenInclude(a => a.Plan)
                .Include(e => e.Payrolls)
                .Include(e => e.Prizes)
                    .ThenInclude(p => p.LvNavigation)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (employee == null) return NotFound();

            var salary = await _context.Salaries
                .FirstOrDefaultAsync(s => s.Occupation == employee.Occupation.ToString());
            decimal amount = (decimal)salary.Amount;

            message.data["id"] = employee.Id.ToString();
            message.data["name"] = employee.Name;
            message.data["gender"] = employee.Gender;
            message.data["occupation"] = employee.Occupation;
            message.data["birthday"] = ((DateTime)employee.Birthday).ToString("yyyy-MM-dd");
            message.data["avatar"] = System.Configuration.ConfigurationManager.AppSettings["ImagesUrl"] + "employees/employee_" + employee.Id.ToString() + ".jpg";
            message.data["cover"] = System.Configuration.ConfigurationManager.AppSettings["ImagesUrl"] + "covers/cover_" + employee.Id.ToString() + ".jpg";
            message.data.Add("attends", new List<AttendInfo>());
            message.data.Add("payrolls", new List<PayrollInfo>());
            message.data.Add("prizes", new List<PrizeInfo>());

            List<AttendInfo> attends = new List<AttendInfo>();
            foreach (Attend attend in employee.Attends)
            {
                AttendInfo attendObj = new AttendInfo();
                attendObj.plan_id = attend.PlanId.ToString();
                attendObj.attendence = attend.Attendance;
                attendObj.time_start = ((DateTime)attend.Plan.TimeStart).ToString("yyyy-MM-dd hh:mm:ss");
                attendObj.time_end = ((DateTime)attend.Plan.TimeEnd).ToString("yyyy-MM-dd hh:mm:ss");
                attendObj.place = attend.Plan.Place;
                attends.Add(attendObj);
            }
            message.data["attends"] = attends;

            List<PrizeInfo> prizes = new List<PrizeInfo>();
            foreach (Prize prize in employee.Prizes)
            {
                PrizeInfo prizesObj = new PrizeInfo();
                prizesObj.prize_datetime = prize.PrizeDatetime.ToString("yyyy-MM-dd hh:mm:ss");
                prizesObj.level = prize.Lv;
                prizesObj.amount = prize.LvNavigation.Amount;
                prizes.Add(prizesObj);
            }
            message.data["prizes"] = prizes;

            List<PayrollInfo> payrolls = new List<PayrollInfo>();
            foreach (Payroll payroll in employee.Payrolls)
            {
                PayrollInfo payrollObj = new PayrollInfo();
                payrollObj.pay_datetime = payroll.PayDatetime.ToString("yyyy-MM-dd hh:mm:ss");
                payrollObj.amount = amount;
                payrolls.Add(payrollObj);
            }
            message.data["payrolls"] = payrolls;

            return Ok(message);
        }

        // POST 删除或修改员工信息
        [HttpPost("PostEmployeeInfo")]
        public void PostEmployeeInfo([FromBody] string value)
        {
        }

        // DELETE 删除一条员工信息
        [HttpDelete("DeleteEmployeeInfo/{id}")]
        public async Task<ActionResult<ModifyMessage>> DeleteEmployeeInfo(int? id)
        {
            ModifyMessage message = new ModifyMessage();
            decimal? del_id = Convert.ToDecimal(id);
            if (del_id == null) return BadRequest();

            var employee = await _context.Employees.FindAsync(del_id);
            if (employee == null) return NotFound();

            try
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                message.data["success"] = true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.ToString());
                return Forbid();
            }

            return Ok(message);
        }
    }
}
