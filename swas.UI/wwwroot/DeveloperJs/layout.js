(function () {
    "use strict";
    function getWatermarkText() {
        const el = document.getElementById("IpAddress");
        return el ? (el.textContent || "").trim() : "";
    }

    function safeInitDataTable(selector, options) {
        if (!window.jQuery) return null;
        if (!$.fn || !$.fn.DataTable) return null;

        const $el = $(selector);
        if (!$el.length) return null;
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
                                let el = $("<input/>").on("input", function () { fn(that, this); });
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
        let popupWin = window.open("", "_blank", "top=100,width=900,height=500,location=no");
        if (!popupWin) return;

        let dt = $(tableSelector).DataTable();
        let filteredData = dt.rows({ search: "applied" }).data().toArray();

        let tableHTML = "<table>";
        tableHTML += "<thead><tr>";
        dt.columns().header().each(function (header) {
            tableHTML += "<th>" + header.innerHTML + "</th>";
        });
        tableHTML += "</tr></thead>";

        tableHTML += "<tbody>";
        for (let i = 0; i < filteredData.length; i++) {
            tableHTML += "<tr>";
            for (let j = 0; j < filteredData[i].length; j++) {
                tableHTML += "<td>" + filteredData[i][j] + "</td>";
            }
            tableHTML += "</tr>";
        }
        tableHTML += "</tbody></table>";

        let watermarkText = (typeof watermarkProvider === "function")
            ? (watermarkProvider() || "")
            : "";
        let styles = `
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
          <div class="datatblwatermark"
          >
            ${watermarkText}
          </div>
        </body>
      </html>
    `);
        popupWin.document.close();
    }
    function safeCallInitializeDataTable(selector) {
        if (typeof window.initializeDataTable === "function") {
            if ($(selector).length) window.initializeDataTable(selector);
        }
    }
    function bindEditUserModal() {
        $(document).on("click", ".edit-user-btn", function () {
            let username = $(this).data("username");
            let rankid = $(this).data("rankid");
            let rolename = $(this).data("rolename");

            const payload = {
                UserName: username,
                RankId: rankid,
                RoleName: rolename
            };
            $.ajax({
                url: "/Account/GetUserEditPartial",
                type: "GET",
                data: {
                    payload: encryptData(payload)
                },
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
        if ($(window).width() < 1000) {
            $(".menusharp1").removeClass("d-none");
            $(".menusharp").addClass("d-none");
        } else {
            $(".menusharp").removeClass("d-none");
            $(".menusharp1").addClass("d-none");
        }

        if ($(window).width() < 765) {
            $(".mainheading").css("margin-top", "7rem");
        } else {
            $(".mainheading").css("margin-top", "");
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
        let $loading = $("#loading1, #loading");
        $loading.addClass("d-none"); // keep hidden by default if you use bootstrap d-none

        $(document)
            .ajaxStart(function () { $loading.removeClass("d-none"); })
            .ajaxStop(function () { $loading.addClass("d-none"); })
            .ajaxError(function () { $loading.addClass("d-none"); });
    }

    function sidebarEvents() {
        let menuBtn = document.getElementById("menusharp");
        if (menuBtn) {
            menuBtn.onclick = function () {
                document.body.classList.toggle("sidebar-collapsed");
            };
        }

        let sidebar = document.querySelector(".sidebar-wrapper");
        if (sidebar) {
            sidebar.onmouseenter = function () { document.body.classList.add("sidebar-hover"); };
            sidebar.onmouseleave = function () { document.body.classList.remove("sidebar-hover"); };
        }
    }

    function dropdownHover() {
        let hideTimeout;

        $(".dropdown").hover(
            function () {
                clearTimeout(hideTimeout);
                $(this).children(".dropdown-menu").stop(true, true).slideDown(200);
            },
            function () {
                let $menu = $(this).children(".dropdown-menu");
                hideTimeout = setTimeout(function () {
                    $menu.stop(true, true).slideUp(200);
                }, 200);
            }
        );
    }

    function highlightCurrentMenu() {
        let currentPageUrl = window.location.pathname;
        $(".dropdown-menu li").removeClass("selected");
        $(".dropdown-menu li a[href='" + currentPageUrl + "']").parent().addClass("selected");
        $(".dropdown-menu.selected").addClass("visible");
    }
    window.createNotification = function (event) {
        event.preventDefault();
        let stakeHolderId = document.getElementById("btnid")?.innerText;

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
        let regex = /[^a-zA-Z0-9 ]/g;
        input.value = input.value.replace(regex, "");
    };
    $(function () {
        initDtWithPdfPopup("#Software", getWatermarkText);
        initializeDataTable("#SoftwareType1");
        initDtWithPdfPopup("#mapunit", getWatermarkText);
        initDtWithPdfPopup("#Inbox1", getWatermarkText);
        initDtWithPdfPopup("#SoftwareType3", getWatermarkText);
        safeCallInitializeDataTable("#IndexTable");
        safeCallInitializeDataTable("#SoftwareType5");
        safeCallInitializeDataTable("#Soft");
        if ($("#SentProjDetails").length && $.fn.DataTable) {
            let t = safeInitDataTable("#SentProjDetails", {});
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
