var tittle = "";
var Status = "";



$(document).ready(function () {
   
    Getbarchart()
    GetAllDashbaordCount();
    InboxNotificationCount();
    CreateChartSummary();
    $("#IsNotduplicate").change(function () {


        var ischecked = $(this).is(':checked');
        if (ischecked)
            getProjGetsummay($("#spndashboardstatusId").html(), false);
        else
            getProjGetsummay($("#spndashboardstatusId").html(), true);
    });
    $('.close-btn').on("click",function () {
        closePopup()
    });


    $('[data-toggle="tooltip"]').tooltip();
})



function GetAllDashbaordCount() {
    $.ajax({
        type: "POST",
        url: '/Home/GetDashboardCount',
        data: { "Id": 0 },
        success: function (data) {

            var dtoDashboardHeaderlst = data.dtoDashboardHeaderlst;
            var dTOApprovedCountlst = data.dtoApprovedCountlst;
            var dTODashboardCountlstForAction = data.dtoDashboardCountlstForAction;

            var listitem = '';
            var stageId = 0;
            var tot = 0;
            var peding = 0;
            var sent = 0;

            if (data != null) {
                dtoDashboardHeaderlst.sort((a, b) => a.stageId - b.stageId || a.statseq - b.statseq);

                for (var i = 0; i < dtoDashboardHeaderlst.length; i++) {

                    if (stageId != dtoDashboardHeaderlst[i].stageId) {
                        if (stageId != 0) {
                            listitem += '</div>';
                        }

                        var stage = "";
                        if (dtoDashboardHeaderlst[i].stageId === 1) stage = "(Sponsor & DDGIT)";
                        if (dtoDashboardHeaderlst[i].stageId === 2) stage = "(Parallel Processing)";
                        if (dtoDashboardHeaderlst[i].stageId === 3) stage = "(Serial Processing)";

                        listitem += '<div class="newprojectheading text-center"> ' + dtoDashboardHeaderlst[i].stages + " " + stage + ' </div>';

                        // ✅ removed inline style
                        listitem += '<div class="r-1 row g-3 mt-2 db-stage-row">';
                    }

                    // ✅ removed inline style
                    listitem += '<div class="cd-1 col-12 col-sm-6 col-md-4 col-lg-1 db-card-box">';

                    tot = 0;
                    peding = 0;
                    sent = 0;

                    var DTODashboardCount = data.dtoDashboardCountlst.filter(function (element) {
                        return element.stagesId == dtoDashboardHeaderlst[i].stageId && element.statusId == dtoDashboardHeaderlst[i].statusId;
                    });

                    if (parseInt(dtoDashboardHeaderlst[i].statusId) == 2 || parseInt(dtoDashboardHeaderlst[i].statusId) == 3
                        || parseInt(dtoDashboardHeaderlst[i].statusId) == 22 || parseInt(dtoDashboardHeaderlst[i].statusId) == 31
                        || parseInt(dtoDashboardHeaderlst[i].statusId) == 37) {

                        if (parseInt(dtoDashboardHeaderlst[i].statusId) == 2)
                            var ForAction = dTODashboardCountlstForAction.filter(function (e) { return e.actionId == 10; });
                        else if (parseInt(dtoDashboardHeaderlst[i].statusId) == 3)
                            var ForAction = dTODashboardCountlstForAction.filter(function (e) { return e.actionId == 11; });
                        else if (parseInt(dtoDashboardHeaderlst[i].statusId) == 22)
                            var ForAction = dTODashboardCountlstForAction.filter(function (e) { return e.actionId == 3 && e.stagesId == 2; });
                        else if (parseInt(dtoDashboardHeaderlst[i].statusId) == 31)
                            var ForAction = dTODashboardCountlstForAction.filter(function (e) { return e.actionId == 3 && e.stagesId == 3; });
                        else if (parseInt(dtoDashboardHeaderlst[i].statusId) == 37)
                            var ForAction = dTODashboardCountlstForAction.filter(function (e) { return e.actionId == 3 && e.stagesId == 1; });

                        for (var j = 0; j < ForAction.length; j++) {
                            tot += ForAction[j].tot;

                            if (ForAction[j].isComplete == false) peding += ForAction[j].tot;
                            if (ForAction[j].isComplete == true) sent += ForAction[j].tot;
                        }

                    } else {
                        for (var j = 0; j < DTODashboardCount.length; j++) {
                            tot += DTODashboardCount[j].tot;

                            if (DTODashboardCount[j].isComplete == false) peding += DTODashboardCount[j].tot;
                            if (DTODashboardCount[j].isComplete == true) sent += DTODashboardCount[j].tot;
                        }
                    }

                    listitem += '<div class="icon-container ApprovedProj cursorpointer"><span class="d-none" id="spnstatusId">' + dtoDashboardHeaderlst[i].statusId + '</span>';

                    // ✅ removed inline style from img + h5
                    if (dtoDashboardHeaderlst[i].statusId == 2 || dtoDashboardHeaderlst[i].statusId == 3 || dtoDashboardHeaderlst[i].statusId == 22 || dtoDashboardHeaderlst[i].statusId == 31 || dtoDashboardHeaderlst[i].statusId == 37) {
                        if (dtoDashboardHeaderlst[i].statusId == 3)
                            listitem += '<img src="/assets/images/icons/prog.png" alt="Icon" class="db-icon-25" data-toggle="tooltip" data-placement="top" title="Total No of proj approved at this stage">';
                        else
                            listitem += '<img src="/assets/images/icons/rejec.png" alt="Icon" class="db-icon-25">';

                        listitem += '<h5 class="db-h5-mt25"> </h5>';

                    } else {
                        listitem += '<img src="/assets/images/icons/prog.png" alt="Icon" class="db-icon-25" data-toggle="tooltip" data-placement="top" title="Total No of proj approved at this stage">';

                        var approvedcount = dTOApprovedCountlst.filter(function (element) { return element.statusId == dtoDashboardHeaderlst[i].statusId; });

                        if (approvedcount.length > 0)
                            if (dtoDashboardHeaderlst[i].status.includes("BISAG-N")) {
                                listitem += '<span class="d-none" id="spnstatusActionsMappingId">' + approvedcount[0].statusActionsMappingId + '</span>' +
                                    '<h5 class="db-h5-mt8-pt10" data-toggle="tooltip" data-placement="top" title="Total No of proj approved at this stage">' + approvedcount[0].total + ' </h5>';
                            }
                            else if (dtoDashboardHeaderlst[i].status.includes("Re-Vetting")) {
                                listitem += '<span class="d-none" id="spnstatusActionsMappingId">' + approvedcount[0].statusActionsMappingId + '</span>' +
                                    '<h5 class="db-h5-mt8-pt10" data-toggle="tooltip" data-placement="top" title="Total No of proj approved at this stage">' + approvedcount[0].total + ' </h5>';
                            }
                            else {
                                listitem += '<span class="d-none" id="spnstatusActionsMappingId">' + approvedcount[0].statusActionsMappingId + '</span>' +
                                    '<h5 class="db-h5-mt8" data-toggle="tooltip" data-placement="top" title="Total No of proj approved at this stage">' + approvedcount[0].total + ' </h5>';
                            }
                        else
                            listitem += '<span class="d-none" id="spnstatusActionsMappingId">0</span><h5 class="db-h5-mt8">0 </h5>';
                    }

                    listitem += '<div class="t-1 statusprojsummry d-none">' + dtoDashboardHeaderlst[i].status + '</div> ';
                    listitem += '</div>';
                    listitem += '<div class="cursorpointer btnGetsummay "><span class="d-none" id="spnstatusId">' + dtoDashboardHeaderlst[i].statusId + '</span>';
                    listitem += '<div class="">';

                    // ✅ removed inline padding-top
                    if (dtoDashboardHeaderlst[i].status.includes("BISAG-N") || dtoDashboardHeaderlst[i].status.includes("Re-Vetting")) {
                        listitem += '<div class="t-1 statusprojsummry db-status-pt7">' + dtoDashboardHeaderlst[i].status + '</div> ';
                    }
                    else {
                        listitem += '<div class="t-1 statusprojsummry">' + dtoDashboardHeaderlst[i].status + '</div> ';
                    }

                    // ✅ removed inline font-size
                    if (dtoDashboardHeaderlst[i].status.includes("BISAG-N") || dtoDashboardHeaderlst[i].status.includes("Re-Vetting")) {

                        listitem += '<span class="badge badge-light text-black d-none db-badge-18" data-toggle="tooltip" data-placement="top"><span class="badge bg-danger">' + peding + '</span> / <span class="badge bg-success">' + sent + '</span></span>';

                        listitem += ' </div>';
                        listitem += ' <div class="mb-2">';
                        listitem += '<span class="badge badge-primary mr-2 d-none db-badge-14" data-toggle="tooltip" data-placement="top" title="Total No of transaction at this stage">' + tot + '</span>';

                    }
                    else {
                        listitem += '<span class="badge badge-light text-black db-badge-18" data-toggle="tooltip" data-placement="top"><span class="badge bg-danger">' + peding + '</span> / <span class="badge bg-success">' + sent + '</span></span>';

                        listitem += ' </div>';
                        listitem += ' <div class="mb-2">';
                        listitem += '<span class="badge badge-primary mr-2 db-badge-14" data-toggle="tooltip" data-placement="top" title="Total No of transaction at this stage">' + tot + '</span>';
                    }

                    listitem += ' </div>';
                    listitem += ' </div>';
                    listitem += ' </div>';

                    stageId = dtoDashboardHeaderlst[i].stageId;
                }

                $("#carddashboardcount").html(listitem);

                $(document).on("click", ".ApprovedProj", function () {
                    var spnstatusId = $(this).closest("div").find("#spnstatusId").html();
                    var spnstatusActionsMappingId = $(this).closest("div").find("#spnstatusActionsMappingId").html();

                    tittle = "Approved: " + $(this).closest("div").find(".statusprojsummry").html();
                    tittleIPA = "Approved by:  " + $(this).closest("div").find(".statusprojsummry").html() + " (Parallel Processing)";

                    if (parseInt(spnstatusActionsMappingId) == 1) Status = 'Accepted';
                    else if (parseInt(spnstatusActionsMappingId) == 9) Status = 'obsn Raised';
                    else if (parseInt(spnstatusActionsMappingId) == 113) Status = 'Rectified';
                    else if (parseInt(spnstatusActionsMappingId) == 48 || parseInt(spnstatusActionsMappingId) == 53) Status = 'Approved';
                    else if (parseInt(spnstatusActionsMappingId) == 60) { tittle = "Closed Project"; Status = 'Closed'; }
                    else if (parseInt(spnstatusActionsMappingId) == 68 || parseInt(spnstatusActionsMappingId) == 73 || parseInt(spnstatusActionsMappingId) == 63
                        || parseInt(spnstatusActionsMappingId) == 78 || parseInt(spnstatusActionsMappingId) == 83 || parseInt(spnstatusActionsMappingId) == 88) Status = 'Completed';
                    else if (parseInt(spnstatusActionsMappingId) == 26 || parseInt(spnstatusActionsMappingId) == 31 || parseInt(spnstatusActionsMappingId) == 37) Status = 'Approved';

                    if (parseInt(spnstatusActionsMappingId) == 0) {
                        Swal.fire({ icon: "error", title: "Oops...", text: "Data Not Found!" });
                    } else {
                        $('#CertName').html("Cert&Att");
                        if (spnstatusId == 2 || spnstatusId == 3 || spnstatusId == 22) {

                        } else if (parseInt(spnstatusId) == 44 || parseInt(spnstatusId) == 46) {
                            $('#ProjectApprovedTittleBisag').html(tittle);
                            $('#BISAG-N').modal('show');
                            getProjBisagN(spnstatusId, spnstatusActionsMappingId);
                        } else if (spnstatusId == 21) {
                            $('#IPAProjectApprovedTittle').html(tittle);
                            $('#IPAProjApproved').modal('show');

                            getProjApproved(spnstatusId, spnstatusActionsMappingId);
                        } else if (spnstatusActionsMappingId == 26 || spnstatusActionsMappingId == 31 || spnstatusActionsMappingId == 37) {

                            $('#ProjectApprovedTittle').html(tittleIPA);
                            $('#ProjApproved').modal('show');
                            getProjApproved(spnstatusId, spnstatusActionsMappingId);
                        }


                        else {
                            //$('#ProjectApprovedTittle').html(tittle);
                            //$('#ProjApproved').modal('show');
                            debugger;
                            var hascert = [24, 25, 26, 27, 28, 29].includes(parseInt(spnstatusId));

                            if (hascert) {
                                $('#IPAProjectApprovedTittle').html(tittle);
                                $('#IPAProjApproved').modal('show');
                            } else {
                                $('#ProjectApprovedTittle').html(tittle);
                                $('#ProjApproved').modal('show');
                            }



                            getProjApproved(spnstatusId, spnstatusActionsMappingId);
                        }
                    }
                });

                $("body").on("click", ".btnGetsummay", function () {
                    var spnstatusId = $(this).closest("div").find("#spnstatusId").html();
                    if (spnstatusId != 1041 && parseInt(spnstatusId) != 44 && parseInt(spnstatusId) != 46) {
                        $('#ProjGetsummay').modal('show');
                        $('#ProjectSummaryTittle').html("Total Proj Movement: " + $(this).closest("div").find(".statusprojsummry").html());
                        $('#IsNotduplicate').prop('checked', false);
                        getProjGetsummay(spnstatusId, true);
                    }
                });
            }
        },
        error: function () {
            alert('Error fetching comments.');
        }
    });
}

