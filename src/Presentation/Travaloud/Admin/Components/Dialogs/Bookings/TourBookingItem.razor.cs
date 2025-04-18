﻿using System.Text.Json;
using Blazored.FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Org.BouncyCastle.Ocsp;
using Travaloud.Admin.Components.EntityTable;
using Travaloud.Admin.Components.Pages.Bookings;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.TourDates.Queries;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Application.Common.Exceptions;
using Travaloud.Application.Identity.Users;
using Travaloud.Infrastructure.Common.Services;
using Travaloud.Shared.Authorization;

namespace Travaloud.Admin.Components.Dialogs.Bookings;

public partial class TourBookingItem : ComponentBase
{
    [Inject] protected IToursService ToursService { get; set; } = default!;

    [Inject] protected ITenantWebsiteService TenantWebsiteService { get; set; } = default!;
    
    [Inject] protected ITourCategoriesService TourCategoriesService { get; set; } = default!;

    [Inject] protected ITourDatesService TourDatesService { get; set; } = default!;

    [Inject] protected IUserService UserService { get; set; } = default!;

    [Parameter] [EditorRequired] public UpdateBookingItemRequest RequestModel { get; set; } = default!;

    [Parameter] public TourBookingViewModel TourBooking { get; set; } = default!;

    [Parameter] public EntityServerTableContext<BookingDto, DefaultIdType, TourBookingViewModel> Context { get; set; } = default!;

    [Parameter] public object? Id { get; set; }
    
    [Parameter] public ICollection<TourCategoryDto> TourCategories { get; set; } = default!;
    
    [Parameter] public bool CanDelete { get; set; }
    
    [Parameter] public bool BookingIsCancelled { get; set; }
    
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = default!;
    
    private ICollection<TourDto>? Tours { get; set; }
    
    public ICollection<TourDateDto>? TourDates { get; set; }
    
    public IEnumerable<TourPriceDto>? TourPrices { get; set; }
    
    public IEnumerable<TourPickupLocationDto>? TourPickupLocations { get; set; }
    
    public EditForm EditForm { get; set; } = default!;

    public bool IsCreate => Id is null;

    private FluentValidationValidator? _fluentValidationValidator;

    private EditContext? EditContext { get; set; }
    
    private bool IsWaiverRequired { get; set; }
    private string? WaiverTermsAndConditions { get; set; }

    protected Func<DefaultIdType?, string> TourDateToStringConverter;
    protected Func<DefaultIdType?, string> TourToStringConverter;
    protected Func<DefaultIdType?, string> TourCategoryToStringConverter;
    protected Func<DefaultIdType?, string> TourPriceToStringConverter;

