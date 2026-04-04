using CartService.Domain.Interfaces;
using CartService.Infrastructure.Data;
using CartService.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
//builder.Services.AddScoped<IBookService, BookServices>();
//builder.Services.AddScoped<ICategoryService, CategoryService>();
//builder.Services.AddScoped<ICoverTypeService, CoverTypeService>();
//builder.Services.AddScoped<IFileService, FileService>();
//builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromHours(2);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
