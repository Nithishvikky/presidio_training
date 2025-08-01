using Microsoft.AspNetCore.Mvc;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Mapper;
using ShopOnline.Models;

namespace ShopOnline.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsService _contactUsService;

        public ContactUsController(IContactUsService contactUsService)
        {
            _contactUsService = contactUsService;
        }

        [HttpPost]
        public async Task<IActionResult> ValidateCaptchaAndAdd([FromBody]ContactusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (isValid, error) = await _contactUsService.ValidateCaptchaAsync(dto.CaptchaToken);
            if (!isValid)
                return BadRequest(new { message = "Captcha validation failed", error });

            var contact = new ContactMapper().MapContact(dto);
            var result = await _contactUsService.AddContactUs(contact);

            return Ok(new { message = "Submitted successfully", result });
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> GetContactUs(int id)
        {
            try
            {
                var contact = await _contactUsService.GetContactUsById(id);
                return Ok(contact);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody]ContactusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var contactus = await _contactUsService.GetContactUsById(id);
                if (contactus == null) throw new KeyNotFoundException("Contactus not found");

                var UpdatingContactus = new ContactMapper().MapContact(dto);
                UpdatingContactus.id = id;

                var updated = await _contactUsService.UpdateContactUs(UpdatingContactus);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _contactUsService.DeleteContactUs(id);
            if (!success)
                return NotFound(new { message = $"Contact with ID {id} not found." });

            return Ok(new { message = "Deleted successfully." });
        }
    }
}
