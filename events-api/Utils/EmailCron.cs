using events_api.Services;
using events_api.Services.Interfaces;
using MailUp.Sdk.Base;
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

            eventsOfTheDay.ForEach(eventEntity =>
            {
                var emails = new List<EmailAddress>();
                eventEntity.Users.ForEach(user => emails.Add(new EmailAddress(user.Email, user.FullName)));
                var mDTO = new TemplateDTO();
                mDTO.To = new List<EmailAddressDTO>();
                eventEntity.Users.ForEach(user => mDTO.To.Add(new EmailAddressDTO { Email = user.Email, Name = user.FullName }));

                mDTO.TemplateId = int.Parse(configuration.GetValue<string>("MailUp:Templates:ComingEvent"));
                mDTO.From = new EmailAddressDTO { Email = configuration.GetValue<string>("MailUp:SenderEmail"), Name = configuration.GetValue<string>("MailUp:SenderName") };

                mDTO.CharSet = "utf-8";
                List<NameValueDTO> vars = new List<NameValueDTO>();
                vars.Add(new NameValueDTO { N = "event_name", V = eventEntity.Title });
                vars.Add(new NameValueDTO { N = "schedule_url", V = $"<a href='https://calendar.google.com/calendar/render?action=TEMPLATE&dates={eventEntity.DateUTC.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture)}%2F{eventEntity.DateUTC.AddHours(2).ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture)}&details={eventEntity.Description}&location=Online&text={eventEntity.Title}' class='es-button' target='_blank' style='mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#FFFFFF;font-size:18px;display:inline-block;background:#31CB4B;border-radius:30px;font-family:arial, \'helvetica neue\', helvetica, sans-serif;font-weight:normal;font-style:normal;line-height:22px;width:auto;text-align:center;padding:10px 20px 10px 20px;mso-padding-alt:0;mso-border-alt:10px solid #31CB4B'>Agendar en Google Calendar</a>" });
                vars.Add(new NameValueDTO { N = "schedule_outlook_url", V = $"<a href='https://outlook.live.com/calendar/0/deeplink/compose?allday=false&body={eventEntity.Description}&enddt={eventEntity.DateUTC.AddHours(2).ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}&location=&path=%2Fcalendar%2Faction%2Fcompose&rru=addevent&startdt={eventEntity.DateUTC.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}&subject={eventEntity.Title}' class='es-button' target='_blank' style='mso-style-priority:100 !important; text-decoration:none; -webkit-text-size-adjust:none; -ms-text-size-adjust:none; mso-line-height-rule:exactly; color:#FFFFFF;font-size:18px;display:inline-block;background:#31CB4B;border-radius:30px;font-family:arial, \'helvetica neue\', helvetica, sans-serif;font-weight:normal;font-style:normal;line-height:22px;width:auto;text-align:center;padding:10px 20px 10px 20px;mso-padding-alt:0;mso-border-alt:10px solid #31CB4B'>Agendar en Outlook Calendar</a>" });
                vars.Add(new NameValueDTO { N = "stream_url", V = $"<a href='{configuration.GetValue<string>("ClientUrl")}/events/{eventEntity.Slug}' class='es-button' target='_blank' style='mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#FFFFFF;font-size:18px;display:inline-block;background:#31CB4B;border-radius:30px;font-family:arial, \'helvetica neue\', helvetica, sans-serif;font-weight:normal;font-style:normal;line-height:22px;width:auto;text-align:center;padding:10px 20px 10px 20px;mso-padding-alt:0;mso-border-alt:10px solid #31CB4B'>Ver Transmisión</a>" });
                vars.Add(new NameValueDTO { N = "day", V = eventEntity.DateUTC.Day.ToString() });
                vars.Add(new NameValueDTO { N = "month", V = (new CultureInfo("es-ES", false)).DateTimeFormat.GetMonthName(eventEntity.DateUTC.Month) });
                vars.Add(new NameValueDTO { N = "time_arg", V = TimeZoneInfo.ConvertTime(eventEntity.DateUTC, TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time")).ToString("HH:mm") });
                vars.Add(new NameValueDTO { N = "time_mex", V = TimeZoneInfo.ConvertTime(eventEntity.DateUTC, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("HH:mm") });
                vars.Add(new NameValueDTO { N = "time_col", V = TimeZoneInfo.ConvertTime(eventEntity.DateUTC, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")).ToString("HH:mm") });
                mDTO.XSmtpAPI = new XSmtpAPIDTO();
                mDTO.XSmtpAPI.DynamicFields = vars;                
                EmailServices.SendEmailWithVariables(mDTO);               
            });

            Console.WriteLine("Sent daily email");
            return Task.CompletedTask;
        }
    }
}
