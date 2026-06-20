using HeThongQuanLyThuVien.Data;
using HeThongQuanLyThuVien.Middlewares;
using HeThongQuanLyThuVien.Options;
using HeThongQuanLyThuVien.Services;
using HeThongQuanLyThuVien.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();

#region Cau hinh logic nghiep vu

builder.Services.Configure<LibrarySettings>(builder.Configuration.GetSection("LibrarySettings"));

#endregion

#region Cau hinh Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        // Tên header HTTP sẽ gửi token: Authorization

        Type = SecuritySchemeType.Http,
        // Kiểu auth là HTTP (chuẩn JWT Bearer)

        Scheme = "bearer",
        // Scheme "bearer" nghĩa là dùng dạng:
        // Authorization: Bearer <token>

        BearerFormat = "JWT",
        // Chỉ để mô tả UI: token là JWT (không ảnh hưởng logic)

        In = ParameterLocation.Header,
        // Token sẽ được gửi trong HTTP Header

        Description = "Nhap: Bearer {your token}"
        // Nội dung hướng dẫn hiển thị trong Swagger UI
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" 
                    // Phải khớp với tên "Bearer" ở AddSecurityDefinition
                }
            },
            new string[] {}
            // Không yêu cầu scope (trống = áp dụng toàn bộ API)
        }
    });
});

#endregion

#region Cau hinh SqlServer
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Cau hinh authentication voi JwtBearer
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

    options.Events = new JwtBearerEvents
    {
        // Token khong hop le / het han 401
        OnChallenge = async context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = 401;

            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                statusCode = 401,
                error = "Unauthorized",
                message = "Token khong hop le hoac da het han",
                path = context.Request.Path,
                timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        },

        OnForbidden = async context =>
        {
            context.Response.StatusCode = 403;

            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                statusCode = 403,
                error = "Forbidden",
                message = "Ban khong co quyen truy cap nguon tai nguyen nay",
                path = context.Request.Path,
                timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    };
});
#endregion

#region Cau hinh Authorization Service
builder.Services.AddAuthorization();
#endregion

#region Dependencies Injection
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookCopyService, BookCopyService>();
builder.Services.AddScoped<IBorrowRecordService, BorrowRecordService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFineService, FineService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<GlobalExceptionMiddleware>();
#endregion

#region Cau hinh HttpContextAccessor
builder.Services.AddHttpContextAccessor();
#endregion


builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseCors("ReactPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();



app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
