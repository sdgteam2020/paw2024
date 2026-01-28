// Function to handle the click event on the "Add" button



function GetwhilteListProject(TypeId) {
    let listItem = '';
    let userdata = {
        "TypeId": TypeId
    };
    $.ajax({
        type: "POST",
        url: "/Home/GetWhiteListedActionProj",
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        success: function (data) {
            if (data != null) {
                let count = 1;
                let listItem = '';

                Object.values(data).forEach(function (item) {
                    listItem += '<tr>';
                    listItem += '<td class="s-no-column">' + count++ + '</td>';

                    var shortName = item.projName || "";
                    if (shortName.length > 10) {
                        shortName = item.projName.slice(0, 10) + "....";
                    }
                    listItem += '<td class="RefLetter-container WhitelistedTable-width8">' +
                        '<div class="noExport">' + shortName + '</div>' +
                        '<div class="RefLetter">' + item.projName + '</div>' +
                        '</td>';

                    listItem += '<td class="WhitelistedTable-width8">' + item.hostedOn + '</td>';
                    listItem += '<td>' + item.appt + '</td>';
                    listItem += '<td>' + item.fmn + '</td>';
                    listItem += '<td class="col-width6">' + item.contactNo + '</td>';
                    listItem += '<td class="WhitelistedTable-width6">' + DateFormated(item.clearence) + '</td>';
                    listItem += '<td class="WhitelistedTable-width15">' + item.certNo + '</td>';

                    if (item.clearence != null) {
                        let clearenceDate = new Date(item.clearence);
                        var afterThreeYears = new Date(clearenceDate.setFullYear(clearenceDate.getFullYear() + 3));
                    }

                    listItem += '<td class="col-width6">' + DateFormated(afterThreeYears) + '</td>';

                    var shortRemarks = item.remarks || "";
                    if (shortRemarks.length > 10) {
                        shortRemarks = item.remarks.slice(0, 10) + "....";
                    }
                    listItem += '<td class="RefLetter-container">' + shortRemarks;
                    if (shortRemarks != "") {
                        listItem += '<div class="RefLetter">' + item.remarks + '</div>';
                    }
                    listItem += '</td>';

                    listItem += '</tr>';
                });

                if ($.fn.DataTable.isDataTable("#WhitelistedTable")) {
                    $("#WhitelistedTable").DataTable().clear().destroy();
                }

                $("#WhitelistedTableData").html(listItem);
                initializeDataTable("#WhitelistedTable");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error loading white list projects:", error);
        }
    });
}
