using Microsoft.AspNetCore.Mvc;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Models;
using ShopOnline.Services;

namespace ShopOnline.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ColorController : ControllerBase
    {
        private readonly IColorService _colorService;
        public ColorController(IColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet("Detail/{id}")]
        public async Task<ActionResult> GetColorById(int id)
        {
            try
            {
                var color = await _colorService.GetColorById(id);
                return Ok(color);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Color color)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _colorService.AddColor(color);
            return CreatedAtAction(nameof(GetColorById), new { id = created?.ColorId }, created);
        }

        [HttpPut("Edit")]
        public async Task<ActionResult> Edit([FromBody] Color color)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var res = await _colorService.UpdateColor(color);
                return Ok(res);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteById(int id)
        {
            if (await _colorService.DeleteColor(id))
            {
                return Ok(new { message = "Color removed successfully" });
            }
            return NotFound("Color not found");
        }

        [HttpGet("List")]
        public async Task<ActionResult> CategoryList()
        {
            var ColorAllList = await _colorService.GetAllColorOrderByName();
            return Ok(ColorAllList);
        }
    }
}