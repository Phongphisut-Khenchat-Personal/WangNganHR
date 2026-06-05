using WangNganHR.API.Data;
using WangNganHR.API.Helpers;
using WangNganHR.API.Services;
using WangNganHR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

// แก้ปัญหา DateTime กับ PostgreSQL (ต้องตั้งก่อนใช้ Npgsql)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration
        .GetConnectionString("DefaultConnection")));

// ── JWT Auth ──────────────────────────────────────────
var jwtKey = builder.Configuration["JwtSettings:SecretKey"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience            = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ── Services ──────────────────────────────────────────
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<IAuthService,        AuthService>();
builder.Services.AddScoped<IJobPostingService,  JobPostingService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IInterviewService,   InterviewService>();

// ── CORS (สำหรับ Blazor + WPF) ────────────────────────
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p =>
        p.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader()));

// ── Swagger ───────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title   = "Wang Ngan HR API",
        Version = "v1"
    });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
    });
    opt.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath  = "/uploads"
});

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    // Idempotent repair if StructuredJobPosting migration was recorded without DDL
    await db.Database.ExecuteSqlRawAsync("""
        ALTER TABLE "JobPostings" ADD COLUMN IF NOT EXISTS "Benefits" text NOT NULL DEFAULT '[]';
        ALTER TABLE "JobPostings" ADD COLUMN IF NOT EXISTS "Qualifications" text NOT NULL DEFAULT '[]';
        ALTER TABLE "JobPostings" ADD COLUMN IF NOT EXISTS "Responsibilities" text NOT NULL DEFAULT '[]';
        ALTER TABLE "JobPostings" ADD COLUMN IF NOT EXISTS "WorkHours" character varying(200);
        ALTER TABLE "JobPostings" ADD COLUMN IF NOT EXISTS "WorkLocation" character varying(300);
        ALTER TABLE "Applications" ADD COLUMN IF NOT EXISTS "ReferralSource" character varying(100);
        ALTER TABLE "Applications" ADD COLUMN IF NOT EXISTS "PdpaConsentedAt" timestamp without time zone;
        """);

    await DbSeeder.SeedAsync(db);
    await DbSeeder.RepairAsync(db);
}

app.Run();