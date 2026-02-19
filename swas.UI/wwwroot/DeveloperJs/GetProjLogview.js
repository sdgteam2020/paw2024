$(document).ready(function () {
    const today = new Date();

    if (!$("#fromDate").val()) {
        $("#fromDate")[0].valueAsDate = today;
        $("#toDate")[0].valueAsDate = today;
    }

    $('#SearchData').click(function (event) {
        event.preventDefault();

        const from = $("#fromDate").val();
        const to = $("#toDate").val();

        if (!from || !to) {
            Swal.fire({
                icon: 'error',
                title: 'Missing Date',
                text: 'Please enter both From and To dates.',
                confirmButtonColor: '#d33'
            });
            return;
        }

        if (from > to) {
            Swal.fire({
                icon: 'error',
                title: 'Invalid Range',
                text: 'From Date must be less than or equal to To Date.',
                confirmButtonColor: '#d33'
            });
            return;
        }

        $('#searchFormProjName').submit();
    });
});