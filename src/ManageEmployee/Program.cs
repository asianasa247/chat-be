using Hangfire;
using ManageEmployee.Constants;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataLayer.Service.ViettelPostServices;
using ManageEmployee.Extends;
using ManageEmployee.Filters;
using ManageEmployee.Helpers;
using ManageEmployee.Hubs;
using ManageEmployee.JobSchedules;
using ManageEmployee.Models;
using ManageEmployee.Services;
using ManageEmployee.Services.ChatboxAI;
using ManageEmployee.Services.CompanyServices;
using ManageEmployee.Services.Configurations;
using ManageEmployee.Services.ConvertToProductServices;
using ManageEmployee.Services.Interfaces;
using ManageEmployee.Services.Interfaces.ChatboxAI;
using ManageEmployee.Services.Interfaces.ConvertToProduct;
using ManageEmployee.Services.Interfaces.Cultivation;
using ManageEmployee.Services.Interfaces.ListCustomers;
using ManageEmployee.Services.Interfaces.NewHotels;
using ManageEmployee.Services.Interfaces.Orders;
using ManageEmployee.Services.ListCustomerServices;
using ManageEmployee.Services.NewHotelServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using ManageEmployee.Services.Interfaces.PaymentServices;
using ManageEmployee.Services.PaymentServices;
using ManageEmployee.Entities.PaymentEntities;

// CHAT SUPPORT >>>
using ManageEmployee.Services.Interfaces.ChatSupport;
using ManageEmployee.Services.ChatSupport;
using ManageEmployee.Services.CustomerServices;
using ManageEmployee.Services.Interfaces.Customers;
// CHAT SUPPORT <<<

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();

// Custom application.json by env
IWebHostEnvironment env = builder.Environment;
Console.WriteLine($"Get configuration from application.{env.EnvironmentName}.json");

builder.Configuration.SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
builder.Services.Configure<GmailApiConfig>(builder.Configuration.GetSection("GmailAPI"));
builder.Services.Configure<WinInvoiceConfig>(builder.Configuration.GetSection("WinInvoice"));
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IAesEncryptionHelper, AesEncryptionHelper>();
builder.Services.AddScoped<IScheduledPostService, ScheduledPostService>();
builder.Services.AddScoped<IFaceAccessService, FaceAccessService>();
builder.Services.AddHostedService<FacebookSchedulerBackgroundService>();
builder.Services.AddTransient<GeminiService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var apiKey = config["Gemini:ApiKey"];
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    return new GeminiService(apiKey, httpClient);
});
builder.Services.AddTransient<IConnectionStringProvider, ConnectionStringProvider>();
builder.Services.AddDbContext<ApplicationDbContext>(
    (serviceProvider, dbContextBuilder) =>
    {
        var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
        var connectionString = connectionStringProvider.GetConnectionString();
        connectionStringProvider.SetupDbContextOptionsBuilder(dbContextBuilder, connectionString);
    });

builder.Services.AddHangfire((serviceProvider, hangfireConfiguration) =>
{
    var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
    var connectionString = connectionStringProvider.GetConnectionString();
    hangfireConfiguration.UseSqlServerStorage(connectionString);
});

// [CHANGED] Bật Hangfire Server để các recurring job thực sự chạy
builder.Services.AddHangfireServer();

builder.Services.AddCors(x =>
    x.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IEventCustomerService, EventCustomerService>();

// DI Services
builder.Services.AddScoped<IOrderSuccessfulService, OrderSuccessfulServicce>();
builder.Services.RegisterServiceInjection();
builder.Services.AddScoped<IPositionMinhService, PositionMinhService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IFloorService, FloorService>();
builder.Services.AddScoped<IFacebookMessengerService, FacebookMessengerService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IListCustomerService, ListCustomerService>();
builder.Services.AddScoped<IConvertProductService, ConvertProductService>();
builder.Services.AddScoped<VitaxInvoiceGetterJob>();

// CHATBOX AI
builder.Services.AddScoped<IChatboxAIQAService, ChatboxAIQAService>();
builder.Services.AddScoped<IChatboxAIScheduledMessageService, ChatboxAIScheduledMessageService>();

// ALEPAY
builder.Services.AddScoped<IAlepayService, AlepayService>();

// CULTIVATION
builder.Services.AddScoped<IPlantingTypeService, ManageEmployee.Services.Cultivation.PlantingTypeService>();
builder.Services.AddScoped<IPlantingRegionService, ManageEmployee.Services.Cultivation.PlantingRegionService>();
builder.Services.AddScoped<IPlantingBedService, ManageEmployee.Services.Cultivation.PlantingBedService>();

// === Zalo Chatbot DI ===
builder.Services.AddHttpClient();
builder.Services.AddScoped<ManageEmployee.Services.Interfaces.Chatbot.ITokenStore, ManageEmployee.Services.Chatbot.FileTokenStore>();
builder.Services.AddScoped<ManageEmployee.Services.Interfaces.Chatbot.ISubscribersStore, ManageEmployee.Services.Chatbot.FileSubscribersStore>();
builder.Services.AddScoped<ManageEmployee.Services.Interfaces.Chatbot.IZaloApiService, ManageEmployee.Services.Chatbot.ZaloApiService>();
builder.Services.AddScoped<ManageEmployee.Services.Interfaces.Chatbot.ICompanyInfoService, ManageEmployee.Services.Chatbot.CompanyInfoService>();
builder.Services.AddScoped<ManageEmployee.Services.Interfaces.Chatbot.IGeminiNlpService, ManageEmployee.Services.Chatbot.GeminiNlpService>();
builder.Services.AddScoped<ManageEmployee.Services.Interfaces.Chatbot.IZaloChatbotService, ManageEmployee.Services.Chatbot.ZaloChatbotService>();
// (bổ sung) đăng ký concrete để job lấy được qua GetRequiredService<ZaloApiService>()
builder.Services.AddScoped<ManageEmployee.Services.Chatbot.ZaloApiService>();

// Jobs
builder.Services.AddScoped<ManageEmployee.Services.Chatbot.ZaloSchedulePollingJob>();
builder.Services.AddScoped<ManageEmployee.Services.Chatbot.ZaloTokenRefreshJob>();

// CHAT SUPPORT
builder.Services.AddScoped<IChatSupportService, ChatSupportEfService>();

builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
builder.Services.Configure<AppSettingInvoice>(configuration.GetSection("Invoice"));
builder.Services.Configure<SettingDatabase>(configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<AppSettingHanet>(configuration.GetSection("Hanet"));
builder.Services.Configure<AppSettingVintaxInvoice>(configuration.GetSection("VintaxInvoice"));

// AuthN/Z
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };

    // SignalR JWT via query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chatsupport"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Controllers (bật endpoint routing)
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDirectoryBrowser();

// SignalR
builder.Services.AddSignalR(o => { o.EnableDetailedErrors = true; });

builder.WebHost.UseKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 1000;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
});

