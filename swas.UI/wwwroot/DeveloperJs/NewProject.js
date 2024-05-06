

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
                confirmButtonColor: '#ffc107', // Optional: Customize confirm button color
                confirmButtonText: 'OK' // Optional: Change confirm button text
            });

            return false;
        }

    }
}
