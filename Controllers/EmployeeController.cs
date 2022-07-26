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
            public decimal Id { get; set; }
            public string? Name { get; set; }
            public string? Gender { get; set; }
            public string? Occupation { get; set; }
            public decimal? Attendance_rate { get; set; }
            public int? Award_times { get; set; }
        }

        public class AttendInfo
        {
            public string plan_id { get; set; }
            public DateTime? time_start { get; set; }
            public DateTime? time_end { get; set; }
            public string? place { get; set; }
            public bool? attendence { get; set; }
        }
        public class PayrollInfo
        {
            public DateTime pay_datetime { get; set; }
            public decimal? amount { get; set; }
        }
        public class PrizeInfo
        {
            public DateTime prize_datetime { get; set; }
            public string? level { get; set; }
            public decimal? amount { get; set; }
        }

        public class EmployeeMessage
        {
            public int errorCode { get; set; }
            public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();
            public EmployeeMessage()
            {
                errorCode = 300;
                data.Add("id", null);
                data.Add("name", null);
                data.Add("gender", null);
                data.Add("occupation", null);
            }
        }

        [HttpGet("GetAllEmployeeInfo")]
        public List<EmployeeInfo> GetAllEmployeeInfo()
        {
            IEnumerable<Employee> employeeInfo = _context.Employees
                .Include(e => e.Attends)
                .Include(e => e.Prizes)
                .ToList();
            List<EmployeeInfo> info = new List<EmployeeInfo>();

            foreach (Employee employee in employeeInfo)
            {
                EmployeeInfo tem = new EmployeeInfo();
                tem.Id = employee.Id;
                tem.Name = employee.Name;
                tem.Gender = employee.Gender;
                tem.Occupation = employee.Occupation;

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

            return info;
        }

        [HttpGet("GetOneEmployeeInfo/{id}")]
        public EmployeeMessage GetOneEmployeeInfo(decimal id)
        {
            EmployeeMessage message = new EmployeeMessage();

            Employee employee = _context.Employees
                .Include(e => e.Attends)
                    .ThenInclude(a => a.Plan)
                .Include(e => e.Payrolls)
                .Include(e => e.Prizes)
                    .ThenInclude(p => p.LvNavigation)
                .SingleOrDefault(x => x.Id == id);
            if (employee == null) return null;
            decimal? salary = (_context.Salaries
                .FirstOrDefault(s => s.Occupation == employee.Occupation.ToString())
                .Amount);

            message.data["id"] = employee.Id;
            message.data["name"] = employee.Name;
            message.data["gender"] = employee.Gender;
            message.data["occupation"] = employee.Occupation;
            message.data.Add("attends", new List<AttendInfo>());
            message.data.Add("payrolls", new List<PayrollInfo>());
            message.data.Add("prizes", new List<PrizeInfo>());

            List<AttendInfo> attends = new List<AttendInfo>();
            foreach (Attend attend in employee.Attends)
            {
                AttendInfo attendObj = new AttendInfo();
                attendObj.plan_id = attend.PlanId.ToString();
                attendObj.attendence = attend.Attendance;
                attendObj.time_start = attend.Plan.TimeStart;
                attendObj.time_end = attend.Plan.TimeEnd;
                attendObj.place = attend.Plan.Place;
                attends.Add(attendObj);
            }
            message.data["attends"] = attends;

            List<PrizeInfo> prizes = new List<PrizeInfo>();
            foreach (Prize prize in employee.Prizes)
            {
                PrizeInfo prizesObj = new PrizeInfo();
                prizesObj.prize_datetime = prize.PrizeDatetime;
                prizesObj.level = prize.Lv;
                prizesObj.amount = prize.LvNavigation.Amount;
                prizes.Add(prizesObj);
            }
            message.data["prizes"] = prizes;

            List<PayrollInfo> payrolls = new List<PayrollInfo>();
            foreach (Payroll payroll in employee.Payrolls)
            {
                PayrollInfo payrollObj = new PayrollInfo();
                payrollObj.pay_datetime = payroll.PayDatetime;
                payrollObj.amount = salary;
                payrolls.Add(payrollObj);
            }
            message.data["payrolls"] = payrolls;

            message.errorCode = 200;
            return message;
        }

        // POST api/<EmployeeController>
        [HttpPost("PostEmployeeInfo")]
        public void PostEmployeeInfo([FromBody] string value)
        {
        }

        // DELETE api/<EmployeeController>/5
        [HttpDelete("DeleteEmployeeInfo/{id}")]
        public void DeleteEmployeeInfo(int id)
        {
        }
    }
}
