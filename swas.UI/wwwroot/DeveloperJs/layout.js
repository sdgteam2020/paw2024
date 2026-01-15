(function () {
    "use strict";

    // -------------------------
    // Helpers
    // -------------------------
    function getWatermarkText() {
        // Prefer hidden span in layout: <span id="IpAddress" class="d-none">...</span>
        var el = document.getElementById("IpAddress");
        return el ? (el.textContent || "").trim() : "";
    }

    function safeInitDataTable(selector, options) {
        if (!window.jQuery) return null;
        if (!$.fn || !$.fn.DataTable) return null;

        var $el = $(selector);
        if (!$el.length) return null;

        // If already DataTable, destroy cleanly
        if ($.fn.dataTable.isDataTable($el)) {
            $el.DataTable().clear().destroy();
        }

        return $el.DataTable(options || {});
    }

    function initDtWithPdfPopup(tableSelector, watermarkProvider) {
        return safeInitDataTable(tableSelector, {
            lengthChange: true,
            dom: "lBfrtip",
            buttons: [
                "copy",
                "excel",
                "csv",
                {
                    text: "PDF",
                    extend: "pdfHtml5",
                    action: function () {
                        pdfPopupFromDataTable(tableSelector, watermarkProvider);
                    }
                }
            ],
            searchBuilder: {
                conditions: {
                    num: {
                        MultipleOf: {
                            conditionName: "Multiple Of",
                            init: function (that, fn, preDefined) {
                                var el = $("<input/>").on("input", function () { fn(that, this); });
                                if (preDefined != null) $(el).val(preDefined[0]);
                                return el;
                            },
                            inputValue: function (el) { return $(el[0]).val(); },
                            isInputValid: function (el) { return $(el[0]).val().length !== 0; },
                            search: function (value, comparison) { return value % comparison === 0; }
                        }
                    }
                }
            }
        });
    }

    function pdfPopupFromDataTable(tableSelector, watermarkProvider) {
        var popupWin = window.open("", "_blank", "top=100,width=900,height=500,location=no");
        if (!popupWin) return;

        var dt = $(tableSelector).DataTable();
        var filteredData = dt.rows({ search: "applied" }).data().toArray();

        var tableHTML = "<table>";
        tableHTML += "<thead><tr>";
        dt.columns().header().each(function (header) {
            tableHTML += "<th>" + header.innerHTML + "</th>";
        });
        tableHTML += "</tr></thead>";

        tableHTML += "<tbody>";
        for (var i = 0; i < filteredData.length; i++) {
            tableHTML += "<tr>";
            for (var j = 0; j < filteredData[i].length; j++) {
                tableHTML += "<td>" + filteredData[i][j] + "</td>";
            }
            tableHTML += "</tr>";
        }
        tableHTML += "</tbody></table>";

        var watermarkText = (typeof watermarkProvider === "function")
            ? (watermarkProvider() || "")
            : "";

        // ✅ valid CSS (fixed vertical-align/background-color syntax)
        var styles = `
      <style type="text/css">
        table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }
        th, td { padding: 8px; border: 1px solid #ddd; text-align: center; }
        th { background-color: #f2f2f2; color: black; }
      </style>
    `;

        popupWin.document.open();
        popupWin.document.write(`
      <html>
        <head>${styles}</head>
        <body onload="window.print()">
          ${tableHTML}
          <div style="
            transform: rotate(-45deg);
            z-index:10000;
            opacity:0.3;
            position:fixed;
            left:6%;
            top:39%;
            color:#8e9191;
            font-size:80px;
            font-weight:500;
            display:grid;
            justify-content:center;
            align-content:center;
          ">
            ${watermarkText}
          </div>
        </body>
      </html>
    `);
        popupWin.document.close();
    }

    // If you already have initializeDataTable elsewhere, keep using it.
    function safeCallInitializeDataTable(selector) {
        if (typeof window.initializeDataTable === "function") {
            if ($(selector).length) window.initializeDataTable(selector);
        }
    }

    // -------------------------
    // UI Handlers
    // -------------------------
    function bindEditUserModal() {
        $(document).on("click", ".edit-user-btn", function () {
            var username = $(this).data("username");
            var rankid = $(this).data("rankid");
            var rolename = $(this).data("rolename");

            $.ajax({
                url: "/Account/GetUserEditPartial",
                type: "GET",
                data: { UserName: username, RankId: rankid, RoleName: rolename },
                success: function (result) {
                    $("#editUserModalBody").html(result);
                    $("#editUserModal").modal("show");
                    if ($.fn.select2) $(".dropdownsearch").select2({ dropdownParent: $("#editUserModal") });
                },
                error: function () {
                    alert("Error loading form.");
                }
            });
        });
    }

    function bindPolicyCorner() {
        $(document).on("click", "#FlowPng", function () {
            $("#iProjibutton").modal("show");
        });

        $(document).on("click", "#WLProjList", function () {
            $(".spnWhitelistedorDues").html("WhtieListed Projects");
            $("#WhiteListedProjectDetail").modal("show");
            if (typeof window.GetwhilteListProject === "function") window.GetwhilteListProject(0);
        });
    }

    function checksize() {
        // ✅ removed stray token "mmittee" (it was breaking JS)
        if ($(window).width() < 1000) {
            $("#menusharp1").removeClass("d-none");
            $("#menusharp").addClass("d-none");
        } else {
            $("#menusharp").removeClass("d-none");
            $("#menusharp1").addClass("d-none");
        }

        if ($(window).width() < 765) {
            $("#mainheading").css("margin-top", "7rem");
        } else {
            $("#mainheading").css("margin-top", "");
        }
    }

    function fetchCounters() {
        $.get("/Home/getCountertoday", function (data) {
            $("#dailyCounter").text("Visitors Today: " + data.today);
            $("#monthlyCounter").text("Monthly: " + data.currentMonth);
            $("#totalCounter").text("Total Visitors: " + data.total);
        });
    }

    function ajaxLoader() {
        // Your layout uses #loading1; earlier code used #loading.
        var $loading = $("#loading1, #loading");
        $loading.addClass("d-none"); // keep hidden by default if you use bootstrap d-none

        $(document)
            .ajaxStart(function () { $loading.removeClass("d-none"); })
            .ajaxStop(function () { $loading.addClass("d-none"); })
            .ajaxError(function () { $loading.addClass("d-none"); });
    }

    function sidebarEvents() {
        var menuBtn = document.getElementById("menusharp");
        if (menuBtn) {
            menuBtn.onclick = function () {
                document.body.classList.toggle("sidebar-collapsed");
            };
        }

        var sidebar = document.querySelector(".sidebar-wrapper");
        if (sidebar) {
            sidebar.onmouseenter = function () { document.body.classList.add("sidebar-hover"); };
            sidebar.onmouseleave = function () { document.body.classList.remove("sidebar-hover"); };
        }
    }

    function dropdownHover() {
        var hideTimeout;

        $(".dropdown").hover(
            function () {
                clearTimeout(hideTimeout);
                $(this).children(".dropdown-menu").stop(true, true).slideDown(200);
            },
            function () {
                var $menu = $(this).children(".dropdown-menu");
                hideTimeout = setTimeout(function () {
                    $menu.stop(true, true).slideUp(200);
                }, 200);
            }
        );
    }

    function highlightCurrentMenu() {
        var currentPageUrl = window.location.pathname;
        $(".dropdown-menu li").removeClass("selected");
        $(".dropdown-menu li a[href='" + currentPageUrl + "']").parent().addClass("selected");
        $(".dropdown-menu.selected").addClass("visible");
    }

    // Expose globally (your old code called it inline sometimes)
    window.createNotification = function (event) {
        event.preventDefault();
        var stakeHolderId = document.getElementById("btnid")?.innerText;

        fetch(`/Home/GetNotification?stakeHolderId=${stakeHolderId}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" }
        })
            .then(function (response) {
                if (response.ok) {
                    window.location.href = event.target.href;
                } else {
                    console.error("Failed to retrieve notification data");
                }
            })
            .catch(function (error) {
                console.error("Error:", error);
            });
    };

    window.ValInData = function (input) {
        var regex = /[^a-zA-Z0-9 ]/g;
        input.value = input.value.replace(regex, "");
    };

    // -------------------------
    // DOM Ready: Initialize
    // -------------------------
    $(function () {

        // ✅ DataTables with PDF popup (only if those tables exist)
        initDtWithPdfPopup("#Software", getWatermarkText);
        initDtWithPdfPopup("#SoftwareType1", function () { return ""; });
        initDtWithPdfPopup("#mapunit", getWatermarkText);
        initDtWithPdfPopup("#Inbox1", getWatermarkText);
        initDtWithPdfPopup("#SoftwareType3", getWatermarkText);

        // Your other tables using your own initializer
        safeCallInitializeDataTable("#IndexTable");
        safeCallInitializeDataTable("#SoftwareType5");
        safeCallInitializeDataTable("#Soft");

        // SentProjDetails (guarded)
        if ($("#SentProjDetails").length && $.fn.DataTable) {
            var t = safeInitDataTable("#SentProjDetails", {});
            if (t && t.buttons && t.buttons().container) {
                t.buttons().container().insertBefore(t.table().container());
            }
        }

        bindEditUserModal();
        bindPolicyCorner();

        checksize();
        $(window).on("resize", checksize);

        fetchCounters();
        ajaxLoader();
        sidebarEvents();
        dropdownHover();
        highlightCurrentMenu();
    });

})();
