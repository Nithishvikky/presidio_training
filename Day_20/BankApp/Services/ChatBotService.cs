using System.Net.Http;
using System.Text;
using System.Text.Json;
using Bank.Models.DTOs;

namespace Bank.Services
{
    public class ChatBotService
    {
        private readonly HttpClient _httpClient;
        private const string _apiKey = "AIzaSyAtiyFA_YEjRorARL8wE9_oO3DsIbv7dZg";

        public ChatBotService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> AskQuestion(string question)
        {
          var promptText = $"""
                You are a helpful banking assistant. Only answer questions related to banking, finance, and accounts. 
                Politely refuse any unrelated questions.{question} 
                """;
            var requestBody = new
            {
              contents = new[]
              {
                new { parts = new[] { new { text = promptText } } }
              }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );
            var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";
            var response = await _httpClient.PostAsync(requestUrl,content);
            
            if (!response.IsSuccessStatusCode)
            {
              return $"Error: {response.StatusCode}";
            }

            var responseContent = await response.Content.ReadAsStringAsync(); // Http response Content-body of http response

            Console.WriteLine(responseContent);
            var FinalResponse = JsonSerializer.Deserialize<QuestionResponseDto>(responseContent);

            var reply = FinalResponse?.Candidates?.FirstOrDefault()?
                        .Content?
                        .Parts?
                        .FirstOrDefault()?
                        .Text;

            return reply ?? "No response";
        }
    }
}