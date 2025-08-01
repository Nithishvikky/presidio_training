using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;
using ShopOnline.DTOs;
using ShopOnline.Models;
using ShopOnline.Services;

namespace ShopOnline.Mapper
{
    public class ProductMapper
    {
        public Product MapProduct(ProductDto productDto)
        {
            Product product = new Product();

            product.ProductName = productDto.ProductName;
            product.Image = productDto.ImageUrl ?? "";
            product.Price = productDto.Price;
            product.UserId = productDto.UserId;
            product.CategoryId = productDto.CategoryId;
            product.ColorId = productDto.ColorId;
            product.ModelId = productDto.ModelId;
            product.StorageId = productDto.StorageId;
            product.SellStartDate = productDto.SellStartDate ?? DateTime.Now;
            product.SellEndDate = productDto.SellEndDate ?? DateTime.Now.AddDays(10);
            product.IsNew = productDto.IsNew; 

            return product;
        }
    }

}