using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopOnline.Context;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Models;

namespace ShopOnline.Services
{
    public class ContactUsService : IContactUsService
    {
        private readonly ShopOnlineContext _context;
        private const string CaptchaVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";
        private readonly IConfiguration _config;
        public ContactUsService(ShopOnlineContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<ContactU?> AddContactUs(ContactU contact)
        {
            _context.ContactUs.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<bool> DeleteContactUs(int id)
        {
            var contact = await _context.ContactUs.FindAsync(id);
            if (contact == null) return false;

            _context.ContactUs.Remove(contact);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ContactU?> GetContactUsById(int id)
        {
            var contactUs = await _context.ContactUs.FindAsync(id);

            if (contactUs == null)
                throw new KeyNotFoundException($"ContactUs with ID {id} not found.");

            return contactUs;
        }

        public async Task<ContactU?> UpdateContactUs(ContactU contact)
        {
            var existing = await _context.ContactUs.FindAsync(contact.id);
            if (existing == null)
                throw new KeyNotFoundException($"ContactUs with ID {contact.id} not found.");

            existing.content = contact.content;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<(bool IsValid, string Error)> ValidateCaptchaAsync(string token)
        {
            var secret = _config["Captcha:SecretKey"];
             
            using var client = new HttpClient();
            var response = await client.PostAsync($"{CaptchaVerifyUrl}?secret={secret}&response={token}", null);

            if (!response.IsSuccessStatusCode)
                return (false, "Failed to contact captcha service");

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CaptchaResponse>(json);

            if (result.Success)
                return (true, null);

            var error = result.ErrorCodes?.FirstOrDefault() ?? "Unknown error";
            return (false, error);
        }
    }
}