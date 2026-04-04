using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookService.Application.Interfaces;
using BookService.Application.Mappings;
using BookService.Application.Services;
using BookService.Domain.Interfaces;
using BookService.Infrastructure.Data;
using BookService.Infrastructure.Repository;

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
builder.Services.AddScoped<IBookService, BookServices>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICoverTypeService, CoverTypeService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;

    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
});
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
