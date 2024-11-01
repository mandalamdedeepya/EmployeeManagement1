using EmployeeManagement;
using EmployeeManagement.Data;
using EmployeeManagement.Models;
using EmployeeManagement.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

string newKey = KeyGenerator.GenerateBase64Key();
Console.WriteLine($"Generated Base64 Key: {newKey}");

// Add services to the container.
builder.Services.AddDbContext<EmployeeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeesDB")));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeesDB")));
//builder.Services.AddDbContext<IdentityDbContext>(options =>
//    options.UseInMemoryDatabase("IdentityDb"));
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddEntityFrameworkStores<IdentityDbContext>()
//    .AddDefaultTokenProviders();

// Configure Entity Framework and Identity
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
// Ensure the JWT Key is present in the configuration
//var key = builder.Configuration["Jwt:Key"];
//if (string.IsNullOrEmpty(jwtKey))
//{
//    throw new InvalidOperationException("JWT key is not configured.");
//}

// Configure JWT Authentication
//builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

//var Key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        //IssuerSigningKey = new SymmetricSecurityKey(Key)
    };
});

// Add services for CORS and Caching
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
builder.Services.AddMemoryCache();
builder.Services.AddLazyCache();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();
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

app.UseRouting();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


