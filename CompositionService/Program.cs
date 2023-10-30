using Application.Abstractions;
using Application.Services;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Domain.Abstractions;
using Infrastructure.ExternalClients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using static CurentaCommonCore.Enums.EnumsCollection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// Add services to the container.

//CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

//Swagger
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo { Title = "Curenta Composition Service WEB API", Version = "v1" });
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer YOUR_TOKEN')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //s.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
});

//JWT
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
string jwtSecret;
if (env == "prod")
{
    SecretClient keyVaultClient = new SecretClient(new Uri("https://authsecret.vault.azure.net/"), new DefaultAzureCredential());
    jwtSecret = keyVaultClient.GetSecret("JwtSecret").Value.Value;
}
else
{
    jwtSecret = "3017a3642ab78d238ef5ab3ac18a629650888e966fa4a85ba533fb4fe2957754c2779451726bdbf690d2df5f81148c4a8824c83031b4f34afd365c3962cafccf";
}
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = "curenta-services",
        ValidIssuer = "curenta",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

//DI - services and repositories
builder.Services.AddScoped<IPatientClient, PatientClient>();
builder.Services.AddScoped<ITaskClient, TaskClient>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IPatientMedicationService, PatientMedicationService>();

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.Configure<HttpClientFactoryOptions>(options =>
{
    options.HttpClientActions.Add(item => item.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Generate2MinutesServerToServerAccessToken()));
});

string? Generate2MinutesServerToServerAccessToken()
{
    var accessTokenValidityMinutes = 2;

    var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, "usama.atteya@servertoken.com"),
                            new Claim(ClaimTypes.Name, "usama.atteya@servertoken.com"),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };
    var allRoles = Enum.GetNames(typeof(UserRoles));
    foreach (var userRole in allRoles)
        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
    var token = new JwtSecurityToken(
        issuer: "curenta",
        audience: "curenta-services",
        expires: DateTime.Now.AddMinutes(accessTokenValidityMinutes),
        claims: authClaims,
        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Curenta TasK Manager WEB API");
});
//}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