// Swagger
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.OperationFilter<CustomHeaderSwaggerAttribute>();
    swagger.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "Isoft API", Description = "Assian API" });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Filters
builder.Services.AddControllersWithViews(options => { options.Filters.Add<ExceptionFilter>(); });

var app = builder.Build();

// Logger to file
var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddFile(builder.Configuration["Logging:LogFilePath"].ToString());

// Auto-migrate DB
app.DatabaseMigration(configuration);

// === Seed AlepayConfig ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var hasCfg = db.AlepayConfigs.Any();

    var tokenFromCfg = configuration["Alepay:TokenKey"];
    var checksumFromCfg = configuration["Alepay:ChecksumKey"];
    var isSandboxFromCfg = configuration.GetValue<bool?>("Alepay:IsSandbox");

    if (!hasCfg && !string.IsNullOrWhiteSpace(tokenFromCfg) && !string.IsNullOrWhiteSpace(checksumFromCfg))
    {
        db.AlepayConfigs.Add(new AlepayConfig
        {
            IsSandbox = isSandboxFromCfg ?? false,
            TokenKey = tokenFromCfg,
            ChecksumKey = checksumFromCfg,
            CustomMerchantId = configuration["Alepay:CustomMerchantId"],
            AdditionWebId = null
        });
        db.SaveChanges();
    }

    if (!db.AlepayConfigs.Any() && app.Environment.IsDevelopment())
    {
        db.AlepayConfigs.Add(new AlepayConfig
        {
            IsSandbox = true,
            TokenKey = "0COVspcyOZRNrsMsbHTdt8zesP9m0y",
            ChecksumKey = "hjuEmsbcohOwgJLCmJlf7N2pPFU1Le",
            CustomMerchantId = null,
            AdditionWebId = null
        });
        db.SaveChanges();
    }
}

// ===== Middleware order =====
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseFileServer();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Uploads"
});

app.UseRouting();
app.UseCors("AllowAll");

app.UseHangfireDashboard(); // Dashboard
app.UseAuthentication();
app.UseAuthorization();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// WebSockets
var webSocketOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromMinutes(2) };
var configUrl = configuration["AppSettings:UrlWebSocket"];
List<string> domains = new() { "http://localhost:4200" };
if (!string.IsNullOrEmpty(configUrl)) domains.Add(configUrl);
domains.ForEach(domain => { webSocketOptions.AllowedOrigins.Add(domain); });

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
    else
    {
        await next();
    }
});

app.UseWebSockets(webSocketOptions);

// Header năm mặc định
app.Use((context, next) =>
{
    context.Request.Headers.TryGetValue(Constants.YearFilterHeaderName, out var yearFilter);
    if (string.IsNullOrWhiteSpace(yearFilter))
    {
        context.Request.Headers[Constants.YearFilterHeaderName] = DateTime.UtcNow.Year.ToString();
    }
    return next(context);
});

// Hubs
if (!app.Environment.IsDevelopment())
{
    app.MapHub<BroadcastHub>("/notify",
        options => { options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets; });
}
app.MapHub<ChatSupportHub>("/hubs/chatsupport");

// Hangfire jobs
app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    jobManager.AddOrUpdate<VitaxInvoiceGetterJob>(
        "VitaxInvoice_MiddleDays",
        job => job.RunGetInvoiceJobWrapper(),
        "0 2 5,10,15,20,25,28,29,30,31 * *"
    );

    jobManager.AddOrUpdate<ManageEmployee.Services.Chatbot.ZaloSchedulePollingJob>(
        "Zalo_Schedule_Polling_Per_Minute",
        job => job.RunAsync(CancellationToken.None),
        "* * * * *"
    );

    jobManager.AddOrUpdate<ManageEmployee.Services.Chatbot.ZaloTokenRefreshJob>(
        "Zalo_Token_Refresh_Every_30min",
        job => job.RunAsync(CancellationToken.None),
        "*/30 * * * *"
    );
});

app.MapControllers();
app.Run();