    private bool FormDisabled { get; set; }
    private bool BookingRefunded { get; set; }
    private bool UserIsAdmin { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState.GetAuthenticationStateAsync();
        
        TourDateToStringConverter = GenerateTourDateDisplayString;
        TourToStringConverter = GenerateTourDisplayString;
        TourCategoryToStringConverter = GenerateTourCategoryDisplayString;
        TourPriceToStringConverter = GenerateTourPriceDisplayString;
        BookingRefunded = (TourBooking.Refunded.HasValue && TourBooking.Refunded.Value);
        UserIsAdmin = authState.User.IsInRole(TravaloudRoles.Admin);
        FormDisabled = BookingIsCancelled || ((TourBooking.IsPaid || BookingRefunded) && !UserIsAdmin);
        EditContext = new EditContext(RequestModel);

        if (RequestModel is {TourId: not null, TourCategoryId: null})
        {
            var tour = await ToursService.GetAsync(RequestModel.TourId.Value);
            RequestModel.TourCategoryId = tour.TourCategoryId;
        }
        
        if (RequestModel is {TourId: not null, TourCategoryId: not null})
        {
            if (RequestModel.Guests != null && RequestModel.Guests.Any())
            {
                var userIds = RequestModel.Guests.Select(x => x.GuestId).ToList();
                var guests = await UserService.SearchAsync(userIds, CancellationToken.None);
                
                foreach (var guest in RequestModel.Guests)
                {
                    var matchedGuest = guests.FirstOrDefault(x => x.Id == DefaultIdType.Parse(guest.GuestId));

                    if (matchedGuest == null) continue;
                    guest.FirstName = matchedGuest.FirstName;
                    guest.LastName = matchedGuest.LastName;
                    guest.EmailAddress = matchedGuest.Email;
                }
            }
            
            var tourDatesRequest = Task.Run(() => TourDatesService.SearchAsync(new SearchTourDatesRequest()
            {
                TourId = RequestModel.TourId.Value,
                RequestedSpaces = 1,
                UserIsAdmin = UserIsAdmin,
                PriceId = RequestModel.TourPriceId
            }));
            
            var tourPickupLocationsRequest = Task.Run(() => ToursService.GetTourPickupLocations(RequestModel.TourId.Value));

            var filteredToursRequest = Task.Run(() => ToursService.SearchAsync(new SearchToursRequest()
            {
                TourCategoryId = RequestModel.TourCategoryId.Value,
                PageNumber = 1,
                PageSize = 99999
            }));
            
            var tourPricesRequest = Task.Run(() => TenantWebsiteService.GetTourPrices(new GetTourPricesRequest(new[] {RequestModel.TourId.Value}), CancellationToken.None));
            
            await Task.WhenAll(tourDatesRequest, tourPickupLocationsRequest, filteredToursRequest, tourPricesRequest);

            Tours = filteredToursRequest.Result?.Data;
            
            var tourDates = tourDatesRequest.Result;
            
            TourDates = tourDates.Data.Where(x => !UserIsAdmin ? x.StartDate > DateTime.Now : x.StartDate > DateTime.Now.AddMonths(-1)).ToList();
            TourPrices = tourPricesRequest.Result;
            TourPickupLocations = tourPickupLocationsRequest.Result;

            if (TourPickupLocations != null)
            {
                var tourPickupLocations = TourPickupLocations.Select(x => x.PropertyName);

                if (!tourPickupLocations.Contains(RequestModel.PickupLocation))
                {
                    RequestModel.OtherPickupLocation = RequestModel.PickupLocation;
                    RequestModel.PickupLocation = "Other";
                }
            }
            
            var tourDate = tourDates.Data.FirstOrDefault(x => x.Id == RequestModel.TourDateId);
            RequestModel.TourPriceId = tourDate?.TourPriceId;

            TourDates = TourDates.Where(x => x.TourPriceId == RequestModel.TourPriceId).ToList();

            var tour = Tours?.FirstOrDefault(x => x.Id == RequestModel.TourId.Value);

            if (tour != null)
            {
                IsWaiverRequired = tour.WaiverRequired.HasValue && tour.WaiverRequired.Value;
                WaiverTermsAndConditions = tour.TermsAndConditions;
            }
        }

        await base.OnInitializedAsync();
    }

    private string GenerateTourDateDisplayString(DefaultIdType? tourDateId)
    {
        var tourDate = TourDates?.FirstOrDefault(u => u.Id == tourDateId);

        return tourDate != null
            ? $"{tourDate.StartDate} - {tourDate.AvailableSpaces} spaces available"
            : string.Empty;
    }

    private string GenerateTourDisplayString(DefaultIdType? tourId)
    {
        var tour = Tours?.FirstOrDefault(u => u.Id == tourId);

        return tour?.Name ?? string.Empty;
    }
    
    private string GenerateTourCategoryDisplayString(DefaultIdType? tourId)
    {
        var tourCategory = TourCategories.FirstOrDefault(u => u.Id == tourId);

        return tourCategory?.Name ?? string.Empty;
    }
    
    private string GenerateTourPriceDisplayString(DefaultIdType? tourPriceId)
    {
        var tourPrice = TourPrices?.FirstOrDefault(u => u.Id == tourPriceId);

        if (tourPrice == null) return string.Empty;
        var details = new List<string>
        {
            $"$ {tourPrice.Price:n2}"
        };
        
        if (!string.IsNullOrEmpty(tourPrice.Title))
        {
            details.Add(tourPrice.Title);
        }
        
        if (!string.IsNullOrEmpty(tourPrice.Description))
        {
            details.Add(tourPrice.Description);
        }

        return string.Join(" - ", details);

    }


    private void Cancel() =>
        MudDialog.Cancel();

