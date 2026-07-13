using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NotificationService.Application.Interface.Background;
using NotificationService.Application.Interface.Provider;
//using NotificationService.Application.Mapping;
using NotificationService.Domain.Configuration;
using NotificationService.Domain.Interfaces;
//using NotificationService.Infrastructure.BackgroundServices;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.DependencyInjection;
using NotificationService.Infrastructure.Provider;
using NotificationService.Infrastructure.Queue;
using NotificationService.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("constr");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(cs, sql => sql.UseCompatibilityLevel(120)));
builder.Services.AddControllers();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IPushNotificationSender, PushNotificationSender>();
//builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.Configure<OtpSettings>(builder.Configuration.GetSection("OtpSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<FirebaseSettings>(builder.Configuration.GetSection(FirebaseSettings.SectionName));
builder.Services.AddFirebase(builder.Configuration);
builder.Services.AddSingleton<INotificationQueue, NotificationQueue>();
//builder.Services.AddHostedService<NotificationBackgroundService>();
builder.Services.AddOptions<EmailSettings>().Bind(builder.Configuration.GetSection("EmailSettings")).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<OtpSettings>().Bind(builder.Configuration.GetSection("OtpSettings")).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<FirebaseSettings>().Bind(builder.Configuration.GetSection("FirebaseSettings")).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<SmsSettings>().Bind(builder.Configuration.GetSection(SmsSettings.SectionName)).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<VoiceCallSettings>().Bind(builder.Configuration.GetSection(VoiceCallSettings.SectionName)).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddHttpClient<ISmsSender, SmsSender>((provider, client) =>
{
    var settings = provider.GetRequiredService<IOptions<SmsSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.RequestTimeoutSeconds);
}).AddStandardResilienceHandler();
builder.Services.AddHttpClient<IVoiceCallSender, VoiceCallSender>((provider, client) =>
{
    var settings = provider.GetRequiredService<IOptions<VoiceCallSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.RequestTimeoutSeconds);
}).AddStandardResilienceHandler();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();