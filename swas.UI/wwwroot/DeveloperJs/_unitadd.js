
    if (TempData.ContainsKey("FailureMessage"))
    {
        <text>
            Swal.fire({
                title: 'Error',
                text: '@TempData["FailureMessage"]',
                icon: 'failure',
                confirmButtonText: 'OK'
            });
        </text>
        TempData.Remove("FailureMessage");
    }




    if (TempData.ContainsKey("SuccessMessage"))
    {
        <text>
            Swal.fire({
                title: 'Success',
                text: '@TempData["SuccessMessage"]',
                icon: 'success',
                confirmButtonText: 'OK'
            });

        </text>
        TempData.Remove("SuccessMessage");
    }








    function ValInData(input) {
        var regex = /[^a-zA-Z0-9/ ]/g;
        input.value = input.value.replace(regex, "");
    }
