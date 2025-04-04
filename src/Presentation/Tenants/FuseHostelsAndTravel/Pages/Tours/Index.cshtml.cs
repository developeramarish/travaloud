﻿using Microsoft.AspNetCore.Mvc;
using Travaloud.Application.Common.Extensions;

namespace FuseHostelsAndTravel.Pages.Tours;

public class IndexModel : TravaloudBasePageModel
{
    public override Guid? PageId()
    {
        return new Guid("8d0879af-df6d-4eb2-440b-08dc303eaa33");
    }
    
    public override string MetaKeywords(string? overrideValue = null)
    {
        return base.MetaKeywords(  "Vietnam tours, budget tours, adventure tours, group tours, Vietnam travel");
    }

    public override string MetaDescription(string? overrideValue = null)
    {
        return base.MetaDescription( "Explore Vietnam with our affordable and exciting budget tours. Join a group tour or customize your own adventure and discover all that Vietnam has to offer.");
    }

    [BindProperty]
    public AlternatingCardHeightComponent? ToursCards { get; private set; }

    [BindProperty]
    public HeaderBannerComponent? HeaderBanner { get; private set; }

    public async Task<IActionResult> OnGet()
    {
        ViewData["Title"] = "Tours";
        
        await base.OnGetDataAsync();

        var (title, subTitle, subSubTitle) = PageDetails.GetTitleSubTitleAndSubSubTitle();
        
        HeaderBanner = new HeaderBannerComponent(@title, @subTitle, "", "https://travaloudcdn.azureedge.net/fuse/assets/images/146f0d66-95b1-42fb-9902-d5466890e60d.jpg?w=2000", new List<OvalContainerComponent>()
        {
            new("hostelPageHeaderBannerOvals1", -160, null, -45, null)
        });
        ToursCards = await WebComponentsBuilder.FuseHostelsAndTravel.GetToursCardsAsync(Tours, "onLoad");

        return Page();
    }
}