$(document).ready(function () {
    // ===========================
    // Initialize Plugins
    // ===========================
    initializeDataTable('#tblPermission');

    $('.select2').select2({
        width: '100%'
    });

    // ===========================
    // Permission Master Events
    // ===========================
    $(document).on('click', '#btnAddPermission', function () {
        OpenAddEdit(0);
    });

    $(document).on('click', '.btnEditPermission', function () {
        OpenAddEdit($(this).data('id'));
    });

    $(document).on('click', '.btnDeletePermission', function () {
        DeletePermission($(this).data('id'));
    });

    $(document).on('click', '#btnSavePermission', function () {
        SavePermission();
    });

    // ===========================
    // Permission Control Events
    // ===========================
    $('#PermissionFor').on('change', function () {
        loadTargets();
    });

    $('#btnLoadPermissions').on('click', function () {
        loadPermissions();
    });

    $(document).on('click', '#btnSavePermissions', function () {
        savePermissionControl();
    });

    $(document).on('click', '#btnSelectAll', function () {
      
        checkAllPermissions(true);
    });

    $(document).on('click', '#btnUnselectAll', function () {
        checkAllPermissions(false);
    });
});

// ===========================
// Core Functions
// ===========================

function OpenAddEdit(id) {
    $.ajax({
        url: '/PermissionMaster/AddEdit',
        type: 'GET',
        data: { id: id },
        success: function (response) {
            $('#permissionModalBody').html(response);
            $('#permissionModal').modal('show');
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Unable to load data.'
            });
        }
    });
}

function SavePermission() {
    const form = $('#frmPermissionMaster');

    if (!form.valid()) {
        return;
    }

    $.ajax({
        url: '/PermissionMaster/Save',
        type: 'POST',
        data: form.serialize(),
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: response.message
                }).then(() => {
                    $('#permissionModal').modal('hide');
                    location.reload();
                });
            } else {
                Swal.fire({
                    icon: 'warning',
                    title: 'Warning',
                    html: response.message
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Something went wrong.'
            });
        }
    });
}

function DeletePermission(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'You want to delete this permission.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes Delete'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/PermissionMaster/Delete',
                type: 'POST',
                data: {
                    id: id,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message
                        }).then(() => {
                            location.reload();
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: response.message
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Something went wrong.'
                    });
                }
            });
        }
    });
}

function loadTargets() {
    debugger;
    let permissionFor = $('#PermissionFor').val();

    $('#TargetId').empty().append('<option value="">-- Select --</option>');
    $('#permissionContainer').html(
        '<div class="alert alert-info mb-0">Please select target and load permissions.</div>'
    );

    if (!permissionFor) {
        return;
    }

    $.ajax({
        url: '/PermissionControl/GetTargets',
        type: 'GET',
        data: { permissionFor: permissionFor },
        success: function (response) {
            if (response.success) {
                $.each(response.data, function (index, item) {
                    $('#TargetId').append(`<option value="${item.value}">${item.text}</option>`);
                });
                $('#TargetId').trigger('change');
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.message
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Unable to load records.'
            });
        }
    });
}

function loadPermissions() {
    let permissionFor = $('#PermissionFor').val();
    let targetId = $('#TargetId').val();

    if (!permissionFor) {
        Swal.fire({
            icon: 'warning',
            title: 'Required',
            text: 'Please select permission type.'
        });
        return;
    }

    if (!targetId) {
        Swal.fire({
            icon: 'warning',
            title: 'Required',
            text: 'Please select role/user/unit.'
        });
        return;
    }

    $('#permissionContainer').html('<div class="text-center p-4">Loading permissions...</div>');

    $.ajax({
        url: '/PermissionControl/GetPermissions',
        type: 'GET',
        data: {
            permissionFor: permissionFor,
            targetId: targetId
        },
        success: function (html) {
            $('#permissionContainer').html(html);
        },
        error: function (xhr) {
            let msg = 'Unable to load permissions.';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                msg = xhr.responseJSON.message;
            }
            $('#permissionContainer').html(`<div class="alert alert-danger mb-0">${msg}</div>`);
        }
    });
}

function savePermissionControl() {
    let permissionFor = $('#PermissionFor').val();
    let targetId = $('#TargetId').val();
    let permissions = [];

    $('.permission-control-checkbox').each(function () {
        permissions.push({
            permissionId: parseInt($(this).data('permission-id')),
            permissionKey: $(this).data('permission-key'),
            displayName: $(this).data('display-name'),
            isSelected: $(this).is(':checked')
        });
    });

    let model = {
        permissionFor: permissionFor,
        targetId: targetId,
        permissions: permissions
    };

    $.ajax({
        url: '/PermissionControl/SavePermissions',
        type: 'POST',
        contentType: 'application/json',
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        data: JSON.stringify(model),
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: response.message,
                    timer: 1800,
                    showConfirmButton: false
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.message
                });
            }
        },
        error: function (xhr) {
            let msg = 'Unable to save permissions.';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                msg = xhr.responseJSON.message;
            }
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: msg
            });
        }
    });
}

function checkAllPermissions(status) {
    $('.permission-control-checkbox').prop('checked', status);
}