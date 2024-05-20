$(document).ready(function () {
    GetAllDashbaordCount();

});

function GetAllDashbaordCount() {
    $.ajax({
        type: "POST",
        url: '/Home/GetDashboardCount',
        data: {
            "Id": 0,
           
        },
        success: function (data) {

            
            var dtoDashboardHeaderlst = data.dtoDashboardHeaderlst;

            var listitem = '';
            var stageId = 0;
            var tot = 0;
            var peding = 0;
            var sent = 0;
            if (data != null) {
                for (var i = 0; i < dtoDashboardHeaderlst.length; i++) {
                    if (stageId != dtoDashboardHeaderlst[i].stageId) {
                        if (stageId != 0) {
                            listitem += '</div>';
                        }
                        listitem += '<div class="newprojectheading text-center"> ' + dtoDashboardHeaderlst[i].stages + ' </div>';
                        listitem += '<div class="r-1" style="margin-top: 13px;">';
                    }
                    
                    
                    listitem += '<div class="cd-1 btn btnGetsummay" style="background-color:white"><span class="d-none" id="spnstatusId">' + dtoDashboardHeaderlst[i].statusId +'</span>';
                    listitem += '<div class="icon-container">';
                    listitem += '<img src="/assets/images/icons/adduser.png" alt="Icon" style="height:25px">';
                    listitem += '</div>';
                    tot = 0;
                    peding = 0;
                    sent = 0;
                    var DTODashboardCount = data.dtoDashboardCountlst.filter(function (element) { return element.stagesId == dtoDashboardHeaderlst[i].stageId && element.statusId == dtoDashboardHeaderlst[i].statusId; });
                    for (var j = 0; j < DTODashboardCount.length; j++) {


                        tot += DTODashboardCount[j].tot;

                        if (DTODashboardCount[j].isComplete == false) {
                            peding += DTODashboardCount[j].tot;
                        }
                        if (DTODashboardCount[j].isComplete == true) {
                            sent += DTODashboardCount[j].tot;
                        }
                    }
                    listitem += '<h5 style="margin-top: 8px;" >' + tot + ' </h5>';
                    listitem += '<div class="t-1">' + dtoDashboardHeaderlst[i].status +'</div> ';
                    listitem += ' <div class="mb-2">';
                  
                       

                     //   listitem += '<button type="button" class="btn btn-primary"> ' + DTODashboardActionlst[j].action +' <span class="badge badge-light">9</span>';
                       
                    listitem += '<span class="badge badge-light text-black" style="font-size:12px">' + peding + '/' + sent +'</span>';
                      
                        

                        //listitem += '<span class="badge badge-primary mr-2"></span>';

                  
                  
                   
                    listitem += ' </div>';
                    listitem += ' </div>';
                    
                    listitem += '';

                    stageId=dtoDashboardHeaderlst[i].stageId;
                }

               
               
                $("#carddashboardcount").html(listitem);
                $("body").on("click", ".btnGetsummay", function () {

                    var spnstatusId = $(this).closest("div").find("#spnstatusId").html();
                    $('#ProjGetsummay').modal('show');
                    getProjGetsummay(spnstatusId);


                })
            }

        },
        error: function () {
            alert('Error fetching comments.');
        }
    });
}
function getProjGetsummay(spnstatusId) {
    var listItem = "";
    var userdata =
    {
        "StatusId": spnstatusId,

    };
    $.ajax({
        url: '/Home/GetDashboardStatusDetails',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',

        success: function (response) {
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({
                        text: ""
                    });
                }
                else if (response == 0) {
                    listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";

                    $("#DetailBodysummary").html(listItem);
                    $("#lblTotal").html(0);
                }

                else {

                    var count = 1;
                    // { attId: 8, psmId: 8, attPath: 'Swas_22ed1265-b2a0-4008-b7ff-b3eb5f704849.pdf', actionId: 0, timeStamp: '2024-05-02T16:17:45.6016413', … }
                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";

                        listItem += "<td class='align-middle'><span id='ProjName'>" + count + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].projName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stakeHolder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].fromUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].toUnitName + "</span></td>";
                        
                        listItem += "<td class='align-middle'><span id='divName'>" + response[i].action + "</span></td>";

                        if (response[i].isComplete) {
                            listItem += "<td ><span class='badge badge-success' id='divName'>Done</span></td>";
                        } else {
                            listItem += "<td ><span class='badge badge-danger' id='divName'>Pending</span></td>";
                        }
                        /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                        listItem += "</tr>";
                        count++;
                    }

                    $("#DetailBodysummary").html(listItem);
                   



                }
            }
            else {
                listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";
               
                $("#DetailBodysummary").html(listItem);
               
            }
        },
        error: function (result) {
            Swal.fire({
                text: ""
            });
        }
    });
}