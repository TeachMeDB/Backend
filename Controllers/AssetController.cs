using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace youAreWhatYouEat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly ModelContext _context;
        public AssetController()
        {
            _context = new ModelContext();
        }

        public class AssetInfo
        {
            public string? assets_id { get; set; }
            public string? assets_type { get; set; }
            public int? employee_id { get; set; }
            public string? employee_name { get; set; }
            public string? assets_status { get; set; }
        }

        public class AssetMessage
        {
            public int? total { get; set; }
            public List<AssetInfo> data = new List<AssetInfo>();
        }

        // GET 获取资产
        [HttpGet("GetAsset")]
        public async Task<ActionResult<AssetMessage>> GetAsset(string? assets_type)
        {
            AssetMessage msg = new AssetMessage();
            if (assets_type != null)
            {
                var assets = await _context.Assets
                    .Include(a => a.Employee)
                    .FirstOrDefaultAsync(a =>a.AssetsType == assets_type);
                if (assets == null) return NoContent();

                msg.total = 1;
                AssetInfo info = new AssetInfo();
                info.assets_id = assets.AssetsId;
                info.assets_type = assets.AssetsType;
                info.employee_id = Convert.ToInt32(assets.EmployeeId);
                info.assets_status = (assets.AssetsStatus == 0) ? "正常" : "已坏";
                info.employee_name = assets.Employee.Name;
                msg.data.Add(info);
            }
            else
            {
                var assets = await _context.Assets
                    .Include(a => a.Employee)
                    .ToListAsync();

                foreach (var asset in assets)
                {
                    AssetInfo info = new AssetInfo();
                    info.assets_id = asset.AssetsId;
                    info.assets_type = asset.AssetsType;
                    info.employee_id = Convert.ToInt32(asset.EmployeeId);
                    info.assets_status = (asset.AssetsStatus == 0) ? "正常" : "已坏";
                    info.employee_name = asset.Employee.Name;
                    msg.data.Add(info);
                }
                msg.total = msg.data.Count;
            }
            return Ok(msg);
        }

        public class AssetRecordInfo
        {
            public string? assets_id { get; set; }
            public string? assets_type { get; set; }
            public int? employee_id { get; set; }
            public string? employee_name { get; set; }
            public string? manage_type { get; set; }
            public string? manage_date { get; set; }
            public string? manage_reason { get; set; }
            public string? manage_cost { get; set; }
        }

        public class AssetRecordMessage
        {
            public List<AssetRecordInfo>? data = new List<AssetRecordInfo>();
            public int? total { get; set; }
        }

        // GET 获取资产维修记录
        [HttpGet("GetAssetRecord")]
        public async Task<ActionResult<AssetRecordMessage>> GetAssetRecord()
        {
            var records = await _context.Manages
                .Include(m => m.Employee)
                .Include(m => m.Assets)
                .ToListAsync();
            AssetRecordMessage msg = new AssetRecordMessage();

            foreach (var record in records)
            {
                AssetRecordInfo info = new AssetRecordInfo();
                info.assets_id = record.AssetsId;
                info.assets_type = record.Assets.AssetsType;
                info.employee_id = Convert.ToInt32(record.EmployeeId);
                info.employee_name = record.Employee.Name;
                info.manage_type = record.ManageType;
                info.manage_reason = record.ManageReason;
                info.manage_date = ((DateTime)record.ManageDate).ToString("yyyy-MM-dd");
                info.manage_cost = record.ManageCost.ToString() + "元";
                msg.data.Add(info);
            }
            msg.total = msg.data.Count;
            return Ok(msg);
        }

        public class PostAddAssetInfo
        {
            public int? employeeId { get; set; }
            public string? assetsType { get; set; }
            public string? assetsStatus { get; set; }
        }

        // POST 添加资产
        [HttpPost("PostAddAsset")]
        public async Task<ActionResult> PostAddAsset(PostAddAssetInfo p)
        {
            if (p.employeeId == null || p.assetsStatus == null || p.assetsType == null)
                return BadRequest();
            Asset info = new Asset();
            info.EmployeeId = Convert.ToDecimal(p.employeeId);
            info.AssetsType = p.assetsType;
            info.AssetsStatus = (p.assetsStatus == "正常") ? 0 : -1;

            string str = "";
            Random rand = new Random();
            int r = rand.Next(0, 3);
            if (r == 0) str += "A";
            else if (r == 1) str += "B";
            else str += "C";

            var assets = await _context.Assets
                .ToListAsync();
            int num = 0;
            foreach (var asset in assets)
            {
                if (asset.AssetsId[0] == str[0])
                {
                    if (Convert.ToInt32(asset.AssetsId.Substring(1)) > num)
                        num = Convert.ToInt32(asset.AssetsId.Substring(1));
                }
            }
            info.AssetsId = str + (num + 1).ToString();

            try
            {
                _context.Assets.Add(info);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        public class PostUpdateAssetInfo
        {
            public string? assetsId { get; set; }
            public int? employeeId { get; set; }
            public string? assetsType { get; set; }
            public string? assetsStatus { get; set; }
        }

        // POST 修改资产
        [HttpPost("PostUpdateAsset")]
        public async Task<ActionResult> PostUpdateAsset(PostUpdateAssetInfo p)
        {
            if (p.employeeId == null || p.assetsStatus == null || p.assetsType == null || p.assetsId == null)
                return BadRequest();
            var info = await _context.Assets
                .FirstOrDefaultAsync(i => i.AssetsId == p.assetsId);
            if (info == null) return NoContent();

            try
            {
                info.EmployeeId = Convert.ToDecimal(p.employeeId);
                info.AssetsType = p.assetsType;
                info.AssetsStatus = (p.assetsStatus == "正常") ? 0 : -1;
                info.AssetsId = p.assetsId;
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        public class PostAssetRecordInfo
        {
            public string? assets_id { get; set; }
            public int? employee_id { get; set; }
            public string? manage_type { get; set; }
            public string? manage_date { get; set; }
            public string? manage_reason { get; set; }
            public string? manage_cost { get; set; }
        }

        // POST 添加资产维修记录
        [HttpPost("PostAddAssetRecord")]
        public async Task<ActionResult> PostAddAssetRecord(PostAssetRecordInfo p)
        {
            if (p.assets_id == null || p.employee_id == null || p.manage_cost == null || p.manage_type == null
                || p.manage_date == null)
                return BadRequest();
            Manage info = new Manage();
            info.AssetsId = p.assets_id;
            info.EmployeeId = Convert.ToDecimal(p.employee_id);
            info.ManageDate = Convert.ToDateTime(p.manage_date);
            info.ManageType = p.manage_type;
            info.ManageCost = p.manage_cost;
            info.ManageReason = p.manage_reason;

            try
            {
                _context.Manages.Add(info);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST 修改资产维修记录
        [HttpPost("PostUpdateAssetRecord")]
        public async Task<ActionResult> PostUpdateAssetRecord(PostAssetRecordInfo p)
        {
            if (p.assets_id == null || p.employee_id == null || p.manage_cost == null || p.manage_type == null
                || p.manage_date == null)
                return BadRequest();
            
            var info = await _context.Manages
                .FirstOrDefaultAsync(m => (m.AssetsId == p.assets_id && m.ManageCost == p.manage_cost && 
                m.ManageDate == Convert.ToDateTime(p.manage_date) && m.ManageType == p.manage_type && 
                m.ManageReason == p.manage_reason));
            if (info == null) return NoContent();
            info.EmployeeId = Convert.ToDecimal(p.employee_id);

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

        // DELETE 删除资产
        [HttpDelete("DeleteAsset")]
        public async Task<ActionResult> DeleteAsset(string id)
        {
            if (id == null) return BadRequest();
            var info = await _context.Assets
                .FirstOrDefaultAsync(a => a.AssetsId == id);
            if (info == null) return NoContent();

            try
            {
                _context.Assets.Remove(info);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
