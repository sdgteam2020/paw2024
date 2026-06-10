$(document).ready(function () {

  
    initializeDataTable('#tblPermission');

});


function OpenAddEdit(id) {

    $.ajax({
        url: '/PermissionMaster/AddEdit',
        type: 'GET',
        data: { id: id },

        success: function (response) {

            $('#permissionModalBody')
                .html(response);

            $('#permissionModal')
                .modal('show');
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

    var form =
        $('#frmPermissionMaster');

    if (!form.valid()) {
        return;
    }

    $.ajax({

        url:
            '/PermissionMaster/Save',

        type: 'POST',

        data:
            form.serialize(),

        success: function (response) {

            if (response.success) {

                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text:
                        response.message
                }).then(() => {

                    $('#permissionModal')
                        .modal('hide');

                    location.reload();
                });
            }
            else {

                Swal.fire({
                    icon: 'warning',
                    title: 'Warning',
                    html:
                        response.message
                });
            }
        },

        error: function () {

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text:
                    'Something went wrong.'
            });
        }
    });
}


function DeletePermission(id) {

    Swal.fire({

        title:
            'Are you sure?',

        text:
            'You want to delete this permission.',

        icon:
            'warning',

        showCancelButton:
            true,

        confirmButtonText:
            'Yes Delete'

    }).then((result) => {

        if (result.isConfirmed) {

            $.ajax({

                url:
                    '/PermissionMaster/Delete',

                type:
                    'POST',

                data: {

                    id: id,

                    __RequestVerificationToken:
                        $('input[name="__RequestVerificationToken"]')
                            .val()
                },

                success:
                    function (response) {

                        if (
                            response.success
                        ) {

                            Swal.fire({

                                icon:
                                    'success',

                                title:
                                    'Success',

                                text:
                                    response.message

                            }).then(() => {

                                location.reload();

                            });
                        }
                        else {

                            Swal.fire({

                                icon:
                                    'error',

                                title:
                                    'Error',

                                text:
                                    response.message
                            });
                        }
                    },

                error:
                    function () {

                        Swal.fire({

                            icon:
                                'error',

                            title:
                                'Error',

                            text:
                                'Something went wrong.'
                        });
                    }
            });
        }
    });
}