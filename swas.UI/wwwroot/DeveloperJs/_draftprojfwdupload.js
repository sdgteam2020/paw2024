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
                url: '/Projects/DeleteConfDft',
                type: 'POST',
                data: { "id": ProjectId, "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                success: function (response) {

                    if (response) {
                        if (response >= 1) {
                            var rowToRemove = $('#SoftwareTypes tbody').find('a[data-id="' + ProjectId + '"]').closest('tr');
                            rowToRemove.remove();

                            Swal.fire({

                                icon: 'success',
                                title: 'Record Deleted successfully',
                                showConfirmButton: false,
                                timer: 1500
                            });



                        }
                    }
                }
            });
        }
    });
}



$(document).ready(function () {

    popRestddlfwdUnit(@Model.DataProjId);


    populateStages();

    $('#ddlStages').on('change', function () {

        var stageIds = $(this).val();

        if (stageIds > 0) {
            $('#ddlStatus').empty();
            $('#ddlActions').empty();
            getStatusByStage(stageIds);

        } else {
            $('#ddlStatus').empty();
            $('#ddlActions').empty();
        }
    });

    $('#ddlStatus').on('change', function () {
        var selectedStatusId = $(this).val();
        var selectedStageIds = $('#ddlStages').val();
        if (selectedStatusId > 0) {
            $('#ddlActions').empty();
            EditActionsByStatus(selectedStatusId, selectedStageIds);
        } else {
            $('#ddlActions').empty();
        }
    });

    ddlUnitId.selectedStatusId == @Model.ProjMov.ToUnitId;







    var table = $('#SoftwareTypes').DataTable();
    table.destroy();

    $('#SoftwareTypes').DataTable({
        lengthChange: false,
        buttons: ['copy', 'excel', 'csv', 'pdf', 'colvis']
    });

    table.buttons().container()
        .appendTo('#SoftwareTypes_wrapper .col-md-6:eq(0)');





    $('#ddlActions').on('change', function () {
        var ddlActions = $(this).val();
        var ddlStages = $('#ddlStatus').val();
        var psmId = '@Model.ProjMov.PsmId';
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


                        $('#ddlStages').empty();
                        $('#ddlStatus').empty();
                        $('#ddlActions').empty();
                        populateStages();
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




});


const pdfFileInput = document.getElementById('pdfFileInput');

pdfFileInput.addEventListener('change', function (event) {
    const file = event.target.files[0];

    if (file) {


        const maxSizeInBytes = 50 * 1024 * 1024;
        if (file.size > maxSizeInBytes) {
            $('#uploadButton').hide();
            pdfFileInput.value = '';
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
            const pdfHeader = new Uint8Array([37, 80, 68, 70, 45]);
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
                pdfFileInput.value = '';
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
    $('.dropdownsearch').select2();
});


$(document).ready(function () {

    function submitFormnew(PsmId, event) {

        event.preventDefault();
        var curPSMid = 0;

        $.ajax({
            type: 'POST',
            url: '/Projects/FwdProjConfirm',
            data: { "projid": PsmId },
            datatype: "json",
            success: function (response) {
                Swal.fire({
                    title: '** Fwd ** ',
                    text: '..Proj...Fwd',
                    icon: 'success',
                    confirmButtonText: 'OK'
                }).then(function () {

                    window.location.href = '/Projects/ProjDetails';
                });
            },
            error: function (error) {
                console.error('Error occurred:', error);
            }
        });
    }


    $("#uploadfinal").click(function (event) {


        submitFormnew(@Model.ProjMov.PsmId, event);

        event.preventDefault();
        var fdset = "fieldset#" + "4";

        $(fdset).show();
        $("fieldset#upload").hide();
        $("fieldset#2").hide();
        $("fieldset#1").hide();
        animateProgressBar();
    });
});


$(document).ready(function () {

    function checkConditions() {
        var remarksLength = $('#AttHisAdd_Reamarks').val().length;
        var pdfFileInput = $('#pdfFileInput')[0].files.length;

        if (remarksLength > 1 && pdfFileInput > 0) {
            $('#uploadButton').show();
        } else {
            $('#uploadButton').hide();
        }
    }


    $('#AttHisAdd_Reamarks, #pdfFileInput').on('input change', function () {
        checkConditions();
    });

    $('#uploadButton').hide();
});