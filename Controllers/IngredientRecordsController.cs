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
    public class IngredientRecordsController : ControllerBase
    {
        private readonly ModelContext _context;

        public IngredientRecordsController()
        {
            _context = new ModelContext();
        }

        // DELETE: api/IngredientRecords/Delete
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteIngredientRecord(decimal record_id)
        {
            if (_context.IngredientRecords == null)
            {
                return NoContent();
            }
            var ingredientRecord = await _context.IngredientRecords.FindAsync(record_id);
            if (ingredientRecord == null)
            {
                return NoContent();
            }

            _context.IngredientRecords.Remove(ingredientRecord);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public class IngredientRecordReplyItem
        {
            public decimal record_id { get; set; } = decimal.MaxValue;
            public string? ing_name { get; set; } = null;
            public DateTime? date { get; set; } = DateTime.UtcNow;
            public decimal? amount { get; set; } = decimal.Zero;
            public decimal? surplus { get; set; } = decimal.Zero;
        }

        // GET: api/IngredientRecords
        [HttpGet]
        public async Task<ActionResult<List<IngredientRecordReplyItem>>> GetIngredientRecords()
        {
            if (_context.IngredientRecords == null)
            {
                return NoContent();
            }
            var ret = new List<IngredientRecordReplyItem>();

            await foreach (var ingredientRecord in _context.IngredientRecords.Include(e => e.Ingr).AsAsyncEnumerable())
            {
                var i = new IngredientRecordReplyItem();
                i.record_id = ingredientRecord.RecordId;
                i.ing_name = ingredientRecord.Ingr.IngrName;
                if (ingredientRecord.Surplus != null)
                    i.surplus = (decimal)ingredientRecord.Surplus;
                else
                    i.surplus = null;
                if (ingredientRecord.Purchases != null)
                    i.amount = (decimal)ingredientRecord.Purchases;
                else
                    i.amount = ingredientRecord.Purchases;
                if (ingredientRecord.ProducedDate != null)
                    i.date = (DateTime)ingredientRecord.ProducedDate;
                else
                    i.date = ingredientRecord.ProducedDate;
                if (i.date != null)
                    ((DateTime)i.date).AddDays(((double)ingredientRecord.ShelfLife));
                ret.Add(i);
            }
            return ret;
        }

        // GET: api/IngredientRecords
        [HttpGet("GetByName")]
        public async Task<ActionResult<List<IngredientRecordReplyItem>>> GetIngredientRecordsByName(string ing_name)
        {
            if (_context.IngredientRecords == null)
            {
                return NoContent();
            }
            var ret = new List<IngredientRecordReplyItem>();

            await foreach (var ingredientRecord in _context.IngredientRecords.Include(e => e.Ingr)
                .Where(e => e.Ingr.IngrName == ing_name).AsAsyncEnumerable())
            {
                var i = new IngredientRecordReplyItem();
                i.record_id = ingredientRecord.RecordId;
                i.ing_name = ingredientRecord.Ingr.IngrName;
                if (ingredientRecord.Surplus != null)
                    i.surplus = (decimal)ingredientRecord.Surplus;
                else
                    i.surplus = null;
                if (ingredientRecord.Purchases != null)
                    i.amount = (decimal)ingredientRecord.Purchases;
                else
                    i.amount = ingredientRecord.Purchases;
                if (ingredientRecord.ProducedDate != null)
                    i.date = (DateTime)ingredientRecord.ProducedDate;
                else
                    i.date = ingredientRecord.ProducedDate;
                if (i.date != null)
                    ((DateTime)i.date).AddDays(((double)ingredientRecord.ShelfLife));
                ret.Add(i);
            }
            return ret;
        }

        public class UpdateIngredientRecordMsg
        {
            public string? record_id { get; set; }
            public decimal? surplus { get; set; }
        }
        // POST: api/IngredientRecords/UpdateIngredientRecord
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("UpdateIngredientRecord")]
        public async Task<ActionResult> UpdateIngredientRecord(UpdateIngredientRecordMsg p)
        {
            if (_context.IngredientRecords == null)
            {
                return Problem("Entity set 'ModelContext.IngredientRecords'  is null.");
            }
            var i = await _context.IngredientRecords.FindAsync(Convert.ToDecimal(p.record_id));
            if (p.surplus >= 0.0M)
            {
                    i.Surplus = p.surplus;
            }
            else
            {
                i.Surplus = 0.0M;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return NoContent();
            }

            return Ok();
        }

        // POST: api/IngredientRecords/AddIngredientRecord
        [HttpPost("AddIngredientRecord")]
        public async Task<ActionResult> AddIngredientRecord(IngredientRecordReplyItem irri)
        {
            if (_context.IngredientRecords == null)
            {
                return Problem("Entity set 'ModelContext.IngredientRecords'  is null.");
            }
            var i = new IngredientRecord();
            i.RecordId = irri.record_id;
            i.ShelfLife = 7;
            i.Surplus = irri.surplus;
            i.ProducedDate = Convert.ToDateTime(irri.date);
            i.PurchasingDate = Convert.ToDateTime(irri.date);
            i.Purchases = irri.amount;
            var ing = await _context.Ingredients.FirstOrDefaultAsync(i => i.IngrName == irri.ing_name);
            if (ing == null) return NoContent();
            i.IngrId = ing.IngrId;
            try
            {
                _context.IngredientRecords.Add(i);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return NoContent();
            }

            return Ok();
        }

        private bool IngredientRecordExists(decimal id)
        {
            return (_context.IngredientRecords?.Any(e => e.RecordId == id)).GetValueOrDefault();
        }
    }
}
