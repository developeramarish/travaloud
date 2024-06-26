let dataTableRows = [];

$(window).on("load", function () {
    if ($('#datatable-custom').is(':visible') && bookingsSerialized.length)
    {
        $(bookingsSerialized).each(function(i, v) {
           dataTableRows.push(new DataTableRow(v.id, v.invoiceId, moment(v.bookingDate).format("DD/MM/YYYY"), v.description, v.itemQuantity));
        });

        const customDatatable = document.getElementById('datatable-custom');

        customDatatable.addEventListener('rowClicked.mdb.datatable', (e) => {
            let index = e.index;
            let dataTableRow = dataTableRows[index];
            let id = dataTableRow.Id;

            window.location.href = '/my-account/bookings/' + id;
        });

        let dataTableColumns = [
            {label: 'Reference', field: 'reference'},
            {label: 'Booking Date', field: 'bookingDate'},
            {label: 'Description', field: 'description'},
            {label: 'Items', field: 'items'}
        ];

        new mdb.Datatable(customDatatable, {
            columns: dataTableColumns,
            rows: dataTableRows.map((row) => {
                return {
                    reference: `<p class='m-0 p-0'>#${row.Reference}</p>`,
                    bookingDate: `<p class='m-0 p-0'>${row.BookingDate}</p>`,
                    description: `<p class='m-0 p-0'>${row.Description}</p>`,
                    items: `<p class='m-0 p-0'>${row.ItemQuantity}</p>`,
                };
            }),
        }, { hover: true, clickableRows: true });
    }
});