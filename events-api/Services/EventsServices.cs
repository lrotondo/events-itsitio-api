using AutoMapper;
using events_api.DTOs;
using events_api.Entities;
using events_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using events_api.Extensions;
using ClosedXML.Excel;
using System.Globalization;
using MailUp.Sdk.Base;

namespace events_api.Services
{
    public class EventsServices : ControllerBase, IEventsServices
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
     
        public EventsServices(ApplicationDbContext context, IMapper mapper, IHttpClientFactory httpClientFactory, IConfiguration configuration, IEmailServices emailServices)
        {
            this.context = context;
            this.mapper = mapper;
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;           
        }

        public async Task<ActionResult> CreateEvent(EventCreationDTO dto)
        {
            var eventEntity = mapper.Map<Event>(dto);

            var slug = eventEntity.Title.ToUrlSlug();
            if (await context.Events.AnyAsync(e => e.Slug == slug))
            {
                slug = slug + (await context.Events.CountAsync()).ToString();
            }

            eventEntity.Slug = slug;
            await context.Events.AddAsync(eventEntity);
            await context.SaveChangesAsync();
            return Ok();
        }

        public async Task<ActionResult<EventListResponseDTO>> GetAllEvents(EventsFilter filter)
        {
            var events = await context.Events
                .Include(e => e.Speakers)
                .ThenInclude(e => e.Speaker)
                .Include(e => e.Sponsors)
                .ThenInclude(e => e.Sponsor)
                .Include(e => e.Users)
                .ToListAsync();

            if (!filter.All && filter.From != null && filter.To != null)
            {
                events = events.Where(e => e.DateUTC >= filter.From && e.DateUTC <= filter.To).ToList();
            }

            if (filter.PerDate) events = events.OrderBy(e => e.DateUTC).ToList();
            if (filter.PerTitle) events = events.OrderBy(e => e.Title).ToList();
            if (filter.PerUsers) events = events.OrderByDescending(e => e.Users.Count()).ToList();

            if (filter.Broadcasted && !filter.NotBroadcasted) events = events.Where(e => e.DateUTC < DateTime.UtcNow).ToList();
            if (filter.NotBroadcasted && !filter.Broadcasted) events = events.Where(e => e.DateUTC >= DateTime.UtcNow).ToList();

            var countPrePag = events.Count();
            events = events.Skip(filter.PerPage * filter.Page).Take(filter.PerPage).ToList();

            var eventsDTO = mapper.Map<List<EventDTO>>(events);
            return new EventListResponseDTO()
            {
                Events = eventsDTO,
                PagesAmount = (int)Math.Ceiling((float)countPrePag / (float)filter.PerPage)
            };
        }

        public async Task<ActionResult<EventDTO>> GetEvent(Guid id)
        {
            var eventEntity = await context.Events
                .Include(e => e.Speakers)
                .ThenInclude(e => e.Speaker)
                .Include(e => e.Sponsors)
                .ThenInclude(e => e.Sponsor)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventEntity == null) return NotFound();

