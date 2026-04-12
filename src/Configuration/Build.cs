using api_camem.src.Filters;
using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Repository;
using api_camem.src.Services;
using api_camem.src.Shared.Templates;
using CloudinaryDotNet;

namespace api_camem.src.Configuration
{
    public static class Build
    {
        public static void AddBuilderConfiguration(this WebApplicationBuilder builder)
        {
            AppDbContext.ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "";
            AppDbContext.DatabaseName     = Environment.GetEnvironmentVariable("DATABASE_NAME")     ?? "";
            AppDbContext.IsSSL = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IS_SSL"))
                && Convert.ToBoolean(Environment.GetEnvironmentVariable("IS_SSL"));
        }

        public static void AddContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<AppDbContext>();
        }

        public static void AddBuilderServices(this WebApplicationBuilder builder)
        {
            // AUTH
            builder.Services.AddTransient<IAuthService, AuthService>();

            // MASTER DATA
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IProfileUserService, ProfileUserService>();
            builder.Services.AddTransient<IProfileUserRepository, ProfileUserRepository>();

            // EVENT
            builder.Services.AddTransient<IEventService, EventService>();
            builder.Services.AddTransient<IEventRepository, EventRepository>();
            builder.Services.AddTransient<IEventParticipantService, EventParticipantService>();
            builder.Services.AddTransient<IEventParticipantRepository, EventParticipantRepository>();
            
            // CERTIFICATE
            builder.Services.AddTransient<ICertificateService, CertificateService>();
            builder.Services.AddTransient<ICertificateRepository, CertificateRepository>();

            // SETTINGS
            builder.Services.AddScoped<ILoggerService, LoggerService>();
            builder.Services.AddScoped<ILoggerRepository, LoggerRepository>();

            builder.Services.AddTransient<ITemplateService, TemplateService>();
            builder.Services.AddTransient<ITemplateRepository, TemplateRepository>();
            builder.Services.AddTransient<ITriggerService, TriggerService>();
            builder.Services.AddTransient<ITriggerRepository, TriggerRepository>();

            // REALTIME
            builder.Services.AddTransient<INotificationService, NotificationService>();
            builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
            builder.Services.AddTransient<IChatService, ChatService>();
            builder.Services.AddTransient<IChatRepository, ChatRepository>();

            // HANDLER
            builder.Services.AddTransient<SmsHandler>();
            builder.Services.AddTransient<MailHandler>();
            builder.Services.AddTransient<UploadHandler>();
            builder.Services.AddTransient<CountHandler>();

            // TEMPLATE
            builder.Services.AddTransient<MailTemplate>();

            // AUTOMAPPER
            builder.Services.AddAutoMapper(cfg => { }, typeof(Program));

            // CLOUDINARY
            Account account = new(
                Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME"),
                Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
                Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
            );
            builder.Services.AddSingleton(new Cloudinary(account));

            // SIGNALR SignalR
            builder.Services.AddSignalR();

            // LOGGER ACTION FILTRO
            builder.Services.AddScoped<LoggerActionFilter>();
        }
    }
}