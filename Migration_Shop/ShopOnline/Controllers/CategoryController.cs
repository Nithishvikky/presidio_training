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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult> CategoryPagedList(int page = 1)
        {
            var CategoryList = await _categoryService.GetCategories(page);
            return Ok(CategoryList);
        }

        [HttpGet("List")]
        public async Task<ActionResult> CategoryList()
        {
            var CategoryAllList = await _categoryService.GetCategoriesOrderByName();
            return Ok(CategoryAllList);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _categoryService.AddCategory(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = created?.CategoryId }, created);
        }

        [HttpGet("Detail/{id}")]
        public async Task<ActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(id);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("Edit")]
        public async Task<ActionResult> Edit([FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var res = await _categoryService.UpdateCategory(category);
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
            if (await _categoryService.DeleteCategory(id))
            {
                return Ok("Category removed successfully");
            }
            return NotFound("Category not found");
        }
    }
}