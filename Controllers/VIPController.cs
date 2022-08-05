using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace youAreWhatYouEat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VIPController : ControllerBase
    {
        private readonly ModelContext _context;
        public VIPController()
        {
            _context = new ModelContext();
        }

        public class VIPInfo
        {
            public string? user_name { get; set; }
            public string? gender { get; set; }
            public decimal? balance { get; set; }
            public decimal? credit { get; set; }
        }

        // GET 获取一个VIP的信息
        [HttpGet("GetOneVIPInfo")]
        public async Task<ActionResult<VIPInfo>> GetOneVIPInfo(string user_name)
        {
            var vip = await _context.Vips
                .FirstOrDefaultAsync(v => v.UserName == user_name);
            if (vip == null) return NoContent();

            VIPInfo info = new VIPInfo();
            info.user_name = vip.UserName;
            info.gender = vip.Gender;
            info.balance = vip.Balance;
            info.credit = vip.Credit;

            return Ok(info);
        }

        public class AttrOfVIP
        {
            public string? name { get; set; }
            public List<decimal> data = new List<decimal>();
        }

        public class ListOfVIP
        {
            public Dictionary<string, dynamic>? xaxis = new Dictionary<string, dynamic>();

            public ListOfVIP()
            {
                xaxis.Add("categories", new List<string>());
            }
        }

        public class VIPInfo2
        {
            public string? user_name { get; set; }
            public string? birthday { get; set; }
            public string? gender { get; set; }
            public decimal? balance { get; set; }
            public decimal? credit { get; set; }
            public string? status { get; set; }
        }

        public class AllVIPInfo
        {
            public List<VIPInfo2>? vips = new List<VIPInfo2>();
            public Dictionary<string, dynamic>? summary = new Dictionary<string, dynamic>();
            public Dictionary<string, dynamic>? summary2 = new Dictionary<string, dynamic>();

            public AllVIPInfo()
            {
                summary.Add("series", new List<AttrOfVIP>());
                summary.Add("options", new ListOfVIP());
                AttrOfVIP a = new AttrOfVIP();
                summary["series"].Add(a);
                summary["series"][0].name = "积分";
                summary2.Add("series", new List<AttrOfVIP>());
                summary2.Add("options", new ListOfVIP());
                AttrOfVIP b = new AttrOfVIP();
                summary2["series"].Add(b);
                summary2["series"][0].name = "余额";
            }
        }

        // GET 获取所有VIP信息
        [HttpGet("GetAllVIPInfo")]
        public async Task<ActionResult<AllVIPInfo>> GetAllVIPInfo()
        {
            var vips = await _context.Vips
                .ToListAsync();

            AllVIPInfo allVIPInfo = new AllVIPInfo();
            foreach(var vip in vips)
            {
                VIPInfo2 v = new VIPInfo2();
                v.user_name = vip.UserName;
                v.gender = vip.Gender;
                v.balance = vip.Balance;
                v.credit = vip.Credit;
                v.status = "正常";
                if (vip.Birthday != null) v.birthday = ((DateTime)vip.Birthday).ToString("yyyy-MM-dd");
                else v.birthday = null;
                allVIPInfo.vips.Add(v);
                allVIPInfo.summary["series"][0].data.Add(vip.Credit);
                allVIPInfo.summary["options"].xaxis["categories"].Add(vip.UserName);
                allVIPInfo.summary2["series"][0].data.Add(vip.Balance);
                allVIPInfo.summary2["options"].xaxis["categories"].Add(vip.UserName);
            }

            return Ok(allVIPInfo);
        }

        public class PostUpdateVIPInfo
        {
            public string? user_name { get; set; }
            public string? gender { get; set; }
            public string? birthday { get; set; }
            public decimal balance { get; set; }
            public decimal credit { get; set; }
        }

        // POST 修改VIP信息
        [HttpPost("PostUpdateVIP")]
        public async Task<ActionResult> PostUpdateVIP(PostUpdateVIPInfo p)
        {
            if (p.user_name == null) return BadRequest();
            var vip = await _context.Vips
                .FirstOrDefaultAsync(v => v.UserName == p.user_name);
            if (vip == null) return NoContent();

            try
            {
                if (p.gender != null) vip.Gender = p.gender;
                if (p.birthday != null) vip.Birthday = Convert.ToDateTime(p.birthday);
                vip.Balance = p.balance;
                vip.Credit = p.credit;
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        public class PostAddVIPInfo
        {
            public string? user_name { get; set; }
            public string? gender { get; set; }
            public string? birthday { get; set; }
        }

        // POST 添加VIP
        //[HttpPost("PostAddVIP")]
        //public async Task<ActionResult> PostAddVIP(PostAddVIPInfo p)
        //{
        //    Vip vip = new Vip();
        //    vip.UserName = p.user_name;
        //    vip.Gender = p.gender;
        //    vip.Birthday = Convert.ToDateTime(p.birthday);

        //    try
        //    {
        //        _context.Vips.Add(vip);
        //        await _context.SaveChangesAsync();
        //        return Ok();
        //    } catch (Exception ex)
        //    {
        //        return BadRequest(ex);
        //    }
        //}
    }
}
