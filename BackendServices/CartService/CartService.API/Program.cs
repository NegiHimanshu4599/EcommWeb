using CartService.API.Middleware;
using CartService.Application.Interfaces;
using CartService.Application.Mappings;
using CartService.Application.Services;
using CartService.Domain.Interfaces;
using CartService.Infrastructure.Data;
using CartService.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//if (!builder.Environment.IsDevelopment())
//{
//    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
//    builder.WebHost.UseUrls($"http://*:{port}");
//}
var conStr = builder.Configuration.GetConnectionString("cs") ?? throw new InvalidOperationException("Connection String 'cs' is not Found");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(conStr));
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IUnitofWork, UnitofWork>();
builder.Services.AddScoped<ICartService, CartServices>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHttpContextAccessor();
var bookServiceUrl = builder.Configuration["ServiceUrls:BookService"];
builder.Services.AddHttpClient("BookService", c =>
{
    c.BaseAddress = new Uri(bookServiceUrl!);
}).AddTransientHttpErrorPolicy(policy =>
policy.WaitAndRetryAsync(3, retry =>
TimeSpan.FromSeconds(2)));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
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
            new List<string>()
        }
    });
}); 
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromHours(2);
});
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseGlobalExceptionHandler();
app.UseMiddleware<CorrelationMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
