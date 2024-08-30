

function AddNotification(ProjId, type, unitid) {

    $.ajax({
        url: '/Notification/AddNotification',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "type": type,
            "unitid": unitid, "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            console.log(response);
            if (response && response === 1) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: 'Project Notification Added  successfully',
                    showConfirmButton: false,
                    timer: 700
                });
            }

            window.location.reload();
        },
        error: function (error) {
            console.error('Error occurred:', error);
            // Handle error if needed
        }
    });
}

function IsReadNotification(ProjId, type) {
    $.ajax({
        url: '/Notification/IsReadNotification',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "type": type
        },
        success: function (response) {
            console.log(response);

        }
    });
}

function UnReadNotification(ProjId, type) {
    $.ajax({
        url: '/Notification/UnReadNotification',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "type": type
        },
        success: function (response) {
            console.log(response);

        }
    });
}