using System.Text.Json.Serialization;

namespace ShopOnline.DTOs
{
    public class CaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}