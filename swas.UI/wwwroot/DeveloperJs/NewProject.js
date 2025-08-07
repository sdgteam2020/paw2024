

$(document).ready(function () {
    initializeDataTable('#WhitelistedTable1');
    $("#ddlUnitId").change(function () {
        var selectedMode = $(this).val();
    });
    $("#projunderprocess").click(function () {

       
        $("#ProjunderprocessModal").modal('show');
        
    });
    $("#projWhiteListed").click(function () {


        $("#ProjWhiteListedProjectModal").modal('show');

    });

    //$(".EditWhiteListedProj").click(function (e) {

    //    var id = $(this).data('id'); 
    //    $('#WhiteListedProjectDetail').modal('hide');

    //    $.ajax({
    //        url: '/Home/GetWhiteListedProjectById',
    //        type: 'GET',
    //        data: { id: id },
    //            success: function (data) {
    //                if (data) {
    //                    $('#edit_Id').val(data.id);
    //                    $('#edit_ProjName').val(data.projName);
                       
    //                    var hostedOnMap = {
    //                        "1": "LAN",
    //                        "2": "ADN",
    //                        "3": "Internet",
    //                        "4": "Standalone"
    //                    };

    //                    var hostedOnVal = data.mHostTypeId ? data.mHostTypeId.toString() : "";
    //                    $('#edit_HostedOn').val(hostedOnVal);
                      
    //                    if ($('#edit_HostedOn').val() != hostedOnVal) {
    //                        $('#edit_HostedOn option').each(function () {
    //                            if ($(this).val() == hostedOnVal) {
    //                                $(this).prop('selected', true);
    //                            } else {
    //                                $(this).prop('selected', false);
    //                            }
    //                        });
    //                    }                        
    //                    $('#edit_Appt').val(data.appt);
    //                    $('#edit_Sponser').val(data.fmn);
    //                    $('#edit_TelNo').val(data.contactNo);
    //                    $('#edit_Clearance').val(data.clearence ? data.clearence.substring(0, 10) : '');
    //                    $('#edit_Cert').val(data.certNo);
    //                    $('#edit_ValidUpto').val(data.validUpto ? data.validUpto.substring(0, 10) : '');
    //                    $('#edit_Remarks').val(data.remarks);

    //                    $('#EditWhiteListedProjectModal').modal('show');
    //                }
    //            },
    //        error: function () {
    //            alert('Failed to fetch project details.');
    //        }
    //    });
    //});

    $('#WhitelistedTable tbody').on('click', '.EditWhiteListedProj', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $('#WhiteListedProjectDetail').modal('hide');

        $.ajax({
            url: '/Home/GetWhiteListedProjectById',
            type: 'GET',
            data: { id: id },
            success: function (data) {
                if (data) {
                    $('#edit_Id').val(data.id);
                    $('#edit_ProjName').val(data.projName);

                    var hostedOnMap = {
                        "1": "LAN",
                        "2": "ADN",
                        "3": "Internet",
                        "4": "Standalone"
                    };

                    var hostedOnVal = data.mHostTypeId ? data.mHostTypeId.toString() : "";
                    $('#edit_HostedOn').val(hostedOnVal);

                    if ($('#edit_HostedOn').val() != hostedOnVal) {
                        $('#edit_HostedOn option').each(function () {
                            if ($(this).val() == hostedOnVal) {
                                $(this).prop('selected', true);
                            } else {
                                $(this).prop('selected', false);
                            }
                        });
                    }
                    $('#edit_Appt').val(data.appt);
                    $('#edit_Sponser').val(data.fmn);
                    $('#edit_TelNo').val(data.contactNo);
                    $('#edit_Clearance').val(data.clearence ? data.clearence.substring(0, 10) : '');
                    $('#edit_Cert').val(data.certNo);
                    $('#edit_ValidUpto').val(data.validUpto ? data.validUpto.substring(0, 10) : '');
                    $('#edit_Remarks').val(data.remarks);

                    $('#EditWhiteListedProjectModal').modal('show');
                }
            },
            error: function () {
                alert('Failed to fetch project details.');
            }
        });
    })
    $('#editWhiteListedForm').submit(function (e) {
        e.preventDefault();       
        $.ajax({
            url: '/Home/UpdateWhiteListedProject', 
            type: 'POST',
            data: $('#editWhiteListedForm').serialize(),
            success: function (data) {
                if (data == 1) {
                    
                    Swal.fire({
                        title: 'Success!',
                        text: 'Updated Successfully',
                        icon: 'success',
                        timer: 3000,             
                        timerProgressBar: true,
                        showConfirmButton: false, 
                        willClose: () => {
                            $('#EditWhiteListedProjectModal').modal('hide');
                            location.reload();
                        }
                    });
                }
                else
                {
                    Swal.fire({
                        title: 'Error!',
                        text: 'Update Failde!',
                        icon: 'error',
                        timer: 2000,
                        confirmButtonText: 'OK'
                    });
                }                
            },
            error: function () {
                Swal.fire({
                    title: 'Error!',
                    text: 'Something went wrong!',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        });
    });

});

//$(document).on('click', '.EditWhiteListedProj', function (e) {
//    e.preventDefault();
//    var id = $(this).data('id');
//    $('#WhiteListedProjectDetail').modal('hide');

//    $.ajax({
//        url: '/Home/GetWhiteListedProjectById',
//        type: 'GET',
//        data: { id: id },
//        success: function (data) {
//            if (data) {
//                $('#edit_Id').val(data.id);
//                $('#edit_ProjName').val(data.projName);

//                var hostedOnVal = data.mHostTypeId ? data.mHostTypeId.toString() : "";
//                $('#edit_HostedOn').val(hostedOnVal);

//                if ($('#edit_HostedOn').val() != hostedOnVal) {
//                    $('#edit_HostedOn option').each(function () {
//                        $(this).prop('selected', $(this).val() == hostedOnVal);
//                    });
//                }

//                $('#edit_Appt').val(data.appt);
//                $('#edit_Sponser').val(data.fmn);
//                $('#edit_TelNo').val(data.contactNo);
//                $('#edit_Clearance').val(data.clearence ? data.clearence.substring(0, 10) : '');
//                $('#edit_Cert').val(data.certNo);
//                $('#edit_ValidUpto').val(data.validUpto ? data.validUpto.substring(0, 10) : '');
//                $('#edit_Remarks').val(data.remarks);

//                $('#EditWhiteListedProjectModal').modal('show');
//            }
//        },
//        error: function () {
//            alert('Failed to fetch project details.');
//        }
//    });
//});


function ValInData(input) {
    var regex = /[^a-zA-Z0-9/ ]/g;
    input.value = input.value.replace(regex, "");
}

function ButtonClick() {

    if (ButtonText === 'Sign In' && flag === 'True') {
        var signInUrl = '/Home/Index';
        window.location.href = signInUrl;
    }
    else {
        if (ButtonText === 'Sign Up') {
            var signUpUrl = '/Identity/Account/Register';
            window.open(signUpUrl, '_blank');
        }
        else {
            Swal.fire({
                title: 'Warning!',
                /*text: 'Contact DDGIT for Necessary Admin Approval (Tel No: 39865, 20862706)',*/
                html: `Contact DDGIT for Necessary Admin Approval<br><strong>(Tele No:</strong> 39865, 20862706)`,
                icon: 'warning',
                confirmButtonColor: '#ffc107',
                confirmButtonText: 'OK'
            }).then((result) => {
                // Check if the user clicked the OK button
                if (result.isConfirmed) {
                    // Clear session on the server-side via an AJAX request
                    $.ajax({
                        url: '/Home/ClearSession', // The server-side action to clear the session
                        type: 'POST',
                        success: function () {
                            // Once session is cleared, open the login page
                            var signUpUrl = '/Identity/Account/Login';
                            window.open(signUpUrl, '_self'); // Open login in the same window or tab
                        },
                        error: function () {
                            // Handle error if the session could not be cleared
                            Swal.fire({
                                title: 'Error!',
                                //text: 'Contact DDGIT for Necessary Admin Approval (Tel No: 39865, 20862706)',
                                html: `Contact DDGIT for Necessary Admin Approval<br><strong>(Tele No:</strong> 39865, 20862706)`,
                                icon: 'error',
                                confirmButtonText: 'OK'
                            });
                        }
                    });
                }
            });
            return false;
        }

    }
}

$("#telNo").on("keypress", function () {
    var input = $(this).val();
    input = input.replace(/\D/g, '');

    // Optional: limit to 10 digits
    if (input.length > 10) {
        input = input.substring(0, 10);
    }

    $(this).val(input);
});
$("#telNo").on("keypress", function (e) {
    var charCode = e.which ? e.which : e.keyCode;

    // Allow only digits (0–9)
    if (charCode < 48 || charCode > 57) {
        e.preventDefault();
        $(this).siblings(".invalid-feedback")
            .text("Only Numbers are allowd.")
            .show();
    }

    else {
        $(this).siblings(".invalid-feedback")
            .text("Only Numbers are allowd.")
            .hide();
    }
});
var validPattern = /^[a-zA-Z0-9 ]*$/;

$("#swName, #hostedOn,  #certNo, #remarks, #appt").on("input", function () {
    var currentVal = $(this).val();
    var maxLength = 200;

    // For remarks, use 500
    if ($(this).attr('id') === 'remarks') {
        maxLength = 500;
    }

    if (!validPattern.test(currentVal)) {
        // Remove any invalid characters
        $(this).val(currentVal.replace(/[^a-zA-Z0-9 ]/g, ""));
        $(this).siblings(".invalid-feedback")
            .text("Special characters are not allowed.")
            .show();
    }
    else if (currentVal.length > maxLength) {
        isValid = false;

        $(this).addClass("is-invalid");
        $(this).siblings(".invalid-feedback")
            .text("Cannot exceed " + maxLength + " characters.")
            .show();

    }

    else {
        $(this).siblings(".invalid-feedback").hide();
    }
    $("input[required], textarea[required]").on("input", function () {
        var maxLength = 200;
        if ($(this).attr('id') === 'remarks') {
            maxLength = 500;
        }

        var value = $(this).val();

        if (value && value.trim() !== "" && value.length <= maxLength) {
            $(this).removeClass("is-invalid");
            $(this).siblings(".invalid-feedback").hide();
        }
    });
});


$("#btn_Save").on('click', function (e) {
    e.preventDefault();
    debugger;
    // Gather form inputs
    var formData = {
        ProjName: $('#swName').val(),
        mHostTypeId: $('#hostedOn').val(),
        appt: $('#appt').val(),
        Fmn: $('#sponsor').val(),
        ContactNo: $('#telNo').val(),
        Clearence: $('#clearanceDate').val(),
        CertNo: $('#certNo').val(),
        ValidUpto: $('#validUpto').val(),
        Remarks: $('#remarks').val(),
    };
   
    // Validation
    var isValid = true;
   
    $("input[required], select[required], textarea[required]").each(function () {
        debugger;
        var value = $(this).val();
        var maxLength = 200;

        // For remarks, use 500
        if ($(this).attr('id') === 'remarks') {
            maxLength = 500;
        }

        if (!value || value.trim() === "") {
           
            isValid = false;

            $(this).addClass("is-invalid");
            $(this).siblings(".invalid-feedback").text("This field is required.").show();

        } else if (value.length > maxLength) {
            isValid = false;

            $(this).addClass("is-invalid");
            $(this).siblings(".invalid-feedback")
                .text("Cannot exceed " + maxLength + " characters.")
                .show();

        } else {
            $(this).removeClass("is-invalid");
            $(this).siblings(".invalid-feedback").hide();
        }
    });

    // Remove invalid class and feedback when user fixes input
    $("input[required], textarea[required]").on("input", function () {
        var maxLength = 200;
        if ($(this).attr('id') === 'remarks') {
            maxLength = 500;
        }

        var value = $(this).val();

        if (value && value.trim() !== "" && value.length <= maxLength) {
            $(this).removeClass("is-invalid");
            $(this).siblings(".invalid-feedback").hide();
        }
    });

    // For selects, use change event
    $("select[required]").on("change", function () {
        var value = $(this).val();
        if (value && value.trim() !== "") {
            $(this).removeClass("is-invalid");
            $(this).siblings(".invalid-feedback").hide();
        }
    });


   

    // Proceed with AJAX if validation passes
    if (isValid) {
        $.ajax({
            url: '/Home/SaveWhiteList',
            type: 'POST',
            data: formData,
            success: function (response) {

                $('#WhiteListedProjectDetail .modal-content').css('filter', '');
                $('#WhiteListModal').modal('hide');



                Swal.fire({
                    title: 'Success',
                    text: response.message,
                    icon: 'success',
                    confirmButtonText: 'OK'
                }).then(() => {
                    // Refresh the page to reload the updated list
                    location.reload();
                });

            },
            error: function (xhr, status, error) {
                Swal.fire('Error', xhr.responseJSON ? xhr.responseJSON.message : 'An error occurred', 'error');
            }
        });
    } else {
        console.log("This is not valid");
    }
});

function cancelModal(elem) {

    $('#WhiteListedProjectDetail .modal-content').css('filter', '');
}

$(document).ready(function () {
    $(document).on('click', function () {
        setTimeout(function () {

        if ($('#WhiteListModal').is(':hidden')) {


            $('#WhiteListedProjectDetail .modal-content').css('filter', '');
        }
        },200)
        
    });

    $('#WhiteListedProjectDetail .modal-content').on('click', function (e) {
        e.stopPropagation(); // prevent closing when clicking inside modal
    });
});

