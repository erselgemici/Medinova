using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Medinova.Services
{
    public class GeminiService
    {
        public async Task<string> AskGeminiAsync(string userQuestion)
        {
            try
            {
                string apiKey = WebConfigurationManager.AppSettings["GeminiApiKey"];
                string baseUrl = WebConfigurationManager.AppSettings["GeminiBaseUrl"] ?? "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
                string url = $"{baseUrl}?key={apiKey}";

                using (var client = new HttpClient())
                {
                    var requestBody = new
                    {
                        contents = new[]
                        {
                            new { parts = new[] { new { text = "Sen bir hastane asistanısın. Kısa ve net cevap ver. Soru: " + userQuestion } } }
                        }
                    };

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, jsonContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        return "Üzgünüm, şu an servise ulaşamıyorum.";
                    }

                    var responseString = await response.Content.ReadAsStringAsync();

                    dynamic json = JsonConvert.DeserializeObject(responseString);
                    string answer = json?.candidates?[0]?.content?.parts?[0]?.text;

                    return answer ?? "Şu an cevap veremiyorum.";
                }
            }
            catch (Exception ex)
            {
                try
                {
                    new LogService().CreateLog(null, "System", "Error", "AI Hatası", ex.Message);
                }
                catch { }

                return "Bağlantı hatası oluştu.";
            }
        }
    }
}
