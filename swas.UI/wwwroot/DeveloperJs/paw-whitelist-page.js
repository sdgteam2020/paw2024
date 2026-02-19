/* =========================
   CSP SAFE: extracted scripts
   ========================= */

(function () {
    var b = document.body;
    window.flag = b.getAttribute("data-flag") || "";
    window.ButtonText = b.getAttribute("data-buttontext") || "";
    window.username = b.getAttribute("data-username") || "";
    window.ip = b.getAttribute("data-ip") || "";
})();
window.ValInData = function (input) {
    var regex = /[^a-zA-Z0-9/ ]/g;
    input.value = input.value.replace(regex, "");
};
window.openForm = function () {
    var el = document.getElementById("myForm");
    if (el) el.style.display = "block";
};

window.closeForm = function () {
    var el = document.getElementById("myForm");
    if (el) el.style.display = "none";
};

$(document).ready(function () {
    $("#AddWhite_proj").on("click", function () {
      
        $("input[required], select[required], textarea[required]").removeClass("is-invalid");
        $("input[required], select[required], textarea[required]").siblings(".invalid-feedback").hide();

        var $m = $("#WhiteListModal");
        $m.find("input").val("");
        $m.find("select").prop("selectedIndex", 0);
        $m.find("textarea").val("");

        $m.modal("show");
        $("#WhiteListedProjectDetail .modal-content").css("filter", "blur(4px)");
    });
    if (typeof initializeDataTable === "function") {
        initializeDataTable("#HeldTable11");
    }
    $("#WhiteListModal").on("shown.bs.modal", function () {
        $("#sponsor").select2({
            placeholder: "-- Select Sponsor --",
            allowClear: true,
            width: "100%",
            dropdownParent: $("#WhiteListModal")
        });
    });
    $.get("/Home/getCountertoday", function (data) {
        $("#dailyCounter").text("Visitors Today: " + data.today);
        $("#monthlyCounter").text("Monthly: " + data.currentMonth);
        $("#totalCounter").text("Total Visitors: " + data.total);
    });
    $("#PolicyWhiteList").on("click", function () { $("#ProjectPolicyModel").modal("show"); });
    $("#whitelistCard").on("click", function () { $("#ProjectUnderProcess").modal("show"); });
    $("#WhiteListedProject").on("click", function () {
       
        $("#WhiteListedProjectDetail").modal("show");
    });
    $("#btnDummyData").on("click", function () {

        function randomString(length) {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var result = "";
            for (var i = 0; i < length; i++) result += chars.charAt(Math.floor(Math.random() * chars.length));
            return result;
        }

        function randomNumber(length) {
            var chars = "0123456789";
            var result = "";
            for (var i = 0; i < length; i++) result += chars.charAt(Math.floor(Math.random() * chars.length));
            return result;
        }

        function randomDate(start, end) {
            var date = new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime()));
            return date.toISOString().split("T")[0];
        }

        $("#swName").val("SW_" + randomString(5));
        $("#hostedOn").val(String(Math.floor(Math.random() * 4) + 1));
        $("#appt").val("Appt_" + randomString(3));

        var sponsorOptions = $("#sponsor option").not(":first");
        if (sponsorOptions.length > 0) {
            var randomSponsor = sponsorOptions.eq(Math.floor(Math.random() * sponsorOptions.length)).val();
            $("#sponsor").val(randomSponsor);
        }

        $("#telNo").val(randomNumber(10));
        $("#clearanceDate").val(randomDate(new Date(2023, 0, 1), new Date()));
        $("#certNo").val("CERT-" + randomNumber(3));
        $("#validUpto").val(randomDate(new Date(), new Date(2026, 11, 31)));
        $("#remarks").val("Dummy data: " + randomString(10));

        if ($("#sponsor").hasClass("select2-hidden-accessible")) {
            $("#sponsor").trigger("change");
        }
    });

});


