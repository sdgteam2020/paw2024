let isSavingUnitPermission = false;

$('#btnSaveUnitPermissions').on('click', function () {

    saveUnitPermission();
})
function saveUnitPermission() {

    if (isSavingUnitPermission) {
        return;
    }

    let unitId = parseInt($('#ddlUnit').val());

    if (isNaN(unitId) || unitId <= 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Unit Required',
            text: 'Please select a valid unit.'
        });

        return;
    }

    let permissions = [];

    $('.permissionCheckbox').each(function () {

        let permissionId = parseInt($(this).val());

        if (!isNaN(permissionId) && permissionId > 0) {
            permissions.push({
                permissionId: permissionId,
                isSelected: $(this).is(':checked')
            });
        }
    });

    if (permissions.length === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'No Permissions Found',
            text: 'No valid permissions available to save.'
        });

        return;
    }

    const token =
        $('input[name="__RequestVerificationToken"]').val();

    if (!token) {
        Swal.fire({
            icon: 'error',
            title: 'Security Token Missing',
            text: 'Please refresh the page and try again.'
        });

        return;
    }

    const model = {
        unitId: unitId,
        permissions: permissions
    };

    isSavingUnitPermission = true;

    $('#btnSaveUnitPermission')
        .prop('disabled', true)
        .text('Saving...');

    $.ajax({
        url: '/UnitPermission/SaveUnitPermissions',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        headers: {
            'RequestVerificationToken': token
        },
        data: JSON.stringify(model),

        success: function (response) {

            if (response && response.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: response.message || 'Permissions saved successfully.'
                });
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.message || 'Unable to save permissions.'
                });
            }
        },

        error: function (xhr) {

            let message = 'Save failed. Please try again.';

            if (xhr.responseJSON && xhr.responseJSON.message) {
                message = xhr.responseJSON.message;
            }
            else if (xhr.status === 400) {
                message = 'Invalid request data.';
            }
            else if (xhr.status === 401) {
                message = 'Your session has expired. Please login again.';
            }
            else if (xhr.status === 403) {
                message = 'You are not allowed to perform this action.';
            }

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: message
            });
        },

        complete: function () {

            isSavingUnitPermission = false;

            $('#btnSaveUnitPermission')
                .prop('disabled', false)
                .text('Save Permissions');
        }
    });
}

$(document).ready(function () {

    $('.select2').select2();

    $('#ddlUnit').on('change', function () {

        let unitId = parseInt($(this).val());

        if (isNaN(unitId) || unitId <= 0) {
            $('#permissionContainer').html('');
            return;
        }

        loadPermissions(unitId);
    });
});

function loadPermissions(unitId) {

    $('#permissionContainer')
        .html('<div class="text-center p-3">Loading permissions...</div>');

    $.ajax({
        url: '/UnitPermission/GetUnitPermissions',
        type: 'GET',
        data: {
            unitId: unitId
        },

        success: function (response) {
            $('#permissionContainer').html(response);
        },

        error: function (xhr) {

            $('#permissionContainer').html('');

            let message = 'Failed to load permissions.';

            if (xhr.status === 400) {
                message = 'Invalid unit selected.';
            }
            else if (xhr.status === 401) {
                message = 'Your session has expired. Please login again.';
            }
            else if (xhr.status === 403) {
                message = 'You are not allowed to view these permissions.';
            }

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: message
            });
        }
    });
}