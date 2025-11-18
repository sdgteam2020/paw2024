$(document).ready(function () {

    ProjectWiseStatus()
});
function ProjectWiseStatus() {
   
    var listItem = "";

    //table.destroy();
    /* alert(spnstatusActionsMappingId)*/
    var userdata = {
        "Id": 0
    };

    $.ajax({
        url: '/Home/GetProjectWiseStatus',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
           
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({ text: "" });
                } else if (response == 0) {




                } else {
                    var count = 1;


                    var StatusProjectlst = response.statusProjectlst;
                    var MovProjectlst = response.movProjectlst;
                    listItem += '<thead>';
                    listItem += '<tr>';
                    listItem += '<th class="d-none noExport"></th>';
                    listItem += '<th class="text-center">Ser No</th>';
                    listItem += '<th>Project Name</th>';
                    for (var i = 0; i < StatusProjectlst.length; i++) {
                        // Skip special cases "BISAG-N" and "Re-Vetting"
                        if (StatusProjectlst[i].status !== "BISAG-N" && StatusProjectlst[i].status !== "Re-Vetting") {
                            listItem += '<th>' + StatusProjectlst[i].status + '</th>';
                        }
                    }
                    listItem += '</tr>';
                    listItem += '</thead>';
                    listItem += '<tbody id="bodyProjectWiseStatus">';
                    var count = 1;
                    var ProjId = 0;
                    for (var j = 0; j < MovProjectlst.length; j++) {
                        if (ProjId != MovProjectlst[j].projId) {
                            ProjId = MovProjectlst[j].projId;
                            if (j != 0) {

                            }
                            listItem += '<tr>';
                            listItem += '<td class="clsspnprojId d-none noExport">' + MovProjectlst[j].projId + '</td>';
                            listItem += '<td class="align-middle text-center">' + count + '</td>';
                            //listItem += '<td class="clsspnprojId d-none">' + MovProjectlst[j].projId + '</td>';
                            //listItem += '<td class="align-middle text-center">' + count + '</td>';
                            listItem += '<td class="btn-clsprojName">' + MovProjectlst[j].projName + '</td>';

                           
                                //var isstatus = MovProjectlst.filter(function (element) { return element.statusId == StatusProjectlst[i].statusId && element.projId == MovProjectlst[j].projId; });

                                //if (isstatus.length != 0) {
                                //    listItem += '<td class="align-middle text-center" data-toggle="tooltip" data-placement="top" title="' + DateFormateddMMyyyyhhmmss(isstatus[0].timeStamp) + '"><i class="fa fa-check-circle" style="color: #28a745;font-size: 20px;" aria-hidden="true"></i></td>';

                                //}
                                //else {
                                //    listItem += '<td class="align-middle text-center"><i class="fa fa-minus-circle" style="color: #ffc107;font-size: 20px;" aria-hidden="true"></i></td>';
                                //}
                                //if (isstatus.length != 0) {

                                //    listItem += '<td class="align-middle text-center" data-toggle="tooltip" data-placement="top" title="'
                                //        + DateFormateddMMyyyyhhmmss(isstatus[0].timeStamp)
                                //        + '"><div style="width: 25px; height: 25px; border-radius: 50%; background-color: #28a745; color: #fff; display: inline-flex; align-items: center; justify-content: center; font-weight: bold;">A</div></td>';
                                //}
                                //else {
                                //    // Pending (P) - yellow circle with P
                                //    listItem += '<td class="align-middle text-center"><div style="width: 25px; height: 25px; border-radius: 50%; background-color: #ffc107; color: #000; display: inline-flex; align-items: center; justify-content: center; font-weight: bold;">P</div></td>';
                                //}
                                for (var i = 0; i < StatusProjectlst.length; i++) {
                                    // Skip special cases "BISAG-N" and "Re-Vetting"
                                    if (StatusProjectlst[i].status !== "BISAG-N" && StatusProjectlst[i].status !== "Re-Vetting") {
                                        var isstatus = MovProjectlst.filter(function (element) {
                                            return element.statusId == StatusProjectlst[i].statusId && element.projId == MovProjectlst[j].projId;
                                        });

                                        if (isstatus.length != 0) {
                                            // Green tick instead of "A"
                                            listItem += '<td class="align-middle text-center" data-toggle="tooltip" data-placement="top" title="'
                                                + DateFormateddMMyyyyhhmmss(isstatus[0].timeStamp)
                                                + '"><div style="width: 25px; height: 25px; border-radius: 50%; background-color: #28a745; color: #fff; display: inline-flex; align-items: center; justify-content: center; font-weight: bold; font-size: 18px;">✔</div></td>';
                                        } else {
                                            listItem += '<td class="align-middle text-center"><img src="/assets/images/icons/Cross_red_circle.png" width="22" height="22" alt="Readed"></td>';
                                        }
                                    }
                                }
                           
                            listItem += '</tr>';
                            count++;
                        }
                        //if (MovProjectlst[j].projName == "AmitSingha") {
                        //    alert(1)
                        //}


                    }
                    listItem += '</tbody>';

                    $("#tblProjectWiseStatus").html(listItem);

                    $(document).unbind().on("click", ".btn-clsprojName", function () {
                      
                        $('#ProjHoldHistory').modal('show');
                        // alert($(this).closest("tr").find(".clsspnprojId").html())
                        $(".lblProjHoldHistory").html($(this).html())
                        $("#cardforProjHoldHistory").removeClass("d-none");
                        GetProjHold($(this).closest("tr").find(".clsspnprojId").html())
                        ProjectWiseStatusByProjid($(this).closest("tr").find(".clsspnprojId").html())
                    });
                    
                    initializeDataTable('#tblProjectWiseStatus');
                }
            }
            else {


            }
        },
        error: function (result) {
            Swal.fire({ text: "" });
        }
    });
} 


function ProjectWiseStatusByProjid(projid) {
    var listItem = "";

    var userdata = {
        "Projid": projid  // Send the Projid as data to the server
    };

    $.ajax({
        url: '/Home/GetProjectWiseStatus', // Ensure the URL is correct for your action
        contentType: 'application/x-www-form-urlencoded', // This is typically used for sending form data
        data: userdata,  // Pass the data (Projid) to the controller
        type: 'POST', // Make a POST request
        success: function (response) {
          
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({ text: "Error fetching data!" });
                } else if (response == 0) {
                    Swal.fire({ text: "No data found for the given project ID." });
                } else {
                    var StatusProjectlst = response.statusProjectlst;
                    var MovProjectlst = response.movProjectlst;
                    listItem += '<thead>';
                    listItem += '<tr>';
                    listItem += '<th class="d-none noExport"></th>';
                    // listItem += '<th class="text-center">Ser No</th>';
                    // listItem += '<th>Project Name</th>';

                    // Generate table header based on StatusProjectlst
                    for (var i = 0; i < StatusProjectlst.length; i++) {
                        // Skip special cases "BISAG-N" and "Re-Vetting"
                        if (StatusProjectlst[i].status !== "BISAG-N" && StatusProjectlst[i].status !== "Re-Vetting") {
                            listItem += '<th>' + StatusProjectlst[i].status + '</th>';
                        }
                    }
                    listItem += '</tr>';
                    listItem += '</thead>';
                    listItem += '<tbody id="bodyProjectWiseStatusByprojid">';

                    var count = 1;
                    var ProjId = 0;

                    // Loop through MovProjectlst and generate rows
                    for (var j = 0; j < MovProjectlst.length; j++) {
                        if (ProjId != MovProjectlst[j].projId) {
                            ProjId = MovProjectlst[j].projId;
                            listItem += '<tr>';
                            listItem += '<td class="clsspnprojId d-none noExport">' + MovProjectlst[j].projId + '</td>';
                            // listItem += '<td class="align-middle text-center">' + count + '</td>';
                            //listItem += '<td class="btn-clsprojName">' + MovProjectlst[j].projName + '</td>';

                            // Loop through StatusProjectlst to match and display status
                            for (var i = 0; i < StatusProjectlst.length; i++) {
                                // Skip special cases "BISAG-N" and "Re-Vetting"
                                if (StatusProjectlst[i].status !== "BISAG-N" && StatusProjectlst[i].status !== "Re-Vetting") {
                                    var isstatus = MovProjectlst.filter(function (element) {
                                        return element.statusId == StatusProjectlst[i].statusId && element.projId == MovProjectlst[j].projId;
                                    });

                                    if (isstatus.length != 0) {
                                        // Green tick instead of "A"
                                        listItem += '<td class="align-middle text-center" data-toggle="tooltip" data-placement="top" title="'
                                            + DateFormateddMMyyyyhhmmss(isstatus[0].timeStamp)
                                            + '"><div style="width: 25px; height: 25px; border-radius: 50%; background-color: #28a745; color: #fff; display: inline-flex; align-items: center; justify-content: center; font-weight: bold; font-size: 18px;">✔</div></td>';
                                    } else {
                                        listItem += '<td class="align-middle text-center"><img src="/assets/images/icons/Cross_red_circle.png" width="22" height="22" alt="Readed"></td>';
                                    }
                                }
                            }
                            listItem += '</tr>';
                            count++;
                        }
                    }

                    listItem += '</tbody>';

                    // Append the dynamically generated table to the DOM
                    $("#tblProjectWiseStatusByprojid").html(listItem);
                }
            }
        },
        error: function (error) {
            console.log('Error:', error);
            Swal.fire({ text: 'An error occurred while fetching data.' });
        }
    })
}