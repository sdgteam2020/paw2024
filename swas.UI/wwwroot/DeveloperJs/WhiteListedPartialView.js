// Function to handle the click event on the "Add" button



function GetwhilteListProject(TypeId) {
    let listItem = '';
    let userdata = {
        "TypeId": TypeId
        
    };
    $.ajax({
        type: "POST",
        url: "/Home/GetWhiteListedActionProj",
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        success: function (data) {
            debugger;
            console.log(data);
            if (data != null) {
                let count = 1;  // Start count from 1
                let listItem = '';  // Initialize listItem as an empty string
                
                Object.values(data.projects).forEach(function (item) {  // Corrected `forEach` loop syntax
                    listItem += '<tr>';
                    listItem += '<td class="s-no-column">' + count++ + '</td>';  // Increment count
                    var shortName = item.projName ||"";
                    if (shortName.length > 10) {
                        shortName = item.projName.slice(0, 10) + "....";
                    }
                    listItem += ' <td class=" RefLetter-container" style="width:8%"><div class="noExport">' + shortName +
                       '</div><div class="RefLetter">'+
                            item.projName +
                    '  </div> </td>';
                       
                    listItem += '<td class="" style="width:8%">' + item.hostedOn + '</td>';
                    listItem += '<td>' + item.appt + '</td>';
                    listItem += '<td>' + item.fmn + '</td>';
                    listItem += '<td class="col-width6">' + item.contactNo + '</td>';
                    listItem += '<td class="" style="width:6%">' + DateFormated(item.clearence) + '</td>';
                    //var shortCetNo = item.certNo || "";
                    //if (shortCetNo.length > 10) {
                    //    shortCetNo = item.certNo.slice(0, 10) + "....";
                    //}
                    listItem += '<td  style="width:15%">' + item.certNo + 
                    '  </td>';  // Use item.Remarks instead of @unitx.Remarks

                /*    listItem += '<td>' + item.certNo + '</td>'; */ // Use item.CertNo instead of @unitx.CertNo
                    if (item.clearence != null) {
                        let clearenceDate = new Date(item.clearence);
                        var afterThreeYears = new Date(clearenceDate.setFullYear(clearenceDate.getFullYear() + 3));
                    }
                 
                
                    listItem += '<td class="col-width6">' + DateFormated(afterThreeYears) + '</td>';  // Use item.ValidUpto instead of unitx.ValidUpto
                    var shortRemarks = item.remarks || "";
                    var Small = shortRemarks; 
                    if (shortRemarks.length > 10) {
                        shortRemarks = item.remarks.slice(0, 10) + "....";
                    }
                    listItem += '<td class="RefLetter-container">' + shortRemarks 
                    if (shortRemarks != "") {
                        listItem += ' <div class="RefLetter" >' + item.remarks + '</div>';
                          
                          

                    }
                    listItem +=  ' </td>';  // Use item.Remarks instead of @unitx.Remarks
                   
                    listItem += '</tr>';
                });
                if ($.fn.DataTable.isDataTable("#WhitelistedTable")) {
                    $("#WhitelistedTable").DataTable().clear().destroy();
                }

                $("#WhitelistedTableData").html(listItem);
                $("#hostedOn").empty().append(`<option value="">-- Select Hosted On --</option>`);

                data.hostTypes.forEach(ht => {
                    $("#hostedOn").append(
                        `<option value="${ht.hostTypeID}">${ht.hostingDesc}</option>`
                    );
                });

                // 🔹 Units Dropdown (Sponsor)
                $("#sponsor").empty().append(`<option value="">-- Select Unit --</option>`);

                data.units.forEach(u => {
                    $("#sponsor").append(
                        `<option value="${u.unitId}">${u.unitName}</option>`
                    );
                });

                initializeDataTable("#WhitelistedTable")// Insert the generated rows into the table

            }
           
           
        },
        error: function (xhr, status, error) {
            console.error("Error loading white list projects:", error);
        }
    });
}

$('#AddWhite_proj').on('click', function () {

    $("input[required], select[required], textarea[required]").removeClass("is-invalid");
    $("input[required], select[required], textarea[required]").siblings(".invalid-feedback").hide();

    $('#WhiteListModal').find('input').val('');

    $('#WhiteListModal').find('select').prop('selectedIndex', 0);
    $('#WhiteListModal').find('textarea').val('');
    $('#WhiteListModal').modal('show');
    $('#WhiteListedProjectDetail .modal-content').css('filter', 'blur(4px)');
});

$('#WhiteListModal').on('shown.bs.modal', function () {
    $('#sponsor').select2({
        placeholder: "-- Select Sponsor --",
        allowClear: true,

        width: '100%',
        dropdownParent: $('#WhiteListModal')
    });


   



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
        }, 200)

    });

    $('#WhiteListedProjectDetail .modal-content').on('click', function (e) {
        e.stopPropagation(); // prevent closing when clicking inside modal
    });
});


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




$('.form-control').keypress(function (e) {
    // Get the key code of the pressed key
    // Get the key code of the pressed key

    var keyCode = e.which;

    // Allow only alphabets (A-Z, a-z) and numbers (0-9)
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

        if (!value) {

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
        alert("valid")
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
