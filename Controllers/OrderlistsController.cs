using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace youAreWhatYouEat.Controllers
{
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

        // GET: api/Orderlists/GetOrdersByTime
        [HttpGet("GetOrdersByTime")]
        public OrderListMessage GetOrderlist(int begin = 0, int end = 2147483647)
        {
            OrderListMessage orderListMessage = new OrderListMessage();
            List<OrderInfo> orderMessages = new List<OrderInfo>();

            IEnumerable<Orderlist> orderListInfo = _context.Orderlists
                .Where(e => e.CreationTime >= UnixTimeUtil.UnixTimeToDateTime(begin) && e.CreationTime <= UnixTimeUtil.UnixTimeToDateTime(end))
                .Include(e => e.Dishorderlists)
                .ToList();

            int tot_cnt = 0;
            decimal tot_cre = 0;

            foreach (Orderlist o in orderListInfo)
            {
                OrderInfo om = new OrderInfo();
                om.order_id = o.OrderId;
                om.creation_time = o.CreationTime.ToString();
                om.table_id = o.TableId.ToString();
                om.order_status = o.OrderStatus;
                decimal price = 0.0M;
                foreach (Dishorderlist c in o.Dishorderlists)
                {
                    price += c.FinalPayment;
                }
                om.final_payment = price;
                om.discount_price = 0;
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
        private bool OrderlistExists(string id)
        {
            return (_context.Orderlists?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
