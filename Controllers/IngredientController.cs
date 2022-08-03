using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;

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

        public class IngredientMessgae
        {
            public int? total { get; set; }
            public List<IngredientInfo>? data = new List<IngredientInfo>();
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
        }

        public class IngredientRecordMessgae
        {
            public List<IngredientRecordInfo>? data = new List<IngredientRecordInfo>();
            public int? total { get; set; }
        }

        // GET 获取原料
        [HttpGet("GetIngredient")]
        public async Task<ActionResult<IngredientMessgae>> GetIngredient(string? ingrName)
        {
            return null;
        }

        // GET 获取原料采购记录
        [HttpGet("GetIngredientRecord")]
        public string GetIngredientRecord()
        {
            return "value";
        }

        // POST 添加原料
        [HttpPost("PostAddIngredient")]
        public void PostAddIngredient([FromBody] string value)
        {
        }

        // POST 修改原料
        [HttpPost("PostUpdateIngredient")]
        public void PostUpdateIngredient([FromBody] string value)
        {
        }

        // POST 添加原料采购记录
        [HttpPost("PostAddIngredientRecord")]
        public void PostAddIngredientRecord([FromBody] string value)
        {
        }

        // POST 修改原料采购记录
        [HttpPost("PostUpdateIngredientRecord")]
        public void PostUpdateIngredientRecord([FromBody] string value)
        {
        }

        // DELETE 删除原料
        [HttpDelete("DeleteIngredient")]
        public void DeleteIngredient(int id)
        {
        }

        // DELETE 删除原料采购记录
        [HttpDelete("DeleteIngredientRecord")]
        public void DeleteIngredientRecord(int id)
        {
        }
    }
}
