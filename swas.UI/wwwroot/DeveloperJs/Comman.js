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

          
            $('#CompletionDate').attr('min', $("#InitiatedDate").val());
            $('#CompletionDate').removeAttr('max');
        }

        $('.datetimepicker1').datepicker(); // always initialize
    }


    applyDateLogic();

    // Re-run when calendar toggle changes
    $('input[name="mcalender_dates"]').change(function () {


        applyDateLogic();
    });

    $('#InitiatedDate').on('change', function () {

        var initiatedDate = $(this).val();
        $('#CompletionDate').attr('min', initiatedDate);

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
    var datef2 = date ? new Date(date) : new Date();
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



function DateFormated(date) {
    if (date == null) {
        return "-";
    }
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
 
  
    if (year > 1902) {

        var datemmddyyyy = dayans + `-` + monthsans + `-` + year 
        return datemmddyyyy;
    }
    else {
        return '';
    }
    // }

    //`${datef2.getFullYear()}/` + monthsans + `/` + dayans ;
}

function DateCalculateago(fmDate, end_actual_time) {
    // Initial variables
    var ago = "";
    var start_actual_time = new Date(fmDate);  // Start time
    /*  var end_actual_time = new Date(end_actual_time);  // End time*/
    var end_actual_time = end_actual_time ? new Date(end_actual_time) : new Date();


    // Calculate difference in milliseconds
    var diff = end_actual_time - start_actual_time;

    // Convert the difference to seconds, minutes, hours, days, months, and years
    var diffSeconds = diff / 1000;
    var diffMinutes = diffSeconds / 60;
    var diffHours = diffMinutes / 60;
    var diffDays = diffHours / 24;
    var diffMonths = diffDays / 30;
    var diffYears = diffDays / 365;

    // Calculate hours and minutes from the difference
    var HH = Math.floor(diffHours);  // Total hours
    var MM = Math.floor(diffMinutes % 60);  // Remaining minutes after calculating hours

    // Format hours and minutes
    var formatted = ((HH < 10) ? ("0" + HH) : HH) + ":" + ((MM < 10) ? ("0" + MM) : MM);

    // Calculate the time difference in days, months, or years and format the result
    if (diffHours < 24) {
        ago = formatted + ' Min';
    } else if (diffHours < 730) {  // Less than 730 hours (~30 days)
        ago = Math.floor(diffDays) + ' Days (' + formatted + ')';
    } else if (diffHours < 8760) {  // Less than 8760 hours (~365 days)
        ago = Math.floor(diffMonths) + ' Months (' + formatted + ')';
    } else {
        ago = Math.floor(diffYears) + ' Years (' + formatted + ')';
    }

    return ago;
}


function DateCalculateagoForChart(fmDate, end_actual_time) {
    const diffMs = new Date(end_actual_time) - new Date(fmDate);
    const totalMinutes = diffMs / 60000;
    const totalHoursDecimal = (totalMinutes / 60).toFixed(1); // 1 decimal place
    return parseFloat(totalHoursDecimal);

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
            return $(this).val("");
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

function trimByWords(text, wordLimit) {
    if (!text) return "";
    var words = text.split(" ");
    if (words.length > wordLimit) {
        return words.slice(0, wordLimit).join(" ") + ".....";
    }
    return text;
}

// 2️⃣ Break by Characters
function trimByChars(text, charLimit) {
    if (!text) return "";
    if (text.length > charLimit) {
        return text.substring(0, charLimit) + ".....";
    }
    return text;
}
// 3️⃣ Break lines after some words
function breakLinesByWords(text, wordLimit) {
    if (!text) return "";
    var words = text.split(" ");
    var result = [];

    for (var i = 0; i < words.length; i += wordLimit) {
        result.push(words.slice(i, i + wordLimit).join(" "));
    }

    return result.join("<br>");
}
