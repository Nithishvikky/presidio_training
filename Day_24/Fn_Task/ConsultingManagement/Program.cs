using ConsultingManagement.Contexts;
using Microsoft.EntityFrameworkCore;
using ConsultingManagement.Interfaces;
using ConsultingManagement.Models;
using ConsultingManagement.Misc;
using ConsultingManagement.Repositories;
using ConsultingManagement.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ConsultingManagement.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Google;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "DoctorConsultancy API", Version = "v1" });
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
builder.Services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    opts.JsonSerializerOptions.WriteIndented = true;
                });

builder.Services.AddDbContext<ConsultancyContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#region Repositories
builder.Services.AddTransient<IRepository<int, Doctor>, DoctorRepository>();
builder.Services.AddTransient<IRepository<int, Patient>, PatientRepository>();
builder.Services.AddTransient<IRepository<int, Speciality>, SpecialityRepository>();
builder.Services.AddTransient<IRepository<string, Appointment>, AppointmentRepository>();
builder.Services.AddTransient<IRepository<int, DoctorSpeciality>, DoctorSpecialityRepository>();
builder.Services.AddTransient<IRepository<string, User>, UserRepository>();
#endregion

#region Services
builder.Services.AddTransient<IDoctorService, DoctorService>();
builder.Services.AddTransient<IOtherContextFunctionities, OtherFuncinalitiesImplementation>();
builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IAuthenticationServiceCustom, AuthenticationService>();
builder.Services.AddTransient<IPatientService, PatientService>();
builder.Services.AddTransient<IAppointmentService, AppointmentService>();
#endregion

#region AuthenticationFilter
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie("AppCookie", options =>
    {
        options.Cookie.Name = "AppCookie";
        options.Cookie.SameSite = SameSiteMode.Lax;  
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; 
    })
    .AddCookie("ExternalCookies", options =>
    {
        options.Cookie.Name = "ExternalCookies";
        options.Cookie.SameSite = SameSiteMode.Lax;  
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtTokenKey"]))
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.CallbackPath = "/signin-google";

        options.SignInScheme = "ExternalCookies";
    });
#endregion

#region Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AppointmentCancellation", policy => policy.Requirements.Add(new MinimumExperienceRequirement(3)));
});
builder.Services.AddTransient<IAuthorizationHandler, ExperienceHandler>();
#endregion

#region  Misc
builder.Services.AddAutoMapper(typeof(User));
builder.Services.AddScoped<CustomExceptionFilter>();
builder.Logging.AddLog4Net();
#endregion

#region CORS
builder.Services.AddCors(options=>{
    options.AddDefaultPolicy(policy=>{
        policy.WithOrigins("http://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
#endregion

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // This enables the UI
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();
app.MapHub<NotificationHub>("/notificationhub");

app.MapControllers();

app.Run();

