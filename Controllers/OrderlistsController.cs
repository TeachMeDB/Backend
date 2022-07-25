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
    public class OrderlistsController : ControllerBase
    {
        private readonly ModelContext _context;

        public OrderlistsController(ModelContext context)
        {
            _context = context;
        }

        public class OrderInfo
        {
            public decimal Id { get; set; }
            public int Time { get; set; }
            public string? Table { get; set; }
            public string? Status { get; set; }
            public decimal Payment { get; set; }
            public decimal? Discount { get; set; }
        }
        // GET: api/Orderlists/GetOrdersByTime
        [HttpGet("GetOrdersByTime")]
        public string GetOrderlist(int begin, int end)
        {
            OrderListMessage orderListMessage = new OrderListMessage();
            OrderListSummaryMessage orderListSummaryMessage = new OrderListSummaryMessage();
            List<OrderMessage> orderMessages = new List<OrderMessage>();

            orderListSummaryMessage.errorCode = 200;
            orderListSummaryMessage.data["order_count"] = 0;
            orderListSummaryMessage.data["total_credit"] = 0;
            var orderlist = orderListSummaryMessage.ReturnJson();

            return orderlist;
        }
        private bool OrderlistExists(string id)
        {
            return (_context.Orderlists?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
