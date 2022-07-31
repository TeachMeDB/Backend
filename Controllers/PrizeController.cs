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

        // GET 获取获奖记录
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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

        // DELETE  删除一个奖金级别
        [HttpDelete("DeleteAward/{level}")]
        public async Task<ActionResult> DeleteAward(string? level)
        {
            if (level == null) return BadRequest();
            var del_award = _context.Awards
                .FirstOrDefault(x => x.Lv == level);
            if (del_award == null) return NotFound();

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
