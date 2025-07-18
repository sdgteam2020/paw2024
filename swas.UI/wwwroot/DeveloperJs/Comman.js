$(document).ready(function () {
   
    var pad = "00"
    var datef2 = new Date();
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    var hh = pad.substring(0, pad.length - `${datef2.getHours()}`.length) + `${datef2.getHours()}`;
    var mm = pad.substring(0, pad.length - `${datef2.getMinutes()}`.length) + `${datef2.getMinutes()}`;
   // var mm = `${datef2.getMinutes()}`;
    var ss = `${datef2.getSeconds()}`;

    //var today = new Date().toISOString().split('T');  // Get today's date in YYYY-MM-DD format

    //today = today[0] + 'T' + today[1].substring(0,5);
    //var today = year + `-` + monthsans + `-` + dayans + `T` + hh + `:` + mm


    //var today = year + `-` + monthsans + `-` + dayans;

    //if ($("#isclaneder").html() == 1) {

    //    $('input[type=date]').attr('min', today);
    //    $('.datepicker1').datepicker({
    //        minDate: 0
    //    });
    //    $("#InitiatedDate").val(today);
    //    $('#InitiatedDate').attr('readonly', true);
    //}
    //else {
    //    $('input[type=date]').attr('max', today);
    //    $('.datepicker1').datepicker();

    //}
    //$('.datetimepicker1').datepicker();

    //// Remove the max date setting for CompletionDate to allow future selection
    //$("#InitiatedDate").change(function () {

    //    $('#CompletionDate').val("");
    //    $('#CompletionDate').attr('min', $("#InitiatedDate").val());
    //    $('input[type=date]').attr('max', null);
    //})


    var today = year + `-` + monthsans + `-` + dayans;


    function applyDateLogic() {
        debugger;
        var selectedMode = $('input[name="mcalender_dates"]:checked').val();
       

        if (selectedMode == "0") {
            debugger;
            $('input[type="date"]').attr('min', today);
            $('input[type="date"]').removeAttr('max');
            $('.datepicker1').datepicker({
                minDate: 0
            });
            $("#InitiatedDate").val(today);
            $('#InitiatedDate').attr('readonly', true);
            $("#RequestRemarks").attr('disabled', true);
        } else {
            $('input[type="date"]').attr('max', today);
            $('input[type="date"]').removeAttr('min');
            $('.datepicker1').datepicker(); // default no min/max
            $('#InitiatedDate').attr('readonly', false);
            $("#RequestRemarks").removeAttr('disabled');

            $('#CompletionDate').val("");
            $('#CompletionDate').attr('min', $("#InitiatedDate").val());
            $('#CompletionDate').removeAttr('max');
        }

        $('.datetimepicker1').datepicker(); // always initialize
    }


    applyDateLogic();

    // Re-run when calendar toggle changes
    $('input[name="mcalender_dates"]').change(function () {
       
        console.log($('input[name="mcalender_dates"]:checked').val());
        applyDateLogic();
    });

    $('#InitiatedDate').on('change', function () {

        var initiatedDate = $(this).val();
        $('#CompletionDate').attr('min', initiatedDate);
        $('#CompletionDate').val(""); // Clear any old value if it’s before the new min
    });

    // Remove the max date setting for CompletionDate to allow future selection when InitiatedDate changes
    //$("#InitiatedDate").change(function () {
    //    $('#CompletionDate').val("");
    //    $('#CompletionDate').attr('min', $("#InitiatedDate").val());
    //    $('input[type="date"]').removeAttr('max');
    //});

    $('input[name="mcalender_dates"]').change(function () {

        var selectedValue = $('input[name="mcalender_dates"]:checked').val();
       

        // Send value to server to store in session
        $.ajax({
            url: '/Projects/SetCalendarModeInSession', // controller endpoint
            type: 'POST',
            data: { mode: selectedValue },
            success: function (response) {
                console.log("Session updated:", response.message);
            },
            error: function (xhr, status, error) {
                console.error('Error saving session:', error);
            }
        });
    });

   
    $('.form-control').keypress(function (e) {
        // Get the key code of the pressed key
        // Get the key code of the pressed key
        var keyCode = e.which;
       
        // Allow only alphabets (A-Z, a-z) and numbers (0-9)
        if ((keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (keyCode >= 48 && keyCode <= 57) || (keyCode == 32)) {
            return true; // Allow the keypress
        } else {

            if (keyCode == 46 || keyCode == 44 || keyCode == 40 || keyCode == 41 || keyCode == 45 || keyCode == 58 || keyCode == 47)
                return true; // Allow the keypress
            else {
                alert('Only Alphabets and Numbers allowed');
            return false; // Block the keypress
            }
            
        }
    });
});
function DateFormateyyy_mm_dd(date) {

    var todaysDate = new Date();
    var datef1 = new Date(date);
    //if (datef1.setHours(0, 0, 0, 0) == todaysDate.setHours(0, 0, 0, 0)) {
    //    // Date equals today's date

    //    return 'Today';
    //}
    //else {
    var datef2 = new Date(date);
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var pad = "00"
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    var hh = `${datef2.getHours()}`;
    var mm = `${datef2.getMinutes()}`;
    var ss = `${datef2.getSeconds()}`;
    if (hh < 10) hh = "0" + hh;
    if (mm < 10) mm = "0" + mm;
    if (ss < 10) ss = "0" + ss;
    if (year > 1902) {

        var datemmddyyyy = year + `-` + monthsans + `-` + dayans
        return datemmddyyyy;
    }
    else {
        return '';
    }
    // }

    //`${datef2.getFullYear()}/` + monthsans + `/` + dayans ;
}
function DateFormateddMMyyyyhhmmss(date) {

    var todaysDate = new Date();
    var datef1 = new Date(date);
    //if (datef1.setHours(0, 0, 0, 0) == todaysDate.setHours(0, 0, 0, 0)) {
    //    // Date equals today's date

    //    return 'Today';
    //}
    //else {
    var datef2 = new Date(date);
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var pad = "00"
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    var hh = `${datef2.getHours()}`;
    var mm = `${datef2.getMinutes()}`;
    var ss = `${datef2.getSeconds()}`;
    if (hh < 10) hh = "0" + hh;
    if (mm < 10) mm = "0" + mm;
    if (ss < 10) ss = "0" + ss;
    if (year > 1902) {

        var datemmddyyyy = dayans + `/` + monthsans + `/` + year + ` ` + hh + `:` + mm + `:` + ss
        return datemmddyyyy;
    }
    else {
        return '';
    }
    // }

    //`${datef2.getFullYear()}/` + monthsans + `/` + dayans ;
}
function DateCalculateago(fmDate, end_actual_time) {
    ////////ago///////////
    var ago = "";
    var start_actual_time = fmDate;
   /* var end_actual_time = new Date();*/

    start_actual_time = new Date(start_actual_time);
    end_actual_time = new Date(end_actual_time);

    var diff = end_actual_time - start_actual_time;

    var diffSeconds = diff / 1000;
    var HH = Math.floor(diffSeconds / 3600);
    var MM = Math.floor(diffSeconds % 3600) / 60;

    var formatted = ((HH < 10) ? ("0" + HH) : HH) + ":" + ((MM < 10) ? ("0" + MM.toFixed(0)) : MM.toFixed(0))

    var futureDate = new Date();
    var todayDate = new Date(fmDate);
    var milliseconds = end_actual_time.getTime() - start_actual_time.getTime();
    var hours = Math.floor(milliseconds / (60 * 60 * 1000));
    var formatted1 = formatted.substring(0, 2);
    if (hours <= 24) {
        ago = formatted +' Min </h6>';
    }
    else /*if (hours <= 730)*/
    {
        
        ago = Math.floor(hours / 24) + ' Days (' + formatted  +')</h6>';;
    }
    //else if (hours <= 8766) {
    //    ago = Math.floor(Math.floor(hours / 24) / 30) + ' Months</h6>';;
    //}
    //else {
    //    ago = Math.floor(Math.floor(Math.floor(hours / 24) / 30) / 12) + ' Years</h6>';;
    //}
    return ago;
}

function formatDateToDDMMYYYY(date) {
    // Parse the input date
    var dateObj = new Date(date);

    // Ensure the date is valid
    if (isNaN(dateObj.getTime())) {
        return ''; // Return an empty string if the date is invalid
    }

    // Extract day, month, and year
    var day = dateObj.getDate().toString().padStart(2, '0'); // Ensure 2 digits
    var month = (dateObj.getMonth() + 1).toString().padStart(2, '0'); // Ensure 2 digits
    var year = dateObj.getFullYear();

    // Return the formatted date
    return `${day}-${month}-${year}`;
}



function bindLiveProjectSearch(inputSelector, dropdownSelector, endpointUrl, onItemSelect) {
    debugger;
   
    $(inputSelector).on("keyup", function () {
        debugger;
        let query = $(this).val();
        //query = query.replace(/\u00A0/g, "");
        //const validpattern =/^[a-zA-Z0-9]*$/;

        //if (!validpattern.test(query)) {
        //    Swal.fire({
        //        icon: 'error',
        //        title: 'Invalid Input',
        //        text: 'Special Characters are not allowed',
        //    });
        //    $(this).val(query.Replace(/[a-zA-Z0-9]/g,' '));
        //    return;
        //}
        if (query.length > 200) {
      
            Swal.fire({
                icon: 'error',
                title: 'Only maximaum characters limit is 200',
            });
            return $(this).val("") ;
        }
        
           
           

        if (query.length < 2) {
            $(dropdownSelector).hide();
            return;
        }

        $.ajax({
            url: endpointUrl,
            method: 'GET',
            data: { searchQuery: query },
            success: function (data) {
                $(dropdownSelector).empty();
                if (data.length > 0) {
                    data.forEach(function (item) {
                        $(dropdownSelector).append(`
                            <li class="dropdown-item" data-id="${item.projId}" data-name="${item.projName}">${item.projName}</li>
                        `);
                    });
                    $(dropdownSelector).show();
                } else {
                    $(dropdownSelector).hide();
                }
            },
            error: function (err) {
                console.error("Error fetching project data:", err);
                $(dropdownSelector).hide();
            }
        });
    });

    $(document).on("click", `${dropdownSelector} li`, function () {
        debugger;
        const projId = $(this).data("id");

        const projName = $(this).data("name");

        Swal.fire({
            title: 'Enter Remarks',
            input: 'text',
            inputLabel: `Project Name: ${projName}`,
            inputPlaceholder: 'Type your Remarks here....',
            ShowCancelButton: true,
            confirmButtonText: 'Submit',
            preConfirm: (remarks) => {
                if (!remarks) {
                    debugger;
                    Swal.ShowValidationMessage('Remarks are Required');
                } if (remarks.length < 10) {
                    Swal.showValidationMessage('Remarks Must be Atleast 10 characters');
                }
                if (remarks.length > 200) {
                    Swal.showValidationMessage('Remarks Must not exceed 200 characters');
                }
                    return remarks;
            }
        }).then((result) => {
            if (result.isConfirmed) {

                const remarks = result.value;

                if (typeof onItemSelect === "function") {
                    onItemSelect(projId, projName, remarks);
                }

                $(dropdownSelector).hide();
            }

        });

        
    });

    // Optional: Hide dropdown when clicking outside
    $(document).on("click", function (e) {
        if (!$(e.target).closest(inputSelector).length && !$(e.target).closest(dropdownSelector).length) {
            $(dropdownSelector).hide();
        }
    });
}