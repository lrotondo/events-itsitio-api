using events_api.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace events_api.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration configuration;
        public static readonly bool isTest = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.ToLowerInvariant().Contains("mvc.testing"));
        //public static readonly bool isTest = true;
        public static readonly string apiKey = Environment.GetEnvironmentVariable("EVENTS_SENDGRID_API_KEY");

        public EmailServices(IConfiguration configuration)
        {
            this.configuration = configuration;
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

        public async Task<Response> SendDynamicTemplateEmail_OneRecipient(string toEmail, string toName, string templateId, Dictionary<string, string> dynamicTemplateData)
        {
            if (isTest) return null;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(configuration.GetValue<string>("Sendgrid:SenderEmail"), configuration.GetValue<string>("Sendgrid:SenderName"));
            var to = new EmailAddress(toEmail, toName);
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, dynamicTemplateData);
            var response = await client.SendEmailAsync(msg);
            return response;
        }

        public async Task<Response> SendDynamicTemplateEmail_MultipleRecipients(List<EmailAddress> recipients, string templateId, Dictionary<string, string> dynamicTemplateData)
        {
            if (isTest) return null;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(configuration.GetValue<string>("Sendgrid:SenderEmail"), configuration.GetValue<string>("Sendgrid:SenderName"));
            var msg = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(from, recipients, templateId, dynamicTemplateData);
            var response = await client.SendEmailAsync(msg);
            return response;
        }
    }
}
