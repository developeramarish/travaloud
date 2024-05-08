﻿namespace Travaloud.Application.Catalog.Bookings.Dto;

public class BookingDto
{
    public DefaultIdType Id { get; set; }
    public string Description { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public int ItemQuantity => Items?.Count ?? 0;
    public bool IsPaid { get; set; }
    public DateTime BookingDate { get; set; }
    public string? GuestId { get; set; }
    public string? GuestName { get; set; }
    public string? GuestEmail { get; set; }

    public int InvoiceId { get; set; }
    public bool ShowDetails { get; set; } = default!;
    public int ConcurrencyVersion { get; set; } = default!;
    public string? AdditionalNotes { get; set; }
    public DefaultIdType CreatedBy { get; set; }
    public string? StaffName { get; set; }
    public bool? WaiverSigned { get; set; }
    public string? BookingSource { get; set; }
    public bool? ConfirmationEmailSent { get; set; }

    public IList<BookingItemDetailsDto>? Items { get; set; }
}