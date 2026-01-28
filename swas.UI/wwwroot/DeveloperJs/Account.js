$(document).ready(function () {
  
    initializeDataTable("#SoftwareXXx");

    // Ensure the button container is not appended more than once
    if ($('.toolbar').find('.dt-buttons').length === 0) {
        var buttonContainer = table.buttons().container();
        $('.toolbar').append(buttonContainer); // Append buttons to toolbar
    }

    // Add search box functionality
    $('#searchBox').on('keyup', function () {
        table.search(this.value).draw();
    });

    // "Add Condition" button functionality
    $('#addConditionButton').on('click', function () {
        alert('Add Condition button clicked!');
        // Add your functionality here for the button
    });

})
   
   





$(document).on('click', '.flag-toggle-btn', function () {

    var username = $(this).data('username');
    var rolename = $(this).data('rolename');
    var flag = $(this).data('flag');

    // Toggle flag
    flag = !flag;

    // Update UI using CSS classes (NO inline styles)
    if (flag) {
        $(this).html('<span class="flag-pill flag-on">ON</span>');
    } else {
        $(this).html('<span class="flag-pill flag-off">OFF</span>');
    }

    // Update data attribute
    $(this).data('flag', flag);

    // AJAX update
    $.ajax({
        type: 'POST',
        url: '/Account/UpdateFlag',
        data: {
            userName: username,
            roleName: rolename,
            flag: flag
        },
        success: function () {
            console.log('Flag updated successfully');
        },
        error: function (xhr, status, error) {
            console.error('Error updating flag:', error);
        }
    });
});




   
