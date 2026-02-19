$(document).ready(function () {

    initializeDataTable("#Inbox");
    initializeDataTable("#Sents");
    initializeDataTable("#Completed");
    initializeDataTable("#Remainders");
 

    var param = sessionStorage.getItem("spntabType");

    if (param != null) {
        if (param == "XR12") {
            $("#tabinbox").removeClass("active-link");
            $("#tabsent").addClass("active-link");

            $("#tabcompleted").removeClass("active-link");
            $("#tabdraft").removeClass("active-link");

            $("#sent").addClass("active-tab");
            $("#inbox").removeClass("active-tab");
            $("#Completed").removeClass("active-tab");
        } else if (param == "XRDC") {
            $("#tabinbox").addClass("active-link");
            $("#tabsent").removeClass("active-link");

            $("#tabcompleted").removeClass("active-link");

            $("#tabdraft").removeClass("active-link");

            $("#sent").removeClass("active-tab");
            $("#inbox").addClass("active-tab");
            $("#Completed").removeClass("active-tab");

        } else if (param == "XR") {
            $("#tabinbox").removeClass("active-link");
            $("#tabsent").removeClass("active-link");
            $("#tabcompleted").addClass("active-link");
            $("#tabdraft").removeClass("active-link");

            $("#sent").removeClass("active-tab");
            $("#inbox").removeClass("active-tab");
            $("#completed").addClass("active-tab");

        }
        sessionStorage.setItem("spntabType", null);
    }


    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })


    $(".processDetail").click(function () {
        ;
        var $row = $(this).closest('tr');
        var ProjId = $row.find("#SpnCurrentProjId").text().trim();
        var date_type = $row.find("#SpnDate_type").text().trim();
        var psmId = $row.find("#SpnCurrentpsmId").text().trim();
        var actiontype = $row.find("#LatestActionType").text().trim();
        fetchServerDate().then(function (S) {

            if (parseInt(actiontype) == 2 && ((actiontype) != 1 || actiontype != "")) {
                $("#datepickerContainer").show();
            } else {
                $("#datepickerContainer").hide();
            }
            $('#confirmationModal').modal('show');
            var pad = "00"
            var datef2 = new Date();
            var months = "" + `${(datef2.getMonth() + 1)}`;
            var days = "" + `${(datef2.getDate())}`;
            var monthsans = pad.substring(0, pad.length - months.length) + months
            var dayans = pad.substring(0, pad.length - days.length) + days
            var year = `${datef2.getFullYear()}`;
            var hh = pad.substring(0, pad.length - `${datef2.getHours()}`.length) + `${datef2.getHours()}`;
            var mm = pad.substring(0, pad.length - `${datef2.getMinutes()}`.length) + `${datef2.getMinutes()}`;
            var ss = `${datef2.getSeconds()}`;

            var todayDateTime = `${year}-${monthsans}-${dayans}T${hh}:${mm}`;

            var claValue = parseInt(actiontype);

            if (claValue == 2) {
                $('#datepicker').attr('type', 'datetime-local');

                $('#datepicker').attr('max', S.todayDateTime);
                $('#datepicker').prop('disabled', false); // Allow user input
                $('#datepicker').val(S.todayDateTime);
            } else {
                $('#datepicker').attr('type', 'date');

            }
            $('#confirmSend').off('click').on('click', function () {


                var dateValue = $('#datepicker').val();
                var currentDate = new Date();
                var FwdDateForComment = '';
                if ($('#datepicker').attr('type') === 'date') {

                    const formattedDate = S.todayDateTime;
                    FwdDateForComment = formattedDate;

                } else if ($('#datepicker').attr('type') === 'datetime-local') {
                    if (!dateValue) {
                        alert('Please select date and time.');
                        return;
                    }
                    FwdDateForComment = dateValue.replace('T', ' '); // Format datetime-local to space-separated
                }
                $('#confirmationModal').modal('hide');
                SentForComment(ProjId, psmId, 0, FwdDateForComment);
                
                ProcessProjConfirm(ProjId);
                IsReadInbox(psmId);
                InboxNotificationCount();
            });

        });
    });

   

    $('#x').on('hidden.bs.modal', function () {
        $('#datepicker').datepicker('setDate', null);
    });


    $("#tabCC").click(function () {

        GetCCProject();
    });
});



