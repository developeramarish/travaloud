﻿namespace Travaloud.Application.Catalog.Bookings.Commands;

public class CreateBookingItemRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public int? RoomQuantity { get; set; }

    [RequiredIfNull("TourId", ErrorMessage = "A Property is required")]
    public DefaultIdType? PropertyId { get; set; }

    [RequiredIfNull("PropertyId", ErrorMessage = "A Tour is required")]
    public DefaultIdType? TourId { get; set; }

    [RequiredIfNull("PropertyId", ErrorMessage = "A Tour Date is required")]
    public DefaultIdType? TourDateId { get; set; }

    public string? CloudbedsReservationId { get; set; }
    public int? CloudbedsPropertyId { get; set; }
    public int ConcurrencyVersion { get; set; } = default!;
    public UpdateTourDateRequest? TourDate { get; set; }
    public IList<CreateBookingItemRoomRequest>? Rooms { get; set; }
}