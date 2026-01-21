

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