            var dto = mapper.Map<EventDTO>(eventEntity);
            var httpClient = httpClientFactory.CreateClient();
            dto.IsImage = await dto.Banner.IsImageUrl(httpClient);
            return Ok(dto);
        }

        public async Task<ActionResult<EventDTO>> GetEventBySlug(string slug)
        {
            var eventEntity = await context.Events
                .Include(e => e.Speakers)
                .ThenInclude(e => e.Speaker)
                .Include(e => e.Sponsors)
                .ThenInclude(e => e.Sponsor)
                .FirstOrDefaultAsync(e => e.Slug == slug);

            if (eventEntity == null) return NotFound();
            var dto = mapper.Map<EventDTO>(eventEntity);

            Console.WriteLine(dto.DateUTC);
            Console.WriteLine(DateTime.UtcNow);

            var httpClient = httpClientFactory.CreateClient();
            dto.IsImage = await dto.Banner.IsImageUrl(httpClient);
            return Ok(dto);
        }

        public async Task<ActionResult> AddSpeakerToEvent(Guid eventId, EventAddSpeakerDTO dto)
        {
            var eventEntity = await context.Events.FindAsync(eventId);
            if (eventEntity == null) return NotFound();

            var speaker = new Speaker();
            mapper.Map(dto, speaker);

            var rel = new EventSpeaker()
            {
                Event = eventEntity,
                Speaker = speaker
            };

            await context.AddAsync(speaker);
            await context.AddAsync(rel);

            eventEntity.Speakers.Add(rel);
            await context.SaveChangesAsync();
            return Ok();
        }

        public async Task<ActionResult> AddSponsorToEvent(Guid eventId, EventAddSponsorDTO dto)
        {
            var eventEntity = await context.Events.FindAsync(eventId);
            if (eventEntity == null) return NotFound();
            var sponsor = new Sponsor();
            mapper.Map(dto, sponsor);

            var rel = new EventSponsor()
            {
                Event = eventEntity,
                Sponsor = sponsor
            };

            await context.AddAsync(sponsor);
            await context.AddAsync(rel);

            eventEntity.Sponsors.Add(rel);
            await context.SaveChangesAsync();
            return Ok();
        }

        public async Task<ActionResult> DeleteEvent(Guid eventId)
        {
            var eventEntity = await context.Events.FindAsync(eventId);
            if (eventEntity == null) return NotFound();
            context.Events.Remove(eventEntity);
            await context.SaveChangesAsync();
            return Ok();
        }

        public async Task<ActionResult> PutEvent(Guid id, EventPutDTO dto)
        {
            var eventEntity = await context.Events.FindAsync(id);
            if (eventEntity == null) return NotFound();
            mapper.Map(dto, eventEntity);
            await context.SaveChangesAsync();
            return Ok();
        }

        public async Task<ActionResult> RemoveSpeakerFromEvent(Guid id)
        {
            var speaker = await context.EventsSpeakers.FindAsync(id);
            if (speaker == null) return NotFound();
            context.Remove(speaker);
            await context.SaveChangesAsync();
            return Ok();
        }

        public async Task<ActionResult> RemoveSponsorFromEvent(Guid id)
        {
            var sponsor = await context.EventSponsors.FindAsync(id);
            if (sponsor == null) return NotFound();
            context.Remove(sponsor);
            await context.SaveChangesAsync();
            return Ok();
        }

        public async Task<ActionResult> AddUserToEvent(Guid eventId, RegisterUserToEventDTO dto)
        {
            var eventEntity = await context.Events.Include(e => e.Users).FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventEntity == null) return NotFound();
            var user = mapper.Map<UserForEvent>(dto);
            await context.AddAsync(user);
            eventEntity.Users.Add(user);
            await context.SaveChangesAsync();
            var mDTO = new TemplateDTO();            
            mDTO.TemplateId = int.Parse(configuration.GetValue<string>("MailUp:Templates:PostRegister"));
            mDTO.From = new EmailAddressDTO { Email = configuration.GetValue<string>("MailUp:SenderEmail"), Name = configuration.GetValue<string>("MailUp:SenderName") };
            mDTO.To = new List<EmailAddressDTO> { new EmailAddressDTO { Email = dto.Email, Name = dto.FullName } };
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
            await EmailServices.SendEmailWithVariables(mDTO);            
            return Ok();
        }

        public async Task<ActionResult> TurnOffEvent(Guid eventId)
        {
            var eventEntity = await context.Events.Include(e => e.Users).FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventEntity == null) return NotFound();
            eventEntity.Live = false;
            await context.SaveChangesAsync();
            var mDTO = new TemplateDTO();
            mDTO.To = new List<EmailAddressDTO>();
            eventEntity.Users.ForEach(user => mDTO.To.Add(new EmailAddressDTO { Email = user.Email, Name = user.FullName }));

            mDTO.TemplateId = int.Parse(configuration.GetValue<string>("MailUp:Templates:PostEvent"));
            mDTO.From = new EmailAddressDTO { Email = configuration.GetValue<string>("MailUp:SenderEmail"), Name = configuration.GetValue<string>("MailUp:SenderName") };

            mDTO.CharSet = "utf-8";
            List<NameValueDTO> vars = new List<NameValueDTO>();
            vars.Add(new NameValueDTO { N = "event_name", V = eventEntity.Title });
            vars.Add(new NameValueDTO { N = "stream_url", V = $"<a href='{configuration.GetValue<string>("ClientUrl")}/events/{eventEntity.Slug}' class='es-button' target='_blank' style='mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#FFFFFF;font-size:18px;display:inline-block;background:#31CB4B;border-radius:30px;font-family:arial, \'helvetica neue\', helvetica, sans-serif;font-weight:normal;font-style:normal;line-height:22px;width:auto;text-align:center;padding:10px 20px 10px 20px;mso-padding-alt:0;mso-border-alt:10px solid #31CB4B'>Ver Transmisión</a>" });
            mDTO.XSmtpAPI = new XSmtpAPIDTO();
            mDTO.XSmtpAPI = new XSmtpAPIDTO();
            mDTO.XSmtpAPI.DynamicFields = vars;            
            await EmailServices.SendEmailWithVariables(mDTO);

            return Ok();
        }

        public async Task<ActionResult<string>> GetUserFromEventReport(Guid eventId)
        {
            var eventEntity = await context.Events.Include(e => e.Users).FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventEntity == null) return NotFound();

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Usuarios");
            worksheet.AddColumns(new string[] {
                "Nombre",
                "Teléfono",
                "Email",
                "Compañía",
                "Fecha de registro",
                "Ciudad",
                "País"
            });

            for (var i = 0; i < eventEntity.Users.Count; i++)
            {
                var user = eventEntity.Users.ElementAt(i);
                worksheet.Cell(i + 2, 1).Value = user.FullName;
                worksheet.Cell(i + 2, 2).Value = user.Phone;
                worksheet.Cell(i + 2, 3).Value = user.Email;
                worksheet.Cell(i + 2, 4).Value = user.Company;
                worksheet.Cell(i + 2, 5).Value = user.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy");
                worksheet.Cell(i + 2, 6).Value = user.City;
                worksheet.Cell(i + 2, 7).Value = user.Country;
            }

            worksheet.ApplyTableStyle();
            return workbook.ToBase64();
        }
    }   
}
