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
    public class DishesController : ControllerBase
    {
        private readonly ModelContext _context;

        public DishesController()
        {
            _context = new ModelContext();
        }

        // GET: api/Dishes/GetDishNameById
        [HttpGet("GetDishNameById")]
        public async Task<ActionResult<string>> GetDishNameById(string? dish_id)
        {
            if (dish_id == null) return BadRequest();
            var dish = await _context.Dishes
                .FirstOrDefaultAsync(d => d.DishId.ToString() == dish_id);
            if (dish == null) return NoContent();
            return Ok(dish.DishName);
        }


        public class GetDishesItem
        {
            public decimal id { get; set; }
            public string? dis_name { get; set; }
            public decimal price { get; set; }
            public string? description { get; set; } = string.Empty;
            public List<string>? tags { get; set; } = new List<string>();
        }

        public class PostDishesItem
        {
            public decimal id { get; set; }
            public string? dis_name { get; set; }
            public decimal price { get; set; }
            public string? description { get; set; } = string.Empty;
            public List<string>? tags { get; set; } = new List<string>();
        }


        // GET: api/Dishes
        [HttpGet]
        public async Task<ActionResult<List<GetDishesItem>>> GetDishes()
        {
            if (_context.Dishes == null)
            {
                return NoContent();
            }
            List<GetDishesItem> ret = new List<GetDishesItem>();

            await foreach (var item in _context.Dishes.Include(e => e.Dtags).AsAsyncEnumerable())
            {
                var dishesItem = new GetDishesItem();
                dishesItem.id = item.DishId;
                dishesItem.dis_name = item.DishName;
                dishesItem.price = item.DishPrice;
                dishesItem.description = item.DishDescription;
                foreach (var t in item.Dtags)
                {
                    dishesItem.tags.Add(t.DtagName);
                }
                ret.Add(dishesItem);
            }

            return Ok(ret);
        }

        [HttpGet("ByName")]
        public async Task<ActionResult<GetDishesItem>> GetDish(string name)
        {
            if (_context.Dishes == null)
            {
                return NoContent();
            }

            try
            {
                var item = await _context.Dishes.Include(e => e.Dtags).Where(e => e.DishName == name).FirstAsync();
                GetDishesItem ret = new GetDishesItem();
                ret.id = item.DishId;
                ret.dis_name = item.DishName;
                ret.price = item.DishPrice;
                ret.description = item.DishDescription;
                foreach (var t in item.Dtags)
                {
                    ret.tags.Add(t.DtagName);
                }
                return ret;
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }

        // GET: api/Dishes/5
        [HttpGet("GetDishById")]
        public async Task<ActionResult<Dish>> GetDish(decimal id)
        {
            if (_context.Dishes == null)
            {
                return NoContent();
            }
            var dish = await _context.Dishes.FindAsync(id);

            if (dish == null)
            {
                return NoContent();
            }

            return dish;
        }


        // POST: api/Dishes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("UpdateDish")]
        public async Task<ActionResult> PostDish(PostDishesItem dish)
        {
            if (_context.Dishes == null)
            {
                return Problem("Entity set 'ModelContext.Dishes'  is null.");
            }

            var dm = await _context.Dishes.FindAsync(dish.id);

            dm.DishDescription = dish.description;
            dm.DishName = dish.dis_name;
            dm.DishPrice = dish.price;
            foreach (var tag in dish.tags)
            {
                try
                {
                    dm.Dtags.Add(_context.Dishtags.Where(e => e.DtagName == tag).First());
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NoContent();
            }

            return Ok();
        }

        [HttpPost("AddDish")]
        public async Task<ActionResult> AddDish(PostDishesItem dish)
        {
            if (_context.Dishes == null)
            {
                return Problem("Entity set 'ModelContext.Dishes'  is null.");
            }

            var dm = await _context.Dishes.FindAsync(dish.id);
            if (dm != null)
            {
                return NoContent();
            }
            dm = new Dish();
            dm.DishDescription = dish.description;
            dm.DishName = dish.dis_name;
            dm.DishPrice = dish.price;
            foreach (var tag in dish.tags)
            {
                try
                {
                    dm.Dtags.Add(_context.Dishtags.Where(e => e.DtagName == tag).First());
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            try
            {
                _context.Dishes.Add(dm);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NoContent();
            }

            return Ok();
        }

        public class PostUpdateDishStatusMsg
        {
            public string? order_id { get; set; }
            public decimal? dish_id { get; set; }
            public string? dish_status { get; set; }
        }
        [HttpPost("UpdateDishStatus")]
        public async Task<ActionResult> UpdateDishStatus(PostUpdateDishStatusMsg p)
        {
            if (_context.Dishes == null)
            {
                return Problem("Entity set 'ModelContext.Dishes'  is null.");
            }
            var l = await _context.Dishorderlists.Where(e => e.OrderId == p.order_id && e.DishId == p.dish_id).FirstAsync();

            l.DishStatus = p.dish_status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NoContent();
            }

            return Ok();
        }

        public class PostUpdateOrderStatusMsg
        {
            public string? order_id { get; set; }
            public string? order_status { get; set; }
        }
        [HttpPost("UpdateOrderStatus")]
        public async Task<ActionResult> UpdateOrderStatus(PostUpdateOrderStatusMsg p)
        {
            if (_context.Dishes == null)
            {
                return Problem("Entity set 'ModelContext.Dishes'  is null.");
            }

            var l = await _context.Orderlists.FindAsync(p.order_id);

            l.OrderStatus = p.order_status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NoContent();
            }

            return Ok();
        }

        // POST: api/Dishes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Dish>> PostDish(Dish dish)
        {
            if (_context.Dishes == null)
            {
                return Problem("Entity set 'ModelContext.Dishes'  is null.");
            }
            _context.Dishes.Add(dish);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DishExists(dish.DishId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDish", new { id = dish.DishId }, dish);
        }

        public class DishOrderItem
        {
            public string? dish_name { get; set; } = string.Empty;
            public string? status { get; set; } = null;
            public string? dish_order_id { get; set; } = String.Empty;
        }

        public class DishOrderListItem
        {
            public string? order_id { get; set; } = string.Empty;
            public string? order_status { get; set; } = null;
            public List<DishOrderItem>? dish { get; set; } = new List<DishOrderItem>();
        }



        // GET: api/Dishes/OrderList
        [HttpGet("OrderList")]
        public async Task<ActionResult<List<DishOrderListItem>>> GetOrderList()
        {
            var ret = new List<DishOrderListItem>();
            var dl = await _context.Dishorderlists.Include(e => e.Dish).ToListAsync();
            foreach (var d in dl.GroupBy(e => e.OrderId))
            {
                string id = d.Key;
                DishOrderListItem dishOrderListItem = new DishOrderListItem();
                dishOrderListItem.order_id = id;
                dishOrderListItem.order_status = _context.Orderlists.Where(e => e.OrderId == id).First().OrderStatus;
                foreach (var item in d.AsEnumerable())
                {
                    DishOrderItem ditem = new DishOrderItem();
                    ditem.status = item.DishStatus;
                    ditem.dish_name = item.Dish.DishName;
                    ditem.dish_order_id = item.DishOrderId;
                    dishOrderListItem.dish.Add(ditem);
                }
                ret.Add(dishOrderListItem);
            }
            return Ok(ret);
        }

        // GET: api/Dishes/OrderListById
        [HttpGet("OrderListById")]
        public async Task<ActionResult<DishOrderListItem>> GetOrderListById(string order_id)
        {
            var ret = new DishOrderListItem();
            var dl = _context.Dishorderlists.Include(e => e.Dish).Where(e => e.OrderId == order_id);
            ret.order_id = order_id;
            ret.order_status = _context.Orderlists.Where(e => e.OrderId == order_id).First().OrderStatus;
            await foreach (var item in dl.AsAsyncEnumerable())
            {
                DishOrderItem ditem = new DishOrderItem();
                ditem.status = item.DishStatus;
                ditem.dish_name = item.Dish.DishName;
                ret.dish.Add(ditem);
            }
            return Ok(ret);
        }

        // DELETE: api/Dishes/5
        [HttpDelete("DelDishById")]
        public async Task<IActionResult> DeleteDish(decimal id)
        {
            if (_context.Dishes == null)
            {
                return NoContent();
            }
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
            {
                return NoContent();
            }

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DishExists(decimal id)
        {
            return (_context.Dishes?.Any(e => e.DishId == id)).GetValueOrDefault();
        }
    }
}
