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
        public static readonly bool isTest = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.ToLowerInvariant().Contains("mvc.testing"));
        public static readonly string mailUp_Username = Environment.GetEnvironmentVariable("MAILUP_USERNAME");
        public static readonly string mailUp_Secret = Environment.GetEnvironmentVariable("MAILUP_SECRET");


        public EmailServices(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public static async Task SendEmailWithVariables(TemplateDTO templateVariables)
        {
            using (HttpClient client = new HttpClient())
            {
                templateVariables.User = new SmtpUserDTO();
                templateVariables.User.Username = mailUp_Username;
                templateVariables.User.Secret = mailUp_Secret;
                var requestUrl = "https://send.mailup.com/API/v2.0/messages/sendtemplate";
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(templateVariables), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    foreach (EmailAddressDTO toMail in templateVariables.To)
                    {
                        Console.WriteLine("Correo enviado exitosamente. Destinatario: " + toMail.Name + " - Mail: " + toMail.Email);
                    }
                }
                else
                {
                    Console.WriteLine($"Error al enviar el correo: {response.ReasonPhrase}");
                }
            }
        }
    }
}
