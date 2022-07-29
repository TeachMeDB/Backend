using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace youAreWhatYouEat.Controllers
{
    public class UnixTimeUtil
    {
        /// <summary>
        /// 将dateTime格式转换为Unix时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int DateTimeToUnixTime(DateTime dateTime)
        {
            return (int)(dateTime - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        /// <summary>
        /// 将Unix时间戳转换为dateTime格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime UnixTimeToDateTime(int time)
        {
            if (time < 0)
                throw new ArgumentOutOfRangeException("time is out of range");

            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(time);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class OrderlistsController : ControllerBase
    {
        private readonly ModelContext _context;

        public OrderlistsController()
        {
            _context = new ModelContext();
        }

        public class OrderInfo
        {
            public string? order_id { get; set; }
            public string? creation_time { get; set; }
            public string? table_id { get; set; }
            public string? order_status { get; set; }
            public decimal? final_payment { get; set; }
            public decimal? discount_price { get; set; }
        }

        /*        [HttpGet("Hello")]
                public string Hello()
                {
                    return "Hello";
                }*/

        public class OrderListMessage
        {
            public Dictionary<string, dynamic> summary { get; set; } = new Dictionary<string, dynamic>();
            public List<dynamic> orders { get; set; } = new List<dynamic>();

            public int errorCode = 0;
            public OrderListMessage()
            {
                summary["order_count"] = 0;
                summary["total_credit"] = 0;
            }
        }

        // GET: api/Orderlists/GetOrdersByTime
        [HttpGet("GetOrdersByTime")]
        public async Task<ActionResult<OrderListMessage>> GetOrderlist(int begin = 0, int end = 2147483647)
        {
            OrderListMessage orderListMessage = new OrderListMessage();
            List<OrderInfo> orderMessages = new List<OrderInfo>();

            IEnumerable<Orderlist> orderListInfo = await _context.Orderlists
                .Where(e => e.CreationTime >= UnixTimeUtil.UnixTimeToDateTime(begin) && e.CreationTime <= UnixTimeUtil.UnixTimeToDateTime(end))
                .Include(e => e.Dishorderlists)
                .ThenInclude(e => e.Dish)
                .ToListAsync();

            int tot_cnt = 0;
            decimal tot_cre = 0;

            var p = await _context.Promotions.Where(e => e.StartTime >= UnixTimeUtil.UnixTimeToDateTime(begin) && e.EndTime <= UnixTimeUtil.UnixTimeToDateTime(end))
                .Include(e => e.Hasdishes)
                .ToListAsync();

            foreach (Orderlist o in orderListInfo)
            {
                OrderInfo om = new OrderInfo();
                om.order_id = o.OrderId;
                om.creation_time = o.CreationTime.ToString();
                om.table_id = o.TableId.ToString();
                om.order_status = o.OrderStatus;
                decimal price = 0.0M;
                decimal discount_price = 0.0M;
                Dictionary<decimal, decimal> discount_dict = new Dictionary<decimal, decimal>();
                foreach (Dishorderlist c in o.Dishorderlists)
                {
                    price += c.FinalPayment;
                    foreach (var pi in p)
                    {
                        var pd = pi.Hasdishes.ToDictionary(e => e.DishId);
                        if (pd.ContainsKey(c.DishId))
                        {
                            if (pd[c.DishId].Discount != null && c.Dish.DishPrice != null)
                            {
                                if (discount_dict.ContainsKey(c.DishId))
                                {
                                    if (discount_dict[c.DishId] > pd[c.DishId].Discount * c.Dish.DishPrice)
                                    {
                                        discount_dict[c.DishId] = (decimal)pd[c.DishId].Discount * c.Dish.DishPrice;
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var d in discount_dict)
                {
                    discount_price += d.Value;
                }
                om.final_payment = price;
                om.discount_price = discount_price;
                orderListMessage.orders.Add(om);
                tot_cnt++;
                tot_cre += price;
            }

            orderListMessage.summary["order_count"] = tot_cnt;
            orderListMessage.summary["total_credit"] = tot_cre;

            orderListMessage.errorCode = 200;
            if (tot_cnt <= 0)
                orderListMessage.errorCode = 404;
            return orderListMessage;
        }

        // GET: api/Orderlists/GetVipOrdersByTime
        [HttpGet("GetVipOrdersByTime")]
        public async Task<ActionResult<OrderListMessage>> GetVipOrderlist(int begin = 0, int end = 2147483647)
        {
            var ono = await _context.OrderNumbers
                .Where(e => e.OrderDate >= UnixTimeUtil.UnixTimeToDateTime(begin) && e.OrderDate <= UnixTimeUtil.UnixTimeToDateTime(end))
                .Where(e => e.UserName != "default01")
                .Include(e => e.Order)
                .ThenInclude(e => e.Dishorderlists)
                .ThenInclude(e => e.Dish)
                .ToListAsync();

            OrderListMessage orderListMessage = new OrderListMessage();
            List<OrderInfo> orderMessages = new List<OrderInfo>();

            List<Orderlist> orderListInfo = new List<Orderlist>();
            foreach (var o in ono)
            {
                orderListInfo.Add(o.Order);
            }

            int tot_cnt = 0;
            decimal tot_cre = 0;

            var p = await _context.Promotions
                .Where(e => e.StartTime >= UnixTimeUtil.UnixTimeToDateTime(begin) && e.EndTime <= UnixTimeUtil.UnixTimeToDateTime(end))
                .Include(e => e.Hasdishes)
                .ToListAsync();

            foreach (Orderlist o in orderListInfo)
            {
                OrderInfo om = new OrderInfo();
                om.order_id = o.OrderId;
                om.creation_time = o.CreationTime.ToString();
                om.table_id = o.TableId.ToString();
                om.order_status = o.OrderStatus;
                decimal price = 0.0M;
                decimal discount_price = 0.0M;
                Dictionary<decimal, decimal> discount_dict = new Dictionary<decimal, decimal>();
                foreach (Dishorderlist c in o.Dishorderlists)
                {
                    price += c.FinalPayment;
                    foreach (var pi in p)
                    {
                        var pd = pi.Hasdishes.ToDictionary(e => e.DishId);
                        if (pd.ContainsKey(c.DishId))
                        {
                            if (pd[c.DishId].Discount != null && c.Dish.DishPrice != null)
                            {
                                if (discount_dict.ContainsKey(c.DishId))
                                {
                                    if (discount_dict[c.DishId] > pd[c.DishId].Discount * c.Dish.DishPrice)
                                    {
                                        discount_dict[c.DishId] = (decimal)pd[c.DishId].Discount * c.Dish.DishPrice;
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var d in discount_dict)
                {
                    discount_price += d.Value;
                }
                om.final_payment = price;
                om.discount_price = discount_price;
                orderListMessage.orders.Add(om);
                tot_cnt++;
                tot_cre += price;
            }

            orderListMessage.summary["order_count"] = tot_cnt;
            orderListMessage.summary["total_credit"] = tot_cre;

            orderListMessage.errorCode = 200;
            if (tot_cnt <= 0)
                orderListMessage.errorCode = 404;
            return orderListMessage;
        }


        public class DishOrderInfo
        {
            public string? dish_order_id { get; set; }
            public string? order_id { get; set; }
            public DateTime? order_creation_time { get; set; }
            public decimal? dish_id { get; set; }
            public string? dish_status { get; set; }
            public decimal? final_payment { get; set; }
        }

        public class DishOrderInfoReply
        {
            public int code { get; set; } = 200;
            public List<DishOrderInfo>? data { get; set; } = null!;
        }

        [HttpGet("GetDishordersByTime")]
        public async Task<ActionResult<DishOrderInfoReply>> GetDishorderlist(int begin = 0, int end = 2147483647)
        {
            var l = await _context.Dishorderlists
                .Where(e => e.Order.CreationTime >= UnixTimeUtil.UnixTimeToDateTime(begin) && e.Order.CreationTime <= UnixTimeUtil.UnixTimeToDateTime(end))
                .Include(e => e.Dish)
                .Include(e => e.Order)
                .ToListAsync();
            DishOrderInfoReply ret = new DishOrderInfoReply();
            ret.data = new List<DishOrderInfo>();
            foreach (var dol in l)
            {
                DishOrderInfo t = new DishOrderInfo();
                t.dish_order_id = dol.DishOrderId;
                t.order_id = dol.OrderId;
                t.order_creation_time = dol.Order.CreationTime;
                t.dish_id = dol.DishId;
                t.dish_status = dol.DishStatus;
                t.final_payment = dol.FinalPayment;
                ret.data.Add(t);
            }

            return ret;
        }

        public class DishOrderNumInfo
        {
            public string? name { get; set; }
            public decimal? dish_id { get; set; }
            public List<string> tags { get; set; } = new List<string>();
            public decimal? price { get; set; }
            public int order_times { get; set; }
            public decimal? total_credit { get; set; }
        }
        public class DishOrderNumInfoReply
        {
            public int code { get; set; } = 200;
            public List<DishOrderNumInfo>? data { get; set; } = null!;
        }

        [HttpGet("GetDishOrderNum")]
        public async Task<ActionResult<DishOrderNumInfoReply>> GetDishOrderNum(int begin = 0, int end = 2147483647)
        {
            var l = _context.Dishorderlists
                .Where(e => e.Order.CreationTime >= UnixTimeUtil.UnixTimeToDateTime(begin) && e.Order.CreationTime <= UnixTimeUtil.UnixTimeToDateTime(end))
                .ToList()
                .GroupBy(e => e.DishId);
            DishOrderNumInfoReply ret = new DishOrderNumInfoReply();
            ret.data = new List<DishOrderNumInfo>();
            var dish = await _context.Dishes.Include(e => e.Dtags).ToDictionaryAsync(e => e.DishId);
            foreach (var d in l)
            {
                DishOrderNumInfo i = new DishOrderNumInfo();
                i.name = dish[d.Key].DishName;
                i.dish_id = d.Key;
                i.price = dish[d.Key].DishPrice;
                foreach (var tag in dish[d.Key].Dtags)
                {
                    i.tags.Add(tag.DtagName);
                }
                i.order_times = d.Count();
                i.total_credit = d.Count() * i.price;
                ret.data.Add(i);
            }

            return ret;
        }

        private bool OrderlistExists(string id)
        {
            return (_context.Orderlists?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