//function GetAllDashbaordCount() {
  
//    $.ajax({
//        type: "POST",
//        url: '/Home/GetDashboardCount',
//        data: { "Id": 0 },
//        success: function (data) {

//            var dtoDashboardHeaderlst = data.dtoDashboardHeaderlst;
//            var dTOApprovedCountlst = data.dtoApprovedCountlst;
//            var dTODashboardCountlstForAction = data.dtoDashboardCountlstForAction;

//            var listitem = '';
//            var stageId = 0;
//            var tot = 0;
//            var peding = 0;
//            var sent = 0;

//            if (data != null) {
//                dtoDashboardHeaderlst.sort((a, b) => a.stageId - b.stageId || a.statseq - b.statseq);

//                for (var i = 0; i < dtoDashboardHeaderlst.length; i++) {

//                    if (stageId != dtoDashboardHeaderlst[i].stageId) {
//                        if (stageId != 0) {
//                            listitem += '</div>';
//                        }

//                        var stage = "";
//                        if (dtoDashboardHeaderlst[i].stageId === 1) stage = "(Sponsor & DDGIT)";
//                        if (dtoDashboardHeaderlst[i].stageId === 2) stage = "(Parallel Processing)";
//                        if (dtoDashboardHeaderlst[i].stageId === 3) stage = "(Serial Processing)";

