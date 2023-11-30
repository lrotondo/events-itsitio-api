using SendGrid;
using SendGrid.Helpers.Mail;

namespace events_api.Services.Interfaces
{
    public interface IEmailServices
    {
        Task<Response> SendDynamicTemplateEmail_MultipleRecipients(List<EmailAddress> recipients, string templateId, Dictionary<string, string> dynamicTemplateData);
        Task<Response> SendDynamicTemplateEmail_OneRecipient(string toEmail, string toName, string templateId, Dictionary<string, string> dynamicTemplateData);
        Task<Response> SendHtmlEmail(string toEmail, string toName, string subject, string htmlContent);
    }
}