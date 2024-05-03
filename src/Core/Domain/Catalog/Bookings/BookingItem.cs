﻿using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Domain.Catalog.Bookings;

public class BookingItem : AuditableEntity, IAggregateRoot
{
    public DefaultIdType BookingId { get; private set; } = default!;
    public DateTime StartDate { get; private set; } = default!;
    public DateTime? EndDate { get; private set; }
    public decimal Amount { get; private set; } = default!;
    public int? RoomQuantity { get; set; }
    public DefaultIdType? PropertyId { get; private set; }
    public DefaultIdType? TourId { get; private set; }
    public DefaultIdType? TourDateId { get; private set; }
    public string? CloudbedsReservationId { get; set; }
    public int? CloudbedsPropertyId { get; set; }
    public int ConcurrencyVersion { get; set; }
    public string? PickupLocation { get; set; }

    public virtual IList<BookingItemRoom>? Rooms { get; set; }
    public virtual Tour? Tour { get; set; }
    public virtual TourDate? TourDate { get; set; }
    public virtual Property? Property { get; set; }
    public virtual Booking Booking { get; private set; } = default!;
    public virtual IList<BookingItemGuest>? Guests { get; set; }
    
    public BookingItem(
        DateTime startDate,
        DateTime? endDate,
        decimal amount,
        int? roomQuantity,
        DefaultIdType? propertyId,
        DefaultIdType? tourId,
        DefaultIdType? tourDateId,
        string? cloudbedsReservationId,
        int? cloudbedsPropertyId,
        string? pickupLocation)
    {
        StartDate = startDate;
        EndDate = endDate;
        Amount = amount;
        RoomQuantity = roomQuantity;
        PropertyId = propertyId;
        TourId = tourId;
        TourDateId = tourDateId;
        CloudbedsReservationId = cloudbedsReservationId;
        CloudbedsPropertyId = cloudbedsPropertyId;
        PickupLocation = pickupLocation;
    }

    public BookingItem Update(
        DateTime? startDate = null,
        DateTime? endDate = null,
        decimal? amount = null,
        int? roomQuantity = null,
        DefaultIdType? propertyId = null,
        DefaultIdType? tourId = null,
        DefaultIdType? tourDateId = null,
        string? cloudbedsReservationId = null,
        int? cloudbedsPropertyId = null,
        IList<BookingItemRoom>? rooms = null,
        string? pickupLocation = null)
    {
        if (startDate is not null && StartDate != startDate)
            StartDate = startDate.Value;

        if (endDate is not null && EndDate != endDate)
            EndDate = endDate.Value;

        if (amount is not null && Amount != amount)
            Amount = amount.Value;

        if (roomQuantity is not null && RoomQuantity != roomQuantity)
            RoomQuantity = roomQuantity;

        if (propertyId is not null && PropertyId != propertyId)
            PropertyId = propertyId;

        if (tourId is not null && TourId != tourId)
            TourId = tourId;

        if (tourDateId is not null && TourDateId != tourDateId)
            TourDateId = tourDateId;

        if (cloudbedsReservationId is not null && CloudbedsReservationId != cloudbedsReservationId)
            CloudbedsReservationId = cloudbedsReservationId;

        if (cloudbedsPropertyId is not null && CloudbedsPropertyId != cloudbedsPropertyId)
            CloudbedsPropertyId = cloudbedsPropertyId;

        if (rooms is not null && Rooms != rooms)
            Rooms = rooms;

        PickupLocation = pickupLocation;
        
        return this;
    }

    public BookingItem SetReservationId(string reservationId)
    {
        CloudbedsReservationId = reservationId;

        return this;
    }

    public BookingItem SetEndDate(DateTime endDate)
    {
        EndDate = endDate;

        return this;
    }
    
    public BookingItem AddGuest(BookingItemGuest guest)
    {
        Guests ??= new List<BookingItemGuest>();
        Guests.Add(guest);

        return this;
    }
}