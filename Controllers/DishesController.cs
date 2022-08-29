using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;
using StackExchange.Redis;

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

        static private string? putredis(string k, string v)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(System.Configuration.ConfigurationManager.ConnectionStrings["Redis"].ConnectionString);
            IDatabase db = redis.GetDatabase();
            db.StringSet(k, v);
            var value = db.StringGet(k);
            return value;
        }

        static private string? getredis(string k)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(System.Configuration.ConfigurationManager.ConnectionStrings["Redis"].ConnectionString);
            IDatabase db = redis.GetDatabase();
            var value = db.StringGet(k);
            return value;
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
            public string? video { get; set; } = string.Empty;
            public string? picture { get; set; } = string.Empty;
            public string? rate { get; set; } = string.Empty;
            public List<string>? tags { get; set; } = new List<string>();
            public List<string>? ingredient { get; set; } = new List<string>();
        }

        public class PostDishesItem
        {
            public decimal id { get; set; }
            public string? dis_name { get; set; }
            public decimal price { get; set; }
            public string? description { get; set; } = string.Empty;
            public List<string>? tags { get; set; } = new List<string>();
            public string? picture { get; set; }
            public string? video { get; set; }
            public List<string>? ingredient { get; set; } = new List<string>();
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

            await foreach (var item in _context.Dishes.Include(e => e.Dtags).Include(e => e.Ingrs).AsAsyncEnumerable())
            {
                var dishesItem = new GetDishesItem();
                dishesItem.id = item.DishId;
                dishesItem.dis_name = item.DishName;
                dishesItem.price = item.DishPrice;
                dishesItem.description = item.DishDescription;
                dishesItem.picture = System.Configuration.ConfigurationManager.AppSettings["ImagesUrl"] + "dishes/dish_" + item.DishId.ToString() + ".png";
                dishesItem.video = item.Video;
                var rates = await _context.CommentOnDishes
                    .Where(d => d.DishId == item.DishId)
                    .ToListAsync();
                decimal avg = 5.0M;
                if (rates.Select(d => d.Stars).Average() != null)
                    avg = (decimal)rates.Select(d => d.Stars).Average();
                dishesItem.rate = Decimal.Round(avg, 2).ToString();
                foreach (var t in item.Dtags)
                {
                    dishesItem.tags.Add(t.DtagName);
                }
                foreach (var i in item.Ingrs)
                {
                    dishesItem.ingredient.Add(i.IngrName);
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
                ret.picture = System.Configuration.ConfigurationManager.AppSettings["ImagesUrl"] + "dishes/dish_" + item.DishId.ToString() + ".png";
                ret.video = item.Video;
                var rates = await _context.CommentOnDishes
                    .Where(d => d.DishId == item.DishId)
                    .ToListAsync();
                decimal avg = 5.0M;
                if (rates.Select(d => d.Stars).Average() != null)
                    avg = (decimal)rates.Select(d => d.Stars).Average();
                ret.rate = Decimal.Round(avg, 2).ToString();
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

            var dm = await _context.Dishes
                .Include(d => d.Dtags)
                .Include(d => d.Ingrs)
                .FirstOrDefaultAsync(d => d.DishId == dish.id);

            dm.DishDescription = dish.description;
            dm.Video = dish.video;
            dm.DishName = dish.dis_name;
            dm.DishPrice = dish.price;
            if (dish.picture != null)
            {
                byte[] base64 = Convert.FromBase64String(dish.picture);
                string path = "/images/dishes/dish_" + dish.id.ToString() + ".png";
                System.IO.File.WriteAllBytes(path, base64);
            }
            dm.Dtags.Clear();

            foreach (var tag in dish.tags)
            {
                try
                {
                    bool dtag = false;
                    foreach (var item in dm.Dtags)
                    {
                        if (tag == item.DtagName)
                        {
                            dtag = true;
                            break;
                        }
                    }

                    if (!dtag)
                        dm.Dtags.Add(_context.Dishtags.Where(e => e.DtagName == tag).First());
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            foreach (var ing in dish.ingredient)
            {
                try
                {
                    bool dingr = false;
                    foreach (var item in dm.Ingrs)
                    {
                        if (ing == item.IngrName)
                        {
                            dingr = true;
                            break;
                        }
                    }

                    if (!dingr)
                        dm.Ingrs.Add(_context.Ingredients.Where(e => e.IngrName == ing).First());
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            try
            {
                await _context.SaveChangesAsync();
                // SYS_C0011127
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
            dm.Video = dish.video;
            //putredis(dm.DishId + ":video", dish.video);
            dm.DishName = dish.dis_name;
            dm.DishPrice = dish.price;
            dm.DishId = dish.id;
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
            foreach (var ing in dish.ingredient)
            {
                try
                {
                    dm.Ingrs.Add(_context.Ingredients.Where(e => e.IngrName == ing).First());
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

                if (dish.picture != null)
                {
                    byte[] base64 = Convert.FromBase64String(dish.picture);
                    string path = "/images/dishes/dish_" + dish.id.ToString() + ".png";
                    System.IO.File.WriteAllBytes(path, base64);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return NoContent();
            }

            return Ok();
        }

        public class PostUpdateDishStatusMsg
        {
            public string? dish_order_id { get; set; }
            public string? dish_status { get; set; }
        }
        [HttpPost("UpdateDishStatus")]
        public async Task<ActionResult> UpdateDishStatus(PostUpdateDishStatusMsg p)
        {
            if (_context.Dishes == null)
            {
                return Problem("Entity set 'ModelContext.Dishes'  is null.");
            }
            var l = await _context.Dishorderlists.Where(e => e.DishOrderId == p.dish_order_id).FirstOrDefaultAsync();

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
            public string? remark { get; set; } = null;
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
                    //ditem.remark = getredis(item.DishOrderId + ":remark");
                    ditem.remark = item.Remark;
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
                ditem.dish_order_id = item.DishOrderId;
                ditem.remark = getredis(item.DishOrderId + ":remark");
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

            return Ok();
        }

        private bool DishExists(decimal id)
        {
            return (_context.Dishes?.Any(e => e.DishId == id)).GetValueOrDefault();
        }
    }
}