    private async Task SaveAsync()
    {
        await LoadingService.ToggleLoaderVisibility(true);

        if (await _fluentValidationValidator!.ValidateAsync())
        {
            if (await ServiceHelper.ExecuteCallGuardedAsync(
                    () =>
                    {
                        if (!IsCreate) return Task.CompletedTask;
                        TourBooking.Items ??= new List<UpdateBookingItemRequest>();

                        if (TourBooking.Items.Count > 0)
                        {
                            var lastItem = TourBooking.Items.Last();
                            if (lastItem?.TourId == null) return Task.CompletedTask;

                            TourBooking.Items.Add(RequestModel);
                            TourBooking.ItemQuantity++;

                            if (RequestModel.Amount.HasValue)
                            {
                                TourBooking.TotalAmount += RequestModel.Amount.Value *
                                                           (RequestModel.Guests != null && RequestModel.Guests.Any()
                                                               ? RequestModel.Guests.Count
                                                               : 1);
                            }
                        }
                        else
                        {
                            TourBooking.Items.Add(RequestModel);
                            TourBooking.ItemQuantity++;

                            if (RequestModel.Amount.HasValue)
                            {
                                TourBooking.TotalAmount += RequestModel.Amount.Value *
                                                           (RequestModel.Guests != null && RequestModel.Guests.Any()
                                                               ? RequestModel.Guests.Count
                                                               : 1);
                            }
                        }

                        return Task.CompletedTask;
                    },
                    Snackbar,
                    Logger,
                    $"Booking Tour {(IsCreate ? L["added"] : L["Updated"])}."))
            {
                MudDialog.Close(RequestModel);
            }
        }
        else
        {
            Snackbar.Add("One or more validation errors occurred.");
        }

        await LoadingService.ToggleLoaderVisibility(false);
    }

    private async Task OnTourCategoryValueChanged(DefaultIdType? tourCategoryId)
    {
        if (tourCategoryId.HasValue)
        {
            RequestModel.TourCategoryId = tourCategoryId.Value;
            
            var filteredTours = await ToursService.SearchAsync(new SearchToursRequest()
            {
                TourCategoryId = tourCategoryId,
                PageNumber = 1,
                PageSize = 99999
            });

            if (filteredTours?.Data != null) Tours = filteredTours.Data;

            RequestModel.TourId = null;
            RequestModel.TourDateId = null;
            RequestModel.TourPriceId = null;
            RequestModel.PickupLocation = null;
            RequestModel.TourDateId = null;
            TourDates = new List<TourDateDto>();
            TourPickupLocations = Array.Empty<TourPickupLocationDto>();
            
            StateHasChanged();
        }
    }

    private async Task OnTourValueChanged(DefaultIdType? tourId)
    {
        RequestModel.WaiverSigned = false;
        RequestModel.TourDateId = null;
        RequestModel.PickupLocation = null;
        RequestModel.TourPriceId = null;
        
        if (tourId.HasValue)
        {
            var tour = Tours.FirstOrDefault(x => x.Id == tourId);
            
            if (tour == null)
                throw new NotFoundException("Tour not found.");
            
            var tourPickupLocationsRequest = Task.Run(() => ToursService.GetTourPickupLocations(tourId.Value));
            var tourPricesReqest = Task.Run(() => TenantWebsiteService.GetTourPrices(new GetTourPricesRequest(new[] {tourId.Value}), CancellationToken.None));

            await Task.WhenAll(tourPickupLocationsRequest, tourPricesReqest);
            
            TourPickupLocations = tourPickupLocationsRequest.Result;
            TourPrices = tourPricesReqest.Result;
            IsWaiverRequired = tour.WaiverRequired.HasValue && tour.WaiverRequired.Value;
            WaiverTermsAndConditions = tour.TermsAndConditions;
            
            RequestModel.TourId = tourId.Value;
            StateHasChanged();
        }
    }
    
