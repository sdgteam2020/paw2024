$(document).ready(function () {

    $(".processDetail").click(function () {

       
        var ProjId = $(this).closest("tr").find("#SpnCurrentProjId").html();
        var psmId = $(this).closest("tr").find("#SpnCurrentpsmId").html();
        GetForCommentStakeHolder(ProjId, psmId);
       
    });
});
function GetForCommentStakeHolder(ProjId, psmId) {

    $.ajax({
        url: '/UnitDtls/GetAllStakeHolderComment',
        type: 'POST',
        data: { "Id": 0 },
        success: function (response) {
            console.log(response);


            if (response != null) {

                for (var i = 0; i < response.length; i++) {
                    SentForComment(ProjId, psmId, response[i].unitid)

                }
                //SentForComment(ProjId, psmId);

                ProcessProjConfirm(ProjId)
            }

        }
    });
}
function SentForComment(ProjId, psmId, unitid) {
    $.ajax({
        url: '/Projects/ProcessMail',
        type: 'POST',
        data: { "ProjId": ProjId, "unitid": unitid, "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                         success: function (response) {
                             console.log(response);
                             if (response && response === 1) {
                                 Swal.fire({
                                     position: 'top-end',
                                     icon: 'success',
                                     title: 'Project Processed successfully',
                                     showConfirmButton: false,
                                     timer: 700
                                 });
                             }

                             window.location.reload();
                         },
                         error: function (error) {
                             console.error('Error occurred:', error);
                             // Handle error if needed
                         }
                     });
}
function ProcessProjConfirm(ProjId) {
    $.ajax({
        url: '/Projects/IsProcessProjConfirm',
        type: 'POST',
        data: { "ProjId": ProjId },
        success: function (response) {
            console.log(response);
            if (response >= 1) {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Project Successfully Submitted..!",
                    showConfirmButton: false,
                    timer: 1500
                });

            }
        }
    });
}
function FwdProjConfirm(psmId) {
        $.ajax({
            url: '/Projects/FwdProjConfirm',
            type: 'POST',
            data: { "PslmId": psmId },
            success: function (response) {
                console.log(response);
                if (response >= 1) {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Project Successfully Submitted..!",
                        showConfirmButton: false,
                        timer: 1500
                    });

                }
            }
        });
    }

function GetProjectMovHistory(ProjId) {
    var listitem = "";
    $.ajax({
        url: '/Projects/ProjectMovHistory',
        type: 'POST',
        data: { "ProjectId": ProjId },
        success: function (response) {
            console.log(response);
            if (response.length) {
                listitem += '<div class="timeline-month">';
                listitem += '  August, 2018';
                listitem += '<span>3 Entries</span>';
                listitem += '</div>';
                for (var i = 0; i < response.length; i++) {
                    listitem += '<div class="timeline-section"> <div class="timeline-date"> ' + DateFormatedd_mm_yyyy(response[i].date)+'</div>';
                    listitem += '<div class="row"><div class="col-sm-4">';
                    listitem += '<div class="timeline-box">';
                    if (response[i].actions == "FWD" && (response[i].undoRemarks == "" || response[i].undoRemarks == null))
                        listitem += '<div class="box-title bg-warning  text-white"><i class="fa-solid fa-forward" style="color: #FFD43B;"></i> ' + response[i].actions + '</div>';
                    else if (response[i].undoRemarks == "" || response[i].undoRemarks == null)
                        listitem += '<div class="box-title bg-success text-white"><i class="fa-solid fa-circle-check fa-xl" style="color: #3adb00;"></i> ' + response[i].actions + '</div>';
                    else 
                        listitem += '<div class="box-title bg-danger text-white"><i class="fa-solid fa-rotate-left fa-xl" style="color: #ffff;"></i> ' + response[i].actions + '</div>';

                    listitem += '<div class="box-content">';
                    /*listitem += '<a class="btn btn-xs btn-default pull-right">Details</a>';*/
                    listitem += '<div class="box-item"><strong>Stages</strong>: ' + response[i].stages +'</div>';
                    listitem += '<div class="box-item"><strong>Sub Stages</strong>: ' + response[i].status +'</div>';
                    listitem += '<div class="box-item"><strong>From Unit</strong>: ' + response[i].fromUnitName +'</div>';
                    listitem += '<div class="box-item"><strong>TO Unit</strong>: ' + response[i].toUnitName +'</div>';
                    listitem += '</div>';
                    listitem += ' <div class="box-footer"></div>';
                    listitem += '</div> </div>';
                    if (response[i].remarks != "") {
                        listitem += '<div class="col-sm-4">';
                        listitem += '<div class="timeline-box">';
                        listitem += '<div class="box-title">';
                        listitem += '<i class="fa fa-pencil text-info" aria-hidden="true"></i> Remarks';
                        listitem += '</div>';
                        listitem += '<div class="box-content">';
                        /*listitem += ' <a class="btn btn-xs btn-default pull-right">Remarks</a>';*/
                        listitem += '<div class="box-item">' + response[i].remarks + '</div>';
                        listitem += '</div>';
                        listitem += '<div class="box-footer"></div>';
                        listitem += '</div></div>';
                    }
                    if (response[i].undoRemarks != null) {
                        listitem += '<div class="col-sm-4">';
                        listitem += '<div class="timeline-box">';
                        listitem += '<div class="box-title">';
                        listitem += '<i class="fa fa-pencil text-info" aria-hidden="true"></i>Undo Remarks';
                        listitem += '</div>';
                        listitem += '<div class="box-content">';
                        /*listitem += ' <a class="btn btn-xs btn-default pull-right">Remarks</a>';*/
                        listitem += '<div class="box-item">' + response[i].undoRemarks + '</div>';
                        listitem += '</div>';
                        listitem += '<div class="box-footer"></div>';
                        listitem += '</div></div>';
                    }
                    listitem += '</div></div>';
                    listitem += '';
                    listitem += '';
                }


                $("#projectmovfistory").html(listitem);
            }
        }
    });
}
function Reset() {
    $("#spanFwdProjectId").html(0);
    $("#ddlfwdStage").val("");
    $("#ddlfwdSubStage").val("");
    $("#ddlfwdAction").val("");
    $("#txtRemarksfwd").val("");
    $("#ddlfwdFwdTo").val(0);
    $("#Reamarks").val("");
    $("#pdfFileInput").val("");
}