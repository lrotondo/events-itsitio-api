using events_api.Services.Interfaces;
using MailUp.Sdk.Base;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace events_api.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration configuration;
        //public static readonly bool isTest = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.ToLowerInvariant().Contains("mvc.testing"));
        public static readonly bool isTest = true;
        public static readonly string apiKey = Environment.GetEnvironmentVariable("EVENTS_SENDGRID_API_KEY");

        private async void Init()
        {
            // itsitio
            string clientId = "6ea30d76-6da7-4040-977b-b612f691dfd3";
            string clientSecret = "b34247cd-c744-4e22-b25c-5db8a247c4ca";
            string username = "Tu_Usuario";
            string password = "Tu_Contraseña";


            // leo
            //string clientId = "f2c803bf-3b82-4cbc-a480-5ccb1b0cc7d5";
            //string clientSecret = "c51e0263-f11f-4c48-bfc9-24b12223ef5e";

            // Obtener el token de acceso
            string token = await GetAccessToken(clientId, clientSecret);
            string baseUrl = "https://services.mailup.com/API/v2.0";
            // Si el token es válido, procede con el envío del correo
            if (!string.IsNullOrEmpty(token))
            {
                string templateId = "2330";
                string senderName = "It Sitio";
                string senderEmail = "info@itsitio.com";
                string recipientEmail = "leorotondo@gmail.com";

                // Construye la URL para enviar el correo con el template
                string sendEmailUrl = $"{baseUrl}/messages/{templateId}/send";

                // Crea el cuerpo del mensaje
                var content = new
                {
                    Headers = new
                    {
                        From = new { Name = senderName, Email = senderEmail },
                        Subject = "Asunto del Correo"
                    },
                    Recipients = new[] { new { Email = recipientEmail } }
                };

                // Convierte el cuerpo del mensaje a JSON
                string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(content);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

                    // Realiza la solicitud POST para enviar el correo con el template
                    var response = await client.PostAsync(sendEmailUrl, new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Correo enviado exitosamente.");
                    }
                    else
                    {
                        Console.WriteLine($"Error al enviar el correo. Código de estado: {response.StatusCode}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No se pudo obtener el token de acceso.");
            }

            Console.ReadLine();
        }
        public EmailServices(IConfiguration configuration)
        {
            this.configuration = configuration;
           // this.Init();
        }


        // Método para obtener el token de acceso
        static async Task<string> GetAccessToken(string clientId, string clientSecret)
        {
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://services.mailup.com/Authorization/OAuth/Token");

                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                var content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                request.Content = content;

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                    return result?.access_token;
                }
                else
                {
                    Console.WriteLine($"Error al obtener el token: {response.ReasonPhrase}");
                    return null;
                }
            }
        }

        // Método para enviar el correo electrónico
        static async Task SendEmail(string accessToken, string senderAddress, string recipientAddress, string subject, string body)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://services.mailup.com/API/v1.1/Rest/ConsoleService.svc/Console/SendEmailMessageToGroupRecipient/1/1/1");

                var email = new
                {
                    From = senderAddress,
                    Subject = subject,
                    Content = body,
                    Recipients = new[] { recipientAddress }
                };

                var content = new StringContent(JsonSerializer.Serialize(email), Encoding.UTF8, "application/json");
                request.Content = content;

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Correo enviado exitosamente.");
                }
                else
                {
                    Console.WriteLine($"Error al enviar el correo: {response.ReasonPhrase}");
                }
            }
        }

        public static async Task SendEmailWithVariables(TemplateDTO templateVariables)
        {
            using (HttpClient client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var requestUrl = "https://send.mailup.com/API/v2.0/messages/sendtemplate";

                /*var emailContent = new
                {
                    Headers = new
                    {
                        To = new[] { new { Email = recipientAddress } }
                    },
                    Data = templateVariables
                };*/

                //var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(emailContent), Encoding.UTF8, "application/json");
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(templateVariables), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(requestUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Correo enviado exitosamente.");
                }
                else
                {
                    Console.WriteLine($"Error al enviar el correo: {response.ReasonPhrase}");
                }

                //var variablesJson = Newtonsoft.Json.JsonConvert.SerializeObject(templateVariables);

                /*var content = new StringContent(variablesJson, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(requestUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Correo enviado exitosamente.");
                }
                else
                {
                    Console.WriteLine($"Error al enviar el correo: {response.ReasonPhrase}");
                }*/
            }
        }

        // Clase para almacenar la respuesta del token
        class TokenResponse
        {
            public string access_token { get; set; }
        }
    



    public async Task<Response> SendHtmlEmail(string toEmail, string toName, string subject, string htmlContent)
        {
            if (isTest) return null;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(configuration.GetValue<string>("Sendgrid:SenderEmail"), configuration.GetValue<string>("Sendgrid:SenderName"));
            var to = new EmailAddress(toEmail, toName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlContent, htmlContent);
            return await client.SendEmailAsync(msg);
        }

        public async Task<Response> SendDynamicTemplateEmail_OneRecipient(string toEmail, string toName, string templateId, TemplateDTO dynamicTemplateData)
        {
            if (isTest) return null;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(configuration.GetValue<string>("Sendgrid:SenderEmail"), configuration.GetValue<string>("Sendgrid:SenderName"));
            var to = new EmailAddress(toEmail, toName);
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, dynamicTemplateData);
            var response = await client.SendEmailAsync(msg);
            return response;
        }
       
    }
}
