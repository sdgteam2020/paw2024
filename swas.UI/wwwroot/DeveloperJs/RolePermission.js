$(document).ready(function () {

    $('#ddlRole').change(function () {

        var roleId =
            $(this).val();

        if (roleId == '') {

            $('#permissionDiv')
                .html('');

            return;
        }

        $.ajax({

            url:
                '/RolePermission/GetRolePermissions',

            type:
                'GET',

            data: {
                roleId:
                    roleId
            },

            success:
                function (response) {

                    $('#permissionDiv')
                        .html(response);
                }
        });
    });
});


function SavePermissions() {

    let roleId = $('#RoleId').val();

    if (!roleId) {
        Swal.fire('Error', 'Role is required.', 'error');
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
        roleId: roleId,
        permissions: permissions
    };

    $.ajax({
        url: '/RolePermission/SaveRolePermissions',
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