function GetForCommentStakeHolder(ProjId, psmId) {

    $.ajax({
        url: '/UnitDtls/GetAllStakeHolderComment',
        type: 'POST',
        data: { "Id": 0 },
        success: function (response) {


            if (response != null) {

                for (var i = 0; i < response.length; i++) {
                    SentForComment(ProjId, psmId, response[i].unitid)

                }

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
        }
    });
}
function SentForNotification(ProjId, psmId, unitid, FwdDateForComment) {
    $.ajax({
        url: '/Projects/ProcessNotification',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "FwdDateForComment": FwdDateForComment,
            "unitid": unitid, "__RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response && response === 1) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: 'Project Submit Successfully',
                    showConfirmButton: false,
                    timer: 700
                });
            }

            window.location.reload();
        },
        error: function (error) {
            console.error('Error occurred:', error);
        }
    });
}
function ProcessProjConfirm(ProjId) {
    $.ajax({
        url: '/Projects/IsProcessProjConfirm',
        type: 'POST',
        data: { "ProjId": ProjId },
        success: function (response) {
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
            debugger;
            if (response.dtoProjectMovHistorypsmlst.length) {

                listitem += '<div class="timeline-month">';

                DTOProjectMovHistorypsmlst = response.dtoProjectMovHistorypsmlst;
                DTOProjectMovHistorycmdlst = response.dtoProjectMovHistorycmdlst;
                DTOProjectCCHistorylst = response.dtoProjectCCHistorylst;

                listitem += '<span>' + DTOProjectMovHistorypsmlst.length + ' Entries</span>';
                listitem += '</div>';

                for (var i = 0; i < DTOProjectMovHistorypsmlst.length; i++) {

                    listitem += '<div class="timeline-section"><div class="timeline-date">' +
                        DateTimeFormatedd_mm_yyyy(DTOProjectMovHistorypsmlst[i].date) + '</div>';

                    listitem += '<div class="row"><div class="col-sm-4">';
                    listitem += '<div class="timeline-box">';

                    if (DTOProjectMovHistorypsmlst[i].isComment == false) {

                        if (DTOProjectMovHistorypsmlst[i].actions == "FWD" &&
                            (DTOProjectMovHistorypsmlst[i].undoRemarks == "" || DTOProjectMovHistorypsmlst[i].undoRemarks == null))
                            listitem += '<div class="box-title bg-warning text-white"><i class="fa-solid fa-forward icon-warning"></i> ' + DTOProjectMovHistorypsmlst[i].actions + '</div>';

                        else if (DTOProjectMovHistorypsmlst[i].actions == "Obsn")
                            listitem += '<div class="box-title bg-warning text-white"><i class="fa-solid fa-rotate-left fa-xl icon-white"></i> ' + DTOProjectMovHistorypsmlst[i].actions + '</div>';

                        else if (DTOProjectMovHistorypsmlst[i].undoRemarks == "" || DTOProjectMovHistorypsmlst[i].undoRemarks == null)
                            listitem += '<div class="box-title bg-success text-white"><i class="fa-solid fa-circle-check fa-xl icon-success"></i> ' + DTOProjectMovHistorypsmlst[i].actions + '</div>';

                        else
                            listitem += '<div class="box-title bg-danger text-white"><i class="fa-solid fa-rotate-left fa-xl icon-white"></i> ' + DTOProjectMovHistorypsmlst[i].actions + '</div>';

                        listitem += '<div class="box-content">';

                        listitem += '<div class="row"><div class="col-md-4"><div class="box-item"><strong>Stage</strong>:</div></div>';
                        listitem += '<div class="col-md-8"><div class="box-item"><span class="badge rounded-pill bg-primary">' +
                            DTOProjectMovHistorypsmlst[i].stages + '</span></div></div></div>';

                        listitem += '<div class="row"><div class="col-md-4"><div class="box-item"><strong>Sub Stage</strong>:</div></div>';
                        listitem += '<div class="col-md-8"><div class="box-item"><span class="badge rounded-pill bg-warning text-dark">' +
                            DTOProjectMovHistorypsmlst[i].status + '</span></div></div></div>';

                        listitem += '<div class="row"><div class="col-md-4"><div class="box-item"><strong>From</strong>:</div></div>';
                        listitem += '<div class="col-md-8"><div class="box-item">';

                        if (DTOProjectMovHistorypsmlst[i].isPulledBack == 0)
                            listitem += '<span class="rounded-pill bg-secondary text-white-force">' +
                                DTOProjectMovHistorypsmlst[i].fromUnitName + '</span>';
                        else {
                            let fromlist = DTOProjectMovHistorypsmlst[i].fromUnitName.split('(');
                            listitem += '<span class="rounded-pill bg-secondary text-white-force">' +
                                fromlist[1].replace(')', '') + '</span>';
                        }

                        listitem += '</div></div></div>';

                        listitem += '<div class="row"><div class="col-md-4"><div class="box-item"><strong>TO</strong>:</div></div>';
                        listitem += '<div class="col-md-8"><div class="box-item"><span class="badge rounded-pill bg-secondary">' +
                            DTOProjectMovHistorypsmlst[i].toUnitName + '</span></div></div></div>';

                        listitem += '</div>';

                        if (DTOProjectMovHistorypsmlst[i].isPulledBack == 0)
                            listitem += '<div class="box-footer">' + DTOProjectMovHistorypsmlst[i].userDetails + '</div>';
                        else {
                            var fromlist1 = DTOProjectMovHistorypsmlst[i].fromUnitName.split('(');
                            listitem += '<div class="box-footer">' + fromlist1[1].replace(')', '') + '</div>';
                        }

                        listitem += '</div></div>';
                    }

                    else {
                        listitem += '<div class="box-title bg-danger text-white"><i class="fa-solid fa-comments fa-xl icon-white"></i> ' +
                            DTOProjectMovHistorypsmlst[i].toUnitName + ' for Comments</div>';

                        listitem += '<div class="box-content">';
                        listitem += '<div class="box-item"><span class="badge rounded-pill bg-primary">' +
                            DTOProjectMovHistorypsmlst[i].stages + '</span></div>';
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
                            if (DTODashboardCount[c].comments.length > 75)
                                listitem += '<div class="box-item" data-toggle="tooltip" data-placement="top" title="' + DTODashboardCount[c].comments + '">' + DTODashboardCount[c].comments.substring(0, 75) + ' ........</div>';
                            else
                                listitem += '<div class="box-item" >' + DTODashboardCount[c].comments + ' </div>';

                            listitem += '</div>';
                            if (DTODashboardCount[c].status == "Obsn")
                                listitem += '<div class="box-footer bg-warning">' + DTODashboardCount[c].status + ' by ' + DTODashboardCount[c].userDetails + '</div>'
                            else if (DTODashboardCount[c].status == "Observation" || DTODashboardCount[c].status == "Rejected")
                                listitem += '<div class="box-footer bg-danger">' + DTODashboardCount[c].status + ' by ' + DTODashboardCount[c].userDetails + '</div>';
                            else if (DTODashboardCount[c].status == "Accepted")
                                listitem += '<div class="box-footer bg-success ">' + DTODashboardCount[c].status + ' by ' + DTODashboardCount[c].userDetails + '</div>';
                            else
                                listitem += '<div class="box-footer">' + DTODashboardCount[c].status + ' by ' + DTODashboardCount[c].userDetails + '</div>';
                            listitem += '</div></div>';
                        }
                    }

                    if (DTOProjectMovHistorypsmlst[i].remarks != "") {
                        listitem += '<div class="col-sm-4">';
                        listitem += '<div class="timeline-box">';
                        listitem += '<div class="box-title">';
                        listitem += '<i class="fa fa-pencil text-info" aria-hidden="true"></i> Remarks On ' + DateTimeFormatedd_mm_yyyy(DTOProjectMovHistorypsmlst[i].date);
                        listitem += '</div>';
                        listitem += '<div class="box-content">';
                        if (DTOProjectMovHistorypsmlst[i]?.isPulledBack === true && DTOProjectMovHistorypsmlst[i]?.undoRemarks == null) {
                            listitem += '<div class="box-item">' + '<strong>Pulled Back by</strong> -  ' + DTOProjectMovHistorypsmlst[i].remarks + '</div>';
                        } else {
                            listitem += '<div class="box-item">' + DTOProjectMovHistorypsmlst[i].remarks + '</div>';
                        }
                        listitem += '</div>';
                        if (DTOProjectMovHistorypsmlst[i].actions == "Obsn") {
                            listitem += '<div class="box-footer bg-warning">' + DTOProjectMovHistorypsmlst[i].userDetails + '</div>';
                        } else if (DTOProjectMovHistorypsmlst[i].isPulledBack == 0 && DTOProjectMovHistorypsmlst[i].actions != "Obsn") {
                            listitem += '<div class="box-footer ">' + DTOProjectMovHistorypsmlst[i].userDetails + '</div>';
                        } else {

                            listitem += '<div class="box-footer ">' + fromlist1[1].replace(')', '') + '</div>';
                        }
                        listitem += '</div></div>';
                    }
                    var DTOProjectCCHistorycccpsmid = DTOProjectCCHistorylst.filter(function (element) { return element.psmId == DTOProjectMovHistorypsmlst[i].psmId; });

                    if (DTOProjectCCHistorycccpsmid.length > 0) {
                        for (let cc = 0; cc < DTOProjectCCHistorycccpsmid.length; cc++) {
                            listitem += '<div class="col-sm-4">';
                            listitem += '<div class="timeline-box">';
                            listitem += '<div class="box-title bg-warning">';
                            listitem += '<i class="fa-solid fa-closed-captioning fa-2x"></i>';
                            listitem += '</div>';
                            listitem += '<div class="box-content">';

                            let readon = "";


                            listitem += '<div class="box-item">' + '<strong>Unit Name : </strong>' + DTOProjectCCHistorycccpsmid[cc].unitName + ' </div>';
                            if (DTOProjectCCHistorycccpsmid[cc].isRead == true) {

                                listitem += '<div class="box-item">' + '<strong>Read on : </strong>' + DateTimeFormatedd_mm_yyyy(DTOProjectCCHistorycccpsmid[cc].readDate) + ' </div>';
                                listitem += '<div class="box-item">' + '<strong>Read By : </strong>' + DTOProjectCCHistorycccpsmid[cc].userDetails + ' </div>';
                            }



                            listitem += '</div>';
                            listitem += '</div></div>';
                        }
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

function GetCCProject() {

    let listitem = "";

    $.ajax({
        url: '/Projects/GetActCcProject',
        type: 'POST',

        success: function (response) {
            if (response != null) {
                let count = 0;

                for (let i = 0; i < response.length; i++) {
                    count++;

                    listitem += '<tr>';

                    listitem += '<td><div class="d-flex">' + count;

                    if (response[i].isRead == false)
                        listitem += '<svg xmlns="" width="16" height="16" class="icon-green" fill="currentColor" viewBox="0 0 16 16">' +
                            '<path d="M10.97 4.97a.75.75 0 0 1 1.07 1.05l-3.99 4.99a.75.75 0 0 1-1.08.02L4.324 8.384a.75.75 0 1 1 1.06-1.06l2.094 2.093 3.473-4.425z" />' +
                            '</svg>';
                    else
                        listitem += '<svg xmlns="" viewBox="0 0 448 512" width="16" height="16" class="icon-green" fill="currentColor">' +
                            '<path d="M342.6 86.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L160 178.7l-57.4-57.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l80 80c12.5 12.5 32.8 12.5 45.3 0l160-160zm96 128c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L160 402.7 54.6 297.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l128 128c12.5 12.5 32.8 12.5 45.3 0l256-256z" />' +
                            '</svg>';

                    listitem += '</div></td>';

                    listitem += '<td>' +
                        '<span class="d-none noExport" id="SpnCurrentccProjId">' + response[i].projId + '</span>' +
                        '<a data-proj-name="' + response[i].projName + '" data-proj-id="' + response[i].projId + '" href="/Projects/ProjHistory?EncyID=' + response[i].encyID + '&amp;Type=XR12">' +
                        '<div class="RefLetter-container" data-tooltip="' + response[i].projName + '">' +
                        '<span id="projNamecc">' + trimByChars(response[i].projName, 20) + '</span>' +
                        '<span class="RefLetter noExport">' + breakLinesByWords(response[i].projName, 4) + '</span>' +
                        '</div></a></td>';

                    listitem += '<td class="RefLetter-container">' + response[i].unitName +
                        '<div class="RefLetter noExport">' + breakLinesByWords(response[i].sponsor, 3) + '</div></td>';

                    listitem += '<td class="RefLetter-container">' + response[i].fromUnitUserDetail +
                        '<div class="RefLetter noExport">' + breakLinesByWords(response[i].fromUnitName, 4) + '</div></td>';

                    listitem += '<td>' + DateFormateddMMyyyyhhmmss(response[i].timeStamp) + '</td>';

                    let user = response[i].userDetails;
                    listitem += '<td class="RefLetter-container"><div class="noExport">' + trimByWords(user, 2) + '</div>';

                    if (user != "")
                        listitem += '<div class="RefLetter">' + breakLinesByWords(user, 4) + '</div>';

                    listitem += '</td>';

                    if (response[i].userDetails != "")
                        listitem += '<td>' + DateFormateddMMyyyyhhmmss(response[i].readDate) + '</td>';
                    else
                        listitem += '<td></td>';

                    listitem += '<td>' + response[i].stage + '</td>';
                    listitem += '<td>' + response[i].status + '</td>';

                    listitem += '<td>' +
                        '<div class="RefLetter-container btn btn-warning p-2 cc-btn">' +
                        '<span>Cc</span>' +
                        '<div class="RefLetter noExport">' + response[i].ccUnitName + '</div>' +
                        '</div></td>';

                    listitem += '<td><div class="row d-flex">' +
                        '<div class="col-md-2">' +
                        '<button type="button" class="btn btn-success btn-FwdHistoryCcc btn-history" title="History">' +
                        '<i class="fa-solid fa-timeline"></i></button>' +
                        '</div></div></td>';

                    listitem += '</tr>';
                }

                $("#cctblData").html(listitem);
                initializeDataTable("#CCtable");

                $(document).on('click', ".btn-FwdHistoryCcc", function () {

                    var projName = $(this).closest("tr").find("#projNamecc").html();
                    var finalTitle = "Mov History: " + projName;

                    $('#ProjFwdHistory').modal('show');
                    $('.lblHistory').text(finalTitle);

                    GetProjectMovHistory($(this).closest("tr").find("#SpnCurrentccProjId").html());
                });
            }
        }
    });
}





function truncateText(text, maxWords) {
    if (!text || text.trim() === '') {
        return '';
    }

    var words = text.trim().split(/\s+/); // split by whitespace
    var truncated = words.slice(0, maxWords).join(' ');
    return truncated;
}

$(document).on("click", ".date-action", function (e) {
    e.preventDefault();
    ;
   
    const action = $(this).data("action");
   
    const userReq = (action === "back"); // true for 'back', false otherwise

    const $row = $(this).closest("tr");
    const projId = $row.find("#SpnCurrentProjId").text().trim();
    const actiontype = $(this).data("actiontype");

    if (!projId) {
        Swal.fire("Error!", "Project ID not found in row.", "error");
        return;
    }

    Swal.fire({
        title: "Are you sure?",
        html: action === "back"
            ? `Do You want to Send the Legacy Project.<br> Please enter remarks:`
            : "You want Current date to this project. Please enter remarks:",
        input: "textarea",
        inputPlaceholder: "Enter remarks here...",
        inputAttributes: {
            "aria-label": "Remarks"
        },
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, Submit",
        preConfirm: (remarks) => {
            if (!remarks) {
                Swal.showValidationMessage("Remarks are required.");
            }
            if (remarks.length < 10) {
                Swal.showValidationMessage('Remarks Must be Atleast 10 characters');
            }
            if (remarks.length > 200) {
                Swal.showValidationMessage('Remarks Must not exceed 200 characters');
            }
            return remarks;
        }
    }).then((result) => {
        ;
        if (result.isConfirmed && result.value) {
            ;
            var remarks = result.value;

            $.ajax({
                url: "/Projects/LogDateApprovalWithRemarks",
                type: "POST",
                data: {

                    ProjId: projId,
                    UserReq: userReq,
                    actiontype: actiontype,
                    remarks: remarks,
                    
                }, headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    Swal.fire({
                        title: response.success ? "Success!" : "Warning!",
                        text: response.message,
                        icon: response.success ? "success" : "warning"
                    }).then(() => {
                        if (response.success) {
                            location.reload();
                        }
                    });
                },
                error: function (xhr, status, error) {
                    ;
                    Swal.fire("Error!", "Something went wrong: " + error, "error");
                }
            });
        }
    });



});





$(document).on('click', '#btnRemainder', function () {
    var projid = $(this).data('projid');
    var projname = $(this).data('projname');
    Swal.fire({
        title: `Project:${projname}`,
        html: `Please Enter Remarks for Reminder`,
        input: "textarea",
        inputPlaceholder: "Enter remarks here...",
        inputAttributes: {
            "aria-label": "Remarks"
        },
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, Submit",
        preConfirm: (remarks) => {
            if (!remarks) {
                Swal.showValidationMessage("Remarks are required.");
            }
            if (remarks.length < 10) {
                Swal.showValidationMessage('Remarks Must be Atleast 10 characters');
            }
            if (remarks.length > 200) {
                Swal.showValidationMessage('Remarks Must not exceed 100 characters');
            }
            return remarks;
        }
    }).then((result) => {
        if (result.isConfirmed && result.value) {
            SendRemainder(projid, result.value); // passing remarks too if needed
        }
    });



});

function SendRemainder(projid, remarks) {
  
    $.ajax({
        url: '/Projects/SendRemainder',
        type: 'POST',
        data: {
            ProjId: projid,
            Remarks: remarks // if you want to send remarks to backend
        },
        success: function (response) {
            ;
            console.log(response);
            if (response > 0) {
                Swal.fire("Success", "Reminder sent successfully", "success");
            } else {
                Swal.fire("Error", "Something went wrong", "error");
            }
        },
        error: function () {
            Swal.fire("Error", "Ajax call failed", "error");
        }
    });
}










$(document).on('click', '#btnRemMove', function () {
    ;


    var ProjId = parseInt($(this).data("action"));

    var projName = $(this).data('proj-name');
    var words = projName.split(" ");
    var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;
    var finalTitle = "Reminder History: " + projName;
    $('#RemProjName').text(finalTitle);


    GetProjRemainderMov(ProjId); // <-- fixed this line
});



function GetProjRemainderMov(ProjId) {
    ;
    $('#ProjRemainderMov').modal('show');

    $.ajax({
        url: '/Projects/GetProjectRemainderHistory',
        type: 'GET',
        data: { ProjectId: ProjId },
        success: function (response) {
            console.log(response); // Debugging
            ;

            const data = response.data || []; // Expecting array
            const length = data.length;
            let listItem = '';

            if (length > 0) {
                for (let i = 0; i < length; i++) {
                    const item = data[i];
                    const projName = item.projName || 'N/A';
                    const words = projName.split(" ");
                    const shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;

                    listItem += "<tr>";
                    listItem += "<td class='align-middle'>" + (i + 1) + "</td>";

                    listItem += '<td class="RefLetter-container align-middle">' +
                        '' + item.unitName + '' +
                        '<div class="RefLetter noExport">' +
                        '' + item.sponsor + '' +
                        '</div></td>';

                   

                    
                    listItem += "<td class='align-middle'><div class='col-md-24'>" + DateFormated(item.sentOn || '-') + "</div></td>";
                    listItem += '<td class="RefLetter-container align-middle">' +
                        '' + item.fromUnit + '' +
                        '<div class="RefLetter noExport">' +
                        '' + item.userDetails + '' +
                        '</div></td>';
                    
                    listItem += '<td class="RefLetter-container align-middle">' +
                        '' + item.toUnit + '' +
                        '<div class="RefLetter noExport">' +
                        '' + (item.touserDetails || 'Action Pending...') + '' +
                        '</div></td>';
                   
                  
                    listItem += "<td class='align-middle' ><div class='col-md-24'>" + (item.readOn || '-') + "</div></td>"; 
                  


                    const remarks = item.remarks || 'No Remarks';
                    const remarkWords = remarks.split(" ");
                    const shortRemarks = remarkWords.length > 3 ? remarkWords.slice(0, 4).join(" ") + "..." : remarks;

                    listItem += '<td class="RefLetter-container shortremarks align-middle">' +
                        shortRemarks +
                        '<div class="RefLetter noExport" >' +
                        remarks +
                        '</div></td>';
                    listItem += "</tr>";
                }


                if ($.fn.DataTable.isDataTable("#Trakingtable")) {
                    $("#Trakingtable").DataTable().clear().destroy();
                }

                $("#ProjRemMov").html(listItem);
                initializeDataTable("#Trakingtable")

                if (data[0].projName) {
                    $("#RemProjName").text(data[0].projName);
                }

            } else {
                $("#ProjRemMov").html('<tr><td colspan="8" class="text-center">No history available.</td></tr>');
                $("#RemProjName").text('');
            }
        },
        error: function () {
            $("#ProjRemMov").html('<tr><td colspan="8" class="text-center text-danger">Failed to load history.</td></tr>');
            $("#RemProjName").text('');
        }
    });
}



$(document).on("click", "#ReadRemainderNoti", function (e) {
    e.preventDefault();
   
    ;

    var $this = $(this);
    var projId = parseInt($this.data("projid"));
    $this.closest("tr").removeClass("bold-text");
    var psmId = $this.closest("tr").find("#SpnCurrentpsmId").text();
    IsReadInbox(psmId);
    $("#Remainderbedge").text("0").hide();
    updateReadDateForRemainder(projId);
   
});


function updateReadDateForRemainder(ProjId) {
    $.ajax({
        url: '/Projects/UpdateRemaRead',
        type: 'GET',
        data: { ProjectId: ProjId },
        success: function (response) {
           
            if (response.success) {
                setTimeout(function () {
                    GetProjRemainderMov(ProjId);
                    InboxNotificationCount();
                }, 200);
            }
          
        },
        error: function (error, xhr) {
            console.log(error);
        }

    });
};
$(document).ready(function () {
    $('.hover-container').hover(
        function () {
            $(this).find('.hover-popup').stop(true, true).fadeIn(200);
        },
        function () {
            $(this).find('.hover-popup').stop(true, true).fadeOut(200);
        }
    );
});




$("#tabParked").click(function () {

    GetParkedProject();
});


function GetParkedProject() {
    ;
    $.ajax({
        url: '/Projects/GetActParkedProject',
        type: 'GET',
        success: function (response) {
            $("#parkedtblData").html("");

            if (response != null && response.length > 0) {
                let listitem = "";
                let count = 0;
                var prjname = response[0].projName;
              
                response.forEach(function (project) {
                   
                    count++;

                    listitem += '<tr>';
                    listitem += `<td><div class="d-flex">${count}</div></td>`;
                    listitem += `
                        <td>
                            <span class="d-none noExport" id="SpnCurrentParkedProjId">${project.projId}</span>
                            <span id="spnCurrentPsmid" class="d-none noExport">${project.psmIds}</span>
                            <a data-proj-name="${project.projName}" data-proj-id="${project.projId}" href="/Projects/ProjHistory?EncyID=${project.encyID}&amp;Type=XR12">
                                <div class="RefLetter-container" data-tooltip="${project.projName}">
                                    <span>${trimByChars(project.projName, 20)}</span>
                                    <span class="RefLetter noExport" >${breakLinesByWords(project.projName, 4)}</span>
                                    <span class="noExport d-none"  id="projNamecc">${project.projName}</span>
                                </div>
                            </a>
                        </td>`;
                    listitem += `
                        <td class="RefLetter-container">
                            ${project.unitName}
                            <div class="RefLetter noExport">${breakLinesByWords(project.sponsor, 3)}</div>
                        </td>`;
                    listitem += `
                        <td class="RefLetter-container">
                            ${project.fromUnitUserDetail}
                            <div class="RefLetter noExport">${breakLinesByWords(project.fromUnitName, 4)}</div>
                        </td>`;
                    listitem += `<td>${DateFormateddMMyyyyhhmmss(project.timeStamp)}</td>`;
                    listitem += `<td>${project.stage}</td>`;
                    listitem += `<td>${project.subStage ?? project.status ?? ""}</td>`;
                    listitem += `
                        <td>
                            <div class="btn btn-warning p-2">
                                <span>Parked</span>
                            </div>
                        </td>`;
                    listitem += `
                        <td>
                            <div class="row d-flex align-items-center">
                                <div class="col-auto p-0 me-1">
                                    <button type="button" class="btn btn-success btn-FwdHistoryParked" data-proj-name="${project.projName}" title="History">
                                        <i class="fa-solid fa-timeline"></i>
                                    </button>
                                </div>
                                <div class="col-auto p-0">
                                    <button class="btn btn-danger btn_unparked">Send to Inbox</button>
                                </div>
                            </div>
                        </td>`;

                    listitem += '</tr>';
                });

                $("#parkedtblData").html(listitem);
            }

            initializeDataTable("#parkedtable");
            $(".btn-FwdHistoryParked").off('click').on('click', function () {
              
                var projName = $(this).closest("tr").find("#projNamecc").html();
                $('.lblHistory').text(`Mov History: ${projName}`);
                $('#ProjFwdHistory').modal('show');

                GetProjectMovHistory($(this).closest("tr").find("#SpnCurrentParkedProjId").html());
            });
            $(".btn_unparked").off('click').on('click', function () {
                var psmid = $(this).closest("tr").find("#spnCurrentPsmid").text();
                Swal.fire({
                    title: "Are you sure?",
                    text: "Do you want to send this project to Inbox.",
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#3085d6",
                    cancelButtonColor: "#d33",
                    confirmButtonText: "Yes"
                }).then((result) => {
                    if (result.isConfirmed) {
                        Unparkedbypsmid(psmid);
                    }

                });
                           
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching parked projects:", status, error);
            if ($.fn.DataTable.isDataTable("#parkedtable")) {
                $("#parkedtable").DataTable().destroy();
            }
            $("#parkedtblData").html("");
            $("#parkedtable").DataTable({
                language: {
                    emptyTable: "Error loading data. Please try again later."
                },
                destroy: true
            });
        }
    });
}




function Unparkedbypsmid(psmid) {

    $.ajax({
        url: '/Projects/ParkedProject',
        type: 'POST',
        data: { psmid: psmid },
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
      
        success: function (response) {

            if (response && response != null) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: response.message,
                    showConfirmButton: false,
                    timer: 1000   // Increased time
                });
            }
            setTimeout(function () {
                window.location.reload();
            }, 1000);
        }
    });
}
$(".parkedProj").on('click', function () {

    var psmid = $(this).closest("tr").find("#SpnCurrentpsmId").text();
    Swal.fire({
        title: "Are you sure?",
        text: "Do you want to Park this project.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes"
    }).then((result) => {
        if (result.isConfirmed) {
            Unparkedbypsmid(psmid);
        }

    });
});




	$(document).ready(function () {
		$(document).on('click', '.pdf', function () {
			$('#AttHistoryView').modal('show');
		});
	});




	const containerTabs = document.getElementById("tabs");

	const getInfoTabs = container => {
		return [...container.querySelectorAll(".tabs__content__item")];
	};

	const getLinksTab = container => {
		return [...container.querySelectorAll("a[data-tab]")];
	};

	const activateTab = (tabId) => {
		const tabsInfo = getInfoTabs(containerTabs);

		tabsInfo.forEach(tab => {
			const isActive = tab.getAttribute("id") === tabId;
			tab.classList.toggle("active-tab", isActive);
		});
	};

	const activateLink = (link) => {
		const linksTab = getLinksTab(containerTabs);
		linksTab.forEach(tabLink => {
			tabLink.classList.toggle("active-link", tabLink === link);
		});
	};

	const handleTabClick = event => {
		const clickedElement = event.target;
		if (clickedElement.tagName === "A" && clickedElement.hasAttribute("data-tab")) {
			event.preventDefault();
			const tabId = clickedElement.getAttribute("data-tab");
			activateTab(tabId);

			activateLink(clickedElement);



		}
	};

	containerTabs.addEventListener("click", handleTabClick);





