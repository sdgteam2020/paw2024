$(document).ready(function () {
    initializeDataTable("#stagesTable");
    
    $('.delete-btn').click(function () {
         
        let stageId = $(this).data('id');

        let url = $(this).data('url');

        $.ajax({
            url: '/Stages/DeleteConfirmed',
            type: 'POST',
            data: { stageId: stageId },
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


    $('.btnupdate').click(function () {
         
        let stageId = $(this).data('id');
        let Stages = $(this).data('name');

        let url = $(this).data('url');

        $.ajax({
            url: '/Stages/EditFrom',
            type: 'POST',
            data: {
                stageId: id,
                Stages: stage
            },
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



});

    function validateForm() {
        let form = document.querySelector('form.needs-validation');
    if (form.checkValidity() === false) {
        event.preventDefault();
    event.stopPropagation();
        }
    form.classList.add('was-validated');
    return form.checkValidity();
    }