    private async Task OnTourPriceValueChanged(DefaultIdType? tourPriceId)
    {
        RequestModel.WaiverSigned = false;
        
        if (tourPriceId.HasValue)
        {
            RequestModel.TourPriceId = tourPriceId.Value;
            
            var tourDates = await TourDatesService.SearchAsync(new SearchTourDatesRequest()
            {
                PriceId = tourPriceId.Value,
                RequestedSpaces = 1,
                StartDate = DateTime.Now.AddMonths(6),
                UserIsAdmin = UserIsAdmin
            });

            TourDates = tourDates.Data.Where(x => !UserIsAdmin ? x.StartDate > DateTime.Now : x.StartDate > DateTime.Now.AddMonths(-1)).ToList();
                 
            RequestModel.TourDateId = null;

            if (tourDates.Data.Count == 0)
            {
                RequestModel.TourDateId = null;
            }
            else if (TourBooking.Items?.Any() == true)
            {
                var tourDateIds = TourBooking.Items.Select(x => x.TourDateId);
    
                TourDates = TourDates.Where(x => !tourDateIds.Contains(x.Id)).ToList();
            }
            
            // if (TourDates.Count > 0 && UserIsAdmin)
            // {
            //     var closestDate = TourDates.MinBy(x => Math.Abs((x.StartDate - DateTime.Now).TotalDays));
            //
            //     RequestModel.TourDateId = closestDate?.Id;
            // }
            
            StateHasChanged();
        }
    }

    private void OnTourDateValueChanged(DefaultIdType? tourDateId)
    {
        if (!tourDateId.HasValue) return;

        var tourDate = TourDates.FirstOrDefault(x => x.Id == tourDateId.Value);

        if (tourDate != null)
        {
            RequestModel.StartDate = tourDate.StartDate;
            RequestModel.EndDate = tourDate.EndDate;
            RequestModel.Amount = tourDate.TourPrice?.Price;
            RequestModel.TourDateId = tourDateId.Value;
            RequestModel.TourDate = tourDate.Adapt<UpdateTourDateRequest>();
        }

        StateHasChanged();
    }

    private void OnTourPickupLocationChanged(string location)
    {
        
    }
    
    public async Task InvokeBookingItemGuestDialog(UpdateBookingItemRequest bookingItem, BookingItemGuestRequest? selectedGuest = null)
    {
        var initialModel =
            JsonSerializer.Deserialize<IList<BookingItemGuestRequest>>(JsonSerializer.Serialize(bookingItem.Guests)) ??
            new List<BookingItemGuestRequest>();
        DialogOptions options = new()
            {CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {nameof(TourBookingItemGuest.RequestModel), new BookingItemGuestRequest() {BookingItemId = bookingItem.Id}},
            {nameof(TourBookingItemGuest.TourBookingItem), bookingItem},
            {nameof(TourBookingItemGuest.GuestToUpdate), selectedGuest?.GuestId},
        };

        var dialog = await DialogService.ShowAsync<TourBookingItemGuest>(string.Empty, parameters, options);

        var result = await dialog.Result;

        if (result.Canceled)
        {
            bookingItem.Guests = initialModel;
        }


        StateHasChanged();
        Context.AddEditModal?.ForceRender();
    }


    public async Task RemoveGuestRow(UpdateBookingItemRequest bookingItem, string id)
    {
        string deleteContent = L["You're sure you want to delete this {0}? Please note, this is not final."];
        var parameters = new DialogParameters
        {
            {nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, "Additional Guest", id)}
        };

        var options = new DialogOptions
            {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true};
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>(L["Delete"], parameters, options);

        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var item = bookingItem.Guests?.FirstOrDefault(x => x.GuestId == id);

            if (item != null)
            {
                bookingItem.Guests?.Remove(item);
            }

            Context.AddEditModal?.ForceRender();
        }
    }
    
    public void ShowWaiverDialog()
    {
        DialogOptions options = new() {CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = false, DisableBackdropClick = true};
        DialogParameters parameters = new()
        {
            {
                nameof(GenericDialog.ContentText),
                WaiverTermsAndConditions
            },
            {nameof(GenericDialog.TitleText), "Waiver"}
        };

        DialogService.Show<GenericDialog>(string.Empty, parameters, options);
    }
    
    private void ClearTourCategories(MouseEventArgs obj)
    {
        RequestModel.TourCategoryId = null;
        RequestModel.TourId = null;
        RequestModel.TourDateId = null;
        RequestModel.PickupLocation = null;
        RequestModel.TourDateId = null;
        TourDates = new List<TourDateDto>();
        TourPickupLocations = Array.Empty<TourPickupLocationDto>();
    }
}