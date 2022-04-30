$('.delete-btn').click(function () {

    let rankId = $(this).data('id');

    let url = $(this).data('url');

    $.ajax({
        url: '/Rank/Delete',
        type: 'POST',
        data: { rankId: rankId },
        success: function (response) {
            if (response === 1) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: 'Record Deleted successfully',
                    showConfirmButton: false,
                    timer: 1500
                }).then(function () {
                    window.location.href = url;
                });
            }
            else {


            }
        },
        error: function (xhr, status, error) {

        }
    });
});