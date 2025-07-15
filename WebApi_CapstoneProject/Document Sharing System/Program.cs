using System.Text;
using DSS.Contexts;
using DSS.Interfaces;
using DSS.Intrefaces;
using DSS.Misc;
using DSS.Models;
using DSS.Repositories;
using DSS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Security.Claims;
using Azure.Storage.Blobs;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;



var builder = WebApplication.CreateBuilder(args);

// var AzureLogConnectionString = builder.Configuration["AzureStorage:LogsConnectionString"];

// var keyVaultUrl = builder.Configuration["AzureStorage:AccessUrl"];
// var SecretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

// KeyVaultSecret azureStorage = SecretClient.GetSecret("BlobConnectionString").Value;
// string blobConnectionString = azureStorage.Value;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// .WriteTo.AzureBlobStorage(
//             connectionString:blobConnectionString,
//             storageContainerName: "logs",
//             outputTemplate:"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
//         )


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Document  Sharing System", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddControllers(options =>
                {
                    options.Filters.Add<CustomExceptionFilter>();
                })
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    opts.JsonSerializerOptions.WriteIndented = true;
                });


// KeyVaultSecret dbSecret = SecretClient.GetSecret("DBConnectionString").Value;
// string dbConnectionString = dbSecret.Value;

// // Console.WriteLine(dbSecret.Value);
// // Console.WriteLine("Raw DB connection string:");
// // Console.WriteLine($"---START---\n{dbConnectionString}\n---END---");

// builder.Services.AddDbContext<DssContext>(opts =>
// {
//     opts.UseNpgsql(dbConnectionString);
// });

builder.Services.AddDbContext<DssContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var user = httpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? httpContext.Connection.RemoteIpAddress?.ToString() 
                     ?? "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromSeconds(10),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });

    options.RejectionStatusCode = 429;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtTokenKey"])),
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationhub"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });



#region Repositories
builder.Services.AddTransient<IRepository<Guid, User>, UserRepository>();
builder.Services.AddTransient<IRepository<Guid, AuthSession>, AuthSessionRepository>();
builder.Services.AddTransient<IRepository<Guid, UserDocument>, UserDocRepository>();
builder.Services.AddTransient<IRepository<Guid, DocumentView>, DocumentViewRepository>();
builder.Services.AddTransient<IRepository<Guid, DocumentShare>, DocumentShareRepository>();
#endregion

#region Services
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IBcryptionService, BcryptionService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IAuthSessionService, AuthSessionService>();
builder.Services.AddTransient<IUserDocService, UserDocService>();
builder.Services.AddTransient<IDocumentViewService, DocumentViewService>();
builder.Services.AddTransient<IDocumentShareService, DocumentShareService>();
builder.Services.AddTransient<IAzureBlobStorageService, AzureBlobStorageService>();

#endregion

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200","http://localhost:4300")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthorization();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapHub<NotificationHub>("/notificationhub");

app.UseRateLimiter();
app.MapControllers();

app.Run();

