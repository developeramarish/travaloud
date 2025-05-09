﻿namespace Travaloud.Application.Basket.Dto;

public class BasketModel
{
	public DefaultIdType Id { get; set; } = DefaultIdType.NewGuid();
	public IList<BasketItemModel> Items { get; set; } = new List<BasketItemModel>();
	public DefaultIdType? BookingId { get; set; }
	public decimal Total { get; set; }
	public string? PromoCode { get; set; }
	public int ItemsCount => Items.Count;
	public string? FirstName { get; set; }
	public string? Surname { get; set; }
	public string? Email { get; set; }
	public DateTime? DateOfBirth { get; set; }
	public string? PhoneNumber { get; set; }
	public string? Nationality { get; set; }
	public string? Gender { get; set; }
	public string? Password { get; set; }
	public string? ConfirmPassword { get; set; }
	public TimeSpan? EstimatedArrivalTime { get; set; }
	public string? CreateId { get; set; }
	public string? AdditionalNotes { get; set; }

	public void SetPromoCode(string promoCode)
	{
		PromoCode = promoCode;
	}

	public void SetBookingId(DefaultIdType bookingId)
	{
		BookingId = bookingId;
	}

	public void SetCreateId(string createId)
	{
		CreateId = createId;
	}
	
	public void CalculateTotal()
	{
		Total = 0M;

		if (!Items.Any()) return;
		
		foreach (var item in Items)
		{
			if (item.TourDates != null && item.TourDates.Any())
			{
				foreach (var tourDate in item.TourDates)
				{
					Total += tourDate.Price * tourDate.GuestQuantity;
				}
			}
			else if (item.Rooms != null && item.Rooms.Any())
			{
				foreach (var room in item.Rooms)
				{
					Total += room.RoomRate * room.RoomQuantity;
				}
			}
		}
	}
	
	public void SetPrimaryContactInformation(string? firstName, string? surname, string? email, DateTime? dateOfBirth, string? phoneNumber, string? nationality, string? gender, TimeSpan? estimatedArrivalTime, string? password, string? confirmPassword)
	{
		FirstName = firstName;
		Surname = surname;
		Email = email;
		DateOfBirth = dateOfBirth;
		PhoneNumber = phoneNumber;
		Nationality = nationality;
		EstimatedArrivalTime = estimatedArrivalTime;
		Gender = gender;
		Password = password;
		ConfirmPassword = confirmPassword;
	}

}