//                        listitem += '<div class="newprojectheading text-center"> ' + dtoDashboardHeaderlst[i].stages + " " + stage + ' </div>';
//                        listitem += '<div class="r-1 row g-3 mt-2 dash-stage-row">';
//                    }

//                    // ✅ removed inline style
//                    listitem += '<div class="cd-1 col-12 col-sm-6 col-md-4 col-lg-1 dash-card">';

//                    tot = 0;
//                    peding = 0;
//                    sent = 0;

//                    var DTODashboardCount = data.dtoDashboardCountlst.filter(function (element) {
//                        return element.stagesId == dtoDashboardHeaderlst[i].stageId && element.statusId == dtoDashboardHeaderlst[i].statusId;
//                    });

//                    if (parseInt(dtoDashboardHeaderlst[i].statusId) == 2 || parseInt(dtoDashboardHeaderlst[i].statusId) == 3
//                        || parseInt(dtoDashboardHeaderlst[i].statusId) == 22 || parseInt(dtoDashboardHeaderlst[i].statusId) == 31
//                        || parseInt(dtoDashboardHeaderlst[i].statusId) == 37) {

//                        var ForAction = [];
//                        if (parseInt(dtoDashboardHeaderlst[i].statusId) == 2)
//                            ForAction = dTODashboardCountlstForAction.filter(function (e) { return e.actionId == 10; });
//                        else if (parseInt(dtoDashboardHeaderlst[i].statusId) == 3)
//                            ForAction = dTODashboardCountlstForAction.filter(function (e) { return e.actionId == 11; });
//                        else if (parseInt(dtoDashboardHeaderlst[i].statusId) == 22)
//                            ForAction = dTODashboardCountlstForAction.filter(function (e) { return e.actionId == 3 && e.stagesId == 2; });
//                        else if (parseInt(dtoDashboardHeaderlst[i].statusId) == 31)
//                            ForAction = dTODashboardCountlstForAction.filter(function (e) { return e.actionId == 3 && e.stagesId == 3; });
//                        else if (parseInt(dtoDashboardHeaderlst[i].statusId) == 37)
//                            ForAction = dTODashboardCountlstForAction.filter(function (e) { return e.actionId == 3 && e.stagesId == 1; });

//                        for (var j = 0; j < ForAction.length; j++) {
//                            tot += ForAction[j].tot;

//                            if (ForAction[j].isComplete == false) peding += ForAction[j].tot;
//                            if (ForAction[j].isComplete == true) sent += ForAction[j].tot;
//                        }

//                    } else {
//                        for (var j = 0; j < DTODashboardCount.length; j++) {
//                            tot += DTODashboardCount[j].tot;

//                            if (DTODashboardCount[j].isComplete == false) peding += DTODashboardCount[j].tot;
//                            if (DTODashboardCount[j].isComplete == true) sent += DTODashboardCount[j].tot;
//                        }
//                    }

//                    listitem += '<div class="icon-container ApprovedProj cursorpointer"><span class="d-none" id="spnstatusId">' + dtoDashboardHeaderlst[i].statusId + '</span>';

//                    // ✅ removed inline image styles + h5 style
//                    if (dtoDashboardHeaderlst[i].statusId == 2 || dtoDashboardHeaderlst[i].statusId == 3 || dtoDashboardHeaderlst[i].statusId == 22 || dtoDashboardHeaderlst[i].statusId == 31 || dtoDashboardHeaderlst[i].statusId == 37) {
//                        if (dtoDashboardHeaderlst[i].statusId == 3)
//                            listitem += '<img src="/assets/images/icons/prog.png" alt="Icon" class="dash-icon-img" data-toggle="tooltip" data-placement="top" title="Total No of proj approved at this stage">';
//                        else
//                            listitem += '<img src="/assets/images/icons/rejec.png" alt="Icon" class="dash-icon-img">';

//                        listitem += '<h5 class="dash-h5-mt25"> </h5>';

//                    } else {
//                        listitem += '<img src="/assets/images/icons/prog.png" alt="Icon" class="dash-icon-img" data-toggle="tooltip" data-placement="top" title="Total No of proj approved at this stage">';

//                        var approvedcount = dTOApprovedCountlst.filter(function (element) {
//                            return element.statusId == dtoDashboardHeaderlst[i].statusId;
//                        });

//                        if (approvedcount.length > 0) {

//                            if (dtoDashboardHeaderlst[i].status.includes("BISAG-N") || dtoDashboardHeaderlst[i].status.includes("Re-Vetting")) {
//                                listitem += '<span class="d-none" id="spnstatusActionsMappingId">' + approvedcount[0].statusActionsMappingId + '</span>' +
//                                    '<h5 class="dash-h5-mt8-pad10" data-toggle="tooltip" data-placement="top" title="Total No of proj approved at this stage">' + approvedcount[0].total + ' </h5>';
//                            } else {
//                                listitem += '<span class="d-none" id="spnstatusActionsMappingId">' + approvedcount[0].statusActionsMappingId + '</span>' +
//                                    '<h5 class="dash-h5-mt8" data-toggle="tooltip" data-placement="top" title="Total No of proj approved at this stage">' + approvedcount[0].total + ' </h5>';
//                            }

//                        } else {
//                            listitem += '<span class="d-none" id="spnstatusActionsMappingId">0</span><h5 class="dash-h5-mt8">0 </h5>';
//                        }
//                    }

//                    listitem += '<div class="t-1 statusprojsummry d-none">' + dtoDashboardHeaderlst[i].status + '</div> ';
//                    listitem += '</div>';

//                    listitem += '<div class="cursorpointer btnGetsummay "><span class="d-none" id="spnstatusId">' + dtoDashboardHeaderlst[i].statusId + '</span>';
//                    listitem += '<div class="">';

//                    // ✅ removed inline padding-top style
//                    if (dtoDashboardHeaderlst[i].status.includes("BISAG-N") || dtoDashboardHeaderlst[i].status.includes("Re-Vetting")) {
//                        listitem += '<div class="t-1 statusprojsummry dash-status-pad7">' + dtoDashboardHeaderlst[i].status + '</div> ';
//                    } else {
//                        listitem += '<div class="t-1 statusprojsummry">' + dtoDashboardHeaderlst[i].status + '</div> ';
//                    }

