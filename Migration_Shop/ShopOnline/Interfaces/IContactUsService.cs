using ShopOnline.DTOs;
using ShopOnline.Models;

namespace ShopOnline.Interfaces
{
    public interface IContactUsService
    {
        public Task<ContactU?> GetContactUsById(int id);
        public Task<ContactU?> AddContactUs(ContactU contact);
        public Task<bool> DeleteContactUs(int id);
        public Task<ContactU?> UpdateContactUs(ContactU contact);
        public Task<(bool IsValid, string Error)> ValidateCaptchaAsync(string token);
    }
}