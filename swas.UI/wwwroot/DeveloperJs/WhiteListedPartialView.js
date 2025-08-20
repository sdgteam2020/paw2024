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
                let count = 1;  // Start count from 1
                let listItem = '';  // Initialize listItem as an empty string

                Object.values(data).forEach(function (item) {  // Corrected `forEach` loop syntax
                    listItem += '<tr>';
                    listItem += '<td class="s-no-column">' + count++ + '</td>';  // Increment count
                    var short_Name = item.projName || ""; // default full text
                    if (short_Name.length > 10) {
                        short_Name = short_Name.slice(0, 10) + "...";
                    }
                    listItem += ' <td class="col-width16 RefLetter-container">' + short_Name +
                        '<div class="RefLetter">' + item.projName + '</div>' +
                        '</td>';
                    listItem += '<td class="col-width9">' + item.hostedOn + '</td>';
                    listItem += '<td>' + item.appt + '</td>';
                    listItem += '<td>' + item.fmn + '</td>';
                    listItem += '<td class="col-width6">' + item.contactNo + '</td>';
                    listItem += '<td class="col-width6">' + DateFormated(item.clearence) + '</td>';
                    var short_CertNO = item.certNo || "";

                    if (short_CertNO.length > 10) {
                        short_CertNO = item.certNo.slice(0, 10) + "...";
                    }

                    listItem += `
    <td class="RefLetter-container">
        ${short_CertNO}
        <div class="RefLetter">${item.certNo}</div>
    </td>
`;
                    /*listItem += '<td>' + short_CertNO + '</td>';*/  // Use item.CertNo instead of @unitx.CertNo
                    if (item.clearence != null) {
                        let clearenceDate = new Date(item.clearence);
                        var afterThreeYears = new Date(clearenceDate.setFullYear(clearenceDate.getFullYear() + 3));
                    }

                    listItem += '<td class="col-width6">' + DateFormated(afterThreeYears) + '</td>';  // Use item.ValidUpto instead of unitx.ValidUpto

                    var short_remarks = item.remarks || ""; // default full text
                    if (short_remarks.length > 10) {
                        short_remarks = short_remarks.slice(0, 10) + "...";
                    }

                    listItem += '<td class="RefLetter-container">' + short_remarks
                    if (short_remarks != "") {
                        listItem += '<div class="RefLetter">' + item.remarks + '</div>';

                    }

                    listItem += '</td>';                       // Use item.Remarks instead of @unitx.Remarks

                    listItem += '</tr>';
                });
                if ($.fn.DataTable.isDataTable("#WhitelistedTable")) {
                    $("#WhitelistedTable").DataTable().clear().destroy();
                }

                $("#WhitelistedTableData").html(listItem);
                initializeDataTable("#WhitelistedTable")// Insert the generated rows into the table
            }


        },
        error: function (xhr, status, error) {
            console.error("Error loading white list projects:", error);
        }
    });
}