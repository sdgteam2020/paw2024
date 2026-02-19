



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

        var table = $('#SoftwareTypesed').DataTable();
        table.destroy();



        $('#ddlActions').on('change', function () {
            var psmId = 0;
            var ddlActions = $(this).val();
            var ddlStages = $('#ddlStatus').val();
           
            psmId = '@Model.ProjMov.PsmId';
               
            var projId = psmId == 0 ? '@Model.DataProjId' : 0;


            $.ajax({
                url: '/Projects/ValidateAction',
                type: 'POST',
                data: {
                    "psmid": psmId,
                    "ActionId": ddlActions,
                    "proId": projId,
                    "ddlStag": ddlStages,

                    "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response) {
                        if (response === "Succeed") {
                            Swal.fire({
                                icon: 'success',
                                title: 'Found Eligible for this action',
                                showConfirmButton: false,
                                timer: 600
                            });
                        } else {

                            Swal.fire({
                                icon: 'error',
                                title: response,
                                showConfirmButton: true,
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    CallbackFunction();
                                }
                            });


                        }

                    }

                }
            });


       });






$('#SoftwareTypesed').DataTable({
            lengthChange: false,
            buttons: ['copy', 'excel', 'csv', 'pdf', 'colvis']
        });

        table.buttons().container()
            .appendTo('#SoftwareTypes_wrapper .col-md-6:eq(0)');
    });






    const pdfFileInput = document.getElementById('pdfFileInputed');

    pdfFileInput.addEventListener('change', function (event) {
        const file = event.target.files[0];

        if (file) {

            
              const maxSizeInBytes = 50 * 1024 * 1024; 
            if (file.size > maxSizeInBytes) {
                 $('#uploadButton').hide();
                pdfFileInputed.value = '';

                Swal.fire({
                    title: 'File Size Exceeded',
                    text: 'Please select a file smaller than 50MB.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
                return;
            }


            const reader = new FileReader();
            reader.onloadend = function () {
                const bytes = new Uint8Array(reader.result);
                const pdfHeader = new Uint8Array([37, 80, 68, 70, 45]); // %PDF-
                const isPDF = compareArrays(bytes.slice(0, 5), pdfHeader);
                if (isPDF) {

                    console.log('PDF file is valid. Proceed with upload.');
                } else {

                    Swal.fire({
                        title: 'Invalid File ....!',
                        text: 'Invalid PDF file. Please select a valid PDF file.',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                     $('#uploadButton').hide();
                    pdfFileInputed.value = '';
                }
            };


            reader.readAsArrayBuffer(file);
        }
    });


    function compareArrays(array1, array2) {
        if (array1.length !== array2.length) {
            return false;
        }
        for (let i = 0; i < array1.length; i++) {
            if (array1[i] !== array2[i]) {
                return false;
            }
        }
        return true;
    }







    $(document).ready(function () {
       
        function checkConditions() {
            var remarksLength = $('#Reamarksed').val().length;
            var pdfFileInput = $('#pdfFileInputed')[0].files.length;

            if (remarksLength > 1 && pdfFileInput > 0) {
                $('#uploadButtoned').show();
            } else {
                $('#uploadButtoned').hide();
            }
        }

        
        $('#Reamarksed, #pdfFileInputed').on('input change', function () {
            checkConditions();
        });

        $('#uploadButtoned').hide();
    });
