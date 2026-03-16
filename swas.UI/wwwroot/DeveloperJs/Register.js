

$(document).ready(function () {
    populateddlUnit();
    $("#btn-close").click(function () {

        var signUpUrl = '/Identity/Account/Login';
        window.location.href = signUpUrl;
    });
    $('.dropdownsearch').select2();

        $(document).on('click', '.plus', function () {
          
            $('#UnitAdd').modal('show');
        });


    $('.pluscircle').on('click', function () {
        $('#UnitAdd').modal('show');
    });


    $(document).on('click', function (e) {
        if (!$(e.target).closest('#UnitAdd').length) {
            $('#UnitAdd').modal('hide');
        }
    });
    });
  
        





   








   



        function myFunction() {
            var x = document.getElementById("myInput");
            if (x.type === "password") {
                x.type = "text";
            } else {
                x.type = "password";
            }
        }
    $("#Input_UserName").change(function () {
        var s = $("#Input_UserName").val().substr(0, 1).toUpperCase() + $("#Input_UserName").val().substr(1);
        $("#myInput").val(s + String.fromCharCode(64) + "123");
        $("#myCnfInput").val(s + String.fromCharCode(64) + "123");
    });




    function ValInData(input) {
        var regex = /[^a-zA-Z0-9 ]/g;
        input.value = input.value.replace(regex, "");
    }


$('.char-limit').each(function () {

    var inputField = $(this);
    var maxLength = inputField.data('maxlength');

    var errorMsg = inputField.siblings('.charErrorMsg');

    inputField.on('input', function () {

        var value = inputField.val();

        if (value.length > maxLength) {
            inputField.val(value.substring(0, maxLength));
            errorMsg.removeClass('d-none');
        }
        else {
            errorMsg.addClass('d-none');
        }

    });

});
$('.form-control').keypress(function (e) {
    var keyCode = e.which;

    if (
        (keyCode >= 65 && keyCode <= 90) ||   // A-Z
        (keyCode >= 97 && keyCode <= 122) ||  // a-z
        (keyCode >= 48 && keyCode <= 57) ||   // 0-9
        keyCode == 32 ||  // space
        keyCode == 189 || // -
        keyCode == 95     // _
    ) {
        return true;
    }
    else {

        if (
            keyCode == 46 ||  // .
            keyCode == 44 ||  // ,
            keyCode == 40 ||  // (
            keyCode == 41 ||  // )
            keyCode == 45 ||  // -
            keyCode == 58 ||  // :
            keyCode == 47 ||  // /
            keyCode == 13 ||  // Enter
            keyCode == 38     // &
        )
            return true;
        else {
            alert('Only Alphabets, Numbers and _ allowed');
            return false;
        }
    }
});

