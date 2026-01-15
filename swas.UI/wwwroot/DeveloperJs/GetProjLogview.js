
    $(document).ready(function () {
        var todaysDate = new Date();
    if ($("#fromDate").val() == "") {
        $("#fromDate").val(DateFormateyyy_mm_dd(todaysDate));
    $("#toDate").val(DateFormateyyy_mm_dd(todaysDate));
        }


    $('#SearchData').click(function (event) {


            var fromTimeStamp = $('#fromDate').val();
    var toTimeStamp = $('#toDate').val();

            if (fromTimeStamp > toTimeStamp) {
        Swal.fire({
            icon: 'error',
            title: 'From Date must be less than To Date',
            text: 'Enter a date value',
            confirmButtonColor: '#d33',
            confirmButtonText: 'OK'
        });
    $('#toDate').val('');

    event.preventDefault();
            }

    else if (fromTimeStamp === '') {
        Swal.fire({
            icon: 'error',
            title: 'Invalid From Date',
            text: 'Enter a date value',
            confirmButtonColor: '#d33',
            confirmButtonText: 'OK'
        });
    $('#toDate').val('');

    event.preventDefault();
            }
    else if (toTimeStamp === '') {
        Swal.fire({
            icon: 'error',
            title: 'Invalid To Date',
            text: 'Enter a date value',
            confirmButtonColor: '#d33',
            confirmButtonText: 'OK'
        });
    $('#toDate').val('');

    event.preventDefault();
            }
    else {
        $('#searchFormProjName').submit();
            }
           
        });
    });
