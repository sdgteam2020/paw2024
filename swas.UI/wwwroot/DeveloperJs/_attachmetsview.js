


    function functionConfirm1(ProjectId) {
        Swal.fire({
            title: 'Are you sure?',
            text: 'Do you want to delete?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, Delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Projects/Delete',
                    type: 'POST',
                    data: { "id": ProjectId, "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                    success: function (response) {
                        //console.log(response);

                        if (response) {
                            if (response >= 1) {
                                Swal.fire({
                                    position: 'top-end',
                                    icon: 'success',
                                    title: 'Record Deleted successfully',
                                    showConfirmButton: false,
                                    timer: 1500
                                });

                                var form = document.createElement('form');
                                form.method = 'POST';
                                form.action = '/Projects/Create';

                                var idInput = document.createElement('input');
                                idInput.type = 'hidden';
                                idInput.name = 'id';
                                idInput.value = response;

                                var tokenInput = document.createElement('input');
                                tokenInput.type = 'hidden';
                                tokenInput.name = '__RequestVerificationToken';
                                tokenInput.value = $('input[name="__RequestVerificationToken"]').val();

                                form.appendChild(idInput);
                                form.appendChild(tokenInput);

                                document.body.appendChild(form);
                                form.submit();

                            }
                        }
                    }
                });
            }
        });
    }




        $(document).ready(function () {

            $("#btnclose").click(function () {

                $('#myModal').modal('hide');
            });



            var table = $('#SoftwareTypes').DataTable();
    table.destroy();

    $('#SoftwareTypes').DataTable({
                lengthChange: false,
                buttons: ['copy', 'excel', 'csv', 'pdf', 'colvis']
            });

            table.buttons().container()
                .appendTo('#SoftwareTypes_wrapper .col-md-6:eq(0)');
        });





