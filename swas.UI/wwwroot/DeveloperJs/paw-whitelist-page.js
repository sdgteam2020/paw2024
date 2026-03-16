// ========================
// App Data (No global pollution)
// ========================
const AppData = (() => {
    const b = document.body;

    return {
        flag: b.getAttribute("data-flag") || "",
        buttonText: b.getAttribute("data-buttontext") || "",
        username: b.getAttribute("data-username") || "",
        ip: b.getAttribute("data-ip") || ""
    };
})();

// ========================
// Input Validation
// ========================
window.ValInData = function (input) {
    const regex = /[^a-zA-Z0-9/ ]/g;
    input.value = input.value.replace(regex, "");
};

// ========================
// Form Controls (CSP Safe)
// ========================
window.openForm = function () {
    const el = document.getElementById("myForm");
    if (el) el.classList.add("form-visible");
};

window.closeForm = function () {
    const el = document.getElementById("myForm");
    if (el) el.classList.remove("form-visible");
};

// ========================
// Document Ready
// ========================
$(document).ready(function () {

    // Add WhiteList Button
    $("#AddWhite_proj").on("click", function () {

        $("input[required], select[required], textarea[required]")
            .removeClass("is-invalid");

        $("input[required], select[required], textarea[required]")
            .siblings(".invalid-feedback")
            .hide();

        const $m = $("#WhiteListModal");

        $m.find("input").val("");
        $m.find("select").prop("selectedIndex", 0);
        $m.find("textarea").val("");

        $m.modal("show");

        $("#WhiteListedProjectDetail .modal-content")
            .addClass("modal-blur");
    });

    // Initialize DataTable
    if (typeof initializeDataTable === "function") {
        initializeDataTable("#HeldTable11");
    }

    // Select2 initialization
    $("#WhiteListModal").on("shown.bs.modal", function () {
        $("#sponsor").select2({
            placeholder: "-- Select Sponsor --",
            allowClear: true,
            width: "100%",
            dropdownParent: $("#WhiteListModal")
        });
    });

    // Visitor Counters
    $.get("/Home/getCountertoday", function (data) {
        $("#dailyCounter").text("Visitors Today: " + data.today);
        $("#monthlyCounter").text("Monthly: " + data.currentMonth);
        $("#totalCounter").text("Total Visitors: " + data.total);
    });

    // Cards Click Events
    $("#PolicyWhiteList").on("click", function () {
        $("#ProjectPolicyModel").modal("show");
    });

    $("#whitelistCard").on("click", function () {
        $("#ProjectUnderProcess").modal("show");
    });

    $("#WhiteListedProject").on("click", function () {
        $("#WhiteListedProjectDetail").modal("show");
    });

    // ========================
    // Dummy Data Generator
    // ========================
    $("#btnDummyData").on("click", function () {

        function randomString(length) {
            const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            let result = "";
            for (let i = 0; i < length; i++) {
                result += chars.charAt(Math.floor(Math.random() * chars.length));
            }
            return result;
        }

        function randomNumber(length) {
            const chars = "0123456789";
            let result = "";
            for (let i = 0; i < length; i++) {
                result += chars.charAt(Math.floor(Math.random() * chars.length));
            }
            return result;
        }

        function randomDate(start, end) {
            const date = new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime()));
            return date.toISOString().split("T")[0];
        }

        $("#swName").val("SW_" + randomString(5));
        $("#hostedOn").val(String(Math.floor(Math.random() * 4) + 1));
        $("#appt").val("Appt_" + randomString(3));

        const sponsorOptions = $("#sponsor option").not(":first");

        if (sponsorOptions.length > 0) {
            const randomSponsor =
                sponsorOptions.eq(Math.floor(Math.random() * sponsorOptions.length)).val();
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