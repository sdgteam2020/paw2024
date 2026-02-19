$(document).on('ready', function () {
    $('.alphanumeric-only').on('keyup', function () {
        ValInDataEdit(this);
    });
        $('.dropdownsearch').select2();
        var isRestricted = '@isRestricted' === 'True';

        if (isRestricted) {
            $('a[href="/Account/GetsAllUsers"]').closest('button').hide();
        }

    });
function ValInDataEdit(input) {
    var regex = /[^a-zA-Z0-9 ]/g;
    input.value = input.value.replace(regex, "");
}
