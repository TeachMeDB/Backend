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
            public int? table_id { get; set; }
        }

        public class OrderDishInfo
        {
            public List<DishInfo> dish_info = new List<DishInfo>();
        }

        // GET 获取订单的菜品信息
        [HttpGet("GetOrderDishInfo")]
        public async Task<ActionResult<OrderDishInfo>> GetOrderDishInfo(string? order_id)
        {
            if (order_id == null) return BadRequest();
            var orderdish = await _context.Orderlists
                .Include(o => o.Dishorderlists)
                    .ThenInclude(d => d.Dish)
                .FirstOrDefaultAsync(o => o.OrderId == order_id);
            if (orderdish != null) return NotFound();

            OrderDishInfo orderDishInfo = new OrderDishInfo();
            List<DishInfo> infos = new List<DishInfo>();
            foreach (var dish in orderdish.Dishorderlists)
            {
                DishInfo dishInfo = new DishInfo();
                dishInfo.dish_name = dish.Dish.DishName;
                dishInfo.dish_price = dish.FinalPayment;
                dishInfo.dish_status = dish.DishStatus;
                dishInfo.dish_picture = "";
                dishInfo.table_id = Convert.ToInt32(orderdish.TableId);
                infos.Add(dishInfo);
            }
            orderDishInfo.dish_info = infos;
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
        }

        public class CategoryDishInfo
        {
            public List<DishInfo2> dish_havethetag = new List<DishInfo2>();
        }

        // GET 获取某一类所有菜品
        [HttpGet("GetCategoryDishes")]
        public async Task<ActionResult<CategoryDishInfo>> GetCategoryDishes(string? dish_tag)
        {
            if (dish_tag == null) return NotFound();
            var tag = await _context.Dishtags
                .Include(d => d.Dishes)
                    .ThenInclude(dish => dish.Hasdishes)
                .FirstOrDefaultAsync(d => d.DtagName == dish_tag);
            if (tag == null) return NotFound();

            CategoryDishInfo info = new CategoryDishInfo();
            List<DishInfo2> list = new List<DishInfo2>();
            foreach (var dish in tag.Dishes)
            {
                DishInfo2 d = new DishInfo2();
                d.dish_id = Convert.ToInt32(dish.DishId);
                d.dish_name = dish.DishName;
                d.dish_price = dish.DishPrice;
                d.dish_description = dish.DishDescription;
                d.dish_picture = "";
                d.dish_rate = 0;
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
            if (order == null) return NotFound();

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

        // GET 获取订单支付状态
        [HttpGet("GetOrderStatus")]
        public async Task<ActionResult<StatusInfo>> GetOrderStatus(string? order_id)
        {
            if (order_id == null) return BadRequest();
            var order = await _context.Orderlists
                .FirstOrDefaultAsync(o => o.OrderId == order_id);
            if (order == null) return NotFound();

            StatusInfo info = new StatusInfo();
            info.order_status = order.OrderStatus;

            return Ok(info);
        }

        // POST 提交订单
        [HttpPost("PostOrder")]
        public void PostOrder([FromBody] string value)
        {
        }

        // POST 更新桌面状态信息
        [HttpPost("PostUpdateTable")]
        public void PostUpdateTable([FromBody] string value)
        {
        }

        // POST 提交菜品评价
        [HttpPost("PostDishComment")]
        public void PostDishComment([FromBody] string value)
        {
        }

        // POST 提交服务评价
        [HttpPost("PostServiceComment")]
        public void PostServiceComment([FromBody] string value)
        {
        }
    }
}
