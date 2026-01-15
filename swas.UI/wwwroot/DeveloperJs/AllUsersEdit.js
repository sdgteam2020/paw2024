
    function ValInDataEdit(input) {
        var regex = /[^a-zA-Z0-9 ]/g;
        input.value = input.value.replace(regex, "");
    }
    $(document).ready(function () {
        $('.dropdownsearch').select2();
    });

    $(document).ready(function () {

        var isRestricted = '@isRestricted' === 'True';

        if (isRestricted) {


            // Hide Back button
            $('a[href="/Account/GetsAllUsers"]').closest('button').hide();
        }

    });
