using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace youAreWhatYouEat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ModelContext _context;
        public OrderController()
        {
            _context = new ModelContext();
        }

        public class OrderInfo
        {
            public string? order_id { get; set; }
            public string? creation_time { get; set; }
            public string? table_id { get; set; }
            public string? order_status { get; set; }
            public decimal? tot_price { get; set; }
        }

        // GET 根据订单号获取订单
        [HttpGet("GetOrderById")]
        public async Task<ActionResult<OrderInfo>> GetOrderById(string order_id)
        {
            var order = await _context.Orderlists
                .Include(o => o.Dishorderlists)
                .FirstOrDefaultAsync(o => o.OrderId == order_id);
            if (order == null) return NotFound();

            OrderInfo orderInfo = new OrderInfo();
            orderInfo.order_id = order.OrderId;
            orderInfo.creation_time = order.CreationTime.ToString("yyyy-MM-dd hh:mm:ss");
            orderInfo.table_id = order.TableId.ToString();
            orderInfo.order_status = order.OrderStatus;

            decimal payment = 0;
            foreach(var item in order.Dishorderlists)
            {
                payment += item.FinalPayment;
            }
            orderInfo.tot_price = payment;

            return Ok(orderInfo);
        }

        // GET 根据桌号订单
        [HttpGet("GetOrderByTable")]
        public async Task<ActionResult<OrderInfo>> GetOrderByTable(int table)
        {
            var order = await _context.Orderlists
                .Include(o => o.Dishorderlists)
                .FirstOrDefaultAsync(o => o.TableId == table);
            if (order == null) return NotFound();

            OrderInfo orderInfo = new OrderInfo();
            orderInfo.order_id = order.OrderId;
            orderInfo.creation_time = order.CreationTime.ToString("yyyy-MM-dd hh:mm:ss");
            orderInfo.table_id = order.TableId.ToString();
            orderInfo.order_status = order.OrderStatus;

            decimal payment = 0;
            foreach (var item in order.Dishorderlists)
            {
                payment += item.FinalPayment;
            }
            orderInfo.tot_price = payment;

            return Ok(orderInfo);
        }

        public class OrderInfo2
        {
            public string? order_id { get; set; }
            public string? creation_time { get; set; }
            public string? table_id { get; set; }
            public string? order_status { get; set; }
        }

        public class AllOrderInfo
        {
            public Dictionary<string, dynamic> summary = new Dictionary<string, dynamic>();
            public List<OrderInfo2>? orders = new List<OrderInfo2>();

            public AllOrderInfo()
            {
                summary.Add("order_count", 0);
                summary.Add("awating_count", 0);
                summary.Add("awating_credit", 0);
                summary.Add("processing_count", 0);
                summary.Add("processing_credit", 0);
                summary.Add("completed_count", 0);
                summary.Add("completed_credit", 0);
                summary.Add("payed_count", 0);
                summary.Add("payed_credit", 0);
                summary.Add("total_credit", 0);
                summary.Add("today_credit", 0);
            }
        }

        // GET 获取全部订单
        [HttpGet("GetAllOrder")]
        public async Task<ActionResult<AllOrderInfo>> GetAllOrder()
        {
            var orders = await _context.Orderlists
                .Include(o => o.Dishorderlists)
                .ToListAsync();
            int order_count = 0;
            int awaiting_count = 0;
            decimal awaiting_credit = 0;
            int processing_count = 0;
            decimal processing_credit = 0;
            int completed_count = 0;
            decimal completed_credit = 0;
            int payed_count = 0;
            decimal payed_credit = 0;
            decimal total_credit = 0;
            decimal today_credit = 0;
            AllOrderInfo info = new AllOrderInfo();

            foreach (var order in orders)
            {
                OrderInfo2 orderInfo = new OrderInfo2();
                orderInfo.order_id = order.OrderId;
                orderInfo.table_id = order.TableId.ToString();
                orderInfo.creation_time = order.CreationTime.ToString("yyyy-MM-dd hh:mm:ss");
                orderInfo.order_status = order.OrderStatus;
                info.orders.Add(orderInfo);

                decimal order_credit = 0;
                foreach(var tem in order.Dishorderlists)
                {
                    order_credit += tem.FinalPayment;
                }

                order_count++;
                if (order.OrderStatus == "待处理")
                {
                    awaiting_count++;
                    awaiting_credit += order_credit;
                    total_credit += order_credit;
                    if (order.CreationTime.Date == DateTime.Now.Date) today_credit += order_credit;
                } else if (order.OrderStatus == "制作中")
                {
                    processing_count++;
                    processing_credit += order_credit;
                    total_credit += order_credit;
                    if (order.CreationTime.Date == DateTime.Now.Date) today_credit += order_credit;
                } else if (order.OrderStatus == "已完成")
                {
                    completed_count++;
                    completed_credit += order_credit;
                    total_credit += order_credit;
                    if (order.CreationTime.Date == DateTime.Now.Date) today_credit += order_credit;
                } else if (order.OrderStatus == "已支付") { 
                    payed_count++; 
                    payed_credit += order_credit;
                    total_credit += order_credit;
                    if (order.CreationTime.Date == DateTime.Now.Date) today_credit += order_credit;
                }
            }
            info.summary["order_count"] = order_count;
            info.summary["awaiting_count"] = awaiting_count;
            info.summary["awaiting_credit"] = awaiting_credit;
            info.summary["processing_count"] = processing_count;
            info.summary["processing_credit"] = processing_credit;
            info.summary["completed_count"] = completed_count;
            info.summary["completed_credit"] = completed_credit;
            info.summary["payed_count"] = payed_count;
            info.summary["payed_credit"] = payed_credit;
            info.summary["total_credit"] = total_credit;
            info.summary["today_credit"] = today_credit;

            return Ok(info);
        }

        // POST 修改订单
        [HttpPost("PostOrder")]
        public async Task<ActionResult> PostOrder(OrderInfo2 p)
        {
            if (p.order_id == null || p.creation_time == null || p.order_status == null || p.table_id == null)
                return BadRequest();

            var order = await _context.Orderlists
                .FirstOrDefaultAsync(o => o.OrderId == p.order_id);
            if (order == null) return NotFound();

            try
            {
                order.CreationTime = Convert.ToDateTime(p.creation_time);
                order.OrderStatus = p.order_status;
                order.TableId = Convert.ToInt32(p.table_id);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        public class DishInfo
        {
            public string? dish_order_id { get; set; }
            public string? order_id { get; set; }
            public string? dish_id { get; set; }
            public decimal? final_payment { get; set; }
            public decimal? original_price { get; set; }
            public string? dish_status { get; set; }
        }

        public class OrderDishInfo
        {
            public Dictionary<string , dynamic>? summary = new Dictionary<string , dynamic>();
            public List<DishInfo>? data = new List<DishInfo>();

            public OrderDishInfo()
            {
                summary.Add("tot_price", 0);
            }
        }

        // GET 获取订单内全部点菜
        [HttpGet("GetOrderDish")]
        public async Task<ActionResult<OrderDishInfo>> GetOrderDish(string order_id)
        {
            var order = await _context.Orderlists
                .Include(o => o.Dishorderlists)
                    .ThenInclude(d => d.Dish)
                .FirstOrDefaultAsync(o => o.OrderId == order_id);
            if (order == null) return NotFound();

            OrderDishInfo info = new OrderDishInfo();
            foreach(var item in order.Dishorderlists)
            {
                info.summary["tot_price"] += item.FinalPayment;
                DishInfo dish = new DishInfo();
                dish.dish_order_id = item.DishOrderId;
                dish.dish_id = item.DishId.ToString();
                dish.order_id = item.OrderId;
                dish.final_payment = item.FinalPayment;
                dish.original_price = item.Dish.DishPrice;
                dish.dish_status = item.DishStatus;

                info.data.Add(dish);
            }

            return Ok(info);
        }
    }
}
