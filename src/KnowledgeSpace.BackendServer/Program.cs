using FluentValidation.AspNetCore;
using KnowledgeSpace.BackendServer.Data;
using KnowledgeSpace.BackendServer.Data.Entities;
using KnowledgeSpace.BackendServer.Extensions;
using KnowledgeSpace.BackendServer.IdentityServer;
using KnowledgeSpace.BackendServer.Services;
using KnowledgeSpace.ViewModels.Other;
using KnowledgeSpace.ViewModels.Validator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Host.UseSerilog((ctx, lc) => lc
    //.MinimumLevel.Information()
    //.Enrich.FromLogContext()
    //.WriteTo.Console()
    //.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, shared: true)
    .ReadFrom.Configuration(builder.Configuration));
builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
builder.WebHost.UseIISIntegration();

string KspSpecificOrigins = "KspSpecificOrigins";

builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


//https://deblokt.com/2019/09/23/04-part-1-identityserver4-asp-net-core-identity/
// small app => save IdentityResources ,api ,client use InMemory
// large app => save to database
builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
}).AddInMemoryApiResources(Config.Apis)
  .AddInMemoryApiScopes(Config.ApiScopes)
  .AddInMemoryClients(builder.Configuration.GetSection("IdentityServer:Clients"))
  //.AddInMemoryClients(Config.Clients)
  .AddInMemoryIdentityResources(Config.Ids)
  .AddAspNetIdentity<User>()
  .AddProfileService<IdentityProfileService>()  //add more data for client
  .AddDeveloperSigningCredential();
var origins = builder.Configuration.GetSection("AllowOrigins").Value;
builder.Services.AddCors(options =>
{
    options.AddPolicy(KspSpecificOrigins,
    builder =>
    {
        builder.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddAuthentication()
               .AddLocalApi("Bearer", option =>
               {
                   option.ExpectedScope = "api.knowledgespace";
               });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Bearer", policy =>
    {
        policy.AddAuthenticationSchemes("Bearer");
        policy.RequireAuthenticatedUser();
    });
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddAreaFolderRouteModelConvention("Identity", "/Account/", model =>
    {
        foreach (var selector in model.Selectors)
        {
            var attributeRouteModel = selector.AttributeRouteModel;
            attributeRouteModel.Order = -1;
            attributeRouteModel.Template = attributeRouteModel.Template.Remove(0, "Identity".Length);
        }
    });
});

builder.Services.AddMvcCore()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RoleCreateRequestValidator>())
    .AddApiExplorer();

builder.Services.AddTransient<DbInitializer>();
builder.Services.AddTransient<IEmailSender, EmailSenderService>();
builder.Services.AddTransient<ISequenceService, SequenceService>();
builder.Services.AddTransient<IStorageService, FileStorageService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IViewRenderService, ViewRenderService>();
builder.Services.AddTransient<ICacheService, DistributedCacheService>();
builder.Services.AddTransient<IOneSignalService, OneSignalService>();

if (environment == Environments.Development || environment == "UAT" || environment == "uat")
{
    builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
}
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Knowledge Space API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
               // AuthorizationUrl = new Uri(builder.Configuration["AuthorityUrl"] + "/connect/authorize"),
                AuthorizationUrl = new Uri("https://localhost:7062/connect/authorize"),
                Scopes = new Dictionary<string, string> { 
                    {"api.knowledgespace", "KnowledgeSpace API" },
                    {"api.swagger","KnowledgeSpace Swagger" }
                }
            },
        },
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>{ "api.knowledgespace", "api.swagger" }
                    }
                });

});
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.ToLower() == "uat")
{
    app.UseDeveloperExceptionPage();
}
app.UseErrorWrapping();
app.UseStaticFiles();
app.UseIdentityServer();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseRouting();
app.UseAuthorization();
app.UseCors(KspSpecificOrigins);

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapRazorPages();
});

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.OAuthClientId("swagger");
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Knowledge Space API V1");
});


using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    try
    {
        Log.Information("Seeding data...");
        var dbInitializer = services.GetService<DbInitializer>();
        await dbInitializer.Seed();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
