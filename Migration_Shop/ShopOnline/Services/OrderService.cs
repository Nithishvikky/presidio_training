using Microsoft.EntityFrameworkCore;
using ShopOnline.Context;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Mapper;
using ShopOnline.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ShopOnline.Services
{
    public class OrderService : IOrderService
    {
        private readonly ShopOnlineContext _context;
        public OrderService(ShopOnlineContext context)
        {
            _context = context;
        }

        public async Task<Order?> AddOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Order?> GetOrderById(int id)
        {
            var order = await _context.Orders
                            .Include(o => o.OrderDetails)
                            .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found.");

            return order;
        }
        public async Task<Order?> UpdateOrder(Order order)
        {
            var existing = await _context.Orders.FindAsync(order.OrderID);
            if (existing == null)
                throw new KeyNotFoundException($"Order with ID {order.OrderID} not found.");

            existing.Status = order.Status;
            existing.CustomerName = order.CustomerName;
            existing.CustomerAddress = order.CustomerAddress;
            existing.CustomerPhone = order.CustomerPhone;
            existing.CustomerEmail = order.CustomerEmail;

            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<PagedResult<Order>> OrderList(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;

            var orders = _context.Orders
                        // .Include(o => o.OrderDetails)
                        .OrderByDescending(o => o.OrderID)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            var totalCount = await _context.Orders.CountAsync();
            return new PagedResult<Order>
            {
                Items = orders,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<byte[]> ExportOrderListAsPdf()
        {
            var orders = await _context.Orders
                // .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.OrderID)
                .ToListAsync();

            using (var stream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter.GetInstance(pdfDoc, stream);

                pdfDoc.Open();

                // Add Title
                var titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                pdfDoc.Add(new Paragraph("Order Listing", titleFont));
                pdfDoc.Add(new Paragraph(" ")); // Spacer

                // Create table with 4 columns
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1.5f, 3f, 3f, 3f, 3f });

                // Add Header
                var headerFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                table.AddCell(new PdfPCell(new Phrase("OrderID", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("CustomerName", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("OrderDate", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("PaymentType", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Status", headerFont)));

                // Add Data
                var bodyFont = FontFactory.GetFont("Arial", 11, Font.NORMAL);
                foreach (var order in orders)
                {
                    table.AddCell(new Phrase(order.OrderID.ToString(), bodyFont));
                    table.AddCell(new Phrase(order.CustomerName ?? "", bodyFont));
                    table.AddCell(new Phrase(order.OrderDate?.ToString("yyyy-MM-dd") ?? "", bodyFont));
                    table.AddCell(new Phrase(order.PaymentType ?? "", bodyFont));
                    table.AddCell(new Phrase(order.Status ?? "", bodyFont));
                }

                pdfDoc.Add(table);
                pdfDoc.Close();

                return stream.ToArray(); // PDF byte[]
            }
        }
    }
}