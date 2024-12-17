

$(document).ready(function () {
    $("#ddlUnitId").change(function () {
        var selectedMode = $(this).val();
    });
    $("#projunderprocess").click(function () {

       
        $("#ProjunderprocessModal").modal('show');
        
    });
    $("#projWhiteListed").click(function () {


        $("#ProjWhiteListedProjectModal").modal('show');

    });
});


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
                text: 'Contact ddgit for authorization before signing in.',
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
                                text: 'Contact ddgit for authorization before signing in.',
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