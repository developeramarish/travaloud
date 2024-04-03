let datepickerWithFilter = $('.confirm-date-on-select');
let tourPrice = 0;
let startDate = null;

$("document").ready(function () {
    filterDates();
    
    $('#GuestQuantity').on('keyup', function() {
        let value = $(this).val();
        if (value.length > 0 && parseInt(value) > 0)
        {
            $('.add-tour-to-basket-button').prop('disabled', false);
        }
        else {
            $('.add-tour-to-basket-button, .proceed-to-checkout-button').prop('disabled', true);
        }
    });
});

function filterDates() {
    let tourDates = [];

    if (tourDatesParsed.length > 0) {
        $(tourDatesParsed).each(function (i, v) {
            var startDate = new Date(v.startDate);
            startDate.setHours(0, 0, 0);

            tourDates.push(startDate.getTime());
        });
    }

    const filterFunction = (date) => {
        return $.inArray(date.getTime(), tourDates) > -1;
    }
    
    datepickerWithFilter.each(function(i, v) {
        new mdb.Datepicker(v, { filter: filterFunction,  confirmDateOnSelect: true });

        v.addEventListener('valueChanged.mdb.datepicker', (e) => {
            let selectedDate = e.date;

            $(tourDatesParsed).each(function (i, v) {
                let tourDate = new Date(v.startDate);
                let tourDateFormatted = tourDate;
                tourDateFormatted.setHours(0, 0, 0);

                let selectedDateParsed = moment(selectedDate).format('DD/MM/YYYY');
                let tourDateParsed = moment(tourDateFormatted).format('DD/MM/YYYY');
                let tourDateTimeParsed = moment(v.startDate).format('HH:mm');
                
                if (tourDateParsed === selectedDateParsed) {
                    $('#TourDateStartTime').append('<option value="' + v.id + '">' + tourDateTimeParsed + '</option>').prop('disabled', false);
                }
            });
        });
    });
}

const selectTourDateTime = () => {
    let tourDateId = $('#TourDateStartTime').val();
    
    let tourDate = $.grep(tourDatesParsed, function(n, i ) {
        return (n.id == tourDateId);
    })[0];

    tourPrice = tourDate.tourPrice.price;
    startDate = tourDate.startDate;
    
    let guestQuantityControl = $('#GuestQuantity');
    guestQuantityControl.attr('max', tourDate.availableSpaces).prop('disabled', false);
    guestQuantityControl.parent().find('.form-helper').html(tourDate.availableSpaces + ' spaces available.');
}

const addTourToBasket = (tourId, tourName, tourImageUrl) => {
    let tourDateId = $('#TourDateStartTime').val();
    let guestQuantity = $('#GuestQuantity').val();

    let request = new BasketItemDateModel(tourDateId, tourId, tourName, tourImageUrl, guestQuantity, tourPrice, startDate);
    
    postAjax("AddTourDateToBasket", request, function (result) {
        let basket = result.basket;
        let item = result.item;
        let itemsCount = basket.itemsCount;

        $('.basketItemsNavQuantity').html(itemsCount).removeClass('d-none');
        $('#basketTotal').html('$ ' + basket.total.toFixed(2));
        $('#selectionTotal').html('$ ' + item.total.toFixed(2));

        let guestQuantityControl = $('#GuestQuantity');
        guestQuantityControl.parent().find('.form-helper').html('Select a Date');
        
        $('#TourDate').val('').removeClass('active');
        guestQuantityControl.val('').prop('disabled', true).removeClass('active');
        $('#TourDateStartTime').html('<option value="" hidden></option>').val('').prop('disabled', true);
        $('.add-tour-to-basket-button').prop('disabled', true);

        let proceedToCheckoutButton = $('.proceed-to-checkout-button');
        
        proceedToCheckoutButton.prop('disabled', false);
        proceedToCheckoutButton.on('click', function() {
            window.location.href = "/checkout";
        })
    });
}