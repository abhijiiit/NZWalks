using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NZWalks;
using NZWalks.API.Mappings;
using NZWalks.Data;
using NZWalks.Middlewares;
using NZWalks.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
var logger = new LoggerConfiguration()    
    .WriteTo.Console()
    .WriteTo.File("Logs/NzWalks_Logs.txt", rollingInterval : RollingInterval.Minute)
    .MinimumLevel.Information()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers();  // this line is added by me to make the controller work
builder.Services.AddHttpContextAccessor(); // by me for the image path n all

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>            // AddSwaggerGen() --> already added by .NET , Inside we provide options so that authorization and add aithorization in the header, without this we have to use the postman
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NZ Walks API", Version = "v1"});
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme                    
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });  
}); 

builder.Services.AddDbContext<NZClassDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksConnectionString"))); // by me for database to work (injected)

builder.Services.AddDbContext<NZWalksAuthDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksAuthConnectionString"))); // by me for auth database to work(injected) (auth part)

builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();  // by me for working of RegionRepository i have created
builder.Services.AddScoped<IwalkRepository, SQLWalkRepository>(); // by me for working of WalkRepository (this we have to do everytime if we create repository)
builder.Services.AddScoped<ITokenRepository, TokenRepository>();  // by me for working of token repo
builder.Services.AddScoped<IImageRepository, LocalImageRepository>(); // by me for working of Image repo

builder.Services.AddAutoMapper(cfg => { },typeof(AutoMapperProfiles)); // automapper version require that "cfg=>{}" thing.

builder.Services.AddIdentityCore<IdentityUser>()     // added identity solution (auth part)
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZWalks")
    .AddEntityFrameworkStores<NZWalksAuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>                         // for configing the password we use this (auth part)
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;     
    options.Password.RequireNonAlphanumeric = false;  
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});
// this whole lines of code for authoriztion  (auth part)

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  
    .AddJwtBearer(options => 
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    }
    );

var app = builder.Build();

// Configure the HTTP request pipeline. ----> not by me

// ********ORDER ALSO MATTER HERE (BELOW) --> bcoz this is a pipeline and it operate top to bottom*******//

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection(); 

app.UseAuthentication(); // added for Authentication (auth part) //(this is a middelware) // jisme app.Something h wo ek middleware h

app.UseAuthorization(); // added for authorization  (auth part) // this is a middlware

app.UseStaticFiles(new StaticFileOptions  // added for image path link to work on website if we pasted the link.
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
});

app.MapControllers();  // this line is added my me to make the controller work

app.Run();  // anything after this line will not be executed because the app is running and waiting for requests

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast")    
// .WithOpenApi();


 

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
