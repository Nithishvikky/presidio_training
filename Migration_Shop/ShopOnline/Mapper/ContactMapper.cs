using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;
using ShopOnline.DTOs;
using ShopOnline.Models;
using ShopOnline.Services;

namespace ShopOnline.Mapper
{
    public class ContactMapper
    {
        public ContactU MapContact(ContactusDto contactusDto)
        {
            ContactU contact = new ContactU();

            contact.name = contactusDto.CustomerName;
            contact.email = contactusDto.CustomerEmail;
            contact.phone = contactusDto.CustomerPhone;
            contact.content = contactusDto.CustomerContent;

            return contact;
        }
    }
}