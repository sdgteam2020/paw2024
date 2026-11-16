$(document).ready(function () {

    // Appointment Select2
    $('#appointment').select2({
        placeholder: "-- Select Appointment --",
        allowClear: true,
        width: '100%'
    });

    // Existing alphanumeric validation
    $('.alphanumeric-only').on('keyup', function () {
        ValInDataEdit(this);
    });

    // Other searchable dropdowns
    $('.dropdownsearch').select2({
        width: '100%'
    });

    let isRestricted = '@isRestricted' === 'True';

    if (isRestricted) {
        $('a[href="/Account/GetsAllUsers"]').closest('button').hide();
    }

});

function ValInDataEdit(input) {
    let regex = /[^a-zA-Z0-9 ]/g;
    input.value = input.value.replace(regex, "");
}