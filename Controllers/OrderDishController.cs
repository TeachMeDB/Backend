using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace youAreWhatYouEat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDishController : ControllerBase
    {
        private readonly ModelContext _context;
        public OrderDishController()
        {
            _context = new ModelContext();
        }

        public class DishInfo
        {
            public string? dish_name { get; set; }
            public string? dish_status { get; set; }
            public decimal? dish_price { get; set; }
            public string? dish_picture { get; set; }
            public int? dish_num { get; set; }
            public int? table_id { get; set; }
            public int? dish_id { get; set; }
        }

        public class OrderDishInfo2
        {
            public List<DishInfo> dish_info = new List<DishInfo>();
        }

        public class ListOrderId
        {
            public List<string>? order_id = new List<string>();
        }

        // GET 获取订单的菜品信息
        [HttpPost("GetOrderDishInfo")]
        public async Task<ActionResult<OrderDishInfo2>> GetOrderDishInfo(ListOrderId? order_id)
        {
            if (order_id == null) return BadRequest();
            OrderDishInfo2 orderDishInfo = new OrderDishInfo2();

            foreach (var id in order_id.order_id)
            {
                var orderdish = await _context.Orderlists
                    .Include(o => o.Dishorderlists)
                        .ThenInclude(d => d.Dish)
                    .FirstOrDefaultAsync(o => o.OrderId == id);
                if (orderdish == null) return NoContent();

                foreach (var dish in orderdish.Dishorderlists)
                {
                    bool tag = false;
                    for (int i = 0; i < orderDishInfo.dish_info.Count; i++)
                    {
                        if (orderDishInfo.dish_info[i].dish_name == dish.Dish.DishName && orderDishInfo.dish_info[i].dish_status == dish.DishStatus)
                        {
                            orderDishInfo.dish_info[i].dish_num++;
                            tag = true;
                            break;
                        }
                    }
                    if (tag) continue;

                    DishInfo dishInfo = new DishInfo();
                    dishInfo.dish_name = dish.Dish.DishName;
                    dishInfo.dish_price = dish.FinalPayment;
                    dishInfo.dish_status = dish.DishStatus;
                    dishInfo.dish_num = 1;
                    dishInfo.dish_picture = System.Configuration.ConfigurationManager.AppSettings["ImagesUrl"] + "dishes/dish_" + dish.DishId.ToString() + ".png";
                    dishInfo.table_id = Convert.ToInt32(orderdish.TableId);
                    dishInfo.dish_id = Convert.ToInt32(dish.DishId);
                    orderDishInfo.dish_info.Add(dishInfo);
                }
            }
            return Ok(orderDishInfo);
        }

        public class DishInfo2
        {
            public int dish_id { get; set; }
            public string? dish_name { get; set; }
            public decimal? dish_price { get; set; }
            public string? dish_picture { get; set; }
            public decimal? dish_rate { get; set; }
            public string? dish_description { get; set; }
            public decimal? dish_discount { get; set; }
        }

        public class CategoryDishInfo
        {
            public List<DishInfo2> dish_havethetag = new List<DishInfo2>();
        }

        // GET 获取某一类所有菜品
        [HttpGet("GetCategoryDishes")]
        public async Task<ActionResult<CategoryDishInfo>> GetCategoryDishes(string? dish_tag, int? promotion_id)
        {
            if (dish_tag == null) return NoContent();
            var tag = await _context.Dishtags
                .Include(d => d.Dishes)
                    .ThenInclude(dish => dish.CommentOnDishes)
                .Include(d => d.Dishes)
                    .ThenInclude(dish => dish.Hasdishes)
                .FirstOrDefaultAsync(d => d.DtagName == dish_tag);
            if (tag == null) return NoContent();

            CategoryDishInfo info = new CategoryDishInfo();
            List<DishInfo2> list = new List<DishInfo2>();
            foreach (var dish in tag.Dishes)
            {
                DishInfo2 d = new DishInfo2();
                d.dish_id = Convert.ToInt32(dish.DishId);
                d.dish_name = dish.DishName;
                d.dish_price = dish.DishPrice;
                d.dish_description = dish.DishDescription;
                d.dish_picture = System.Configuration.ConfigurationManager.AppSettings["ImagesUrl"] + "dishes/dish_" + dish.DishId.ToString() + ".png";

                d.dish_discount = 1;
                foreach(var pro in dish.Hasdishes)
                {
                    if (pro.PromotionId == promotion_id)
                    {
                        d.dish_discount = pro.Discount;
                        break;
                    }
                }
                
                decimal rate = 0;
                decimal count = 0;
                foreach(var cmt in dish.CommentOnDishes)
                {
                    if (cmt.Stars == null) continue;
                    rate += Convert.ToInt32(cmt.Stars);
                    count++;
                }
                if (count == 0) rate = 0;
                else rate = rate / count;
                d.dish_rate = rate;
                list.Add(d);
            }

            info.dish_havethetag = list;
            return Ok(info);
        }

        public class PriceInfo
        {
            public decimal? orderTotalPrice { get; set; }
        }

        // GET 获取订单总价格
        [HttpGet("GetOrderPrice")]
        public async Task<ActionResult<PriceInfo>> GetOrderPrice(string? order_id)
        {
            if (order_id == null) return BadRequest();
            var order = await _context.Orderlists
                .Include(o => o.Dishorderlists)
                .FirstOrDefaultAsync(o => o.OrderId == order_id);
            if (order == null) return NoContent();

            PriceInfo info = new PriceInfo();
            decimal price = 0;
            foreach(var item in order.Dishorderlists)
            {
                price += item.FinalPayment;
            }
            info.orderTotalPrice = price;

            return Ok(info);
        }

        public class StatusInfo
        {
            public string? order_status { get; set; }
        }

/*        // GET 获取订单支付状态
        [HttpGet("GetOrderStatus")]
        public async Task<ActionResult<StatusInfo>> GetOrderStatus(string? order_id)
        {
            if (order_id == null) return BadRequest();
            var order = await _context.Orderlists
                .FirstOrDefaultAsync(o => o.OrderId == order_id);
            if (order == null) return NoContent();

            StatusInfo info = new StatusInfo();
            info.order_status = order.OrderStatus;

            return Ok(info);
        }*/

        public class PromotionDish2
        {
            public decimal dish_id { get; set; }
            public string? dish_name { get; set; }
            public decimal dish_price { get; set; }
            public string? dish_description { get; set; }

            public PromotionDish2(Dish d)
            {
                dish_id = d.DishId;
                dish_name = d.DishName;
                dish_price = d.DishPrice;
                dish_description = d.DishDescription;
            }
        }

        public class PromotionDishRecord2
        {
            public PromotionDish2? dish { get; set; } = null!;
            public decimal discount { get; set; } = 1.0M;
        }

        public class PromotionRecord2
        {
            public decimal promotion_id { get; set; }
            public string? description { get; set; } = null!;
            public string? picture { get; set; }
            public List<PromotionDishRecord2> dishes { get; set; } = new List<PromotionDishRecord2>();
        }

        // Get 获取正在进行的促销活动
        [HttpGet("GetPromotion")]
        public async Task<ActionResult<List<PromotionRecord2>>> GetPromotion()
        {
            if (_context.Promotions == null)
            {
                return NoContent();
            }

            List<PromotionRecord2> ret = new List<PromotionRecord2>();
            await foreach (var p in _context.Promotions.Include(e => e.Hasdishes).ThenInclude(e => e.Dish).AsAsyncEnumerable())
            {
                if (p.StartTime > DateTime.Now || p.EndTime < DateTime.Now) continue;
                PromotionRecord2 pr = new PromotionRecord2();
                pr.promotion_id = p.PromotionId;
                pr.description = p.Description;
                pr.picture = System.Configuration.ConfigurationManager.AppSettings["ImagesUrl"] + "promotions/promotion_" + p.PromotionId.ToString() + ".png";
                var d = p.Hasdishes;
                foreach (var di in d)
                {
                    PromotionDishRecord2 dt = new PromotionDishRecord2();
                    if (di.Discount != null) dt.discount = (decimal)di.Discount; else dt.discount = 0.0M;
                    dt.dish = new PromotionDish2(di.Dish);
                    pr.dishes.Add(dt);
                }
                ret.Add(pr);
            }
            return Ok(ret);
        }

        public class DishMessage
        {
            public List<DishesInfo> dish_all = new List<DishesInfo>();
        }

        public class DishesCommentInfo
        {
            public string? comment_content { get; set; }
            public string? comment_time { get; set; }
            public decimal? comment_star { get; set; }
        }

        public class DishesInfo
        {
            public int? dish_id { get; set; }
            public string? dish_name { get; set; }
            public string? dish_picture { get; set; }
            public decimal? dish_price { get; set; }
            public decimal? dish_rate { get; set; }
            public string? dish_description { get; set; }
            public List<decimal>? dish_discount = new List<decimal>();
            public List<string>? dish_tag = new List<string>();
            public List<DishesCommentInfo> dish_comment = new List<DishesCommentInfo>();
        }

        // Get 获取所有菜品
        [HttpGet("GetAllDishes")]
        public async Task<ActionResult<DishMessage>> GetAllDishes()
        {
            var dishes = await _context.Dishes
                .Include(d => d.CommentOnDishes)
                .Include(d => d.Hasdishes)
                .Include(d => d.Dtags)
                .ToListAsync();

            DishMessage msg = new DishMessage();
            foreach (var d in dishes)
            {
                DishesInfo info = new DishesInfo();
                info.dish_id = Convert.ToInt32(d.DishId);
                info.dish_name = d.DishName;
                info.dish_price = d.DishPrice;
                info.dish_description = d.DishDescription;
                info.dish_picture = System.Configuration.ConfigurationManager.AppSettings["ImagesUrl"] + "dishes/dish_" + d.DishId.ToString() + ".png";

                decimal rate = 0;
                decimal count = 0;
                foreach (var cmt in d.CommentOnDishes)
                {
                    DishesCommentInfo info2 = new DishesCommentInfo();
                    info2.comment_star = cmt.Stars;
                    info2.comment_time = ((DateTime)cmt.CommentTime).ToString("yyyy-MM-dd HH:mm:ss");
                    info2.comment_content = cmt.CommentContent;
                    info.dish_comment.Add(info2);

                    if (cmt.Stars == null) continue;
                    rate += Convert.ToInt32(cmt.Stars);
                    count++;
                }
                if (count == 0) rate = 0;
                else rate = rate / count;
                info.dish_rate = rate;

                foreach(var tag in d.Dtags)
                {
                    info.dish_tag.Add(tag.DtagName);
                }
                foreach(var dis in d.Hasdishes)
                {
                    info.dish_discount.Add(Convert.ToDecimal(dis.Discount));
                }
                msg.dish_all.Add(info);
            }

            return Ok(msg);
        }

        public class RealPrice
        {
            public decimal? price { get; set; }
            public decimal? discount { get; set; }
        }

        [HttpGet("GetRealPrice")]
        public async Task<ActionResult<RealPrice>> GetRealPrice(int promotion_id, int dish_id)
        {
            var pro = await _context.Promotions
                .Include(p => p.Hasdishes)
                    .ThenInclude(h => h.Dish)
                .FirstOrDefaultAsync(p => p.PromotionId == promotion_id);
            if (pro == null) return NoContent();

            foreach(var dish in pro.Hasdishes)
            {
                if (dish.DishId == dish_id)
                {
                    RealPrice p = new RealPrice();
                    p.price = dish.Dish.DishPrice;
                    p.discount = dish.Discount;
                    return Ok(p);
                }
            }
            return NoContent();
        }

        public class OrderInfo3
        {
            public int? dish_id { get; set; }
            public int? dish_num { get; set; }
        }

        public class PostOrderInfo
        {
            public List<OrderInfo3> dishes_info { get; set; }
        }

        public class ReturnOrder
        {
            public string? order_id { get; set; }
        }

        // POST 提交订单
        [HttpPost("PostOrder")]
        public async Task<ActionResult<ReturnOrder>> PostOrder(PostOrderInfo p)
        {
            var orders = await _context.Orderlists
                .Select(o => o.OrderId)
                .ToListAsync();

            Orderlist order = new Orderlist();
            order.OrderStatus = "待处理";
            order.CreationTime = DateTime.Now;

            Random random = new Random();
            string order_id = "";
            do
            {
                order_id = "";
                for (int i = 0; i < 11; i++)
                {
                    int r = random.Next(0, 62);
                    if (r < 10) order_id += r.ToString();
                    else if (r < 36) order_id += (char)(97 + r - 10);
                    else order_id += (char)(65 + r - 36);
                }
            } while (orders.IndexOf(order_id) != -1);
            order.OrderId = order_id;

            try
            {
                _context.Orderlists.Add(order);
                await _context.SaveChangesAsync();
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }

            for (int t = 0; t < p.dishes_info.Count; t++) {
                for (int k = 0; k < p.dishes_info[t].dish_num; k++)
                {
                    Dishorderlist dish_order = new Dishorderlist();
                    dish_order.OrderId = order_id;
                    dish_order.DishId = Convert.ToDecimal(p.dishes_info[t].dish_id);
                    dish_order.DishStatus = "待处理";

                    string dish_order_id = "";
                    var dish_orders = new List<string>();
                    do
                    {
                        dish_orders = await _context.Dishorderlists
                            .Select(d => d.DishOrderId)
                            .ToListAsync();

                        dish_order_id = "";
                        for (int i = 0; i < 11; i++)
                        {
                            int r = random.Next(0, 62);
                            if (r < 10) dish_order_id += r.ToString();
                            else if (r < 36) dish_order_id += (char)(97 + r - 10);
                            else dish_order_id += (char)(65 + r - 36);
                        }
                    } while (dish_orders.IndexOf(dish_order_id) != -1);
                    dish_order.DishOrderId = dish_order_id;

                    try
                    {
                        _context.Dishorderlists.Add(dish_order);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex);
                    }
                }
            }

            return Ok(order_id);
        }

        public class PostTableInfo
        {
            public int? table_id { get; set; }
        }

        // POST 更新桌面状态信息
        [HttpPost("PostUpdateTable")]
        public async Task<ActionResult> PostUpdateTable(PostTableInfo p)
        {
            if (p.table_id == null) return BadRequest();
            var table = await _context.Dinningtables
                .FirstOrDefaultAsync(t => t.TableId == p.table_id);
            if (table == null) return NoContent();

            table.Occupied = "否";
            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        public class DishCommentInfo
        {
            public decimal? rate { get; set; }
            public string? content { get; set; }
            public int? dish_id { get; set; }
            public string? username { get; set; }
        }

        // POST 提交菜品评价
        [HttpPost("PostDishComment")]
        public async Task<ActionResult> PostDishComment(DishCommentInfo p)
        {
            if (p.dish_id == null || p.username == null) return BadRequest();
            CommentOnDish cmt = new CommentOnDish();
            cmt.Stars = p.rate;
            cmt.DishId = Convert.ToDecimal(p.dish_id);
            cmt.UserName = p.username;
            cmt.CommentContent = p.content;
            cmt.CommentTime = DateTime.Now;

            Random random = new Random();
            var cod = new List<string>();
            string cid = "";
            do
            {
                cod = await _context.CommentOnDishes
                    .Select(d => d.CommentId)
                    .ToListAsync();

                cid = "";
                for (int i = 0; i < 16; i++)
                {
                    int r = random.Next(0, 62);
                    if (r < 10) cid += r.ToString();
                    else if (r < 36) cid += (char)(97 + r - 10);
                    else cid += (char)(65 + r - 36);
                }
            } while (cod.IndexOf(cid) != -1);
            cmt.CommentId = cid;

            try
            {
                _context.CommentOnDishes.Add(cmt);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        public class ServiceCommentInfo
        {
            public decimal? rate { get; set; }
            public string? content { get; set; }
            public string? username { get; set; }
        }

        // POST 提交服务评价
        [HttpPost("PostServiceComment")]
        public async Task<ActionResult> PostServiceComment(ServiceCommentInfo p)
        {
            if (p.username == null) return BadRequest();
            CommentOnService cms = new CommentOnService();
            cms.Stars = p.rate;
            cms.UserName = p.username;
            cms.CommentContent = p.content;
            cms.CommentTime = DateTime.Now;

            Random random = new Random();
            var cos = new List<string>();
            string cid = "";
            do
            {
                cos = await _context.CommentOnServices
                    .Select(d => d.CommentId)
                    .ToListAsync();

                cid = "";
                for (int i = 0; i < 16; i++)
                {
                    int r = random.Next(0, 62);
                    if (r < 10) cid += r.ToString();
                    else if (r < 36) cid += (char)(97 + r - 10);
                    else cid += (char)(65 + r - 36);
                }
            } while (cos.IndexOf(cid) != -1);
            cms.CommentId = cid;

            try
            {
                _context.CommentOnServices.Add(cms);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
