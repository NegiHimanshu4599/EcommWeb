using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Interface.Background;
using NotificationService.Application.Interface.Provider;
using NotificationService.Application.Interface.Services;
using NotificationService.Application.Mapping;
using NotificationService.Application.Services;
using NotificationService.Application.Services.Provider;
using NotificationService.Domain.Configuration;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.BackgroundServices;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("constr");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(cs, sql => sql.UseCompatibilityLevel(120)));
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<NotificationService.Application.Interface.Provider.IEmailSender, EmailSender>();
builder.Services.AddScoped<INotificationService, NotificationService.Application.Services.NotificationService>();
builder.Services.AddScoped<IPushNotificationSender, PushNotificationSender>();
builder.Services.AddScoped<IDeviceTokenService, DeviceTokenService>();
builder.Services.AddScoped<INotificationLogService, NotificationLogService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<ISmsSender, SmsSender>();
builder.Services.AddAutoMapper(typeof(MappingProfile)); 
builder.Services.Configure<OtpSettings>(builder.Configuration.GetSection("OtpSettings"));
builder.Services.AddHostedService<ExpiredOtpCleanupService>(); 
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IEmailTemplateRenderer, EmailTemplateRenderer>();
builder.Services.AddScoped<IVoiceCallSender, VoiceCallSender>();
builder.Services.AddSingleton<INotificationQueue, NotificationQueue>();
builder.Services.AddHostedService<NotificationBackgroundService>();

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
