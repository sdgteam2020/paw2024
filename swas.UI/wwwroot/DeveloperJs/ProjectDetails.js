$(document).ready(function () {

    initializeDataTable("#Inbox");
    initializeDataTable("#Sents");
    initializeDataTable("#Completed");
    initializeDataTable("#Remainders");
 

    let param = sessionStorage.getItem("spntabType");

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


    let tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    let tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })


    $(document).on("click", ".processDetail",function () {
        
        let $row = $(this).closest('tr');
        let ProjId = $row.find(".SpnCurrentProjId").text().trim();
        let Encriptedprojdid = $row.find(".SpnEncryptedProjId").text().trim();
        let date_type = $row.find("#SpnDate_type").text().trim();
        let psmId = $row.find(".SpnCurrentpsmId").text().trim();
        let actiontype = $row.find("#LatestActionType").text().trim();
        fetchServerDate().then(function (S) {

            if (parseInt(actiontype) == 2 && ((actiontype) != 1 || actiontype != "")) {
                $("#datepickerContainer").show();
            } else {
                $("#datepickerContainer").hide();
            }
            $('#confirmationModal').modal('show');
            let pad = "00"
            let datef2 = new Date();
            let months = "" + `${(datef2.getMonth() + 1)}`;
            let days = "" + `${(datef2.getDate())}`;
            let monthsans = pad.substring(0, pad.length - months.length) + months
            let dayans = pad.substring(0, pad.length - days.length) + days
            let year = `${datef2.getFullYear()}`;
            let hh = pad.substring(0, pad.length - `${datef2.getHours()}`.length) + `${datef2.getHours()}`;
            let mm = pad.substring(0, pad.length - `${datef2.getMinutes()}`.length) + `${datef2.getMinutes()}`;
            let ss = `${datef2.getSeconds()}`;

            let todayDateTime = `${year}-${monthsans}-${dayans}T${hh}:${mm}`;

            let claValue = parseInt(actiontype);

            if (claValue == 2) {
                $('#datepicker').attr('type', 'datetime-local');

                $('#datepicker').attr('max', S.todayDateTime);
                $('#datepicker').prop('disabled', false); // Allow user input
                $('#datepicker').val(S.todayDateTime);
            } else {
                $('#datepicker').attr('type', 'date');

            }
            $('#confirmSend').off('click').on('click', function () {


                let dateValue = $('#datepicker').val();
                let currentDate = new Date();
                let FwdDateForComment = '';
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
                
                ProcessProjConfirm(Encriptedprojdid);
                IsReadInbox(psmId);
                InboxNotificationCount();
            });

        });
    });

   

    $('#x').on('hidden.bs.modal', function () {
        $('#datepicker').datepicker('setDate', null);
    });


    $(document).on("click", "#tabCC",function () {

        GetCCProject();
    });
});



