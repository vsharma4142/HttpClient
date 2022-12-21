using ETL.Data.Models;
using ETL.JWT;
using ETL.SalesForce;
using ETL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;


var builder = WebApplication.CreateBuilder(args);

var keyValue = builder.Configuration["ExternalClientServer:RsaPrivateKey"].ToByteArray();
using RSA rsa = RSA.Create();
rsa.ImportRSAPrivateKey(keyValue, out _);
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["ExternalClientServer:Issuer"],
        ValidAudience = builder.Configuration["ExternalClientServer:Audience"],
        IssuerSigningKey = new RsaSecurityKey(rsa),
        //IssuerSigningKey = new SymmetricSecurityKey
        //    (Encoding.UTF8.GetBytes(builder.Configuration["ExternalClientServer:RsaPrivateKey"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
var securityReq = new OpenApiSecurityRequirement()
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
        new string[] {}
    }
};
builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityRequirement(securityReq);
});
builder.Services.Configure<ExternalClientJsonConfiguration>(builder.Configuration.GetSection("ExternalClientServer"));
builder.Services.Configure<SalesforceClinetConfiguration>(builder.Configuration.GetSection("Salesforce"));
builder.Services.AddDbContext<ETLDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnStr")));
builder.Services.TryAddScoped<IAccountService, AccountService>();
builder.Services.TryAddScoped<ICustomerService, CustomerService>();
builder.Services.TryAddScoped<ISalesForceHandler, SalesForceHandler>();
builder.Services.AddTransient<IJwtHandler, JwtHandler>();
builder.Services.AddControllers();

builder.Services.AddMvcCore();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(options =>
    {
        options.AllowAnyHeader();
        options.AllowAnyMethod();
        options.AllowAnyOrigin();
    });
});
var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization(); 
app.MapControllers();
app.Run();