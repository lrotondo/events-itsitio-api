using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using events_api;
using events_api.Entities;
using events_api.Extensions;
using events_api.Services;
using events_api.Services.Interfaces;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Quartz.Impl;
using Quartz;
using events_api.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient();

builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IEventsServices, EventsServices>();
builder.Services.AddScoped<IEmailServices, EmailServices>();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();
    var jobKey = new JobKey("EmailCron");
    q.AddJob<EmailCron>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("EmailCron-trigger")
        .WithCronSchedule("0 0 5 ? * * *"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

string connectionString = Environment.GetEnvironmentVariable("EVENTS_DB_CONNECTION");

if (connectionString == null) throw new Exception("EVENTS_DB_CONNECTION environment variable not set");

builder.Services.AddDbContextPool<ApplicationDbContext>(options => {

    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

/*builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionString.BuildPostgresConnectionString());
    });
*/

string jwtKey = Environment.GetEnvironmentVariable("EVENTS_JWT_KEY");
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero
            });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User = new UserOptions
    {
        RequireUniqueEmail = true
    };
}).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

    var fileXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var routeXML = Path.Combine(AppContext.BaseDirectory, fileXML);
    c.IncludeXmlComments(routeXML, includeControllerXmlComments: true);
});

builder.Services.AddCors(c => c.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EVENTS API");
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();


