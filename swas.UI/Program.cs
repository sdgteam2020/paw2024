using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using swas.DAL;
using swas.BAL;

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

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DB");
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



builder.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);




builder.Services.AddIdentity<ASPNetCoreIdentityCustomFields.Data.ApplicationUser, IdentityRole>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.User.RequireUniqueEmail = false;
    options.SignIn.RequireConfirmedAccount = true ;
    options.SignIn.RequireConfirmedEmail = false;
    options.Lockout.AllowedForNewUsers = true;

})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024;
});


builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Identity/Account/Register");
    options.Conventions.ConfigureFilter(new ValidateAntiForgeryTokenAttribute());

});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.ConfigureFilter(
        new AutoValidateAntiforgeryTokenAttribute());
});




builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin",
        builder => builder.RequireRole("Dte", "Dte"));
    options.AddPolicy("Unit",
       builder => builder.RequireRole("Unit", "Unit"));
    options.AddPolicy("StakeHolders",
      builder => builder.RequireRole("Unit", "Dte"));
});


builder.Services.AddControllersWithViews();

builder.Services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromSeconds(30));

builder.Services.AddDataProtection();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

});


builder.Services.Configure<CookiePolicyOptions>(options =>
{


    options.CheckConsentNeeded = context => true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowArmyApp",
        builder =>
        {
            builder.WithOrigins("https://192.168.10.92", "https://dgisapp.army.mil:55102")
                   .WithHeaders("Content-Type", "RequestVerificationToken") // allow your custom header
                   .WithMethods("GET", "POST", "OPTIONS");
        });
});

builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<SanitizeActionFilter>(); 
});


builder.Logging.ClearProviders();
builder.Logging.AddProvider(new DbLoggerProvider(builder.Services.BuildServiceProvider()));
builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection("SiteSettings"));
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
});

var app = builder.Build();
app.Use(async (ctx, next) =>
{
    string styleHashes =
        "'sha256-47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=' " +
        "'sha256-nmbYH9QL932nGzG6pP3juQVw3fieOoiq7lDMU409Uyk=' " +
        "'sha256-Kfztztcv/X/MB3GPFWC63/lmUeORVtnXht+HcmqoIKA=' " +
        "'sha256-OPXav01Qif81Tq84iQ+sHdcLqfFebewGp3RzUL8vy00=' " +
        "'sha256-7oERheaqPgauHfP5d4xw0v6p4MUYc+/Quwioe/4rjOI='";

    bool isDev = app.Environment.IsDevelopment();
    string connectSrc = isDev
        ? "connect-src 'self' https: wss:; "
        : "connect-src 'self'; ";

    string csp =
        "default-src 'self'; " +
        "script-src 'self'; " +
        $"style-src 'self' 'unsafe-hashes' {styleHashes}; " +
        "img-src 'self' data: blob:; " +
        "font-src 'self' data:; " +
        "frame-src 'self' blob:; " +
        "object-src 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self'; " +
        "frame-ancestors 'none'; " +
        connectSrc;

    ctx.Response.Headers["Content-Security-Policy"] = csp;

    ctx.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["Referrer-Policy"] = "no-referrer";

    await next();
});


app.UseCookiePolicy(
new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always,
    HttpOnly = HttpOnlyPolicy.Always,
    MinimumSameSitePolicy = SameSiteMode.None

});


if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHsts();
app.UseHttpsRedirection();  
app.UseStaticFiles();

app.UseCors("AllowArmyApp");
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapRazorPages();
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chatHub");
});

app.MapRazorPages();
app.Run();

