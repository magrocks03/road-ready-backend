using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RoadReadyAPI.Contexts;
using RoadReadyAPI.Data;
using RoadReadyAPI.Interfaces;
using RoadReadyAPI.Mappers;
using RoadReadyAPI.Repositories;
using RoadReadyAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Logger Configuration
// 1. Add log4net configuration
builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");
#endregion

#region Controller and Swagger Configuration
// 2. Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 3. Configure Swagger to use JWT Authentication
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "RoadReady API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    });
});
#endregion

#region JWT Authentication Configuration
// 4. Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:JWT"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Tokens:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Tokens:Audience"]
        };
    });
#endregion

#region Database Context Configuration
// 5. Configure DbContext
builder.Services.AddDbContext<RoadReadyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Repository Registration
// 6. Register Repositories for Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IBookingStatusRepository, BookingStatusRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IIssueRepository, IssueRepository>();
builder.Services.AddScoped<IRefundRepository, RefundRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IExtraRepository, ExtraRepository>();
builder.Services.AddScoped<IBookingExtraRepository, BookingExtraRepository>();
#endregion

#region Service Registration
// 7. Register Services for Dependency Injection
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IIssueService, IssueService>();
builder.Services.AddScoped<IAdminService, AdminService>();
// --- NEW SERVICE ADDED HERE ---
builder.Services.AddScoped<IOperationsService, OperationsService>();
builder.Services.AddScoped<IHelperService, HelperService>();
#endregion

#region AutoMapper Registration
// 8. Register AutoMapper
builder.Services.AddAutoMapper(
    typeof(UserMappingProfile),
    typeof(VehicleMappingProfile),
    typeof(ProfileMappingProfile),
    typeof(BookingMappingProfile),
    typeof(ReviewMappingProfile),
    typeof(IssueMappingProfile),
    typeof(AdminMappingProfile),
    typeof(OperationsMappingProfile),// --- NEW MAPPER ADDED HERE ---
    typeof(HelperMappingProfile)
);
#endregion

#region CORS Configuration
// 9. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
#endregion

var app = builder.Build();

#region Database Seeder Execution
// This section will run the database seeder at startup.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbSeeder.SeedAllAsync(services);
    }
    catch (System.Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database seeding.");
    }
}
#endregion

#region HTTP Request Pipeline Configuration
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use the configured CORS policy
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion