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
        var input = document.getElementById("myInput");
        if (!input) return;
        input.type = (input.type === "password") ? "text" : "password";
    }

    document.addEventListener("DOMContentLoaded", function () {
        var container = document.getElementById("show_hide_password");
        if (!container) return;

        var cb = container.querySelector('input[type="checkbox"]');
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

    const encUser = encryptData($('#Input_UserName').val());
    const encPass = encryptData($('input[name="Input.Password"]').val());

    $('#Input_UserName').val(encUser);
    $('input[name="Input.Password"]').val(encPass);

    this.submit();
});
