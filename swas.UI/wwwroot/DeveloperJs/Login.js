$(document).ready(function () {
  

    $.ajax({
        url: '../../Home/CheckLogin',
        type: 'POST',
        data: { "RoleId": 1 }, //get the search string
        success: function (result) {

            if (result == 1) {
                window.location.replace("../../Home/Index");

            }


        }
    });
});
(function () {
    "use strict";

    function togglePassword() {
        let input = document.getElementById("myInput");
        if (!input) return;
        input.type = (input.type === "password") ? "text" : "password";
    }

    document.addEventListener("DOMContentLoaded", function () {
        let container = document.getElementById("show_hide_password");
        if (!container) return;

        let cb = container.querySelector('input[type="checkbox"]');
        if (!cb) return;

        cb.addEventListener("change", togglePassword);
    });

})();
function encryptData(text) {
    const key = "DGIS-Login-AES-256-Key-Change-Me";
    return CryptoJS.AES.encrypt(text, key).toString();
}
$('#account').on('submit', function (e) {
    e.preventDefault();
   
    const username = $('#Input_UserName').val().trim();
    const password = $('input[name="Input.Password"]').val().trim();

    // ❌ Validation
    if (!username || !password) {
        Swal.fire({
            icon: 'warning',
            title: 'Missing Fields',
            text: 'Username and Password are required!'
        });
        return;
    }

    // ✅ Encrypt only if valid
    const encUser = encryptData(username);
    const encPass = encryptData(password);

    $('#Input_UserName').val(encUser);
    $('input[name="Input.Password"]').val(encPass);

    this.submit();
});


$('.form-control').keypress(function (e) {
    let keyCode = e.which;
   
    if ((keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122) || (keyCode >= 48 && keyCode <= 57) || (keyCode == 32)) {
        return true; // Allow the keypress
    } else {

        if (keyCode == 64||keyCode == 46 || keyCode == 44 || keyCode == 40 || keyCode == 41 || keyCode == 45 || keyCode == 58 || keyCode == 47 || keyCode == 13 || keyCode == 38 || keyCode == 95 )
            return true; // Allow the keypress
        else {
          
            alert('Only Alphabets and Numbers allowed');
            return false; // Block the keypress
        }

    }
});

$('.char-limit').each(function () {

    let inputField = $(this);
    let maxLength = inputField.attr('maxlength'); // use actual maxlength

    // go up to parent col and find error
    let errorMsg = inputField.closest('.col-12').find('.charErrorMsg');

    inputField.on('input', function () {

        let value = inputField.val();

        if (value.length >= maxLength) {
            errorMsg.removeClass('d-none');
        } else {
            errorMsg.addClass('d-none');
        }

    });

});