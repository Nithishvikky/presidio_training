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
    public class ModelController : ControllerBase
    {
        private readonly IModelService _modelService;
        public ModelController(IModelService modelService)
        {
            _modelService = modelService;
        }

        [HttpGet("Detail/{id}")]
        public async Task<ActionResult> GetModelById(int id)
        {
            try
            {
                var Model = await _modelService.GetModelById(id);
                return Ok(Model);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Model Model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _modelService.AddModel(Model);
            return CreatedAtAction(nameof(GetModelById), new { id = created?.ModelId }, created);
        }

        [HttpPut("Edit")]
        public async Task<ActionResult> Edit([FromBody] Model Model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var res = await _modelService.UpdateModel(Model);
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
            if (await _modelService.DeleteModel(id))
            {
                return Ok("Model removed successfully");
            }
            return NotFound("Model not found");
        }
    }
}