function SentForComment(ProjId, psmId, unitid, FwdDateForComment) {


    let userdata = {
        ProjId: ProjId,
        FwdDateForComment: FwdDateForComment,
        unitid: unitid
    };

    let encrypted_data = encryptData(userdata);

    $.ajax({
        url: '/Projects/ProcessMail',
        type: 'POST',
        data: {
            encrypted_data: encrypted_data,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {

            if (response && response.success) {
                Swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: response.message || 'Project processed successfully',
                    showConfirmButton: false,
                    timer: 1000
                }).then(() => {
                    window.location.reload();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response?.message || 'Something went wrong'
                });
            }
        },
        error: function (xhr) {

            let msg = "Something went wrong";

            if (xhr.responseJSON && xhr.responseJSON.message) {
                msg = xhr.responseJSON.message;
            }

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: msg
            });

            console.error('Error occurred:', xhr);
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
    let encrypted = encryptData(ProjId)
    let listitem = "";

    $.ajax({
        url: '/Projects/ProjectMovHistory',
        type: 'POST',
        data: { "ProjectId": encrypted },
        success: function (response) {
            
            if (!response || !response.data) return;

            let data = response.data;

            let DTOProjectMovHistorypsmlst = data.dtoProjectMovHistorypsmlst || [];
            let DTOProjectMovHistorycmdlst = data.dtoProjectMovHistorycmdlst || [];
            let DTOProjectCCHistorylst = data.dtoProjectCCHistorylst || [];

            if (DTOProjectMovHistorypsmlst.length) {

                for (let i = 0; i < DTOProjectMovHistorypsmlst.length; i++) {

                    listitem += '<div class="timeline-section"><div class="timeline-date">' +
                        DateTimeFormatedd_mm_yyyy(DTOProjectMovHistorypsmlst[i].date) + '</div>';

                    listitem += '<div class="row"><div class="col-sm-4">';
                    listitem += '<div class="timeline-box">';
                    let fromlist1 = DTOProjectMovHistorypsmlst[i].fromUnitName.split('(');
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

                        let DTODashboardCount = DTOProjectMovHistorycmdlst.filter(function (element) { return element.psmId == DTOProjectMovHistorypsmlst[i].psmId; });


                        for (let c = 0; c < DTODashboardCount.length; c++) {
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
                    let DTOProjectCCHistorycccpsmid = DTOProjectCCHistorylst.filter(function (element) { return element.psmId == DTOProjectMovHistorypsmlst[i].psmId; });

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
        },
          error: function (xhr) {
            console.error("Error fetching Project Movement History", xhr);
            $("#projectmovfistory").html("<div class='text-danger'>Failed to load data</div>");
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

                    let projName = $(this).closest("tr").find("#projNamecc").html();
                    let finalTitle = "Mov History: " + projName;

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

    let words = text.trim().split(/\s+/); // split by whitespace
    let truncated = words.slice(0, maxWords).join(' ');
    return truncated;
}

$(document).on("click", ".date-action", function (e) {
    e.preventDefault();
    
   
    const action = $(this).data("action");
   
    const userReq = (action === "back"); // true for 'back', false otherwise

    const $row = $(this).closest("tr");
    const projId = $row.find(".SpnCurrentProjId").text().trim();
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
        
        if (result.isConfirmed && result.value) {
            ;
            let remarks = result.value;
            $.ajax({
                url: "/Projects/LogDateApprovalWithRemarks",
                type: "POST",
                data: {
                    ProjId: projId,
                    UserReq: userReq,
                    actiontype: actiontype,
                    remarks: remarks
                },
                headers: {
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
                error: function (xhr) {

                    let msg = "Something went wrong.";

                    if (xhr.responseJSON) {

                        msg = xhr.responseJSON.message || msg;

                        if (xhr.responseJSON.errors) {
                            msg += "\n\n";

                            $.each(xhr.responseJSON.errors, function (field, messages) {
                                msg += field + ": " + messages.join(", ") + "\n";
                            });
                        }
                    }

                    Swal.fire({
                        title: "Validation Error!",
                        text: msg,
                        icon: "error"
                    });
                }
            });
        }
    });



});





$(document).on('click', '.btnRemainder', function () {
    let projid = $(this).data('projid');
   
    let projname = $(this).data('projname');
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
    debugger;
    $.ajax({
        url: '/Projects/SendRemainder',
        type: 'POST',
        data: {
            ProjId: projid,
            Remarks: encryptData(remarks) // if you want to send remarks to backend
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
    


    let ProjId = $(this).data("action");
   
   
    let projName = $(this).data('proj-name');
    let words = projName.split(" ");
    let shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;
    let finalTitle = "Reminder History: " + projName;
    $('#RemProjName').text(finalTitle);


    GetProjRemainderMov(ProjId,true); // <-- fixed this line
});



function GetProjRemainderMov(ProjId,enc) {
    
    $('#ProjRemainderMov').modal('show');

    $.ajax({
        url: '/Projects/GetProjectRemainderHistory',
        type: 'GET',
        data: { ProjectId: ProjId,Encrypted:enc},
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
   
    

    let $this = $(this);
    let projId = parseInt($this.data("projid"));
    $this.closest("tr").removeClass("bold-text");
    let psmId = $this.closest("tr").find(".SpnCurrentpsmId").text();
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
                    GetProjRemainderMov(ProjId,false);
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
    
    $.ajax({
        url: '/Projects/GetActParkedProject',
        type: 'GET',
        success: function (response) {
            $("#parkedtblData").html("");

            if (response != null && response.length > 0) {
                let listitem = "";
                let count = 0;
                let prjname = response[0].projName;
              
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
              
                let projName = $(this).closest("tr").find("#projNamecc").html();
                $('.lblHistory').text(`Mov History: ${projName}`);
                $('#ProjFwdHistory').modal('show');

                GetProjectMovHistory($(this).closest("tr").find("#SpnCurrentParkedProjId").html());
            });
            $(".btn_unparked").off('click').on('click', function () {
                let psmid = $(this).closest("tr").find('#spnCurrentPsmid').text();
            
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




function Unparkedbypsmid(id) {
    let psmid = encryptData(id);
 
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
$(document).on('click', ".parkedProj",function () {

    const psmid = $(this).closest("tr").find(".SpnCurrentpsmId").text();
    
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


    //FORECLOSED TAB

$(".btnPresentClosed").click(function () {
    let ClosedType = 1;

    GetCloseProjects(ClosedType);
});
$("#btnPastClosed").click(function () {
    let ClosedType = 2;
    GetCloseProjects(ClosedType);
});
function GetCloseProjects(Type) {
    let type = encryptData(Type)
    $.ajax({
        url: '/Projects/GetForecloseitems',
        type: 'GET',
        data: { Type: type },

        success: function (data) {

            let response = data.data || [];
            console.log(response);
            if ($.fn.DataTable.isDataTable("#foreclosetable")) {
                $("#foreclosetable").DataTable().destroy();
            }

            $("#Foreclosedata").html("");

            if (response.length > 0) {

                let listitem = "";
                let count = 0;

                response.forEach(function (project) {

                    count++;

                    listitem += `<tr>`;

                    listitem += `<td><div class="d-flex">${count}</div></td>`;

                    listitem += `
                        <td>
                            <span class="d-none noExport" id="SpnforcloseProjId">
                                ${project.projid}
                            </span>

                            <a data-proj-name="${project.projName}" 
                               data-proj-id="${project.projid}" 
                               href="/Projects/ProjHistory?EncyID=${project.encyID}&Type=XR12">

                                <div class="RefLetter-container" 
                                     data-tooltip="${project.projName ?? ""}">

                                    <span>${trimByChars(project.projName ?? "", 20)}</span>

                                    <span class="RefLetter noExport">
                                        ${breakLinesByWords(project.projName ?? "", 4)}
                                    </span>

                                    <span class="noExport d-none" id="projNamecc">
                                        ${project.projName ?? ""}
                                    </span>
                                </div>
                            </a>
                        </td>`;

                    listitem += `
                        <td class="RefLetter-container">
                            ${project.sponsor ?? ""}
                            <div class="RefLetter noExport">
                                ${breakLinesByWords(project.sponsor ?? "", 3)}
                            </div>
                        </td>`;

                    listitem += `
                        <td class="RefLetter-container">
                            ${project.approved_By ?? ""}
                            <div class="RefLetter noExport">
                                ${breakLinesByWords(project.approved_By ?? "", 4)}
                            </div>
                        </td>`;

                    listitem += `
                        <td>
                            ${project.timeStamp
                            ? DateFormateddMMyyyyhhmmss(project.timeStamp)
                            : ""}
                        </td>`;

                    listitem += `<td>${project.stages ?? ""}</td>`;
                    listitem += `<td>${project.status ?? ""}</td>`;
                    if (Type == 1) {
                        actionButton = `
        <button type="button"
                class="btn btn-sm btn-primary btnSendToInbox"
                data-psmid="${project.psmId}">
            Send to Inbox
        </button>`;
                    } else {
                        actionButton = `
    <button type="button"
            class="btn btn-sm btn-danger btnCloseAgain"
            data-psmid="${project.psmId}">
        Close Again
    </button>`;
                    }

                    listitem += `<td>${actionButton}</td>`;

                    listitem += `</tr>`;
                });

                $("#Foreclosedata").html(listitem);
            }

            initializeDataTable("#foreclosetable");
        },

        error: function (xhr, status, error) {

            console.error("Error fetching foreclosed projects:", status, error);

            if ($.fn.DataTable.isDataTable("#foreclosetable")) {
                $("#foreclosetable").DataTable().destroy();
            }

            $("#Foreclosedata").html("");

            $("#foreclosetable").DataTable({
                language: {
                    emptyTable: "Error loading data. Please try again later."
                },
                destroy: true
            });
        }
    });
}
$(document).on("click", ".btnSendToInbox", function () {
   
    let psmId = $(this).data("psmid");
    alert(psmId);
    Swal.fire({
        title: "Are you sure?",
        text: "Do you want to send this project back to Inbox?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Yes, send",
        cancelButtonText: "Cancel"
    }).then((result) => {

        if (result.isConfirmed) {
            SendForecloseToInbox(psmId);
        }

    });
});


$(document).on("click", ".btnCloseAgain", function () {

    let psmId = $(this).data("psmid");

    Swal.fire({
        title: "Are you sure?",
        text: "Do you want to close this project again?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Yes, close",
        cancelButtonText: "Cancel"
    }).then((result) => {

        if (result.isConfirmed) {
            CloseProjectAgain(psmId);
        }

    });
});

function CloseProjectAgain(psmId) {

    $.ajax({
        url: '/Projects/CloseProjectAgain',
        type: 'POST',
        data: { psmId: psmId },

        success: function (response) {

            if (response.success) {
                Swal.fire("Success", response.message, "success");
                GetCloseProjects(2); // reload past closed tab
            } else {
                Swal.fire("Error", response.message, "error");
            }
        },

        error: function () {
            Swal.fire("Error", "Something went wrong.", "error");
        }
    });
}
function SendForecloseToInbox(psmId) {
 
    $.ajax({
        url: '/Projects/SendForecloseToInbox',
        type: 'POST',
        data: { Psmid: psmId },

        success: function (response) {

            if (response.success) {
                Swal.fire({
                    icon: "success",
                    title: "Success",
                    text: response.message
                });

                GetCloseProjects(1);
            } else {
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: response.message
                });
            }
        },

        error: function () {
            Swal.fire({
                icon: "error",
                title: "Error",
                text: "Something went wrong."
            });
        }
    });
}

let currentUnitId = parseInt($('#hdnInboxUnitId').val());

//FORECLOSED TAB



// TODO: set these to the real integer values of your C# ActionTypeEnum
const ActionTypeEnum = {
    Approved: 1,
    RequestSent: 2,
    Rejected: 3
};
var currentProjectTab;

$(window).on("load", function() {
    currentProjectTab = "Inbox";
    GetInboxData();
})

/**
 * Tab click handlers – switch current tab and load corresponding data.
 */
$(document).on("click", "#tabinbox", function(e) {
    e.preventDefault();

    currentProjectTab = "Inbox";
    GetInboxData();
});

$(document).on("click", "#tabsent", function(e) {
    e.preventDefault();

    currentProjectTab = "Sent";
    GetSentData();
});

$(document).on("click", "#tabcompleted", function(e) {
    e.preventDefault();

    currentProjectTab = "Completed";
    GetCompletedData();
});
$(document).on("click", "#tabRemainder", function(e) {
    e.preventDefault();

    currentProjectTab = "Remainder";
    GetRemainderData();
});
function GetInboxData() {

    $.ajax({
        url: "/Projects/GetTabData", // adjust controller path if different
        type: "GET",
        data: {
            type: "tabinbox",
            projectSearchTypeId: 1
        },
        dataType: "json",
        beforeSend: function() {
            // optional: show a loader here
        },
        success: function(result) {
            // Store all inbox records
            allInboxData = result.response || [];

            // Apply current dropdown selection
            const selectedType =
                Number($("#ddlProjectProjdetailsType").val()) || 1;

            filterInboxByProjectType(selectedType);
        },
        error: function(xhr, status, err) {
            console.error("GetInboxData failed:", status, err);
        }
    });
}

/**
* Filters Inbox data by project type (1=All, 2=STC Auto, 3=STC AI/ML) and rebinds table.
*/
function filterInboxByProjectType(selectedType) {
    debugger;
    let filteredData = allInboxData;

    // 1 = All Projects
    if (selectedType === 2) {

        // STC Auto
        filteredData = allInboxData.filter(function(project) {
            return project.aiml === false;
        });

    } else if (selectedType === 3) {

        // STC AI/ML
        filteredData = allInboxData.filter(function(project) {
            return project.aiml === true;
        });
    }

    bindInboxTable(filteredData);
}

/**
 * Binds filtered inbox list to the #Inbox table (destroys/re-inits DataTable).
 */
function bindInboxTable(inboxList) {

    const $tbody = $("#Inbox tbody"); // adjust selector to your actual table id

    // if this table uses DataTables, destroy the existing instance first
    if ($.fn.DataTable && $.fn.DataTable.isDataTable("#Inbox")) {
        $("#Inbox").DataTable().destroy();
    }

    $tbody.empty();
    snoinbox = 0; // reset counter each time inbox is (re)loaded

    if (!inboxList || inboxList.length === 0) {
        $tbody.html('<tr><td colspan="11" class="text-center">No records found</td></tr>');
        return;
    }

    let rowsHtml = "";
    inboxList.forEach(function(project) {
        snoinbox++;
        rowsHtml += renderInboxRow(project, snoinbox);
    });

    $tbody.html(rowsHtml);
    //initializeDataTable("inbox")
    // re-init DataTables if this table is a DataTable
    if ($.fn.DataTable) {
        $("#Inbox").DataTable();
    }
}

/**
 * Builds a single Inbox table row HTML (mirrors the Razor markup).
 */
function renderInboxRow(project, sno) {
    const classname = project.isRead === false ? "bold-text" : "";

    return `
        <tr class="${classname}">
            <td data-order="${sno}">
                <span>${sno}</span>
                ${renderNotificationBlock(project)}
            </td>

            <td class="col-w-150">${project.projId}</td>

            <td class="f-12">
                <span class="d-none noExport SpnCurrentProjId">${project.projId}</span>
                <span class="d-none noExport SpnEncryptedProjId">${project.encyID}</span>
                <span class="d-none noExport SpnCurrentpsmId">${project.psmIds}</span>
                <span class="d-none noExport SpnStakeHolderId">${project.stakeHolderId}</span>
                <span class="d-none noExport" id="LatestActionType">${project.latestActionType ?? ""}</span>

                <span class="d-none noExport SpnTimeStampId">${project.timeStamp}</span>
                <span class="d-none noExport" id="SpnStageId">${project.stageId}</span>
                <span class="d-none noExport" id="SpnTimeStatusId">${project.statusId}</span>
                <span class="d-none noExport" id="SpnWhiteListed">${project.isWhitelisted}</span>

                <span class="d-none noExport SpnprojectIsProcess">${project.isProcess}</span>
                <span class="d-none noExport" id="SpnTimeActionId">${project.actionId}</span>
                <span class="d-none noExport" id="SpnTimeToUnitId">${project.toUnitId}</span>
                <span class="d-none noExport" id="SpnTimeFromUnitId">${project.fromUnitId}</span>
                <span class="d-none noExport" id="SpnDate_type">${project.date_type}</span>
                <span class="d-none noExport" id="SpnApprove">${project.adminApprovalStatus}</span>

                <a data-proj-name="${project.projName}" id="ProjectName" class="ProjName"
                   data-proj-id="${project.projId}"
                   href="/Projects/ProjHistory?EncyID=${project.encyID}&Type=XRDC&psmid=${project.psmIds}">
                    <div class="tooltip-container" data-tooltip="${project.projName}">
                        <span class="short-text noExport">${project.projName && project.projName.length > 17 ? project.projName.substring(0, 17) + "..." : (project.projName || "")}</span>
                        <span class="tooltip tooltip-text">${project.projName}</span>
                    </div>
                </a>
            </td>

            <td class="RefLetter-container">
                <div class="tooltip-container" data-tooltip="${project.sponsor}">
                    <span class="short-text noExport">${trimByWords(project.unitName, 2)}</span>
                    <span class="tooltip tooltip-text">${project.sponsor}</span>
                </div>
            </td>

            <td>
                <div class="d-flex">
                    <div class="col-md-8">
                        <div class="RefLetter-container">
                            <div class="tooltip-container" data-tooltip="${project.fromUnitName}">
                                <span class="short-text noExport">${trimByWords(project.fromUnitUserDetail, 2)}</span>
                                <span class="tooltip tooltip-text">${project.fromUnitName}</span>
                            </div>
                        </div>
                    </div>
                    ${renderCcBlock(project)}
                </div>
            </td>

            <td class="TimeStampForcheckdate col-w-190">${DateTimeFormatedd_mm_yyyy(project.timeStamp)}</td>
            <td class="col-w-150">${project.stage}</td>
            <td class="col-w-190">${project.status}</td>
            <td class="col-w-150">${project.action}</td>

            <td class="noExport">
                <div class="row">
                    <div class="col-md-12 d-flex">
                        <div class="mr-2">
                            ${renderFwdOrProcessButtons(project)}
                        </div>

                        ${renderObsnButton(project)}

                        <div class="mr-2">
                            ${project.isCc === true
            ? '<button type="button" class="btn btn-success invisible btn-mini">Fwd</button>'
            : ""}
                        </div>

                        <div class="col-md-4">
                            <button type="button" class="btn btn-success btn-FwdHistory btn-mt-1" data-proj-name="${project.projName}" title="History"><i class="fa-solid fa-timeline"></i></button>
                        </div>
                    </div>
                </div>
            </td>

            <td>
                <div class="row d-flex align-items-center">
                    <div class="col-auto p-0 me-1">
                        ${renderDateActionCell(project)}
                    </div>
                    <div class="col-auto p-0 parkedProj">
                        <button class="btn btn-success" title="Click here to Park the project in parked tab">Park</button>
                    </div>
                </div>
            </td>
        </tr>`;
}


/**
 * Notification icon + badge + hover popup for reminder remarks.
 */
function renderNotificationBlock(project) {
    if (!(project.hasRemainder1 === true && project.isCc === false)) {
        return "";
    }

    let badgeHtml = "";
    if (project.remainderCount > 0) {
        badgeHtml = `<span class="badge3 rem-badge" id="Remainderbedge">${project.remainderCount}</span>`;
    }

    let popupHtml = "";
    if (project.latestRemarks && project.latestRemarks.trim() !== "") {
        const remarks = project.latestRemarks
            .split(/(?=\([^)]+\):)/)
            .filter(function(r) { return r && r.trim() !== ""; });

        let itemsHtml = "";
        remarks.forEach(function(remark) {
            itemsHtml += `<li><i class="fas fa-comment-dots text-primary"></i> ${remark}</li>`;
        });

        popupHtml = `
            <div class="hover-popup custom-popup-glass">
                <h6 class="popup-title"> Remarks</h6>
                <ul class="popup-list">
                    ${itemsHtml}
                </ul>
            </div>`;
    }

    return `
        <div class="hover-container notif-hover-container" id="notificationContainer ${project.projId}">
            <div class="notif-inline-block" id="ReadRemainderNoti" data-projid="${project.projId}">
                <img src="/assets/images/icons/notification-unscreen.gif" alt="Notification" class="ico-w30h30" />
                ${badgeHtml}
            </div>
            ${popupHtml}
        </div>`;
}

/**
 * CC pill block.
 */
function renderCcBlock(project) {
    if (!(project.isCc === true || project.issentCC === true)) {
        return "";
    }

    const ccSpan = project.isCc === true
        ? '<span class="short-text IsccForRemoveRow">Cc</span>'
        : '<span class="short-text">Cc</span>';

    return `
        <div class="col-md-2">
            <div class="RefLetter-container btn btn-warning p-2 cc-pill">
                <div class="tooltip-container" data-tooltip="${project.ccUnitName}">
                    ${ccSpan}
                    <span class="tooltip tooltip-text">${project.ccUnitName}</span>
                </div>
            </div>
        </div>`;
}

/**
 * Process icon or Send button.
 */
function renderFwdOrProcessButtons(project) {
    const isProcessCase =
        (project.stageId === 1 && project.isProcess === false && currentUnitId === 1 && project.aiml === false) ||
        (project.aiml === true && project.stageId === 1 && project.isProcess === false && currentUnitId === 2);

    if (isProcessCase) {
        return `<a href="javascript:void(0);" class="processDetail mr-2" data-id="${project.encyPsmID}">
                    <img src="/assets/images/icons/process1.png" alt="Icon" class="ico-h-22">
                </a>`;
    }

    if (project.isCc === false) {
        return `<button type="button" class="btn btn-success btn-Fwd btn-mini" data-Date_type="${project.adminApprovalStatus}" data-proj-name="${project.projName}">Send</button>`;
    }

    return "";
}

/**
 * Obsn button.
 */
function renderObsnButton(project) {
    if (project.stageId === 1 && project.isProcess === false && project.isCc === false &&
        (currentUnitId === 1 || currentUnitId === 2)) {
        return `<div class="col-md-4">
                    <button type="button" class="btn btn-success btn-Obsn btn-mini" data-proj-name="${project.projName}">Obsn</button>
                </div>`;
    }
    return "";
}

/**
 * Date-action icons (Approved / Pending / Request back-date).
 */
function renderDateActionCell(project) {
    if (project.isCc !== false) {
        return "<span></span>";
    }

    const backDateLink = `
        <a href="#" class="date-action mr-2" data-actiontype="1" data-action="back" data-ids="${project.encyPsmID}" title="Send request to Admin to ingest legacy Project">
            <img src="/assets/images/icons/Back_date.png" alt="Icon" class="ico-h-27">
        </a>`;

    const hasAction = project.latestActionType !== null && project.latestActionType !== undefined;

    if (project.isProcess === true && hasAction) {
        if (project.latestActionType === ActionTypeEnum.Approved) {
            return `<span title="Approved Legacy Project">
                        <a href="#" class="date-action mr-2 disabled-link" data-action="current" data-ids="${project.encyPsmID}">
                            <img src="/assets/images/icons/Current_date.png" alt="Icon" class="ico-h-27">
                        </a>
                    </span>`;
        }
        if (project.latestActionType === ActionTypeEnum.RequestSent) {
            return `<span title="Pending From Admin">
                        <a href="#" class="date-action mr-2 disabled-link" data-action="Pending">
                            <img src="/assets/images/icons/Pendingbtn.png" alt="Icon" class="ico-h-30">
                        </a>
                    </span>`;
        }
        if (project.latestActionType === ActionTypeEnum.Rejected) {
            return backDateLink;
        }
        return "";
    }

    if (project.isProcess === true && !hasAction) {
        return backDateLink;
    }

    return "";
}


/* =========================================================================
Sent tab - AJAX binding
Replicates the Razor foreach block for Model.SendItems exactly (same
markup, classes, ids, hidden fields, tooltips, icons, buttons).

Same assumptions as inbox-ajax-bind.js:
1) camelCase JSON property names (System.Text.Json default).
2) trimByWords(text, wordLimit) already exists and is loaded before this.
3) currentUnitId not needed here (this table doesn't use ViewBag.unitid).
========================================================================= */

// running serial number for the "sno" column



// =========================================================================
// SENT TAB – DATA FETCH, FILTER & RENDER FUNCTIONS
// All functions related to loading, filtering and rendering the Sent table.
// =========================================================================

/**
 * Fetches Sent data from server and applies current project-type filter.
 */
function GetSentData() {

    $.ajax({
        url: "/Projects/GetTabData",
        type: "GET",
        data: {
            type: "tabsent",
            projectSearchTypeId: 1
        },
        dataType: "json",

        success: function(result) {

            allSentData = result.response || [];

            const selectedType =
                Number($("#ddlProjectProjdetailsType").val()) || 1;

            filterSentByProjectType(selectedType);
        },

        error: function(xhr, status, err) {
            console.error(
                "GetSentData failed:",
                status,
                err
            );
        }
    });
}

/**
 * Binds filtered sent list to the #Sents table (destroys/re-inits DataTable).
 */
function bindSentTable(sentList) {

    const $tbody = $("#Sents tbody"); // adjust selector to your actual table id

    if ($.fn.DataTable && $.fn.DataTable.isDataTable("#Sents")) {
        $("#Sents").DataTable().destroy();
    }

    $tbody.empty();
    snosent = 0; // reset counter each time sent items are (re)loaded

    // NOTE: original Razor order was Model.SendItems.OrderByDescending(i => i.TimeStamp).ToList()
    // if GetActSendItemsAsync() on the server does not already return it sorted this way,
    // sort here to match, e.g.:
    // sentList = sentList.slice().sort((a, b) => new Date(b.timeStamp) - new Date(a.timeStamp));

    if (!sentList || sentList.length === 0) {
        $tbody.html('<tr><td colspan="9" class="text-center">No records found</td></tr>');
        return;
    }

    let rowsHtml = "";
    sentList.forEach(function(project) {
        snosent++;
        rowsHtml += renderSentRow(project, snosent);
    });

    $tbody.html(rowsHtml);

    if ($.fn.DataTable) {
        $("#Sents").DataTable();
    }
}


let allSentData = [];
let snosent = 0;

/**
 * Builds a single Sent table row HTML (mirrors the Razor markup).
 */
function renderSentRow(project, sno) {
    const projName = project.projName || "";
    const shortName = projName.length > 17 ? projName.substring(0, 17) + "..." : projName;

    return `
        <tr>
            <td class="s-no-column">
                <div class="d-flex">
                    ${sno}
                    ${renderReadIcon(project)}
                    ${renderReminderIcon(project)}
                </div>
            </td>

            <td>${project.projId}</td>

            <td class="f-12">
                <span class="d-none noExport SpnCurrentProjId">${project.projId}</span>
                <span class="d-none noExport SpnEncProjId">${project.encyID}</span>
                <span class="d-none noExport SpnCurrentpsmId">${project.psmIds}</span>
                <span class="d-none noExport SpnStakeHolderId">${project.stakeHolderId}</span>

                <span class="d-none noExport SpnTimeStampId">${project.timeStamp}</span>
                <span class="d-none noExport SpnprojectIsProcess">${project.isProcess}</span>
                <span class="d-none noExport" id="SpnprojectStageId">${project.stageId}</span>
                <span class="d-none noExport" id="SpnprojectToUnitId">${project.toUnitId}</span>

                <a data-proj-name="${projName}" data-proj-id="${project.projId}"
                   href="/Projects/ProjHistory?EncyID=${project.encyID}&Type=XR12">
                    <div class="tooltip-container" data-tooltip="${projName}">
                        <span class="short-text noExport">${shortName}</span>
                        <span class="tooltip tooltip-text">${projName}</span>
                    </div>
                </a>
            </td>

            <td class="f-12">
                <div class="tooltip-container" data-tooltip="${project.sponsor}">
                    <span class="short-text noExport">${trimByWords(project.unitName, 2)}</span>
                    <span class="tooltip tooltip-text">${project.sponsor}</span>
                </div>
            </td>

            <td class="RefLetter-container">
                <div class="d-flex">
                    <div class="col-md-10">
                        <div class="tooltip-container" data-tooltip="${project.toUnitName}">
                            <span class="short-text noExport">${trimByWords(project.toUnitName, 2)}</span>
                            <span class="tooltip tooltip-text">${project.toUnitName}</span>
                        </div>
                    </div>
                    ${renderCcBlockSent(project)}
                </div>
            </td>

            <td>${DateTimeFormatedd_mm_yyyy(project.timeStamp)}</td>
            <td>${project.stage}</td>
            <td>${project.status}</td>
            <td>${project.action}</td>

            <td>
                <div class="row d-flex">
                    ${renderPullBackBlock(project)}
                    <div class="col-md-2">
                        <button type="button" class="btn btn-success btn-FwdHistorySent btn-mt-1" data-proj-name="${projName}" title="History"><i class="fa-solid fa-timeline"></i></button>
                    </div>
                </div>
            </td>
        </tr>`;
}

/**
 * Read / unread tick icon.
 */
function renderReadIcon(project) {
    if (project.isRead === false) {
        return `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                     fill="currentColor" class="bi bi-check text-teal-custom" viewBox="0 0 16 16">
                    <path d="M10.97 4.97a.75.75 0 0 1 1.07 1.05l-3.99 4.99a.75.75 0 0 1-1.08.02L4.324 8.384a.75.75 0 1 1 1.06-1.06l2.094 2.093 3.473-4.425z" />
                </svg>`;
    }

    return `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" width="16" class="text-teal-custom"
                 height="16" fill="currentColor">
                <path d="M342.6 86.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L160 178.7l-57.4-57.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l80 80c12.5 12.5 32.8 12.5 45.3 0l160-160zm96 128c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L160 402.7 54.6 297.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l128 128c12.5 12.5 32.8 12.5 45.3 0l256-256z" />
            </svg>`;
}

/**
 * Reminder icon (sent / add reminder).
 */
function renderReminderIcon(project) {
    if (project.hasRemainder === true && project.hideReminderIcon === true) {
        return `<img src="/assets/images/icons/greenRemainder.png" class="btnRemainder" alt="Reminder Sent" title="UnRead Reminder" data-projid="${project.encyID}" data-projname="${project.projName}" />`;
    }

    if (project.hideReminderIcon === true) {
        return `<img src="/assets/images/icons/add-reminder1.png" class="btnRemainder" alt="Reminder" title="Send Reminder" data-projid="${project.encyID}" data-projname="${project.projName}" />`;
    }

    return "";
}

/**
 * CC pill.
 */
function renderCcBlockSent(project) {
    if (project.isCc !== true) {
        return "";
    }

    return `
        <div class="btn btn-warning p-2 cc-pill">
            <div class="tooltip-container" data-tooltip="${project.ccUnitName}">
                <span class="short-text IsccForRemoveRow">Cc</span>
                <span class="tooltip tooltip-text">${project.ccUnitName}</span>
            </div>
        </div>`;
}

/**
 * Pull Back / Pulled Back block.
 */
function renderPullBackBlock(project) {
    if (project.isHosted !== 0) {
        return `<div class="col-md-6">
                    <button class="btn btn-success invisible" disabled>Pull Back </button>
                </div>`;
    }

    let inner = "";

    if (project.isPullBack === false || project.isPullBack === null || project.isPullBack === undefined) {
        if (project.pullbackAction === true) {
            inner = `<button type="button" class="btn btn-success btn-Undo ml-2 btn-mini">Pull Back </button>`;
        }
    } else if (project.stageId !== 1 && project.isPullBack === true) {
        inner = `<span class="btn btn-success ml-2 btn-mini btn_pulledpack">Pulled Back</span>`;
    }

    return `<div class="col-md-6">
                ${inner}
            </div>`;
}
/**
 * Filters Sent data by project type and rebinds table.
 */
function filterSentByProjectType(selectedType) {
    debugger;
    let filteredData = allSentData;

    // 1 = All Projects
    if (selectedType === 2) {

        // STC Auto
        filteredData = allSentData.filter(function(project) {
            return project.aiml === false;
        });

    } else if (selectedType === 3) {

        // STC AI/ML
        filteredData = allSentData.filter(function(project) {
            return project.aiml === true;
        });
    }

    bindSentTable(filteredData);
}


/* =========================================================================
Completed tab - AJAX binding
Replicates the Razor foreach block for Model.CompletedItems exactly
(same markup, classes, ids, tooltips).

Same assumptions as inbox-ajax-bind.js / sent-ajax-bind.js:
1) camelCase JSON property names (System.Text.Json default).
2) trimByWords(text, wordLimit) already exists and is loaded before this.
========================================================================= */

// running serial number for the "sno" column
let snocompleted = 0;


// =========================================================================
// COMPLETED TAB – DATA FETCH, FILTER & RENDER FUNCTIONS
// All functions related to loading, filtering and rendering the Completed table.
// =========================================================================

/**
 * Fetches Completed data from server and applies current project-type filter.
 */
function GetCompletedData() {

    $.ajax({
        url: "/Projects/GetTabData",
        type: "GET",
        data: {
            type: "tabcompleted",
            projectSearchTypeId: 1
        },
        dataType: "json",

        success: function(result) {

            allCompletedData = result.response || [];

            const selectedType =
                Number($("#ddlProjectProjdetailsType").val()) || 1;

            filterCompletedByProjectType(selectedType);
        }
    });
}

/**
 * Binds filtered completed list to the #Completed table.
 */
function bindCompletedTable(completedList) {
    const $tbody = $("#Completed tbody"); // adjust selector to your actual table id

    if ($tbody.length === 0) {
        console.error("bindCompletedTable: could not find '#completedTable tbody' in the DOM - fix the selector.");
        return;
    }

    if ($.fn.DataTable && $.fn.DataTable.isDataTable("#Completed")) {
        $("#Completed").DataTable().destroy();
    }

    $tbody.empty();
    snocompleted = 0; // reset counter each time completed items are (re)loaded

    if (!completedList || completedList.length === 0) {
        $tbody.html('<tr><td colspan="7" class="text-center">No records found</td></tr>');
        return;
    }

    let rowsHtml = "";
    completedList.forEach(function(project) {
        snocompleted++;
        rowsHtml += renderCompletedRow(project, snocompleted);
    });

    $tbody.html(rowsHtml);

    if ($.fn.DataTable) {
        $("#Completed").DataTable();
    }
}

/**
 * Builds a single Completed table row HTML.
 */
// declare at the top of the file (allCompletedData was an implicit global before)
let allCompletedData = [];


/**
 * Builds a single Completed table row HTML (mirrors the Razor markup).
 */
function renderCompletedRow(project, sno) {
    const projName = project.projName || "";
    const shortName = projName.length > 17 ? projName.substring(0, 17) + "..." : projName;

    return `
        <tr>
            <td>${sno}</td>

            <td>
                <span class="d-none SpnCurrentProjId">${project.projId}</span>
                ${project.projId}
            </td>

            <td>
                <a data-proj-name="${projName}" data-proj-id="${project.projId}"
                   href="/Projects/ProjHistory?EncyID=${project.encyID}&Type=XR">
                    <div class="tooltip-container" data-tooltip="${projName}">
                        <span class="short-text noExport">${shortName}</span>
                        <span class="tooltip tooltip-text">${projName}</span>
                    </div>
                </a>
            </td>

            <td>${project.stakeHolder}</td>
            <td>${project.recdFmUser}</td>
            <td>${DateTimeFormatedd_mm_yyyy(project.dateTimeOfUpdate)}</td>
            <td>${project.status}</td>
            <td>${project.action}</td>
        </tr>`;
}


/**
 * Filters Completed data by project type and rebinds table.
 */
function filterCompletedByProjectType(selectedType) {
    let filteredData = allCompletedData;

    // 1 = All Projects
    if (selectedType === 2) {

        // STC Auto
        filteredData = allCompletedData.filter(function(project) {
            return project.aiml === false;
        });

    } else if (selectedType === 3) {

        // STC AI/ML
        filteredData = allCompletedData.filter(function(project) {
            return project.aiml === true;
        });
    }

    bindCompletedTable(filteredData);
}


/**
 * Filters Remainder data by project type and rebinds table.
 */
function GetRemainderData() {

    $.ajax({
        url: "/Projects/GetTabData", // adjust controller path if different
        type: "GET",
        data: {
            type: "tabRemainder",
            projectSearchTypeId: 1
        },
        dataType: "json",
        beforeSend: function() {
            // optional: show a loader here
        },
        success: function(result) {
            allRemainderData = result.response || [];
            const selectedType =
                Number($("#ddlProjectProjdetailsType").val()) || 1;
            filterRemainderByProjectType(selectedType)
        },
        error: function(xhr, status, err) {
            console.error("GetRemainderData failed:", status, err);
        }
    });
}
function filterRemainderByProjectType(selectedType) {

    let filteredData = allRemainderData;

    // 1 = All Projects
    if (selectedType === 2) {

        // STC Auto
        filteredData = allRemainderData.filter(function(project) {
            return project.aiml === false;
        });

    } else if (selectedType === 3) {

        // STC AI/ML
        filteredData = allRemainderData.filter(function(project) {
            return project.aiml === true;
        });
    }

    bindRemainderTable(filteredData);
}
/**
 * Binds filtered remainder list to the #Remainders table.
 */
function bindRemainderTable(remainderList) {
    const $tbody = $("#Remainders tbody"); // adjust selector to your actual table id

    if ($tbody.length === 0) {
        console.error("bindRemainderTable: could not find '#remainderTable tbody' in the DOM - fix the selector.");
        return;
    }

    if ($.fn.DataTable && $.fn.DataTable.isDataTable("#Remainders")) {
        $("#Remainders").DataTable().destroy();
    }

    $tbody.empty();
    serNo = 0; // reset counter each time reminder data is (re)loaded

    if (!remainderList || remainderList.length === 0) {
        $tbody.html('<tr><td colspan="9" class="text-center">No records found</td></tr>');
        return;
    }

    let rowsHtml = "";
    remainderList.forEach(function(project) {
        serNo++;
        rowsHtml += renderRemainderRow(project, serNo);
    });

    $tbody.html(rowsHtml);

    if ($.fn.DataTable) {
        $("#Remainders").DataTable();
    }
}

/**
 * Builds a single Remainder table row HTML (including short remarks and history button).
 */

/**
 * Builds a single Remainder table row HTML (mirrors the Razor markup).
 */
function renderRemainderRow(project, sno) {
    const projName = project.projName || "";
    const shortName = projName.length > 17 ? projName.substring(0, 17) + "..." : projName;
    const fullRemarks = project.remarks ?? "";
    const shortRemarks = buildShortRemarks(project.remarks);
    const sentDateFormatted = formatSentOn(project.sentOn);

    return `
        <tr>
            <td class="sorting">${sno}</td>

            <td class="RefLetter-container">
                <a data-proj-name="${projName}" class="ProjName" data-proj-id="${project.projid}"
                   href="/Projects/ProjHistory?EncyID=${project.encyID}&Type=XRDC">
                    <div class="tooltip-container" data-tooltip="${projName}">
                        <span class="short-text">${shortName}</span>
                        <span class="tooltip tooltip-text noExport">${projName}</span>
                    </div>
                </a>
            </td>

            <td class="RefLetter-container">
                <div class="tooltip-container" data-tooltip="${project.sponsor ?? ""}">
                    <span class="short-text">${trimByWords(project.unitName, 3)}</span>
                    <span class="tooltip tooltip-text noExport">${project.sponsor ?? ""}</span>
                </div>
            </td>

            <td>${project.fromUnit ?? ""}</td>
            <td>${sentDateFormatted}</td>
            <td>${project.toUnit ?? ""}</td>
            <td>${DateTimeFormatedd_mm_yyyy(project.readOn)}</td>

            <td class="RefLetter-container">
                <div class="tooltip-container" data-tooltip="${fullRemarks}">
                    <span class="short-text noExport">${shortRemarks}</span>
                    <span class="tooltip tooltip-text">${fullRemarks}</span>
                </div>
            </td>

            <td>
                <a href="#" id="btnRemMove" data-proj-name="${projName}" data-action="${project.encyID}">
                    <img src="/assets/images/icons/Legacyhistory.png" alt="Icon" class="ico-h-27">
                </a>
            </td>
        </tr>`;
}

/**
 * Formats SentOn date to "dd-MM-yyyy:hh:mm:ss" (12-hour, matching original .NET format).
 */
function formatSentOn(sentOn) {
    if (!sentOn) {
        return "";
    }

    const d = new Date(sentOn);
    if (isNaN(d.getTime())) {
        return "";
    }

    const pad2 = n => n.toString().padStart(2, "0");

    const day = pad2(d.getDate());
    const month = pad2(d.getMonth() + 1);
    const year = d.getFullYear();

    let hours12 = d.getHours() % 12;
    if (hours12 === 0) hours12 = 12;

    const hh = pad2(hours12);
    const mm = pad2(d.getMinutes());
    const ss = pad2(d.getSeconds());

    return `${day}-${month}-${year}:${hh}:${mm}:${ss}`;
}

/**
 * Builds short remarks (max 4 words + "...") for display in Remainder table.
 */
function buildShortRemarks(rawRemarks) {
    const remarks = (!rawRemarks || rawRemarks.trim() === "") ? "No Remarks" : rawRemarks;
    const remarkWords = remarks.split(" ");

    if (remarkWords.length > 3) {
        return remarkWords.slice(0, 4).join(" ") + "...";
    }
    return remarks;
}