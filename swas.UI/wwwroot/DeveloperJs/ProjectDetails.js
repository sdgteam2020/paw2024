$(document).ready(function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
  return new bootstrap.Tooltip(tooltipTriggerEl)
})
   

    $(".processDetail").click(function () {
        var $row = $(this).closest('tr');
        var ProjId = $row.find("#SpnCurrentProjId").text().trim();
        var psmId = $row.find("#SpnCurrentpsmId").text().trim();
        $('#confirmationModal').modal('show');

        $('#confirmSend').off('click').on('click', function () {
            var FwdDateForComment = $('#datepicker').val();
            if (FwdDateForComment === '') {
                alert('Please select date & time.');
                return; 
            }
            $('#confirmationModal').modal('hide');

            SentForComment(ProjId, psmId, 0, FwdDateForComment);
            AddNotification(ProjId,1,0);
            ProcessProjConfirm(ProjId);
        });
        IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());
       
        IsReadNotification1($(this).closest("tr").find("#SpnCurrentProjId").html(), 2);
    });

    $('#confirmationModal').on('hidden.bs.modal', function () {
        $('#datepicker').datepicker('setDate', null);
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

function SentForComment(ProjId, psmId, unitid, FwdDateForComment) {
    $.ajax({
        url: '/Projects/ProcessMail',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "FwdDateForComment": FwdDateForComment,
            "unitid": unitid, "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
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
function AddNotification(ProjId, type, unitid) {
    $.ajax({
        url: '/Notification/AddNotification',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "type": type,
            "unitid": unitid, "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            console.log(response);
            if (response && response === 1) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: 'Project Notification Added  successfully',
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
            if (response.dtoProjectMovHistorypsmlst.length) {
                listitem += '<div class="timeline-month">';

                /*listitem += '  ' + DateTimeFormatedd_mm_yyyy(new Date($.now())) + '';*/
                DTOProjectMovHistorypsmlst = response.dtoProjectMovHistorypsmlst;
                DTOProjectMovHistorycmdlst = response.dtoProjectMovHistorycmdlst;
                listitem += '  ' + DateTimeFormatedd_mm_yyyy(new Date($.now())) + '';
                listitem += '<span>' + DTOProjectMovHistorypsmlst.length + ' Entries</span>';
                listitem += '</div>';
                for (var i = 0; i < DTOProjectMovHistorypsmlst.length; i++) {

                    listitem += '<div class="timeline-section"> <div class="timeline-date"> ' + DateTimeFormatedd_mm_yyyy(DTOProjectMovHistorypsmlst[i].date) + '</div>';

                   /* listitem += '<div class="timeline-section"> <div class="timeline-date"> ' + DateFormatedd_mm_yyyy(DTOProjectMovHistorypsmlst[i].date) + '</div>';*/
                    listitem += '<div class="row"><div class="col-sm-4">';
                    listitem += '<div class="timeline-box">';
                    if (DTOProjectMovHistorypsmlst[i].isComment == false) {
                        if (DTOProjectMovHistorypsmlst[i].actions == "FWD" && (DTOProjectMovHistorypsmlst[i].undoRemarks == "" || DTOProjectMovHistorypsmlst[i].undoRemarks == null))
                            listitem += '<div class="box-title bg-warning  text-white"><i class="fa-solid fa-forward" style="color: #FFD43B;"></i> ' + DTOProjectMovHistorypsmlst[i].actions + '</div>';
                        else if (DTOProjectMovHistorypsmlst[i].actions == "Obsn")
                            listitem += '<div class="box-title bg-danger text-white"><i class="fa-solid fa-rotate-left fa-xl" style="color: #ffff;"></i> ' + DTOProjectMovHistorypsmlst[i].actions + '</div>';

                        else if (DTOProjectMovHistorypsmlst[i].undoRemarks == "" || DTOProjectMovHistorypsmlst[i].undoRemarks == null)
                            listitem += '<div class="box-title bg-success text-white"><i class="fa-solid fa-circle-check fa-xl" style="color: #3adb00;"></i> ' + DTOProjectMovHistorypsmlst[i].actions + '</div>';
                         else
                            listitem += '<div class="box-title bg-danger text-white"><i class="fa-solid fa-rotate-left fa-xl" style="color: #ffff;"></i> ' + DTOProjectMovHistorypsmlst[i].actions + '</div>';



                        listitem += '<div class="box-content">';

                        listitem += '<div class="row">';
                        listitem += '<div class="col-md-4">';
                        listitem += '<div class="box-item"><strong>Stage</strong>: </div >';
                        listitem += '</div>';

                        listitem += '<div class="col-md-8">';
                        listitem += '<div class="box-item"><span class="badge rounded-pill bg-primary">' + DTOProjectMovHistorypsmlst[i].stages + '</span></div>';
                        listitem += '</div>';
                        listitem += '</div>';

                        listitem += '<div class="row">';
                        listitem += '<div class="col-md-4">';
                        listitem += '<div class="box-item"><strong>Sub Stage</strong>: </div >';
                        listitem += '</div>';

                        listitem += '<div class="col-md-8">';
                        listitem += '<div class="box-item"><span class="badge rounded-pill bg-warning text-dark">' + DTOProjectMovHistorypsmlst[i].status + '</span></div>'; /*+ DTOProjectMovHistorypsmlst[i].status + */
                        listitem += '</div>';
                        listitem += '</div>';


                        listitem += '<div class="row">';
                        listitem += '<div class="col-md-4">';
                        listitem += '<div class="box-item"><strong>From</strong>: </div >';
                        listitem += '</div>';

                        listitem += '<div class="col-md-8">';
                        listitem += '<div class="box-item"><span class="rounded-pill bg-secondary" style="color: white;">' + DTOProjectMovHistorypsmlst[i].fromUnitName + '</span></div>';
                        listitem += '</div>';
                        listitem += '</div>';


                        listitem += '<div class="row">';
                        listitem += '<div class="col-md-4">';
                        listitem += '<div class="box-item"><strong>TO</strong>: </div >';
                        listitem += '</div>';

                        listitem += '<div class="col-md-8">';
                        listitem += '<div class="box-item"><span class="badge rounded-pill bg-secondary">' + DTOProjectMovHistorypsmlst[i].toUnitName + '</span></div>';
                        listitem += '</div>';
                        listitem += '</div>';


                        listitem += '</div>';
                        listitem += '<div class="box-footer">' + DTOProjectMovHistorypsmlst[i].userDetails + '</div>';
                        listitem += '</div></div>';




                    }
                    else if (DTOProjectMovHistorypsmlst[i].isComment == true) {
                        listitem += '<div class="box-title bg-danger text-white"><i class="fa-solid fa-comments fa-xl" style="color: #ffff;"></i> ' + DTOProjectMovHistorypsmlst[i].toUnitName +' For Comments</div>';


                        listitem += '<div class="box-content">';

                        listitem += '<div class="row">';
                        listitem += '<div class="col-md-4">';
                        listitem += '<div class="box-item"><strong>Stage</strong>: </div >';
                        listitem += '</div>';

                        listitem += '<div class="col-md-8">';
                        listitem += '<div class="box-item"><span class="badge rounded-pill bg-primary">' + DTOProjectMovHistorypsmlst[i].stages + '</span></div>';
                        listitem += '</div>';
                        listitem += '</div>';

                        listitem += '<div class="row">';
                        listitem += '<div class="col-md-4">';
                        listitem += '<div class="box-item"><strong>Sub Stage</strong>: </div >';
                        listitem += '</div>';

                        listitem += '<div class="col-md-8">';
                        listitem += '<div class="box-item"><span class="badge rounded-pill bg-warning text-dark">' + DTOProjectMovHistorypsmlst[i].toUnitName + ' For Comments</span></div>'; /*+ DTOProjectMovHistorypsmlst[i].status + */
                        listitem += '</div>';
                        listitem += '</div>';


                        listitem += '<div class="row">';
                        listitem += '<div class="col-md-4">';
                        listitem += '<div class="box-item"><strong>From</strong>: </div >';
                        listitem += '</div>';

                        listitem += '<div class="col-md-8">';
                        listitem += '<div class="box-item"><span class="rounded-pill bg-secondary" style="color: white;">' + DTOProjectMovHistorypsmlst[i].fromUnitName + '</span></div>';
                        listitem += '</div>';
                        listitem += '</div>';


                        listitem += '<div class="row">';
                        listitem += '<div class="col-md-4">';
                        listitem += '<div class="box-item"><strong>TO</strong>: </div >';
                        listitem += '</div>';

                        listitem += '<div class="col-md-8">';
                        listitem += '<div class="box-item"><span class="badge rounded-pill bg-secondary">' + DTOProjectMovHistorypsmlst[i].toUnitName + '</span></div>';
                        listitem += '</div>';
                        listitem += '</div>';
                        listitem += '</div>';
                        listitem += '<div class="box-footer">' + DTOProjectMovHistorypsmlst[i].userDetails + '</div>';
                        listitem += '</div></div>';

                        var DTODashboardCount = DTOProjectMovHistorycmdlst.filter(function (element) { return element.psmId == DTOProjectMovHistorypsmlst[i].psmId; });

                        
                        for (var c = 0; c < DTODashboardCount.length; c++) {
                            listitem += '<div class="col-sm-4">';
                            listitem += '<div class="timeline-box">';
                            listitem += '<div class="box-title">';
                            listitem += '<i class="fa fa-pencil text-info" aria-hidden="true"></i>  Comment On ' + DateFormateddMMyyyyhhmmss(DTODashboardCount[c].dateTimeOfUpdate) + '';
                            listitem += '</div>';
                            listitem += '<div class="box-content">';
                            if (DTODashboardCount[c].comments.length>75)
                                listitem += '<div class="box-item" data-toggle="tooltip" data-placement="top" title="' + DTODashboardCount[c].comments + '">' + DTODashboardCount[c].comments.substring(0, 75) + ' ........</div>';
                            else
                                listitem += '<div class="box-item" >' + DTODashboardCount[c].comments + ' </div>';

                            listitem += '</div>';
                            if (DTODashboardCount[c].status == "Observation" || DTODashboardCount[c].status == "Rejected")
                                listitem += '<div class="box-footer bg-danger">' + DTODashboardCount[c].status + '</div>';
                            else if (DTODashboardCount[c].status == "Accepted")
                                listitem += '<div class="box-footer bg-success ">' + DTODashboardCount[c].status + '</div>';
                            else 
                                listitem += '<div class="box-footer">' + DTODashboardCount[c].status + '</div>';
                            listitem += '</div></div>';
                        }
                    }
                    
                    if (DTOProjectMovHistorypsmlst[i].remarks != "") {
                        listitem += '<div class="col-sm-4">';
                        listitem += '<div class="timeline-box">';
                        listitem += '<div class="box-title">';
                        listitem += '<i class="fa fa-pencil text-info" aria-hidden="true"></i> Remarks';
                        listitem += '</div>';
                        listitem += '<div class="box-content">';
                        /*listitem += ' <a class="btn btn-xs btn-default pull-right">Remarks</a>';*/
                        listitem += '<div class="box-item">' + DTOProjectMovHistorypsmlst[i].remarks + '</div>';
                        listitem += '</div>';
                        listitem += '<div class="box-footer">' + DTOProjectMovHistorypsmlst[i].userDetails + '</div>';
                        listitem += '</div></div>';
                    }
                    if (DTOProjectMovHistorypsmlst[i].undoRemarks != null) {
                        listitem += '<div class="col-sm-4">';
                        listitem += '<div class="timeline-box">';
                        listitem += '<div class="box-title">';
                        listitem += '<i class="fa fa-pencil text-info" aria-hidden="true"></i>Undo Remarks';
                        listitem += '</div>';
                        listitem += '<div class="box-content">';
                        /*listitem += ' <a class="btn btn-xs btn-default pull-right">Remarks</a>';*/
                        listitem += '<div class="box-item">' + DTOProjectMovHistorypsmlst[i].undoRemarks + '</div>';
                        listitem += '</div>';
                        listitem += '<div class="box-footer">' + DTOProjectMovHistorypsmlst[i].userDetails + '</div>';
                        listitem += '</div></div>';
                    }
                    listitem += '</div></div>';
                    listitem += '';
                    listitem += '';
                }


                $("#projectmovfistory").html(listitem);

                //$(document).on('click', function () {
                //    location.reload();
                //});
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
function IsReadInbox(psmId) {
   
    $.ajax({
        url: '/Projects/IsReadInbox',
        type: 'POST',
        data: { "PsmId": psmId },
        success: function (response) {
            console.log(response);
            
        }
    });
}

function IsReadNotification1() {
    $.ajax({
        url: '/Notification/IsReadNotification',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "type": type
            },
        success: function (response) {
            console.log(response);

        }
    });
}

