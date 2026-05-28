using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.Middlewares;
using HeThongQuanLyThuVien.Services;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Cau hinh Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        // Tên header HTTP sẽ gửi token: Authorization

        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        // Kiểu auth là HTTP (chuẩn JWT Bearer)

        Scheme = "bearer",
        // Scheme "bearer" nghĩa là dùng dạng:
        // Authorization: Bearer <token>

        BearerFormat = "JWT",
        // Chỉ để mô tả UI: token là JWT (không ảnh hưởng logic)

        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        // Token sẽ được gửi trong HTTP Header

        Description = "Nhap: Bearer {your token}"
        // Nội dung hướng dẫn hiển thị trong Swagger UI
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer" 
                    // Phải khớp với tên "Bearer" ở AddSecurityDefinition
                }
            },
            new string[] {}
            // Không yêu cầu scope (trống = áp dụng toàn bộ API)
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// DI
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMailService, MailService>();


builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<GlobalExceptionMiddleware>();


app.Run();
