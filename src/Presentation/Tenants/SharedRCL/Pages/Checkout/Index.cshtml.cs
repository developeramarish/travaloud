using Microsoft.Extensions.Options;
using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Cloudbeds;
using Travaloud.Application.Common.Extensions;
using Travaloud.Application.PaymentProcessing;
using Travaloud.Shared.Authorization;

namespace Travaloud.Tenants.SharedRCL.Pages.Checkout;

public class IndexModel : TravaloudBasePageModel
{
    private readonly ICloudbedsService _cloudbedsService;
    private readonly StripeSettings _stripeSettings;
    public string StripePublicKey { get; set; }

    public IndexModel(ICloudbedsService cloudbedsService, IOptions<StripeSettings> stripeOptions)
    {
        _cloudbedsService = cloudbedsService;
        _stripeSettings = stripeOptions.Value;
        StripePublicKey = _stripeSettings.ApiPublishKey;
    }

    public BasketModel? Basket { get; set; }

    [BindProperty] public CheckoutFormComponent CheckoutFormComponentModel { get; set; } = new();

    public ApplicationUser? AuthenticatedUser { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? bookingId)
    {
        await OnGetDataAsync();

        if (bookingId.HasValue)
            await BookingService.DeleteAsync(bookingId.Value);
        
        Basket = await BasketService.GetBasket();

        var url = Request.GetEncodedUrl();

        LoginModal.ReturnUrl = url;
        RegisterModal.ReturnUrl = url;

        var estimatedArrivalTimeRequired = Basket.Items.Any(x => x.PropertyId.HasValue);
        CheckoutFormComponentModel.EstimatedArrivalTimeRequired = estimatedArrivalTimeRequired;
        
        if (!CurrentUser.IsAuthenticated() || !UserId.HasValue) return Page();

        AuthenticatedUser = await UserManager.FindByIdAsync(UserId.ToString()!);
        
        if (AuthenticatedUser != null)
        {
            CheckoutFormComponentModel.Email = AuthenticatedUser.Email;
            CheckoutFormComponentModel.FirstName = AuthenticatedUser.FirstName;
            CheckoutFormComponentModel.Surname = AuthenticatedUser.LastName;
            CheckoutFormComponentModel.Nationality = AuthenticatedUser.Nationality;
            CheckoutFormComponentModel.DateOfBirth = AuthenticatedUser.DateOfBirth;
            CheckoutFormComponentModel.Gender = AuthenticatedUser.Gender;
            CheckoutFormComponentModel.PhoneNumber = AuthenticatedUser.PhoneNumber;
        }
        else
        {
            CheckoutFormComponentModel.Email = Basket.Email;
            CheckoutFormComponentModel.FirstName = Basket.FirstName;
            CheckoutFormComponentModel.Surname = Basket.Surname;
            CheckoutFormComponentModel.Nationality = Basket.Nationality;
            CheckoutFormComponentModel.DateOfBirth = Basket.DateOfBirth;
            CheckoutFormComponentModel.Gender = Basket.Gender;
            CheckoutFormComponentModel.PhoneNumber = Basket.PhoneNumber;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostProceedToPayment()
    {
        var basket = await BasketService.SetPrimaryContactInformation(
            CheckoutFormComponentModel.FirstName,
            CheckoutFormComponentModel.Surname,
            CheckoutFormComponentModel.Email,
            CheckoutFormComponentModel.DateOfBirth,
            CheckoutFormComponentModel.PhoneNumber,
            CheckoutFormComponentModel.Nationality,
            CheckoutFormComponentModel.Gender,
            CheckoutFormComponentModel.EstimatedArrivalTime,
            CheckoutFormComponentModel.Password,
            CheckoutFormComponentModel.ConfirmPassword
        );

        var errorMessages = new List<string>();

        // We need to check if availability has changed since the session was created 
        basket = await errorMessages.CheckCloudbedsReservations(basket, TenantWebsiteService, _cloudbedsService, BasketService);

        // No concurrency errors, continue to payment
        if (errorMessages.Count == 0)
        {
            var guestId = UserId;

            if (!guestId.HasValue)
            {
                var existingUser = await UserManager.FindByEmailAsync(basket.Email);

                if (existingUser == null)
                {
                    var user = new ApplicationUser
                    {
                        FirstName = basket.FirstName,
                        LastName = basket.Surname,
                        PhoneNumber = basket.PhoneNumber,
                        Gender = basket.Gender,
                        Nationality = basket.Nationality,
                        DateOfBirth = basket.DateOfBirth,
                        UserName = basket.Email,
                        Email = basket.Email,
                        SignUpDate = DateTime.Now,
                        IsActive = true,
                        EmailConfirmed = true,
                        RefreshTokenExpiryTime = DateTime.Now
                    };

                    var result = await UserManager.CreateAsync(user, basket.Password ?? "P@55w0rd");

                    if (result.Succeeded)
                    {
                        guestId = Guid.Parse(user.Id);
                        var userResult = await UserManager.AddToRoleAsync(user, TravaloudRoles.Guest);

                        if (userResult.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(basket.Password))
                                await SignInManager.SignInAsync(user, isPersistent: false);
                        }
                    }
                }
                else
                {
                    guestId = Guid.Parse(existingUser.Id);
                    await SignInManager.SignInAsync(existingUser, isPersistent: false);
                }
            }

            if (guestId.HasValue)
                HttpContextAccessor.HttpContext?.Session.SetString("GuestId", guestId.Value.ToString());
            
            return LocalRedirect($"/payment");
        }

        foreach (var errorMessage in errorMessages)
        {
            ModelState.AddModelError(Guid.NewGuid().ToString(), errorMessage);
        }

        return Page();
    }
}