//                    // ✅ removed inline font-size styles
//                    if (dtoDashboardHeaderlst[i].status.includes("BISAG-N") || dtoDashboardHeaderlst[i].status.includes("Re-Vetting")) {

//                        listitem += '<span class="badge badge-light text-black d-none dash-badge-lg" data-toggle="tooltip" data-placement="top"><span class="badge bg-danger">' + peding + '</span> / <span class="badge bg-success">' + sent + '</span></span>';
//                        listitem += ' </div>';
//                        listitem += ' <div class="mb-2">';
//                        listitem += '<span class="badge badge-primary mr-2 d-none dash-badge-md" data-toggle="tooltip" data-placement="top" title="Total No of transaction at this stage">' + tot + '</span>';

//                    } else {

//                        listitem += '<span class="badge badge-light text-black dash-badge-lg" data-toggle="tooltip" data-placement="top"><span class="badge bg-danger">' + peding + '</span> / <span class="badge bg-success">' + sent + '</span></span>';
//                        listitem += ' </div>';
//                        listitem += ' <div class="mb-2">';
//                        listitem += '<span class="badge badge-primary mr-2 dash-badge-md" data-toggle="tooltip" data-placement="top" title="Total No of transaction at this stage">' + tot + '</span>';
//                    }

//                    listitem += ' </div>';
//                    listitem += ' </div>';
//                    listitem += ' </div>';
//                    listitem += ' </div>';

//                    stageId = dtoDashboardHeaderlst[i].stageId;
//                }

//                $("#carddashboardcount").html(listitem);

//                // Optional: initialize tooltips if you use Bootstrap tooltip
//                // $('[data-toggle="tooltip"]').tooltip();

//                $(document).off("click", ".ApprovedProj").on("click", ".ApprovedProj", function () {
//                    var spnstatusId = $(this).closest("div").find("#spnstatusId").html();
//                    var spnstatusActionsMappingId = $(this).closest("div").find("#spnstatusActionsMappingId").html();

//                    tittle = "Approved: " + $(this).closest("div").find(".statusprojsummry").html();
//                    tittleIPA = "Approved by:  " + $(this).closest("div").find(".statusprojsummry").html() + " (Parallel Processing)";

//                    if (parseInt(spnstatusActionsMappingId) == 1) {
//                        Status = 'Accepted';
//                    } else if (parseInt(spnstatusActionsMappingId) == 9) {
//                        Status = 'obsn Raised';
//                    } else if (parseInt(spnstatusActionsMappingId) == 113) {
//                        Status = 'Rectified';
//                    } else if (parseInt(spnstatusActionsMappingId) == 48 || parseInt(spnstatusActionsMappingId) == 53) {
//                        Status = 'Approved';
//                    } else if (parseInt(spnstatusActionsMappingId) == 60) {
//                        tittle = "Closed Project";
//                        Status = 'Closed';
//                    } else if (parseInt(spnstatusActionsMappingId) == 68 || parseInt(spnstatusActionsMappingId) == 73 || parseInt(spnstatusActionsMappingId) == 63
//                        || parseInt(spnstatusActionsMappingId) == 78 || parseInt(spnstatusActionsMappingId) == 83 || parseInt(spnstatusActionsMappingId) == 88) {
//                        Status = 'Completed';
//                    } else if (parseInt(spnstatusActionsMappingId) == 26 || parseInt(spnstatusActionsMappingId) == 31 || parseInt(spnstatusActionsMappingId) == 37) {
//                        Status = 'Approved';
//                    }

//                    if (parseInt(spnstatusActionsMappingId) == 0) {
//                        Swal.fire({
//                            icon: "error",
//                            title: "Oops...",
//                            text: "Data Not Found!"
//                        });
//                    } else {

//                        $('#CertName').html("Cert&Att");

//                        if (spnstatusId == 2 || spnstatusId == 3 || spnstatusId == 22) {
//                            // no action
//                        }
//                        else if (parseInt(spnstatusId) == 44 || parseInt(spnstatusId) == 46) {
//                            $('#ProjectApprovedTittleBisag').html(tittle);
//                            $('#BISAG-N').modal('show');
//                            getProjBisagN(spnstatusId, spnstatusActionsMappingId);
//                        }
//                        else if (spnstatusId == 21) {
//                            $('#IPAProjectApprovedTittle').html(tittle);
//                            $('#IPAProjApproved').modal('show');
//                            getProjApproved(spnstatusId, spnstatusActionsMappingId);
//                        }
//                        else if (spnstatusActionsMappingId == 26 || spnstatusActionsMappingId == 31 || spnstatusActionsMappingId == 37) {
//                            $('#ProjectApprovedTittle').html(tittleIPA);
//                            $('#ProjApproved').modal('show');
//                            getProjApproved(spnstatusId, spnstatusActionsMappingId);
//                        }
//                        else {
//                            debugger;
//                            var hascert = [24, 25, 26, 27, 28, 29].includes(parseInt(spnstatusId));

//                            if (hascert) {
//                                $('#IPAProjectApprovedTittle').html(tittle);
//                                $('#IPAProjApproved').modal('show');
//                            } else {
//                                $('#ProjectApprovedTittle').html(tittle);
//                                $('#ProjApproved').modal('show');
//                            }

//                            getProjApproved(spnstatusId, spnstatusActionsMappingId);
//                        }
//                    }
//                });

//                $("body").off("click", ".btnGetsummay").on("click", ".btnGetsummay", function () {
//                    var spnstatusId = $(this).closest("div").find("#spnstatusId").html();

//                    if (spnstatusId != 1041 && parseInt(spnstatusId) != 44 && parseInt(spnstatusId) != 46) {
//                        $('#ProjGetsummay').modal('show');
//                        $('#ProjectSummaryTittle').html("Total Proj Movement: " + $(this).closest("div").find(".statusprojsummry").html());
//                        $('#IsNotduplicate').prop('checked', false);

//                        getProjGetsummay(spnstatusId, true);
//                    }
//                });
//            }

