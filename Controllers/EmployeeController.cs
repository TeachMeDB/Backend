using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

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
        public class GetPrizeInfo
        {
            public string prize_datetime { get; set; }
            public string? level { get; set; }
            public decimal? amount { get; set; }
        }

        public class EmployeeMessage
        {
            public string? id { get; set; }
            public string? name { get; set; }
            public string? gender { get; set; }
            public string? occupation { get; set; }
            public string? birthday { get; set; }
            public string? avatar { get; set; }
            public string? cover { get; set; }
            public List<AttendInfo>? attends { get; set; }
            public List<PayrollInfo>? payrolls { get; set; }
            public List<GetPrizeInfo>? prizes { get; set; }
        }
        public class ModifyMessage
        {
            public bool? success { get; set; }
        }

        public class EmployeePostInfo
        {
            public string? id { get; set; }
            public string? name { get; set; }
            public string? gender { get; set; }
            public string? occupation { get; set; }
            public string? birthday { get; set; }
            public string? avatar { get; set; }
            public string? cover { get; set; }
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
                if (employee.Birthday != null) tem.Birthday = ((DateTime)employee.Birthday).ToString("yyyy-MM-dd");
                else tem.Birthday = null;
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
        [HttpGet("GetOneEmployeeInfo")]
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

            decimal amount = 0;
            if (employee.Occupation != null)
            {
                var salary = await _context.Salaries
                    .FirstOrDefaultAsync(s => s.Occupation == employee.Occupation.ToString());
                amount = (decimal)salary.Amount;
            }

            message.id = employee.Id.ToString();
            message.name = employee.Name;
            message.gender = employee.Gender;
            message.occupation = employee.Occupation;
            if (employee.Birthday != null) message.birthday = ((DateTime)employee.Birthday).ToString("yyyy-MM-dd");
            else message.birthday = null;
           message.avatar = System.Configuration.ConfigurationManager.AppSettings["ImagesUrl"] + "employees/employee_" + employee.Id.ToString() + ".jpg";
            message.cover = System.Configuration.ConfigurationManager.AppSettings["ImagesUrl"] + "covers/cover_" + employee.Id.ToString() + ".jpg";

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
            message.attends = attends;

            List<GetPrizeInfo> prizes = new List<GetPrizeInfo>();
            foreach (Prize prize in employee.Prizes)
            {
                GetPrizeInfo prizesObj = new GetPrizeInfo();
                prizesObj.prize_datetime = prize.PrizeDatetime.ToString("yyyy-MM-dd hh:mm:ss");
                prizesObj.level = prize.Lv;
                prizesObj.amount = prize.LvNavigation.Amount;
                prizes.Add(prizesObj);
            }
            message.prizes = prizes;

            List<PayrollInfo> payrolls = new List<PayrollInfo>();
            foreach (Payroll payroll in employee.Payrolls)
            {
                PayrollInfo payrollObj = new PayrollInfo();
                payrollObj.pay_datetime = payroll.PayDatetime.ToString("yyyy-MM-dd hh:mm:ss");
                payrollObj.amount = amount;
                payrolls.Add(payrollObj);
            }
            message.payrolls = payrolls;

            return Ok(message);
        }

        // POST 删除或修改员工信息
        [HttpPost("PostEmployeeInfo")]
        public async Task<ActionResult<bool>> PostEmployeeInfo(EmployeePostInfo p)
        {
            try
            {
                string? id = p.id;
                string? name = p.name;
                string? gender = p.gender;
                string? occupation = p.occupation;
                string? birthday = p.birthday;
                string? avatar = p.avatar;
                string? cover = p.cover;

                if (id != null)
                {
                    try
                    {
                        var employee = await _context.Employees
                            .FirstOrDefaultAsync(x => x.Id.ToString() == id);
                        if (employee == null)
                        {
                            return NotFound();
                        }
                        if (name != null) employee.Name = name;
                        if (gender != null) employee.Gender = gender;
                        if (occupation != null) employee.Occupation = occupation;
                        if (birthday != null) employee.Birthday = Convert.ToDateTime(birthday);

                        await _context.SaveChangesAsync();
                        if (avatar != null)
                        {
                            byte[] base64 = Convert.FromBase64String(avatar);
                            string path = "/images/employees/employee_" + id + ".jpg";
                            System.IO.File.WriteAllBytes(path, base64);
                        }
                        if (cover != null)
                        {
                            byte[] base64 = Convert.FromBase64String(cover);
                            string path = "/images/covers/cover_" + id + ".jpg";
                            System.IO.File.WriteAllBytes(path, base64);
                        }
                        return Ok(true);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex);
                    }
                } else
                {
                    try
                    {
                        var employees = _context.Employees
                            .OrderBy(e => e.Id);

                        decimal newId = 1001;
                        Employee employee = new Employee();

                        if (employees != null)
                        {
                            foreach (var emp in employees)
                            {
                                if (emp.Id != newId)
                                {
                                    break;
                                }
                                newId++;
                            }
                        }

                        employee.Id = newId;
                        employee.Name = name;
                        if (gender != null) employee.Gender = gender;
                        if (occupation != null) employee.Occupation = occupation;
                        if (birthday != null) employee.Birthday = Convert.ToDateTime(birthday);

                        _context.Employees.Add(employee);
                        await _context.SaveChangesAsync();
                        if (avatar != null)
                        {
                            byte[] base64 = Convert.FromBase64String(avatar);
                            string path = "/images/employees/employee_" + id + ".jpg";
                            System.IO.File.WriteAllBytes(path, base64);
                        }
                        if (cover != null)
                        {
                            byte[] base64 = Convert.FromBase64String(cover);
                            string path = "/images/covers/cover_" + id + ".jpg";
                            System.IO.File.WriteAllBytes(path, base64);
                        }
                        return Created("", true);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex);
                    }
                }
            } catch (Exception e){ 
                Console.WriteLine(e.Message);
                return BadRequest();
            }
            return Ok(true);
        }

        // DELETE 删除一条员工信息
        [HttpDelete("DeleteEmployeeInfo")]
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
                message.success = true;
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
