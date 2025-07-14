using ChatappLC.Application.Interfaces.Admin;
using ChatappLC.Infrastructure.Services.Admin;
using ChatappLC.Infrastructure.ServicesPlugin;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────
// 1. Cấu hình MongoDB từ appsettings.json
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Đăng ký MongoDbContext
builder.Services.AddSingleton<MongoDbContext>();

// ✅ Đăng ký IMongoDatabase từ MongoDbContext
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var context = sp.GetRequiredService<MongoDbContext>();
    return context.Database;
});

// ─────────────────────────────────────────────
// 2. Đăng ký Dependency Injection (DI)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Đăng ký Chat services và repositories
builder.Services.InfrastructureServices(builder.Configuration);

// ─────────────────────────────────────────────
// 3. Cấu hình SignalR
builder.Services.AddSignalR();

// ─────────────────────────────────────────────
// 4. Cấu hình CORS (cho phép frontend truy cập)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ─────────────────────────────────────────────
// 5. Cấu hình JWT Authentication
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSection["SecretKey"];

if (string.IsNullOrEmpty(secretKey))
    throw new Exception("JWT SecretKey is missing in appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Cho phép truyền JWT qua query string cho SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chatHub") || (path.StartsWithSegments("/chatV2Hub"))  || path.StartsWithSegments("/videoCallHub")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ChatappLC Web API",
        Description = "Chat Application API with JWT Authentication"
    });

    // Enable authorization using Swagger (JWT)
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// ─────────────────────────────────────────────
// 6. Build app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatappLC API V1");
        c.RoutePrefix = string.Empty; // Makes Swagger available at root URL
    });
}

// ─────────────────────────────────────────────
// 7. Middleware pipeline
app.UseHttpsRedirection();

app.UseRouting();

// ❗ Đặt CORS trước Authentication
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Endpoint cho controller và SignalR hub
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.MapHub<ChatV2Hub>("/chatV2Hub");
app.MapHub<VideoCallHub>("/videoCallHub");

app.Run();