//        },
//        error: function () {
//            alert('Error fetching comments.');
//        }
//    });
//}


    function Getbarchart() {
        // Fetching data for the first chart
        $.ajax({
            url: '/Home/indexToBarChartS',
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                if (data.error) {
                    console.error('Error fetching data:', data.error);
                    return;
                }

                var monthNames = [...new Set(data.map(item => item.MonthNameYr))];
                var unitNames = [...new Set(data.map(item => item.unitname))];

                var datasets = unitNames.map(unitName => {
                    var totalInData = [];
                    var totalOutData = [];

                    monthNames.forEach(month => {
                        var monthData = data.find(item => item.MonthNameYr === month && item.unitname === unitName);
                        if (monthData) {
                            totalInData.push(monthData.TotalIn);
                            totalOutData.push(monthData.TotalOut);
                        } else {
                            totalInData.push(0);
                            totalOutData.push(0);
                        }
                    });

                    var totalInColor = getRandomColor();
                    var totalOutColor = getRandomColor();

                    return [{
                        label: unitName + ' Proj In',
                        data: totalInData,
                        backgroundColor: totalInColor,
                        stack: unitName,
                    }, {
                        label: unitName + ' Proj Out',
                        data: totalOutData,
                        backgroundColor: totalOutColor,
                        stack: unitName,
                    }];

                }).flat();

                var ctx = document.getElementById('myChart').getContext('2d');
                var myChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: monthNames,
                        datasets: datasets
                    },
                    options: {
                        scales: {
                            x: {
                                stacked: true,
                                title: {
                                    display: true,
                                    text: 'Month Name'
                                }
                            },
                            y: {
                                stacked: true,
                                title: {
                                    display: true,
                                    text: 'Total In/Total Out'
                                }
                            }
                        }
                    }
                });

                // Set the title for the first chart
                $('#chartTitle').text('Bar Chart Title');
            }
        });

        // Fetching data for the second chart
        $.ajax({
            url: '/Home/indexToPieChart',
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                if (data.error) {
                    console.error('Error fetching data:', data.error);
                    return;
                }

                updatePieChart(data);

                // Set the title for the second chart
                $('#chart1Title').text('Pie Chart Title');
            },
            error: function (error) {
                console.error('Error fetching data:', error);
            }
        });
    }
   




