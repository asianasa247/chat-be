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
builder.Services.AddCors(x =>
    x.AddPolicy("AllowAll", builders => builders.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

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
// DANG KÝ CHO CHATBOX AI
builder.Services.AddScoped<IChatboxAIQAService, ChatboxAIQAService>();
builder.Services.AddScoped<IChatboxAIScheduledMessageService, ChatboxAIScheduledMessageService>();
//ALEPAY
builder.Services.AddScoped<IAlepayService, AlepayService>();
// DANG KÝ CHO CULTIVATION
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

// Jobs
builder.Services.AddScoped<ManageEmployee.Services.Chatbot.ZaloSchedulePollingJob>();
builder.Services.AddScoped<ManageEmployee.Services.Chatbot.ZaloTokenRefreshJob>(); // NEW: job refresh token định kỳ

// CHAT SUPPORT >>>
builder.Services.AddScoped<IChatSupportService, ChatSupportEfService>();
// CHAT SUPPORT <<<

var appSettingsSection = configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);
var appSettingsInvoice = configuration.GetSection("Invoice");
builder.Services.Configure<AppSettingInvoice>(appSettingsInvoice);
var appSettingsDatabase = configuration.GetSection("ConnectionStrings");
builder.Services.Configure<SettingDatabase>(appSettingsDatabase);
var appSettingsHanet = configuration.GetSection("Hanet");
builder.Services.Configure<AppSettingHanet>(appSettingsHanet);
var appSettingsVintacInvoice = configuration.GetSection("VintaxInvoice");
builder.Services.Configure<AppSettingVintaxInvoice>(appSettingsVintacInvoice);

// Adding Authentication
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

    // CHAT SUPPORT >>>
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
    // CHAT SUPPORT <<<
});

//builder.Services.AddHostedService<SendMailCronJob>();//datnguyen-dev: 24-01-2025

builder.Services.AddScoped<IEventRegistrationService, EventRegistrationService>();
builder.Services.AddHangfireServer();

builder.Services.Configure<ViettelPostOption>(configuration.GetSection("ViettelPost"));
builder.Services.AddHttpClient<IViettelPostService, ViettelPostService>((servicePRovider, httpClient) =>
{
    var option = servicePRovider.GetRequiredService<IOptions<ViettelPostOption>>().Value;
    httpClient.BaseAddress = new Uri(option.BaseUrl);
    httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
});

//builder.Services.AddControllers();
builder.Services.AddControllers(
                    option =>
                    {
                        //option.Filters.Add(typeof(OnExceptionFilter));
                        option.EnableEndpointRouting = false;
                    }
).AddNewtonsoftJson();

builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDirectoryBrowser();

// add signalR
builder.Services.AddSignalR(o => { o.EnableDetailedErrors = true; });
builder.WebHost.UseKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 1000;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddSwaggerGen(swagger =>
{
    swagger.OperationFilter<CustomHeaderSwaggerAttribute>();

    //This is to generate the Default UI of Swagger Documentation
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Isoft API",
        Description = "Assian API"
    });

    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
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
            new string[] { }
        }
    });

});

// Filter
builder.Services.AddControllersWithViews(options => { options.Filters.Add<ExceptionFilter>(); });

var app = builder.Build();

var loggerFactory = app.Services.GetService<ILoggerFactory>();
loggerFactory.AddFile(builder.Configuration["Logging:LogFilePath"].ToString());

app.DatabaseMigration(configuration);

// === Seed AlepayConfig ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var hasCfg = db.AlepayConfigs.Any();

    // Ưu tiên lấy từ appsettings khi có (dùng cho DEPLOY/PROD)
    var tokenFromCfg = configuration["Alepay:TokenKey"];
    var checksumFromCfg = configuration["Alepay:ChecksumKey"];
    var isSandboxFromCfg = configuration.GetValue<bool?>("Alepay:IsSandbox");

    if (!hasCfg && !string.IsNullOrWhiteSpace(tokenFromCfg) && !string.IsNullOrWhiteSpace(checksumFromCfg))
    {
        db.AlepayConfigs.Add(new AlepayConfig
        {
            IsSandbox = isSandboxFromCfg ?? false, // PROD thường = false
            TokenKey = tokenFromCfg,
            ChecksumKey = checksumFromCfg,
            CustomMerchantId = configuration["Alepay:CustomMerchantId"],
            AdditionWebId = null
        });
        db.SaveChanges();
    }

    // DEV fallback
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

app.UseFileServer();
app.UseRouting();
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials());

app.UseHangfireDashboard();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Uploads"
});

app.Use((context, next) =>
{
    context.Request.Headers.TryGetValue(Constants.YearFilterHeaderName, out var yearFilter);
    if (string.IsNullOrWhiteSpace(yearFilter))
    {
        context.Request.Headers[Constants.YearFilterHeaderName] = DateTime.UtcNow.Year.ToString();
    }
    return next(context);
});

//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

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

var configUrl = configuration["AppSettings:UrlWebSocket"];
List<string> domains = new();
domains.Add("http://localhost:4200");

if (!string.IsNullOrEmpty(configUrl))
{
    domains.Add(configUrl);
}

domains.ForEach(domain => { webSocketOptions.AllowedOrigins.Add(domain); });

app.UseWebSockets(webSocketOptions);

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
if (!app.Environment.IsDevelopment())
{
    app.MapHub<BroadcastHub>("/notify",
        options => { options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets; });
}

// CHAT SUPPORT >>>
app.MapHub<ChatSupportHub>("/hubs/chatsupport");
// CHAT SUPPORT <<<

app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    // (đã có)
    jobManager.AddOrUpdate<VitaxInvoiceGetterJob>(
        "VitaxInvoice_MiddleDays",
        job => job.RunGetInvoiceJobWrapper(),
        "0 2 5,10,15,20,25,28,29,30,31 * *"
    );

    // === ĐĂNG KÝ JOB GỬI TIN THEO BẢNG ChatboxAIScheduledMessage ===
    jobManager.AddOrUpdate<ManageEmployee.Services.Chatbot.ZaloSchedulePollingJob>(
        "Zalo_Schedule_Polling_Per_Minute",
        job => job.RunAsync(CancellationToken.None),
        "* * * * *" // chạy mỗi phút
                    // , timeZone: TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
    );

    // === NEW: Job làm mới token định kỳ (nếu đã cấu hình OAuth refresh) ===
    jobManager.AddOrUpdate<ManageEmployee.Services.Chatbot.ZaloTokenRefreshJob>(
        "Zalo_Token_Refresh_Every_30min",
        job => job.RunAsync(CancellationToken.None),
        "*/30 * * * *"
    );
});

app.MapControllers();
app.Run();