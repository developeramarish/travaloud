﻿using Travaloud.Domain.Catalog.Partners;
using Travaloud.Shared.Common.Extensions;

namespace Travaloud.Domain.Catalog.Tours;

public class Tour : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string? ShortDescription { get; private set; }
    public decimal? Price { get; private set; }
    public string? PriceLabel { get; private set; }
    public string? ImagePath { get; private set; }
    public string? ThumbnailImagePath { get; private set; }
    public int? MaxCapacity { get; private set; }
    public int? MinCapacity { get; private set; }
    public string? DayDuration { get; private set; }
    public string? NightDuration { get; private set; }
    public string? HourDuration { get; private set; }
    public string? Address { get; private set; }
    public string? TelephoneNumber { get; private set; }
    public string? WhatsIncluded { get; private set; }
    public string? WhatsNotIncluded { get; private set; }
    public string? AdditionalInformation { get; private set; }
    public string? ImportantInformation { get; private set; }
    public string? MetaKeywords { get; private set; }
    public string? MetaDescription { get; private set; }
    public bool? PublishToSite { get; private set; }
    public string? UrlSlug { get; private set; }
    public string? H1 { get; private set; }
    public string? H2 { get; private set; }
    public string? VideoPath { get; private set; }
    public string? MobileVideoPath { get; private set; }
    public string? BookingConfirmationEmailDetails { get; private set; }
    public string? TermsAndConditions { get; private set; }
    public string? CancellationPolicy { get; private set; }
    public bool? AdditionalGuestDetailsRequired { get; private set; }
    public bool? WaiverRequired { get; private set; }
    public string? SupplierId { get; private set; }
    public string? SupplierEmailText { get; private set; }
    public bool? ShowBookingQRCode { get; private set; }
    public DefaultIdType? TourCategoryId { get; private set; }
    public string? CustomSeoScripts { get; private set; }
    public string? SeoPageTitle { get; private set; }
    
    public virtual IList<TourCategoryLookup>? TourCategoryLookups { get; set; }
    public virtual IList<TourPropertyLookup>? TourPropertyLookups { get; private set; }
    public virtual IList<TourDestinationLookup>? TourDestinationLookups { get; set; }
    public virtual IList<TourPrice>? TourPrices { get; set; }
    public virtual IList<TourDate>? TourDates { get; set; }
    public virtual IList<TourItinerary>? TourItineraries { get; set; }
    public virtual IList<TourImage>? Images { get; set; }
    public virtual IList<TourPickupLocation>? TourPickupLocations { get; set; }
    public virtual TourCategory? TourCategory { get; set; }
    
    public Tour()
    {
    }

    public Tour(string name,
        string description,
        string? shortDescription,
        decimal? price,
        string? priceLabel,
        string? imagePath,
        string? thumbnailImagePath,
        int? maxCapacity,
        int? minCapacity,
        string? dayDuration,
        string? nightDuration,
        string? hourDuration,
        string? address,
        string? telephoneNumber,
        string? whatsIncluded,
        string? whatsNotIncluded,
        string? additionalInformation,
        string? metaKeywords,
        string? metaDescription,
        string? importantInformation,
        bool? publishToSite,
        string? urlSlug,
        string? h1,
        string? h2,
        string? videoPath,
        string? mobileVideoPath, 
        string? bookingConfirmationEmailDetails,
        string? termsAndConditions,
        string? cancellationPolicy,
        bool? additionalGuestDetailsRequired,
        bool? waiverRequired,
        string? supplierId,
        string? supplierEmailText,
        DefaultIdType? tourCategoryId,
        bool? showBookingQrCode, 
        string? customSeoScripts,
        string? seoPageTitle)
    {
        Name = name;
        Description = description.FormatTextEditorString() ?? string.Empty;
        ShortDescription = shortDescription.FormatTextEditorString();
        Price = price;
        PriceLabel = priceLabel;
        ImagePath = imagePath != null ? !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath : null;
        ThumbnailImagePath = thumbnailImagePath != null ? !thumbnailImagePath.Contains("w-700") ? $"{thumbnailImagePath}?w=700" : thumbnailImagePath : null;
        MaxCapacity = maxCapacity;
        MinCapacity = minCapacity;
        DayDuration = dayDuration;
        NightDuration = nightDuration;
        HourDuration = hourDuration;
        Address = address;
        TelephoneNumber = telephoneNumber;
        WhatsIncluded = whatsIncluded.FormatTextEditorString();
        WhatsNotIncluded = whatsNotIncluded.FormatTextEditorString();
        AdditionalInformation = additionalInformation.FormatTextEditorString();
        MetaKeywords = metaKeywords;
        MetaDescription = metaDescription;
        ImportantInformation = importantInformation.FormatTextEditorString();
        PublishToSite = publishToSite;
        UrlSlug = urlSlug;
        H1 = h1;
        H2 = h2;
        VideoPath = videoPath;
        MobileVideoPath = mobileVideoPath;
        BookingConfirmationEmailDetails = bookingConfirmationEmailDetails.FormatTextEditorString();
        TermsAndConditions = termsAndConditions.FormatTextEditorString();
        CancellationPolicy = cancellationPolicy.FormatTextEditorString();
        AdditionalGuestDetailsRequired = additionalGuestDetailsRequired;
        WaiverRequired = waiverRequired;
        SupplierId = supplierId;
        SupplierEmailText = supplierEmailText;
        TourCategoryId = tourCategoryId;
        ShowBookingQRCode = showBookingQrCode;
        CustomSeoScripts = customSeoScripts;
        SeoPageTitle = seoPageTitle;
    }

    public Tour Update(string? name,
        string? description,
        string? shortDescription,
        decimal? price,
        string? priceLabel,
        string? imagePath,
        string? thumbnailImagePath,
        int? maxCapacity,
        int? minCapacity,
        string? dayDuration,
        string? nightDuration,
        string? hourDuration,
        string? address,
        string? telephoneNumber,
        string? whatsIncluded,
        string? whatsNotIncluded,
        string? additionalInformation,
        string? metaKeywords,
        string? metaDescription,
        string? importantInformation,
        bool? publishToSite,
        string? urlSlug,
        string? h1,
        string? h2,
        string? videoPath,
        string? mobileVideoPath, 
        string? bookingConfirmationEmailDetails,
        string? termsAndConditions,
        string? cancellationPolicy,
        bool? additionalGuestDetailsRequired,
        bool? waiverRequired,
        string? supplierId,
        string? supplierEmailText,
        DefaultIdType? tourCategoryId,
        bool? showBookingQrCode,
        string? customSeoScripts,
        string? seoPageTitle)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        if (description is not null && Description?.Equals(description) is not true) Description = description.FormatTextEditorString() ?? string.Empty;
        if (shortDescription is not null && ShortDescription?.Equals(shortDescription) is not true) ShortDescription = shortDescription.FormatTextEditorString();
        if (price.HasValue && Price != price) Price = price.Value;
        if (priceLabel is not null && PriceLabel?.Equals(priceLabel) is not true) PriceLabel = priceLabel;
        if (imagePath is not null && ImagePath?.Equals(imagePath) is not true) ImagePath = !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath;
        if (thumbnailImagePath is not null && ThumbnailImagePath?.Equals(thumbnailImagePath) is not true) ThumbnailImagePath = thumbnailImagePath.Contains("w-700") ? $"{thumbnailImagePath}?w=700" : thumbnailImagePath;
        if (maxCapacity.HasValue && MaxCapacity != maxCapacity) MaxCapacity = maxCapacity.Value;
        if (minCapacity.HasValue && MinCapacity != minCapacity) MinCapacity = minCapacity.Value;
        if (dayDuration is not null && DayDuration?.Equals(dayDuration) is not true) DayDuration = dayDuration;
        if (nightDuration is not null && NightDuration?.Equals(nightDuration) is not true) NightDuration = nightDuration;
        if (hourDuration is not null && HourDuration?.Equals(hourDuration) is not true) HourDuration = hourDuration;
        if (address is not null && Address?.Equals(address) is not true) Address = address;
        if (telephoneNumber is not null && TelephoneNumber?.Equals(telephoneNumber) is not true) TelephoneNumber = telephoneNumber;
        if (whatsIncluded is not null && WhatsIncluded?.Equals(whatsIncluded) is not true) WhatsIncluded = whatsIncluded.FormatTextEditorString();
        if (whatsNotIncluded is not null && WhatsNotIncluded?.Equals(whatsNotIncluded) is not true) WhatsNotIncluded = whatsNotIncluded.FormatTextEditorString();
        if (additionalInformation is not null && AdditionalInformation?.Equals(additionalInformation) is not true) AdditionalInformation = additionalInformation.FormatTextEditorString();
        if (metaKeywords is not null && MetaKeywords?.Equals(metaKeywords) is not true) MetaKeywords = metaKeywords;
        if (metaDescription is not null && MetaDescription?.Equals(metaDescription) is not true) MetaDescription = metaDescription;
        if (importantInformation is not null && ImportantInformation?.Equals(importantInformation) is not true) ImportantInformation = importantInformation.FormatTextEditorString();
        if (urlSlug is not null && UrlSlug?.Equals(urlSlug) is not true) UrlSlug = urlSlug;
        if (h1 is not null && H1?.Equals(h1) is not true) H1 = h1;
        if (h2 is not null && H2?.Equals(h2) is not true) H2 = h2;
        if (videoPath is not null && VideoPath?.Equals(videoPath) is not true) VideoPath = videoPath;
        if (mobileVideoPath is not null && MobileVideoPath?.Equals(mobileVideoPath) is not true) MobileVideoPath = mobileVideoPath;
        if (bookingConfirmationEmailDetails is not null && BookingConfirmationEmailDetails?.Equals(bookingConfirmationEmailDetails) is not true) BookingConfirmationEmailDetails = bookingConfirmationEmailDetails.FormatTextEditorString();
        if (termsAndConditions is not null && TermsAndConditions?.Equals(termsAndConditions) is not true) TermsAndConditions = termsAndConditions.FormatTextEditorString();
        if (cancellationPolicy is not null && CancellationPolicy?.Equals(cancellationPolicy) is not true) CancellationPolicy = cancellationPolicy.FormatTextEditorString();
        if (supplierEmailText is not null && SupplierEmailText?.Equals(supplierEmailText) is not true) SupplierEmailText = supplierEmailText.FormatTextEditorString();
        if (customSeoScripts is not null && CustomSeoScripts?.Equals(customSeoScripts) is not true) CustomSeoScripts = customSeoScripts;
        if (seoPageTitle is not null && SeoPageTitle?.Equals(seoPageTitle) is not true) SeoPageTitle = seoPageTitle;
        
        PublishToSite = publishToSite;
        ShowBookingQRCode = showBookingQrCode;
        AdditionalGuestDetailsRequired = additionalGuestDetailsRequired;
        WaiverRequired = waiverRequired;
        SupplierId = supplierId;
        TourCategoryId = tourCategoryId;
        
        return this;
    }

    public Tour ClearImagePath()
    {
        ImagePath = string.Empty;
        ThumbnailImagePath = string.Empty;
        return this;
    }

    public Tour ClearVideoPath()
    {
        VideoPath = string.Empty;
        return this;
    }

    public Tour ClearMobileVideoPath()
    {
        MobileVideoPath = string.Empty;
        return this;
    }
}