function getProjApproved(spnstatusId, spnstatusActionsMappingId) {

    var listItem = "";
    var table = new DataTable('#dashboardApproved');
    table.destroy();
    /* alert(spnstatusActionsMappingId)*/
    var userdata = {
        "StatusId": spnstatusId,
        "statusActionsMappingId": spnstatusActionsMappingId,
    };

    $.ajax({
        url: '/Home/GetDashboardApproved',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            debugger;
            if (response != "null" && response != null) {

                var hasIPA = response.some(item =>
                    [53, 63, 68, 73, 78, 83, 88].includes(item.statusactionMappingid)
                );

                if (response == -1) {
                    Swal.fire({ text: "" });
                } else if (response == 0) {

                    $('#DetailBodyApproved').empty();
                    listItem += "<tr><td class='text-center' colspan='7'>No Record Found</td></tr>";
                    $("#DetailBodyApproved").html(listItem);
                    $("#lblTotal").html(0);

                } else {
                    var count = 1;

                    var unitId = $('#spndashboardUnitId').text().trim();

                    $('#DetailBodyApproved').empty();
                    $('#dashboardApproved').dataTable().fnClearTable();
                    $('#dashboardApproved').dataTable().fnDestroy();

                    for (var i = 0; i < response.length; i++) {

                        var projName = response[i].projName;
                        var words = projName.split(" ");
                        var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;

                        listItem += "<tr>";
                        listItem += "<td class='align-middle dp-w-3'>" + count + "</td>";

                        //listItem += "<td class='align-middle nowrap'><span id='ProjName'>" + response[i].projName + "</span></td>";
                        if (unitId == 1 || unitId == 2 || unitId == 3 || unitId == 4 || unitId == 5 || unitId == 7) {
                            listItem += "<td class='align-middle nowrap'>" +
                                "<a class='ProjName' title='" + shortProjName + "' data-proj-id='" + response[i].projId + "' data-proj-name='" + shortProjName + "' " +
                                "href='/Projects/ProjHistory?EncyID=" + response[i].encyID + "&Type=XRDC'>" +
                                shortProjName + "</a></td>";
                        } else {
                            listItem += "<td class='align-middle nowrap'><span id='ProjName'>" + shortProjName + "</span></td>";
                        }

                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stakeHolder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + DateFormateddMMyyyyhhmmss(response[i].timeStamp) + "</span></td>";
                        //if (response[i].status != "BISAG-N") {
                        listItem += "<td ><span class='badge badge-success' id='divName'>" + Status + "</span></td>";
                        //}

                        if (response[i].statusactionMappingid == 53) {
                            if (response[i].approvedDt != null && response[i].approvedRemarks != null) {

                                // ✅ removed inline visibility style -> class
                                var visClass = (response[i].statusactionMappingid == 53) ? "dp-vis-visible" : "dp-vis-hidden";

                                listItem += `
<td class='text-center noExport'>
  <button class='badge badge-success generateCertificate ${visClass}'
      onclick="window.open('/Home/Generate?ProjectName=${encodeURIComponent(response[i].projName)}&ApprovedRemarks=${encodeURIComponent(response[i].approvedRemarks)}&ApprovedDt=${encodeURIComponent(response[i].approvedDt)}', '_blank')">
      PDF
  </button>
</td>`;

                            } else {
                                listItem += `<td></td>`;
                            }

                        } else if ([63, 68, 73, 78, 83, 88].includes(response[i].statusactionMappingid)) {

                            if (response[i].hasAttachment == true && response[i].isSponsor == true) {
                                listItem += `
<td>
    <a href="javascript:void(0);" 
       class="anchorDetail" 
       data-id="${response[i].psmIds}">
        <img src="/assets/images/icons/attachemnts_clip.png" 
             alt="Icon" 
             class="dp-attach-icon">
    </a>
</td>`;
                            }
                            else {
                                listItem += `<td></td>`;
                            }

                        }

                        listItem += "</tr>";
                        count++;
                    }

                    if (hasIPA) {
                        //initializeDataTable("#IPAdashboardApproved");
                        refreshDataTable('#IPAdashboardApproved');
                        $("#IPADetailBodyApproved").html(listItem);
                        var table = $('#IPAdashboardApproved').DataTable({
                            lengthChange: true,
                            dom: 'lBfrtip',
                            retrieve: true,
                            destroy: true,
                            pageLength: 25,
                            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                            "order": [[0, "asc"]],

                            buttons: [
                                'copy',
                                'excel',
                                'csv',
                            ],
                            searchBuilder: {
                                conditions: {
                                    num: {
                                        'MultipleOf': {
                                            conditionName: 'Multiple Of',
                                            init: function (that, fn, preDefined = null) {
                                                var el = $('<input/>').on('input', function () { fn(that, this) });

                                                if (preDefined !== null) {
                                                    $(el).val(preDefined[0]);
                                                }

                                                return el;
                                            },
                                            inputValue: function (el) {
                                                return $(el[0]).val();
                                            },
                                            isInputValid: function (el, that) {
                                                return $(el[0]).val().length !== 0;
                                            },
                                            search: function (value, comparison) {
                                                return value % comparison === 0;
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    } else {

                        refreshDataTable('#dashboardApproved');
                        $("#DetailBodyApproved").html(listItem);

                        var table = $('#dashboardApproved').DataTable({
                            lengthChange: true,
                            dom: 'lBfrtip',
                            retrieve: true,
                            destroy: true,
                            pageLength: 25,
                            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                            "order": [[0, "asc"]],

                            buttons: [
                                'copy',
                                'excel',
                                'csv',
                            ],
                            searchBuilder: {
                                conditions: {
                                    num: {
                                        'MultipleOf': {
                                            conditionName: 'Multiple Of',
                                            init: function (that, fn, preDefined = null) {
                                                var el = $('<input/>').on('input', function () { fn(that, this) });

                                                if (preDefined !== null) {
                                                    $(el).val(preDefined[0]);
                                                }

                                                return el;
                                            },
                                            inputValue: function (el) {
                                                return $(el[0]).val();
                                            },
                                            isInputValid: function (el, that) {
                                                return $(el[0]).val().length !== 0;
                                            },
                                            search: function (value, comparison) {
                                                return value % comparison === 0;
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            }
            else {

                $('#DetailBodyApproved').empty();
                listItem += "<tr><td class='text-center' colspan='7'>No Record Found</td></tr>";
                $("#DetailBodyApproved").html(listItem);

            }
        },
        error: function (result) {
            Swal.fire({ text: "" });
        }
    });
}

function getProjGetsummay(spnstatusId, IsDuplicate) {
   
    var listItem = "";
   
    $("#spndashboardstatusId").html(spnstatusId);
    var userdata = {
        "StatusId": spnstatusId,
        "IsDuplicate": IsDuplicate
    };

    $.ajax({
        url: '/Home/GetDashboardStatusDetails',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            console.log("GetDashboardStatusDetails", response);
            
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({ text: "" });
                } else if (response == 0) {

                    $('#DetailBodysummary1').empty();
                    listItem += "<tr><td class='text-center' colspan='10'>No Record Found</td></tr>";
                    $("#DetailBodysummary1").html(listItem);
                    $("#lblTotal").html(0);


                } else {
                    var count = 1;
                    $('#dashboardDeatils').dataTable().fnClearTable();
                    $('#dashboardDeatils').dataTable().fnDestroy();

                    var unitId = $('#spndashboardUnitId').text().trim();
                    for (var i = 0; i < response.length; i++) {
                        
                        var projName = response[i].projName;
                        var words = projName.split(" ");
                        var shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;

                        listItem += "<tr>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + count + "</span></td>";
                        //listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].projName + "</span></td>";
                        //listItem += "<td class='align-middle'><span id='ProjName' title='" + projName + "'>" + shortProjName + "</span></td>";
                        if (unitId == 1 || unitId == 2 || unitId == 3 || unitId == 4 || unitId == 5 || unitId == 7/*|| response[i].stakeHolder == unitId*/) {
                            listItem += "<td class='align-middle'>" +
                                "<a class='ProjName' title='" + projName + "' data-proj-id='" + response[i].projId + "' data-proj-name='" + projName + "' " +
                                "href='/Projects/ProjHistory?EncyID=" + response[i].encyID + "&Type=XRDC'>" +
                                shortProjName + "</a></td>";
                        }
                        else {
                            listItem += "<td class='align-middle'><span id='ProjName' title='" + projName + "'>" + shortProjName + "</span></td>";
                        }                       
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stakeHolder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].fromUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].toUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stage + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].status + "</span></td>";
                        listItem += "<td class='align-middle'><span id='divName'>" + response[i].action + "</span></td>";
                        if (parseInt(spnstatusId) === 1) {

                            listItem += "<td class='align-middle'><span id='divName'>" + DateFormateddMMyyyyhhmmss(response[i].initiatedDate) + "</span></td>";
                        } else {

                            listItem += "<td class='align-middle'><span id='divName'>" + DateFormateddMMyyyyhhmmss(response[i].dateTimeOfUpdate) + "</span></td>";
                        }
                        
                       
                        if (response[i].isComplete) {
                            
                            if (response[i].stkStatusId == 2) {
                                listItem += "<td ><span class='badge badge-warning' id='divName'>Obsn</span></td>";
                            } else if (response[i].stkStatusId == 3) {
                                listItem += "<td ><span class='badge badge-danger' id='divName'>Rejected</span></td>";
                            }
                            else if (response[i].stkStatusId == 5) {
                                listItem += "<td ><span class='badge badge-success' id='divName'>Info</span></td>";
                            }
                            else {
                                //listItem += "<td ><span class='badge badge-success' id='divName'>Processed</span></td>";
                                listItem += `<td ><span class='badge badge-success' id='divName' title='Processed by ${response[i].toUnitName}'>${response[i].toUnitName}</span></td>`;
                            }
                        }
                        else {
                            if (response[i].stkStatusId == 2) {
                                listItem += "<td ><span class='badge badge-warning' id='divName'>Obsn</span></td>";
                            } else if (response[i].stkStatusId == 3) {
                                listItem += "<td ><span class='badge badge-danger' id='divName'>Rejected</span></td>";
                            }
                            else if (response[i].stkStatusId == 5) {
                                listItem += "<td ><span class='badge badge-success' id='divName'>Info</span></td>";
                            } else {
                                //listItem += "<td ><span class='badge badge-danger' id='divName'>Pending</span></td>";
                                listItem += `<td ><span class='badge badge-danger' id='divName' title='Pending with ${response[i].toUnitName}'>${response[i].toUnitName}</span></td>`;
                            }
                        }

                        listItem += "</tr>";
                        count++;
                    }

                    $("#DetailBodysummary1").html(listItem);


                    var table = $('#dashboardDeatils').DataTable({
                        lengthChange: true,
                        retrieve: true,
                        bDestroy: true,
                        destroy: true,
                        searching: true,
                        /* stateSave: true,*/
                        "order": [[0, "asc"]],
                        "ordering": true,
                        "paging": true,
                        pageLength: 25,
                        dom: 'lBfrtip',
                        buttons: [
                            'copy',
                            'excel',
                            'csv',
                            //{
                            //    text: 'PDF',
                            //    extend: 'pdfHtml5',
                            //    action: function (e, dt, node, config) {
                            //        PdfDiv();
                            //    }
                            //},
                        ],
                        searchBuilder: {
                            conditions: {
                                num: {
                                    'MultipleOf': {
                                        conditionName: 'Multiple Of',
                                        init: function (that, fn, preDefined = null) {
                                            var el = $('<input/>').on('input', function () { fn(that, this) });

                                            if (preDefined !== null) {
                                                $(el).val(preDefined[0]);
                                            }

                                            return el;
                                        },
                                        inputValue: function (el) {
                                            return $(el[0]).val();
                                        },
                                        isInputValid: function (el, that) {
                                            return $(el[0]).val().length !== 0;
                                        },
                                        search: function (value, comparison) {
                                            return value % comparison === 0;
                                        }
                                    }
                                }
                            }
                        }
                    });

                    //setTimeout(function () {   //calls click event after a certain time
                    //    $('input[type=search]').val('');
                    //    $('input[type=search]').trigger("click");
                    //    $('input[type=search]').focus();
                    //}, 1000);

                }
            }
            else {

                $('#DetailBodysummary1').empty();
                listItem += "<tr><td class='text-center' colspan='10'>No Record Found</td></tr>";
                $("#DetailBodysummary1").html(listItem);



            }
        },
        error: function (result) {
            Swal.fire({ text: "" });
        }
    });
}

function updatePieChart(data) {
    var titles = data.map(item => item.Status);
    var chartData = data.map(item => item.TotalProj);

    var canvas = document.getElementById('myChart1');
    if (!canvas) {
        console.error("Canvas element 'myChart1' not found.");
        return;
    }
    var backgroundColors = generateRandomColors(titles.length);

    var ctx = canvas.getContext('2d');

    var myChart1 = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: titles,
            datasets: [{
                data: chartData,
                backgroundColor: backgroundColors,
                borderColor: backgroundColors, // Border color same as background color for consistency
                borderWidth: 1,
            }],
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                },
            },
        },
    });
}

function getRandomColor() {
    const minBrightness = 130;
    let color;
    do {
        color = '#' + Math.floor(Math.random() * 16777215).toString(16);
        const rgb = parseInt(color.slice(1), 16);
        const r = (rgb >> 16) & 0xff;
        const g = (rgb >> 8) & 0xff;
        const b = (rgb >> 0) & 0xff;
        const brightness = (r + g + b) / 3;
        if (brightness < minBrightness) {
            color = null;
        }
    } while (color === null);
    return color;
}

function generateRandomColors(count) {
    const colors = [];
    for (let i = 0; i < count; i++) {
        let color;
        do {
            color = getRandomColor();
        } while (colors.includes(color));
        colors.push(color);
    }
    return colors;
}





document.addEventListener('DOMContentLoaded', function () {
    const modal = document.querySelector('.modal-content');
    const header = modal.querySelector('.modal-header');

    let isDragging = false;
    let startX, startY, initialX, initialY;

    header.addEventListener('mousedown', (e) => {
        isDragging = true;
        startX = e.clientX;
        startY = e.clientY;
        initialX = modal.offsetLeft;
        initialY = modal.offsetTop;
        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    });

    function onMouseMove(e) {
        if (isDragging) {
            const dx = e.clientX - startX;
            const dy = e.clientY - startY;
            modal.style.left = `${initialX + dx}px`;
            modal.style.top = `${initialY + dy}px`;
        }
    }

    function onMouseUp() {
        isDragging = false;
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);
    }
});
function DateFormateddMMyyyyhhmmss(date) {

    var todaysDate = new Date();
    var datef1 = new Date(date);
    //if (datef1.setHours(0, 0, 0, 0) == todaysDate.setHours(0, 0, 0, 0)) {
    //    // Date equals today's date

    //    return 'Today';
    //}
    //else {
    var datef2 = new Date(date);
    var months = "" + `${(datef2.getMonth() + 1)}`;
    var days = "" + `${(datef2.getDate())}`;
    var pad = "00"
    var monthsans = pad.substring(0, pad.length - months.length) + months
    var dayans = pad.substring(0, pad.length - days.length) + days
    var year = `${datef2.getFullYear()}`;
    var hh = `${datef2.getHours()}`;
    var mm = `${datef2.getMinutes()}`;
    var ss = `${datef2.getSeconds()}`;
    if (hh < 10) hh = "0" + hh;
    if (mm < 10) mm = "0" + mm;
    if (ss < 10) ss = "0" + ss;
    if (year > 1902) {

        var datemmddyyyy = dayans + `/` + monthsans + `/` + year + ` ` + hh + `:` + mm + `:` + ss
        return datemmddyyyy;
    }
    else {
        return '';
    }
    // }

    //`${datef2.getFullYear()}/` + monthsans + `/` + dayans ;
}


function getProjBisagN(spnstatusId, spnstatusActionsMappingId) {
    var listItem = "";
    var table = $('#dashboardApprovedBisagN').DataTable(); // Initialize DataTable
    table.clear().destroy(); // Destroy the existing table instance

    var userdata = {
        "StatusId": spnstatusId,
        "statusActionsMappingId": spnstatusActionsMappingId,
    };

    $.ajax({
        url: '/Home/GetDashboardApproved',
        contentType: 'application/x-www-form-urlencoded',
        data: userdata,
        type: 'POST',
        success: function (response) {
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({ text: "No data found." });
                } else if (response == 0) {

                    $('#DetailBodyBisagN').empty();
                    listItem += "<tr><td class='text-center' colspan='7'>No Record Found</td></tr>";
                    $("#DetailBodyBisagN").html(listItem);
                    $("#lblTotal").html(0);

                } else {
                    var count = 1;

                    $('#DetailBodyBisagN').empty();
                    for (var i = 0; i < response.length; i++) {
                        listItem += "<tr>";
                        listItem += "<td class='align-middle'>" + count + "</td>";
                        listItem += "<td class='align-middle nowrap'><span id='ProjName'>" + response[i].projName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stakeHolder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + DateFormateddMMyyyyhhmmss(response[i].timeStamp) + "</span></td>";
                        listItem += "</tr>";
                        count++;
                    }

                    $("#DetailBodyBisagN").html(listItem);

                    // Initialize the DataTable
                    var table = $('#dashboardApprovedBisagN').DataTable({
                        lengthChange: true,
                        retrieve: true,
                        bDestroy: true,
                        searching: true,
                        stateSave: true,
                        "order": [[0, "asc"]],
                        "ordering": true,
                        "paging": true,
                        dom: 'lBfrtip',
                        buttons: [
                            'copy',
                            'excel',
                            'csv',
                        ],
                        searchBuilder: {
                            conditions: {
                                num: {
                                    'MultipleOf': {
                                        conditionName: 'Multiple Of',
                                        init: function (that, fn, preDefined = null) {
                                            var el = $('<input/>').on('input', function () { fn(that, this) });

                                            if (preDefined !== null) {
                                                $(el).val(preDefined[0]);
                                            }

                                            return el;
                                        },
                                        inputValue: function (el) {
                                            return $(el[0]).val();
                                        },
                                        isInputValid: function (el, that) {
                                            return $(el[0]).val().length !== 0;
                                        },
                                        search: function (value, comparison) {
                                            return value % comparison === 0;
                                        }
                                    }
                                }
                            }
                        }
                    });

                }
            } else {
                $('#DetailBodyBisagN').empty();
                listItem += "<tr><td class='text-center' colspan='7'>No Record Found</td></tr>";
                $("#DetailBodyBisagN").html(listItem);
            }
        },
        error: function (result) {
            Swal.fire({ text: "Error loading data." });
        }
    });
}



function CreateChartSummary() {

    $.ajax({
        type: "POST",
        url: '/Home/CreateChartSummary',
        data: {
            "Id": 0,

        },
        success: function (data) {
            const ProjectStatus = data.projectStatus;
            const colors = [
                "#73a3f9", "#fbbb4b", "#c3cad4", "#48d0ad", "#a88bfa",
                "#4ee17e", "#fee47d", "#f76e6e", "#8f88f9", "#3edfd0",
                "#f9cb48", "#d7aaff", "#4fd4ff", "#faa4a4", "#a6eb48",
                "#faa6d6", "#c6b4ff", "#40dff4", "#fa4d78", "#a285ff"
            ];
          
            // Map names and totals
            const labels = ProjectStatus.map(x => x.name);
            const totals = ProjectStatus.map(x => x.total);

            new Chart(document.getElementById('yearsStatusChart'), {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Projects',
                        backgroundColor: colors,
                        data: totals,
                        barThickness: 50,      
                    }]
                },
               
                      options: {
                    plugins: {
                        title: {
                            display: true,
                           /* text: 'Years Wise Status'*/
                        },
                        legend: {
                            display: false
                        },
                        datalabels: {
                            anchor: 'end',
                            align: 'end',
                            color: '#000',
                            font: {
                                weight: 'bold'
                            },
                            formatter: function (value) {
                                return value;
                            }
                        }
                    }
                },
                plugins: [ChartDataLabels] // Register the plugin
            });
            //Pre  Approved Projects Chart
            const ApprovedProjects = data.approvedProjectsPre;

            // Map names and totals
            const labels1 = ApprovedProjects.map(x => x.name);
            const totals1 = ApprovedProjects.map(x => x.total);
            new Chart(document.getElementById('approvedProjectsChart'), {
                type: 'bar',
                data: {
                    labels: labels1,
                    datasets: [{
                        label: 'Projects',
                        backgroundColor: colors,
                        data: totals1,barThickness: 50,    
                    }]
                },
                options: {
                    plugins: {
                        title: {
                            display: true,
                           /* text: 'Stage Approve Status'*/
                        },
                        legend: {
                            display: false
                        },
                        datalabels: {
                            anchor: 'end',
                            align: 'end',
                            color: '#000',
                            font: {
                                weight: 'bold'
                            },
                            formatter: function (value) {
                                return value;
                            }
                        }
                    }
                },
                plugins: [ChartDataLabels] // Register the plugin
            });

            //Pre  Approved Projects Chart
            const ApprovedProjectsPost = data.approvedProjectsPost;

            // Map names and totals
            const labels11 = ApprovedProjectsPost.map(x => x.name);
            const totals11 = ApprovedProjectsPost.map(x => x.total);
            new Chart(document.getElementById('approvedProjectsChartPost'), {
                type: 'bar',
                data: {
                    labels: labels11,
                    datasets: [{
                        label: 'Projects',
                        backgroundColor: colors,
                        data: totals11, barThickness: 50,
                    }]
                },
                options: {
                    plugins: {
                        title: {
                            display: true,
                            /* text: 'Stage Approve Status'*/
                        },
                        legend: {
                            display: false
                        },
                        datalabels: {
                            anchor: 'end',
                            align: 'end',
                            color: '#000',
                            font: {
                                weight: 'bold'
                            },
                            formatter: function (value) {
                                return value;
                            }
                        }
                    }
                },
                plugins: [ChartDataLabels] // Register the plugin
            });



            // Whitelisted Projects Chart
            const WhitelistedProjects = data.whitelistedProjects;
            const colorspie = [
                "green", "red"]
            // Map names and totals
            const labels2 = WhitelistedProjects.map(x => x.name);
            const totals2 = WhitelistedProjects.map(x => x.total);
            new Chart(document.getElementById('whitelistedChart'), {
                    type: 'pie',
                    data: {
                        labels: labels2, // e.g. ['Processed - 72', 'Pending - 9']
                        datasets: [{
                            backgroundColor: colorspie,
                            data: totals2,     // e.g. [72, 9]

                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: {
                                position: 'top'
                            },
                            title: {
                                display: true,
                                text: 'Total Projects'
                            },
                            datalabels: {
                                color: '#fff',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                            formatter: (value) => value
                        }
                    },
                    onClick: (event, elements) => {
                        if (elements.length > 0) {
                            const elementIndex = elements[0].index;
                            showPopup(elementIndex);
                        }
                    }
                },
                plugins: [ChartDataLabels]
            });

            // Total Projects Pie Chart
            const TotalProjects = data.totalProjects;

            // Map names and totals
            //const labels3 = TotalProjects.map(x => x.name);
            //const totals3 = TotalProjects.map(x => x.total);
            //new Chart(document.getElementById('totalProjectsChart'), {
            //    type: 'pie',
            //    data: {
            //        labels: labels3, // e.g. ['Processed - 72', 'Pending - 9']
            //        datasets: [{
            //            backgroundColor: colors,
            //            data: totals3,     // e.g. [72, 9]
                           
            //        }]
            //    },
            //    options: {
            //        responsive: true,
            //        maintainAspectRatio: false,
            //        plugins: {
            //            legend: {
            //                position: 'top'
            //            },
            //            title: {
            //                display: true,
            //                text: 'Total Projects'
            //            },
            //            datalabels: {
            //                color: '#fff',
            //                font: {
            //                    weight: 'bold',
            //                    size: 18
            //                },
            //                formatter: (value) => value
            //            }
            //        }
            //    },
            //    plugins: [ChartDataLabels] // register the plugin
            //});

        }

    });

 
}
function showPopup(segmentIndex) {
   
    const popupOverlay = document.getElementById('popupOverlay');
    const popupTitle = document.getElementById('popupTitle');
    const projectList = document.getElementById('projectList');

    let projects = [];
    let title = '';
    let statusActionsMappingId = 0;
    if (segmentIndex === 0) 
        {
            statusActionsMappingId = 88;
        }
    else if (segmentIndex === 1) {
        statusActionsMappingId = 880;
        }
    let userdata = {
        "StatusId": 29,
        "statusActionsMappingId": statusActionsMappingId,
    };

    $("#WhiteListedProjectDetail").modal("show");
    if (segmentIndex === 0) {
     
     title = `Whitelisted Projects`;
     } else if (segmentIndex === 1) {
     
      title = `Due for Re-vetting`;
     
      }

    $(".spnWhitelistedorDues").html(title);
    GetwhilteListProject(statusActionsMappingId)




}

function closePopup() {
    document.getElementById('popupOverlay').style.display = 'none';
}

// Close popup when clicking outside
document.getElementById('popupOverlay').addEventListener('click', function (event) {
    if (event.target === this) {
        closePopup();
    }
});

// Close popup with Escape key
document.addEventListener('keydown', function (event) {
    if (event.key === 'Escape') {
        closePopup();
    }
});

$(document).ready(function () {

    var TeamDetailPostBackURL = '/Projects/AttDetails';

    // Click event for dynamically generated anchors
    $(document).on("click", ".anchorDetail", function (e) {
      
        e.preventDefault(); // prevent default anchor behavior
       
        debugger;
        var id = $(this).data('id'); // better than attr()

        $.ajax({
            type: "GET",
            url: TeamDetailPostBackURL,
            data: { Id: id }, // no quotes needed
            success: function (response) {
                debugger;
                $('#myModalContenthistoryAttechment').html(response);
                $('#myModalPagehistoryAttechment').modal({
                    backdrop: 'static',
                    keyboard: true
                }).modal('show');

            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });

    // PDF click handler
    $(document).on('click', '.pdf', function () {
        $('#ViewRecordsHistory').modal('show');
    });

});
