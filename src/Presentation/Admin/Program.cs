using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using Travaloud.Admin.Components;
using Travaloud.Admin.Components.Account;
using Travaloud.Admin.Configurations;
using Travaloud.Application;
using Travaloud.Infrastructure;
using Travaloud.Infrastructure.Common;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.AddConfigurations();
    builder.Host.UseSerilog((_, config) =>
    {
        config.WriteTo.Console()
            .ReadFrom.Configuration(builder.Configuration);
    });

    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents().AddHubOptions(options =>
            {
                options.MaximumReceiveMessageSize = ApplicationConstants.MaxAllowedVideoSize;
            }).AddCircuitOptions(o =>
        {
            if (builder.Environment.IsDevelopment())
            {
                o.DetailedErrors = true;   
            }
        });
    
    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddScoped<IdentityUserAccessor>();
    builder.Services.AddScoped<IdentityRedirectManager>();
    builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
    
    builder.Services.AddInfrastructure(builder.Configuration, "Travaloud.Admin");
    builder.Services.AddApplication();
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    
    builder.Services.AddMudServices(configuration =>
    {
        configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
        configuration.SnackbarConfiguration.HideTransitionDuration = 100;
        configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
        configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
        configuration.SnackbarConfiguration.ShowCloseIcon = false;
    });

    var app = builder.Build();

    await app.Services.InitializeDatabasesAsync();

    app.UseInfrastructure(builder.Configuration, builder.Environment);

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.MapAdditionalIdentityEndpoints();
    app.UseStatusCodePagesWithRedirects("/error/{0}");
    
    Log.Information("Application running successfully");

    app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}