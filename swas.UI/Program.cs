using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using swas.DAL;
using swas.BAL;
using DotNetEnv;
using swas.BAL.Interfaces;
using swas.Exceptions;
using swas.UI.Controllers;
using swas.BAL.Repository;
using swas.BAL.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BAL;
using swas.BAL.DTO;
using swas.DAL.Logger;
using swas.BAL.Helpers;
using swas.DAL.Models;
using swas.UI.Models;
using System;
using swas.UI.NewFolder;
using swas.UI.Middleware;

var builder = WebApplication.CreateBuilder(args);
string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings");

//var connectionString = builder.Configuration.GetConnectionString("DB");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();
builder.Services.AddScoped<AccountController, AccountController>();
builder.Services.AddScoped<HomeController, HomeController>();
builder.Services.AddScoped<CommentController, CommentController>();
builder.Services.AddScoped<ActionsController, ActionsController>();
builder.Services.AddScoped<StagesController, StagesController>();
builder.Services.AddScoped<AttHistoryController, AttHistoryController>();
builder.Services.AddScoped<ProjectsController, ProjectsController>();
builder.Services.AddScoped<StatusController, StatusController>();
builder.Services.AddScoped<DdlController, DdlController>();
builder.Services.AddScoped<ProjStakeHolderMovController, ProjStakeHolderMovController>();
builder.Services.AddScoped<StakeHolderController, StakeHolderController>();
builder.Services.AddScoped<UnitDtlsController, UnitDtlsController>();
builder.Services.AddScoped<IActionsRepository, ActionsRepository>();
builder.Services.AddScoped<IAttHistoryRepository, AttHistoryRepository>();
builder.Services.AddScoped<IChartService, ChartService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IDdlRepository, DdlRepository>();
builder.Services.AddScoped<ISoftwareTypeRepository, SoftwareTypeRepository>();
builder.Services.AddScoped<IProjectsRepository, ProjectsRepository>();
builder.Services.AddScoped<IProjStakeHolderMovRepository, ProjStakeHolderMovRepository>();
builder.Services.AddScoped<IProjStakeHolderCcMovRepository, ProjStakeHolderCcMovRepository>();
builder.Services.AddScoped<IStagesRepository, StagesRepository>();
builder.Services.AddScoped<IStakeHolderRepository, StakeHolderRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRankRepository, RankRepository>();
builder.Services.AddScoped<IAttHistComment, AttHistCommentRepository>();
builder.Services.AddScoped<IProjComments, ProjComments>();
builder.Services.AddScoped<IStkStatusRepository, StkStatusRepository>();
builder.Services.AddScoped<IStkCommentRepository, StkCommentRepository>();
builder.Services.AddScoped<IUnitStatusMapping, UnitStatusMapping>();
builder.Services.AddScoped<IStatusActionsMapping, StatusActionsMapping>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IUserMapChatRepository, UserMapChatRepository>();
builder.Services.AddScoped<ITrnChatMsgRepository, TrnChatMsgRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IActionExceptionRepository, ActionExceptionRepository>();
builder.Services.AddScoped<IDateApprovalRepository, DateApprovalRepository>();
builder.Services.AddScoped<IRemainder, RemainderRepository>();
builder.Services.AddScoped<ILegacyHistoryRepository, LegacyHistoryRepository>();
builder.Services.AddScoped<IWatermarkRepository, WatermarkRepository>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<PdfCertificateBuilder>();
builder.Services.AddScoped<LoginCryptoKeyService>();


// ===============================
// KESTREL MAX REQUEST SIZE - 100 MB
// ===============================
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
});

// ===============================
// ANTIFORGERY
// ===============================
builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});

// ===============================
// IDENTITY
// ===============================
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.AllowedForNewUsers = true;

        options.User.RequireUniqueEmail = false;

        options.SignIn.RequireConfirmedAccount = true;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromSeconds(30);
});

