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

    //jQuery.ajax({
    //    type: "POST",
    //    url: "/Login.cshtml.cs/CheckLogin",
    //    success: function (data) { alert(data); },
    //    failure: function (errMsg) {
    //        alert(errMsg);
    //    }
    //});
});
// CSP-safe password toggle (replaces onclick="myFunction()")
(function () {
    "use strict";

    function togglePassword() {
        var input = document.getElementById("myInput");
        if (!input) return;
        input.type = (input.type === "password") ? "text" : "password";
    }

    document.addEventListener("DOMContentLoaded", function () {
        // checkbox inside #show_hide_password
        var container = document.getElementById("show_hide_password");
        if (!container) return;

        var cb = container.querySelector('input[type="checkbox"]');
        if (!cb) return;

        cb.addEventListener("change", togglePassword);
    });

})();
