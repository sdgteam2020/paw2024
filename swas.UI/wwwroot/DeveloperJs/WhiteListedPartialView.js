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
                    listItem += ' <td class="col-width16">' + item.projName + '</td>';
                    listItem += '<td class="col-width9">' + item.hostedOn + '</td>';
                    listItem += '<td>' + item.appt + '</td>';
                    listItem += '<td>' + item.fmn + '</td>';
                    listItem += '<td class="col-width6">' + item.contactNo + '</td>';
                    listItem += '<td class="col-width6">' + DateFormated(item.clearence) + '</td>';
                    listItem += '<td>' + item.certNo + '</td>';  // Use item.CertNo instead of @unitx.CertNo
                    if (item.clearence != null) {
                        let clearenceDate = new Date(item.clearence);
                        var afterThreeYears = new Date(clearenceDate.setFullYear(clearenceDate.getFullYear() + 3));
                    }
                  
                    listItem += '<td class="col-width6">' + DateFormated(afterThreeYears) + '</td>';  // Use item.ValidUpto instead of unitx.ValidUpto
                    listItem += '<td>' + item.remarks + '</td>';  // Use item.Remarks instead of @unitx.Remarks
                   
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