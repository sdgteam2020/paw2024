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
          
            console.log(data);
            if (data != null) {
                let count = 1;  // Start count from 1
                let listItem = '';  // Initialize listItem as an empty string
                
                Object.values(data).forEach(function (item) {  // Corrected `forEach` loop syntax
                    listItem += '<tr>';
                    listItem += '<td class="s-no-column">' + count++ + '</td>';  // Increment count
                    var shortName = item.projName ||"";
                    if (shortName.length > 10) {
                        shortName = item.projName.slice(0, 10) + "....";
                    }
                    listItem += ' <td class=" RefLetter-container" style="width:8%"><div class="noExport">' + shortName +
                       '</div><div class="RefLetter">'+
                            item.projName +
                    '  </div> </td>';
                       
                    listItem += '<td class="" style="width:8%">' + item.hostedOn + '</td>';
                    listItem += '<td>' + item.appt + '</td>';
                    listItem += '<td>' + item.fmn + '</td>';
                    listItem += '<td class="col-width6">' + item.contactNo + '</td>';
                    listItem += '<td class="" style="width:6%">' + DateFormated(item.clearence) + '</td>';
                    //var shortCetNo = item.certNo || "";
                    //if (shortCetNo.length > 10) {
                    //    shortCetNo = item.certNo.slice(0, 10) + "....";
                    //}
                    listItem += '<td  style="width:15%">' + item.certNo + 
                    '  </td>';  // Use item.Remarks instead of @unitx.Remarks

                /*    listItem += '<td>' + item.certNo + '</td>'; */ // Use item.CertNo instead of @unitx.CertNo
                    if (item.clearence != null) {
                        let clearenceDate = new Date(item.clearence);
                        var afterThreeYears = new Date(clearenceDate.setFullYear(clearenceDate.getFullYear() + 3));
                    }
                 
                
                    listItem += '<td class="col-width6">' + DateFormated(afterThreeYears) + '</td>';  // Use item.ValidUpto instead of unitx.ValidUpto
                    var shortRemarks = item.remarks || "";
                    var Small = shortRemarks; 
                    if (shortRemarks.length > 10) {
                        shortRemarks = item.remarks.slice(0, 10) + "....";
                    }
                    listItem += '<td class="RefLetter-container">' + shortRemarks 
                    if (shortRemarks != "") {
                        listItem += ' <div class="RefLetter" >' + item.remarks + '</div>';
                          
                          

                    }
                    listItem +=  ' </td>';  // Use item.Remarks instead of @unitx.Remarks
                   
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