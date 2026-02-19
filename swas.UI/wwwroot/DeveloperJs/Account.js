$(document).ready(function () {
  
    initializeDataTable("#SoftwareXXx");
    if ($('.toolbar').find('.dt-buttons').length === 0) {
        var buttonContainer = table.buttons().container();
        $('.toolbar').append(buttonContainer); // Append buttons to toolbar
    }
    $('#searchBox').on('keyup', function () {
        table.search(this.value).draw();
    });
    $('#addConditionButton').on('click', function () {
        alert('Add Condition button clicked!');
    });

})
   
   





$(document).on('click', '.flag-toggle-btn', function () {

    var username = $(this).data('username');
    var rolename = $(this).data('rolename');
    var flag = $(this).data('flag');
    flag = !flag;
    if (flag) {
        $(this).html('<span class="flag-pill flag-on">ON</span>');
    } else {
        $(this).html('<span class="flag-pill flag-off">OFF</span>');
    }
    $(this).data('flag', flag);
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




   