// ===============================
// FORM UPLOAD SIZE - 100 MB
// ===============================
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024;
});

// ===============================
// MVC + FILTERS
// ===============================
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    options.Filters.Add<SanitizeActionFilter>();
});

// ===============================
// RAZOR PAGES
// ===============================
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Identity/Account/Register");
    options.Conventions.ConfigureFilter(new AutoValidateAntiforgeryTokenAttribute());
});

// ===============================
// AUTHORIZATION POLICIES
// ===============================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Dte"));

    options.AddPolicy("Unit", policy =>
        policy.RequireRole("Unit"));

    options.AddPolicy("StakeHolders", policy =>
        policy.RequireRole("Unit", "Dte"));
});

// ===============================
// DATA PROTECTION / SESSION
// ===============================
builder.Services.AddDataProtection();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});

builder.Services.AddHttpContextAccessor();

// ===============================
// COOKIE POLICY
// ===============================
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // If true, cookies may not work unless consent is implemented.
    // For internal authenticated app, keep false.
    options.CheckConsentNeeded = context => false;
    options.Secure = CookieSecurePolicy.Always;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// ===============================
// CORS
// ===============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowArmyApp", policy =>
    {
        var allowedOrigins = new[]
        {
            "https://192.168.10.92",
            "https://dgisapp.army.mil:55102"
        };

        policy.SetIsOriginAllowed(origin =>
        {
            return allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
        });

        policy.WithMethods("GET", "POST", "HEAD", "OPTIONS");

        policy.WithHeaders(
            "Authorization",
            "Content-Type",
            "X-Requested-With",
            "RequestVerificationToken"
        );

        policy.AllowCredentials();

        policy.SetPreflightMaxAge(TimeSpan.FromMinutes(20));
    });
});

// ===============================
// SIGNALR
// ===============================
builder.Services.AddSignalR();

// ===============================
// SITE SETTINGS
// ===============================
builder.Services.Configure<SiteSettings>(
    builder.Configuration.GetSection("SiteSettings")
);

// ===============================
// HSTS
// ===============================
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

// ===============================
// LOGGING
// ===============================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Avoid BuildServiceProvider here if possible.
// If DbLoggerProvider is mandatory, better inject dependencies properly.
// builder.Logging.AddProvider(new DbLoggerProvider(...));

var app = builder.Build();

// ===============================
// BLOCK DANGEROUS METHODS + REMOVE HEADERS
// ===============================
app.Use(async (ctx, next) =>
{
    var blockedMethods = new[] { "TRACE", "TRACK", "CONNECT" };

    if (blockedMethods.Contains(ctx.Request.Method, StringComparer.OrdinalIgnoreCase))
    {
        app.Logger.LogWarning(
            "Security: Blocked {Method} request to {Path}",
            ctx.Request.Method,
            ctx.Request.Path
        );

        ctx.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
        ctx.Response.Headers["Allow"] = "GET, HEAD, POST, OPTIONS";
        await ctx.Response.WriteAsync("Method Not Allowed");
        return;
    }

    ctx.Response.OnStarting(() =>
    {
        ctx.Response.Headers.Remove("Server");
        ctx.Response.Headers.Remove("Expires");
        ctx.Response.Headers.Remove("X-Powered-By");
        ctx.Response.Headers.Remove("X-AspNet-Version");

        return Task.CompletedTask;
    });

    await next();
});

// ===============================
// ERROR / HSTS
// ===============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ===============================
// HTTPS / STATIC FILES
// ===============================
app.UseHttpsRedirection();

app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseStaticFiles();

app.UseCookiePolicy();

app.UseRouting();

app.UseCors("AllowArmyApp");

// IMPORTANT: Session before custom session middleware
app.UseSession();

// Add your custom session validation middleware here
app.UseMiddleware<SessionValidationMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

// ===============================
// ENDPOINTS
// ===============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapHub<ChatHub>("/chatHub");

app.Run();
