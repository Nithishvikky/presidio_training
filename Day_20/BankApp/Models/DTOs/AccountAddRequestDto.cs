namespace Bank.Models.DTOs
{
    public class AccountAddRequestDto
    {
        public int UserId { get; set; }
        public decimal Balance{ get; set; }
    }
}