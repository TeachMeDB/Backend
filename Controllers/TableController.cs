using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace youAreWhatYouEat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ModelContext _context;
        public TableController()
        {
            _context = new ModelContext();
        }

        public class TableInfo
        {
            public decimal table_id { get; set; }
            public decimal? customer_number { get; set; }
            public decimal? table_capacity { get; set; }
            public string? occupied { get; set; }
        }

        // GET 获取桌子状态
        [HttpGet("GetOneTable")]
        public async Task<ActionResult<TableInfo>> GetOneTable(string table_id)
        {
            var table = await _context.Dinningtables.
                FirstOrDefaultAsync(d => d.TableId.ToString() == table_id);
            if (table == null) return NoContent();

            TableInfo tableInfo = new TableInfo();
            tableInfo.table_id = table.TableId;
            tableInfo.customer_number = table.CustomerNumber;
            tableInfo.table_capacity = table.TableCapacity;
            tableInfo.occupied = table.Occupied;
            return Ok(tableInfo);
        }

        public class AttrOfTable
        {
            public string? name { get; set; }
            public List<decimal> data = new List<decimal>();
        }

        public class ListOfTable
        {
            public Dictionary<string, dynamic>? xaxis = new Dictionary<string, dynamic>();

            public ListOfTable()
            {
                xaxis.Add("categories", new List<string>());
            }
        }

        public class AllTableInfo
        {
            public Dictionary<string, dynamic> summary = new Dictionary<string, dynamic>();
            public Dictionary<string, dynamic> summary2 = new Dictionary<string, dynamic>();
            public List<TableInfo>? tables { get; set; } = new List<TableInfo>();

            public AllTableInfo()
            {
                summary.Add("available_count", 0);
                summary.Add("occupied_count", 0);
                summary.Add("total_count", 0);
                summary.Add("today_customer", 0);
                summary.Add("total_customer", 0);

                summary2.Add("series", new List<AttrOfTable>());
                summary2.Add("options", new ListOfTable());
                AttrOfTable a = new AttrOfTable();
                summary2["series"].Add(a);
                summary2["series"][0].name = "空闲";
                AttrOfTable b = new AttrOfTable();
                summary2["series"].Add(b);
                summary2["series"][1].name = "占用";
            }
        }

        // GET 获取全部餐桌
        [HttpGet("GetAllTable")]
        public async Task<ActionResult<AllTableInfo>> GetAllTable()
        {
            var tables = await _context.Dinningtables
                .Include(d => d.Orderlists)
                .ToListAsync();

            int ava = 0;
            int occ = 0;
            int tot = 0;
            int today_cnt = 0;
            int tot_cnt = 0;
            AllTableInfo info = new AllTableInfo();
            foreach(var table in tables)
            {
                TableInfo tableInfo = new TableInfo();
                tableInfo.table_id = table.TableId;
                tableInfo.customer_number = table.CustomerNumber;
                tableInfo.table_capacity = table.TableCapacity;
                tableInfo.occupied = (table.Occupied == "是") ? "占用" : "空闲";

                if (table.Occupied == "是") ava++;
                else occ++;
                tot++;
                info.tables.Add(tableInfo);

                string type = table.TableCapacity.ToString() + "人座";
                int index = info.summary2["options"].xaxis["categories"].IndexOf(type);
                if (index == -1)
                {
                    info.summary2["options"].xaxis["categories"].Add(type);
                    index = info.summary2["options"].xaxis["categories"].IndexOf(type);
                    info.summary2["series"][0].data.Add(0);
                    info.summary2["series"][1].data.Add(0);
                }
                if (table.Occupied != "是") info.summary2["series"][0].data[index]++;
                else info.summary2["series"][1].data[index]++;

                foreach(var order in table.Orderlists)
                {
                    tot_cnt += Convert.ToInt32(table.TableCapacity);
                    if (order.CreationTime.Year == DateTime.Now.Year && order.CreationTime.Month 
                        == DateTime.Now.Month && order.CreationTime.Day == DateTime.Now.Day)
                        today_cnt += Convert.ToInt32(table.TableCapacity);
                }
            }
            info.summary["available_count"] = ava;
            info.summary["occupied_count"] = occ;
            info.summary["total_count"] = tot;
            info.summary["today_customer"] = today_cnt;
            info.summary["total_customer"] = tot_cnt;

            return Ok(info);
        }

        public class SeatInfo
        {
            public bool has_table { get; set; }
            public string? table_id { get; set; }
            public string? queue_id { get; set; }
        }

        // GET 安排座位
        [HttpGet("GetSeat")]
        public async Task<ActionResult<SeatInfo>> GetSeat(int customer_number)
        {
            var tables = await _context.Dinningtables
                .Where(d => d.TableCapacity >= customer_number)
                .Where(d =>d.Occupied == "否")
                .OrderBy(d =>d.TableCapacity)
                .ToListAsync();

            SeatInfo info = new SeatInfo();
            foreach(var table in tables)
            {
                if (table.Occupied == "否")
                {
                    info.has_table = true;
                    info.table_id = table.TableId.ToString();
                    return Ok(info);
                }
            }

            string str = "";
            Random rand = new Random();
            int r = rand.Next(0, 3);
            if (r == 0) str += "A";
            else if (r == 1) str += "B";
            else str += "C";

            info.has_table = false;
            var orders = _context.OrderNumbers
                .Where(o => o.OrderDate.Date == DateTime.Now.Date);
            int num = 0;
            foreach(var order in orders)
            {
                if (order.OrderNumber1[0] == str[0])
                {
                    if (Convert.ToInt32(order.OrderNumber1.Substring(1)) > num) 
                        num = Convert.ToInt32(order.OrderNumber1.Substring(1));
                }
            }

            info.queue_id = str + (num + 1).ToString();

            return Ok(info);
        }

        // GET 按桌号安排座位
        [HttpGet("GetSeatByTable")]
        public async Task<ActionResult> GetSeatByTable(string table_id)
        {
            var table = await _context.Dinningtables
                .FirstOrDefaultAsync(t => t.TableId.ToString() == table_id);
            if (table == null) return NoContent();
            table.Occupied = "是";

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST 修改桌子状态
        [HttpPost("PostTableStatus")]
        public async Task<ActionResult> PostTableStatus(TableInfo p)
        {
            if (p.table_id == null || p.customer_number == null || p.occupied == null || p.table_capacity == null)
                return BadRequest();

            var table = await _context.Dinningtables
                .FirstOrDefaultAsync(t => t.TableId == p.table_id);
            if (table == null) return NoContent();

            table.TableCapacity = Convert.ToInt32(p.table_capacity);
            table.CustomerNumber = Convert.ToInt32(p.customer_number);
            table.Occupied = p.occupied;

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
