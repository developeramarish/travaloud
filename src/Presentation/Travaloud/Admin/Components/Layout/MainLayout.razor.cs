using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Travaloud.Admin.Theme;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Multitenancy;
using Color = MudBlazor.Color;

namespace Travaloud.Admin.Components.Layout;

public partial class MainLayout
{
    [Inject] private IUserService UserService { get; set; } = default!;

    private string? _currentUrl;
    private bool _isDarkMode;
    private MudTheme CurrentTheme;
    private bool _drawerOpen = true;
    private TravaloudTenantInfo? TenantInfo;
    private List<UserDetailsDto>? Guests;
    
    protected override void OnInitialized()
    {
        if (HttpContextAccessor.HttpContext == null) return;

        TenantInfo = MultiTenantContextAccessor.MultiTenantContext?.TenantInfo;
        CurrentTheme = new LightTheme(TenantInfo);
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return base.OnAfterRenderAsync(firstRender);
        
        LoadingService.OnLoaderVisibilityChanged += SetModalLoading;
        InteractiveSubmittingService.OnBusySubmittingChanged += SetBusySubmitting;
        TenantInfo = MultiTenantContextAccessor.MultiTenantContext?.TenantInfo;
        StateHasChanged();

        return base.OnAfterRenderAsync(firstRender);
    }

    private Task SetModalLoading()
    {
        StateHasChanged();
        return Task.CompletedTask;
    }
    
    private async Task SetBusySubmitting()
    {
        await Task.Delay(1);
        StateHasChanged();
    }
    
    private void GetCurrentUrl(string url)
    {
        _currentUrl = url;
    }
    
    public void ToggleDarkMode(bool toggled)
    {
        _isDarkMode = toggled;
        CurrentTheme = _isDarkMode ? new DarkTheme() : new LightTheme(TenantInfo);
    }

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }
    
    private void Profile()
    {
        NavigationManager.NavigateTo("/account");
    }
    
    private void Logout()
    {
        var parameters = new DialogParameters
        {
            { nameof(Dialogs.Logout.ContentText), $"{L["Logout Confirmation"]}"},
            { nameof(Dialogs.Logout.ButtonText), $"{L["Logout"]}"},
            { nameof(Dialogs.Logout.Color), Color.Error}
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        DialogService.Show<Dialogs.Logout>(L["Logout"], parameters, options);
    }
}