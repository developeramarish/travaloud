﻿namespace Travaloud.Application.Catalog.Bookings.Commands;

public class UpdateBookingItemRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Amount { get; set; }
    public int? RoomQuantity { get; set; }
    public bool WaiverSigned { get; set; }
    public bool? CheckedIn { get; set; }
    public bool? NoShow { get; set; }
    public bool? Cancelled { get; set; }

    public int GuestQuantity
    {
        get => Guests?.Count + 1 ?? 1;
        set { }
    }

    [RequiredIfNull("TourId", ErrorMessage = "A Property is required")]
    public DefaultIdType? PropertyId { get; set; }

    [RequiredIfNull("PropertyId", ErrorMessage = "A Tour is required")]
    public DefaultIdType? TourId { get; set; }

    [RequiredIfNull("PropertyId", ErrorMessage = "A Tour Date is required")]
    public DefaultIdType? TourDateId { get; set; }
    
    [RequiredIfNull("PropertyId", ErrorMessage = "A Tour Price is required")]
    public DefaultIdType? TourPriceId { get; set; }
    
    [RequiredIfNull("PropertyId", ErrorMessage = "A Tour Category is required")]
    public DefaultIdType? TourCategoryId { get; set; }
    
    public string? PickupLocation { get; set; }
    public string? OtherPickupLocation { get; set; }

    public string? CloudbedsReservationId { get; set; }
    public int? CloudbedsPropertyId { get; set; }
    public int ConcurrencyVersion { get; set; }
    public UpdateTourDateRequest? TourDate { get; set; }
    public IList<UpdateBookingItemRoomRequest>? Rooms { get; set; }
    public IList<BookingItemGuestRequest>? Guests { get; set; }
}