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
    public class PromotionsController : ControllerBase
    {
        private readonly ModelContext _context;

        public PromotionsController()
        {
            _context = new ModelContext();
        }


        public class PromotionDish
        {
            public decimal dish_id { get; set; }
            public string? dish_name { get; set; }
            public decimal dish_price { get; set; }
            public string? dish_description { get; set; }

            public PromotionDish(Dish d)
            {
                dish_id = d.DishId;
                dish_name = d.DishName;
                dish_price = d.DishPrice;
                dish_description = d.DishDescription;
            }
        }

        public class PromotionDishRecord
        {
            public PromotionDish? dish { get; set; } = null!;
            public decimal discount { get; set; } = 1.0M;
        }

        public class PromotionRecord
        {
            public decimal promotion_id { get; set; }
            public DateTime? begin { get; set; }
            public DateTime? end { get; set; }
            public string? description { get; set; } = null!;
            public List<PromotionDishRecord> dishes { get; set; } = new List<PromotionDishRecord>();
        }

        public class PromotionPostDishRecord
        {
            public string? name { get; set; } = null!;
            public decimal discount { get; set; } = 1.0M;
        }

        public class PromotionPostRecord
        {
            public decimal? promotion_id { get; set; }
            public DateTime? begin { get; set; }
            public DateTime? end { get; set; }
            public string? description { get; set; } = null!;
            public List<PromotionPostDishRecord> dishes { get; set; } = new List<PromotionPostDishRecord>();
        }


        // GET: api/Promotions
        [HttpGet]
        public async Task<ActionResult<List<PromotionRecord>>> GetPromotions()
        {
            if (_context.Promotions == null)
            {
                return NoContent();
            }

            List<PromotionRecord> ret = new List<PromotionRecord>();
            await foreach (var p in _context.Promotions.Include(e => e.Hasdishes).ThenInclude(e => e.Dish).AsAsyncEnumerable())
            {
                PromotionRecord pr = new PromotionRecord();
                pr.promotion_id = p.PromotionId;
                pr.description = p.Description;
                pr.begin = p.StartTime;
                pr.end = p.EndTime;
                var d = p.Hasdishes;
                foreach (var di in d)
                {
                    PromotionDishRecord dt = new PromotionDishRecord();
                    if (di.Discount != null) dt.discount = (decimal)di.Discount; else dt.discount = 0.0M;
                    dt.dish = new PromotionDish(di.Dish);
                    pr.dishes.Add(dt);
                }
                ret.Add(pr);
            }
            return Ok(ret);
        }

        // POST: api/Promotions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Promotion>> PostPromotion(PromotionPostRecord p)
        {
            if (_context.Promotions == null)
            {
                return Problem("Entity set 'ModelContext.Promotions'  is null.");
            }

            var ret = new Promotion();
            ret.PromotionId = 1 + await _context.Promotions.CountAsync();
            ret.StartTime = p.begin;
            ret.EndTime = p.end;
            ret.Description = p.description;

            if (p.promotion_id != null)
            {
                ret.PromotionId = (decimal)p.promotion_id;
                var promotion = await _context.Promotions.FindAsync(p.promotion_id);
                if (promotion == null)
                {
                    return NoContent();
                }
                _context.Promotions.Remove(promotion);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    throw;
                }
            }
            foreach (var di in p.dishes)
            {
                Hasdish nd = new Hasdish();
                nd.Dish = _context.Dishes.Where(e => e.DishName == di.name).First();
                nd.Discount = di.discount;
                ret.Hasdishes.Add(nd);
            }
            _context.Promotions.Add(ret);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok();
        }

        // DELETE: api/Promotions/5
        [HttpDelete("DelPromotionById")]
        public async Task<IActionResult> DeletePromotion(decimal id)
        {
            if (_context.Promotions == null)
            {
                return NoContent();
            }
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NoContent();
            }

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PromotionExists(decimal id)
        {
            return (_context.Promotions?.Any(e => e.PromotionId == id)).GetValueOrDefault();
        }
    }
}
