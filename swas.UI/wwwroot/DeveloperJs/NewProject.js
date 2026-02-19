
$(document).ready(function () {
 
   
    initializeDataTable('#WhitelistedTable');
    const requestIncrementWithDomainId = {
        method: "POST",
        redirect: "follow"
    };
    $(document).on('click', '[data-cancel="true"]', function () {
        cancelModal(this);
    });


  
    $("#ddlUnitId").change(function () {
        var selectedMode = $(this).val();
    });
    $("#projunderprocess").click(function () {

       
        $("#ProjunderprocessModal").modal('show');
        
    });
    $("#projWhiteListed").click(function () {


        $("#ProjWhiteListedProjectModal").modal('show');

    });

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


function ValInData(input) {
    var regex = /[^a-zA-Z0-9/ ]/g;
    input.value = input.value.replace(regex, "");
}


$('#ProcessId').on('click', function(){
    ButtonClick();
})

function ButtonClick() {
    
    var ButtonText = $('#ProcessId').data('button-text');
    var flag = $('#ProcessId').data('flag');
   

    if (ButtonText === 'Sign In' &&flag === 'True') {
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
                
                html: `Contact DDGIT for Necessary Admin Approval<br><strong>(Tele No:</strong> 39865, 20862706)`,
                icon: 'warning',
                confirmButtonColor: '#ffc107',
                confirmButtonText: 'OK'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Home/ClearSession', // The server-side action to clear the session
                        type: 'POST',
                        success: function () {
                            var signUpUrl = '/Identity/Account/Login';
                            window.open(signUpUrl, '_self'); // Open login in the same window or tab
                        },
                        error: function () {
                            Swal.fire({
                                title: 'Error!',
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
    if (input.length > 10) {
        input = input.substring(0, 10);
    }

    $(this).val(input);
});
$("#telNo").on("keypress", function (e) {
    var charCode = e.which ? e.which : e.keyCode;
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




$('.form-control').keypress(function (e) {
  
    var keyCode = e.which;
    if ((keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (keyCode >= 48 && keyCode <= 57) || (keyCode == 32)) {
        $(this).siblings(".invalid-feedback").hide();
        return true; // Allow the keypress
    } else {

        if (keyCode == 46 || keyCode == 44 || keyCode == 40 || keyCode == 41 || keyCode == 45 || keyCode == 58 || keyCode == 47 || keyCode == 13 || keyCode == 38)

            return true; // Allow the keypress
        else {
            $(this).siblings(".invalid-feedback")
                .text("Special characters are not allowed.")
                .show();
            return false; // Block the keypress
        }

    }
  
});

$("#hostedOn,  #certNo, #remarks, #appt").on("input", function () {
    var currentVal = $(this).val();
    var maxLength = 200;
    if ($(this).attr('id') === 'remarks') {
        maxLength = 500;
    }

    if (!validPattern.test(currentVal)) {
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
    var isValid = true;
   
    $("input[required], select[required], textarea[required]").each(function () {
        
        var value = $(this).val();
        var maxLength = 200;
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
    $("select[required]").on("change", function () {
        var value = $(this).val();
        if (value && value.trim() !== "") {
            $(this).removeClass("is-invalid");
            $(this).siblings(".invalid-feedback").hide();
        }
    });
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
function cancelModal(button) {
    $('#WhiteListModal').modal('hide');
    $('#WhiteListForm')[0].reset();
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

$(document).ready(function () {
    $("#CommentProject").autocomplete({
        source: function (request, response) {
            var query = request.term;
            if (query.length > 3) { // Only trigger search when length > 3
                $.ajax({
                    url: '/Projects/FindProjectForComment',
                    type: 'POST',
                    data: {
                        searchQuery: query
                    },
                    success: function (data) {
                        if (data && data.length > 0) {
                            response(data.map(function (item) {
                                return {
                                    label: item.projectName,   // Displayed text in the dropdown
                                    value: item.projectName,   // Value when selected
                                    encyId: item.encyID,
                                    projid: item.projId,
                                    psmid: item.psmId,
                                    stkholder: item.stakeholder,
                                    timeStamp: item.timeStamp,
                                    statusId: item.stkStatusId,
                                    adminApprovalStatus: item.adminApprovalStatus
                                };
                            }));
                        }

                    },
                    error: function (error) {
                        console.error('Error fetching project names:', error);
                    }
                });
            }
        },
        minLength: 4, // Minimum characters before search is triggered
        select: function (event, ui) {
            let selectedOption = ui.item;

            var date = new Date(selectedOption.timeStamp);
            var formattedDate =
                ("0" + date.getDate()).slice(-2) + '-' +
                ("0" + (date.getMonth() + 1)).slice(-2) + '-' +
                date.getFullYear() + ' ' +
                ("0" + date.getHours()).slice(-2) + ':' +
                ("0" + date.getMinutes()).slice(-2) + ':' +
                ("0" + date.getSeconds()).slice(-2);
            let statusText = "";
            let buttonClass = "";

            if (selectedOption.statusId == 1) {
                statusText = "Accepted";
                buttonClass = "btn-success";
            } else if (selectedOption.statusId == 5) {
                statusText = "Info";
                buttonClass = "btn-success";
            } else if (selectedOption.statusId == 2) {
                statusText = "Obsn";
                buttonClass = "btn-warning";
            } else if (selectedOption.statusId == 3) {
                statusText = "Rejected";
                buttonClass = "btn-danger";
            } else {
                statusText = "Pending";
                buttonClass = "btn-danger";
            }
            let newRow = `
            <tr class="cmntrow">
                <td class='noExport d-none'><span class='noExport d-none' id='spnProjId'>${selectedOption.projid}</span><span class='noExport d-none' id='spnpsmId'>${selectedOption.psmid}</span><span class='noExport d-none' id='DateType'>${selectedOption.adminApprovalStatus}</span></td>
                <td class="s-no-column">${1}</td>
                <td class='align-middle'>
                    <a href='/Projects/ProjHistory?EncyID=${encodeURIComponent(selectedOption.encyId)}'>
                        <span id='projectName' class='projNameDetail'>${selectedOption.value}</span>
                    </a>
                </td>
                <td>${selectedOption.stkholder}</td>
                <td>${formattedDate}</td>
                <td>${statusText}</td>
                <td class="align-middle">
                    <span id="btnedit">
                        <button  type="button" class="cls-btncomment btn-icon btn-round tbncmnt ${buttonClass} mr-1">
                            <i class="fas fa-comment"></i>
                        </button>
                    </span>
                </td>
            </tr>
        `;



            $("#DetailBody").html("");
            $("#DetailBody").prepend(newRow);
            setTimeout(() => {
                $("#CommentProject").val('');
            }, 0);
        }
    });

})

