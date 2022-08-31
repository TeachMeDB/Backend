using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;
#pragma warning disable CS8629
#pragma warning disable CS8604
#pragma warning disable CS8601
#pragma warning disable CS8618
#pragma warning disable CS0162
#pragma warning disable CS0168
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace youAreWhatYouEat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly ModelContext _context;
        public IngredientController()
        {
            _context = new ModelContext();
        }

        public class IngredientInfo
        {
            public int ingrId { get; set; }
            public string? ingrType { get; set; }
            public string? ingrName { get; set; }
            public string? ingrDescription { get; set; }
        }

        public class IngredientMessage
        {
            public int? total { get; set; }
            public List<IngredientInfo>? data = new List<IngredientInfo>();
        }

        // GET 获取原料
        [HttpGet("GetIngredient")]
        public async Task<ActionResult<IngredientMessage>> GetIngredient(string? ingrName)
        {
            IngredientMessage msg = new IngredientMessage();
            if (ingrName != null)
            {
                var ingrs = await _context.Ingredients
                    .FirstOrDefaultAsync(i => i.IngrName == ingrName);
                if (ingrs == null) return NoContent();

                msg.total = 1;
                IngredientInfo info = new IngredientInfo();
                info.ingrId = Convert.ToInt32(ingrs.IngrId);
                info.ingrName = ingrs.IngrName;
                info.ingrDescription = ingrs.IngrDescription;
                info.ingrType = ingrs.IngrType;
                msg.data.Add(info);
            } else
            {
                var ingrs = await _context.Ingredients
                    .ToListAsync();
                
                foreach(var ingr in ingrs)
                {
                    IngredientInfo info = new IngredientInfo();
                    info.ingrId = Convert.ToInt32(ingr.IngrId);
                    info.ingrName = ingr.IngrName;
                    info.ingrDescription = ingr.IngrDescription;
                    info.ingrType = ingr.IngrType;
                    msg.data.Add(info);
                }
                msg.total = msg.data.Count;
            }
            return Ok(msg);
        }

        public class IngredientRecordInfo
        {
            public int? record_id { get; set; }
            public int? ingr_id { get; set; }
            public string? ingr_name { get; set; }
            public string? purchasing_date { get; set; }
            public string? measure_unit { get; set; }
            public int? shelf_life { get; set; }
            public string? produced_date { get; set; }
            public decimal? price { get; set; }
            public int? director_id { get; set; }
            public string? director_name { get; set; }
            public decimal? surplus { get; set; }
            public decimal? purchases { get; set; }
        }

        public class IngredientRecordMessage
        {
            public List<IngredientRecordInfo>? data = new List<IngredientRecordInfo>();
            public int? total { get; set; }
        }

        // GET 获取原料采购记录
        [HttpGet("GetIngredientRecord")]
        public async Task<ActionResult<IngredientRecordMessage>> GetIngredientRecord()
        {
            var records = await _context.IngredientRecords
                .Include(i => i.Ingr)
                .Include(i => i.Director)
                .ToListAsync();
            IngredientRecordMessage msg = new IngredientRecordMessage();

            foreach (var record in records)
            {
                IngredientRecordInfo info = new IngredientRecordInfo();
                info.record_id = Convert.ToInt32(record.RecordId);
                info.ingr_id = Convert.ToInt32(record.IngrId);
                if (record.PurchasingDate != null) info.purchasing_date = ((DateTime)record.PurchasingDate).ToString("yyyy-MM-dd");
                info.measure_unit = record.MeasureUnit;
                info.director_id = Convert.ToInt32(record.DirectorId);
                info.shelf_life = Convert.ToInt32(record.ShelfLife);
                if (record.ProducedDate != null) info.produced_date = ((DateTime)record.ProducedDate).ToString("yyyy-MM-dd");
                info.price = record.Price;
                info.ingr_name = record.Ingr.IngrName;
                if (record.Director != null) info.director_name = record.Director.Name;
                if (record.Purchases != null) info.purchases = record.Purchases;
                else info.purchases = 0;
                if(record.Surplus != null) info.surplus = record.Surplus;
                else info.surplus = 0;

                msg.data.Add(info);
            }
            msg.total = msg.data.Count;
            return Ok(msg);
        }

        public class PostIngredientInfo
        {
            public int? ingr_id { get; set; }
            public string? ingr_name { get; set; }
            public string? ingr_type { get; set; }
            public string? ingr_description { get; set; }
        }

        // POST 添加原料
        [HttpPost("PostAddIngredient")]
        public async Task<ActionResult> PostAddIngredient(PostIngredientInfo p)
        {
            if (p.ingr_id == null || p.ingr_type == null || p.ingr_name == null || p.ingr_description == null)
                return BadRequest();
            Ingredient info = new Ingredient();
            info.IngrId = Convert.ToDecimal(p.ingr_id);
            info.IngrName = p.ingr_name;
            info.IngrDescription = p.ingr_description;
            info.IngrType = p.ingr_type;

            try
            {
                _context.Ingredients.Add(info);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST 修改原料
        [HttpPost("PostUpdateIngredient")]
        public async Task<ActionResult> PostUpdateIngredient(PostIngredientInfo p)
        {
            if (p.ingr_id == null || p.ingr_type == null || p.ingr_name == null || p.ingr_description == null)
                return BadRequest();
            var info = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.IngrId == Convert.ToDecimal(p.ingr_id));
            if (info == null) return NoContent();

            try
            {
                info.IngrType = p.ingr_type;
                info.IngrName = p.ingr_name;
                info.IngrDescription = p.ingr_description;
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        public class PostIngredientRecordInfo
        {
            public int? record_id { get; set; }
            public int? ingr_id { get; set; }
            public string? purchasing_date { get; set; }
            public string? measure_unit { get; set; }
            public int? shelf_life { get; set; }
            public string? produced_date { get; set; }
            public decimal? price { get; set; }
            public int? director_id { get; set; }
            public decimal? surplus { get; set; }
            public decimal? purchases { get; set; }
        }

        // POST 添加原料采购记录
        [HttpPost("PostAddIngredientRecord")]
        public async Task<ActionResult> PostAddIngredientRecord(PostIngredientRecordInfo p)
        {
            if (p.ingr_id == null || p.record_id == null)
                return BadRequest();
            IngredientRecord info = new IngredientRecord();
            info.IngrId = Convert.ToDecimal(p.ingr_id);
            info.RecordId = Convert.ToDecimal(p.record_id);
            info.PurchasingDate = Convert.ToDateTime(p.purchasing_date);
            info.ProducedDate = Convert.ToDateTime(p.produced_date);
            info.MeasureUnit = p.measure_unit;
            info.ShelfLife = p.shelf_life;
            info.Price = p.price;
            info.DirectorId = Convert.ToDecimal(p.director_id);
            info.Surplus = p.surplus;
            info.Purchases = p.purchases;

            try
            {
                _context.IngredientRecords.Add(info);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST 修改原料采购记录
        [HttpPost("PostUpdateIngredientRecord")]
        public async Task<ActionResult> PostUpdateIngredientRecord(PostIngredientRecordInfo p)
        {
            if (p.ingr_id == null || p.record_id == null)
                return BadRequest();
            var info = await _context.IngredientRecords
                .FirstOrDefaultAsync(i => i.RecordId == Convert.ToDecimal(p.record_id));
            if (info == null) return NoContent();

            try
            {
                info.IngrId = Convert.ToDecimal(p.ingr_id);
                info.PurchasingDate = Convert.ToDateTime(p.purchasing_date);
                info.ProducedDate = Convert.ToDateTime(p.produced_date);
                info.MeasureUnit = p.measure_unit;
                info.ShelfLife = p.shelf_life;
                info.Price = p.price;
                info.DirectorId = Convert.ToDecimal(p.director_id);
                info.Surplus = p.surplus;
                info.Purchases = p.purchases;

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE 删除原料
        [HttpDelete("DeleteIngredient")]
        public async Task<ActionResult> DeleteIngredient(string id)
        {
            if (id == null) return BadRequest();
            var info = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.IngrId.ToString() == id);
            if (info == null) return NoContent();

            try
            {
                _context.Ingredients.Remove(info);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // DELETE 删除原料采购记录
        [HttpDelete("DeleteIngredientRecord")]
        public async Task<ActionResult> DeleteIngredientRecord(string id)
        {
            if (id == null) return BadRequest();
            var info = await _context.IngredientRecords
                .FirstOrDefaultAsync(i => i.RecordId.ToString() == id);
            if (info == null) return NoContent();

            try
            {
                _context.IngredientRecords.Remove(info);
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
