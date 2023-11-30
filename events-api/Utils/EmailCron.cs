using events_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SendGrid.Helpers.Mail;
using System.Globalization;

namespace events_api.Utils
{
    public class EmailCron : IJob
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IEmailServices emailServices;
        private readonly IConfiguration configuration;

        public EmailCron(ApplicationDbContext dbContext, IEmailServices emailServices, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.emailServices = emailServices;
            this.configuration = configuration;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var eventsOfTheDay = dbContext.Events.Include(e => e.Users).Where(e => e.DateUTC.Date == DateTime.Today).ToList();

            eventsOfTheDay.ForEach(ev =>
            {
                var emails = new List<EmailAddress>();
                ev.Users.ForEach(user => emails.Add(new EmailAddress(user.Email, user.FullName)));

                var emailData = new Dictionary<string, string>()
                {
                    ["event_name"] = ev.Title,
                    ["schedule_url"] = $"https://calendar.google.com/calendar/render?action=TEMPLATE&dates={ev.DateUTC.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture)}%2F{ev.DateUTC.AddHours(2).ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture)}&details={ev.Description}&location=Online&text={ev.Title}",
                    ["stream_url"] = $"{configuration.GetValue<string>("ClientUrl")}/events/{ev.Slug}",
                    ["day"] = ev.DateUTC.Day.ToString(),
                    ["month"] = (new CultureInfo("es-ES", false)).DateTimeFormat.GetMonthName(ev.DateUTC.Month),
                    ["time_arg"] = TimeZoneInfo.ConvertTime(ev.DateUTC, TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time")).ToString("HH:mm"),
                    ["time_mex"] = TimeZoneInfo.ConvertTime(ev.DateUTC, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("HH:mm"),
                    ["time_col"]
                    = TimeZoneInfo.ConvertTime(ev.DateUTC, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("HH:mm")
                };

                emailServices.SendDynamicTemplateEmail_MultipleRecipients(
                    emails,
                    configuration.GetValue<string>("Sendgrid:Templates:ComingEvent"),
                    emailData).Wait();
            });

            Console.WriteLine("Sent daily email");
            return Task.CompletedTask;
        }
    }
}
