using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace youAreWhatYouEat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrizeController : ControllerBase
    {
        private readonly ModelContext _context;
        public PrizeController()
        {
            _context = new ModelContext();
        }

        public class PrizeInfo
        {
            public string level { get; set; }
            public decimal? amount { get; set; }
            public decimal? summary { get; set; }
        }

        // GET 获取奖金信息
        [HttpGet("GetPrizeInfo")]
        public async Task<ActionResult<List<PrizeInfo>>> GetPrizeInfo()
        {
            var prizes = await _context.Awards
                .Include(a => a.Prizes)
                .ToListAsync();

            List<PrizeInfo> prizeInfos = new List<PrizeInfo>();
            foreach(var priz in prizes)
            {
                PrizeInfo p = new PrizeInfo();
                p.level = priz.Lv;
                p.amount = priz.Amount;
                p.summary = priz.Prizes.Count;
                prizeInfos.Add(p);
            }

            return Ok(prizeInfos);
        }

        public class PrizeRecordInfo
        {
            public string? id { get; set; }
            public string? name { get; set; }
            public string? level { get; set; }
            public string? time { get; set; }
            public decimal? amount { get; set; }
        }

        // GET 获取获奖记录
        [HttpGet("GetPrizeRecord")]
        public async Task<ActionResult<List<PrizeRecordInfo>>> GetPrizeRecord(string?level, string? id, string? time_start, string? time_end)
        {
            var records = await _context.Awards
                .Include(a => a.Prizes)
                    .ThenInclude(p => p.Employee)
                .ToListAsync();

            try
            {
                List<PrizeRecordInfo> prizeRecords = new List<PrizeRecordInfo>();
                foreach (var record in records)
                {
                    if (level != null && level != record.Lv) continue;
                    foreach (var priz in record.Prizes)
                    {
                        if (id != null && priz.EmployeeId.ToString() != id) continue;
                        DateTime start, end;
                        if (time_start != null) start = Convert.ToDateTime(time_start);
                        else start = DateTime.MinValue;
                        if (time_end != null) end = Convert.ToDateTime(time_end);
                        else end = DateTime.MaxValue;

                        if (priz.PrizeDatetime >= start && priz.PrizeDatetime <= end)
                        {
                            PrizeRecordInfo prizeRecordInfo = new PrizeRecordInfo();
                            prizeRecordInfo.id = priz.EmployeeId.ToString();
                            prizeRecordInfo.name = priz.Employee.Name;
                            prizeRecordInfo.time = priz.PrizeDatetime.ToString("yyyy-MM-dd HH:mm:ss");
                            prizeRecordInfo.level = priz.Lv;
                            prizeRecordInfo.amount = record.Amount;
                            prizeRecords.Add(prizeRecordInfo);
                        }
                    }
                }
                return Ok(prizeRecords);
            } catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }

        public class PostAwardInfo
        {
            public string? level { get; set; }
            public decimal? amount { get; set; }
        }

        // POST 增加/更新一条奖金级别
        [HttpPost("PostAward")]
        public async Task<ActionResult> PostAward(PostAwardInfo p)
        {
            if (p.level == null || p.amount == null) return BadRequest();

            var a = await _context.Awards
                .FirstOrDefaultAsync(a => a.Lv == p.level);
            if (a == null)
            {
                try
                {
                    Award award = new Award();
                    award.Lv = p.level;
                    award.Amount = p.amount;
                    _context.Awards.Add(award);
                    await _context.SaveChangesAsync();
                    return Created("", true);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            } else
            {
                try
                {
                    a.Lv = p.level;
                    a.Amount = p.amount;
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        public class PostPrizeInfo
        {
            public string? id { get; set; }
            public string? level { get; set; }
            public string? time { get; set; }
        }

        // POST 增加一个发奖记录
        [HttpPost("PostPrizeRecord")]
        public async Task<ActionResult> PostPrizeRecord(PostPrizeInfo p)
        {
            if (p.id == null || p.level == null || p.time == null) return BadRequest();

            try
            {
                Prize prize = new Prize();
                prize.Lv = p.level;
                prize.EmployeeId = Convert.ToDecimal(p.id);
                prize.PrizeDatetime = Convert.ToDateTime(p.time);

                _context.Prizes.Add(prize);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

            // DELETE  删除一个奖金级别
            [HttpDelete("DeleteAward")]
        public async Task<ActionResult> DeleteAward(string? level)
        {
            if (level == null) return BadRequest();
            var del_award = _context.Awards
                .FirstOrDefault(x => x.Lv == level);
            if (del_award == null) return NoContent();

            try
            {
                _context.Remove(del_award);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
