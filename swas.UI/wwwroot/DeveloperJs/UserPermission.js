$(document).ready(function () {

    $('#ddlUser').change(function () {

        let userId = $(this).val();

        $('#userPermissionDiv').html('');

        if (!userId) {
            return;
        }

        $.ajax({
            url: '/UserPermission/GetUserPermissions',
            type: 'GET',
            data: {
                userId: userId
            },

            beforeSend: function () {
                $('#userPermissionDiv').html(
                    '<div class="text-muted">Loading permissions...</div>'
                );
            },

            success: function (response) {
                $('#userPermissionDiv').html(response);
            },

            error: function (xhr) {

                $('#userPermissionDiv').html('');

                let message = 'Failed to load permissions.';

                if (xhr.responseJSON && xhr.responseJSON.message) {
                    message = xhr.responseJSON.message;
                }

                console.error(xhr.responseText);

                Swal.fire('Error', message, 'error');
            }
        });
    });
});

function SaveUserPermissions() {

    let userId = $('#UserId').val();

    if (!userId) {
        Swal.fire('Error', 'User is required.', 'error');
        return;
    }

    let permissions = [];

    $('.permission-checkbox').each(function () {
        permissions.push({
            permissionKey: $(this).val(),
            isSelected: $(this).is(':checked')
        });
    });

    let model = {
        userId: userId,
        permissions: permissions
    };

    $.ajax({
        url: '/UserPermission/SaveUserPermissions',
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',

        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },

        data: JSON.stringify(model),

        success: function (response) {

            if (response.success === true) {
                Swal.fire('Success', response.message, 'success');
            } else {
                Swal.fire('Error', response.message || 'Operation failed.', 'error');
            }
        },

        error: function (xhr) {

            let message = 'Something went wrong.';

            if (xhr.responseJSON && xhr.responseJSON.message) {
                message = xhr.responseJSON.message;
            }

            console.error(xhr.responseText);

            Swal.fire('Error', message, 'error');
        }
    });
}