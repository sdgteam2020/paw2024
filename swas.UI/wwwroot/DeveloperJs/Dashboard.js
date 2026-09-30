let tittle = "";
let Status = "";



$(document).ready(function () {
   
    Getbarchart()
    GetAllDashbaordCount();
    InboxNotificationCount();
    CreateChartSummary();
    $("#IsNotduplicate").change(function () {


        let ischecked = $(this).is(':checked');
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

        data: {
            "Id": 0
        },

        success: function(data) {


            if (data == null) {
                return;
            }


            var dtoDashboardHeaderlst =
                data.dtoDashboardHeaderlst || [];


            var dTOApprovedCountlst =
                data.dtoApprovedCountlst || [];


            var dTODashboardCountlstForAction =
                data.dtoDashboardCountlstForAction || [];


            var dtoDashboardCountlst =
                data.dtoDashboardCountlst || [];



            var listitem = '';

            var stageId = 0;

            var cd = 1;



            dtoDashboardHeaderlst.sort(function(a, b) {

                return (a.stageId - b.stageId) ||
                    (a.statseq - b.statseq);

            });



            for (var i = 0; i < dtoDashboardHeaderlst.length; i++) {



                var header = dtoDashboardHeaderlst[i];



                if (stageId != header.stageId) {


                    if (stageId != 0) {

                        listitem += '</div>';

                    }



                    var stage = "";



                    if (header.stageId === 1) {

                        stage =
                            "(Sponsor & DDGIT)";

                    }

                    else if (header.stageId === 2) {

                        stage =
                            "(Parallel Processing)";

                    }

                    else if (header.stageId === 3) {

                        stage =
                            "(Serial Processing)";

                    }




                    listitem +=

                        '<div class="header-container text-center text-white shadow-container">' +
                        header.stages +
                        " " +
                        stage +
                        '</div>';



                    listitem +=

                        '<div class="r-1 row g-3 mt-2 db-stage-row">';


                }




                listitem +=

                    '<div class="cd-' + cd +
                    ' col-12 col-sm-6 col-md-4 col-lg-1 db-card-box">';



                cd = cd === 1 ? 2 : 1;



                var tot = 0;

                var peding = 0;

                var sent = 0;

                var pendingComment = 0;

                var sentComment = 0;




                var statusIdInt =
                    parseInt(header.statusId);




                var isCombinedStage =

                    statusIdInt === 2 ||
                    statusIdInt === 3 ||
                    statusIdInt === 22 ||
                    statusIdInt === 31 ||
                    statusIdInt === 37;




                if (isCombinedStage) {



                    var forAction = [];



                    if (statusIdInt === 2) {


                        forAction =
                            dTODashboardCountlstForAction.filter(function(e) {

                                return e.actionId == 10;

                            });


                    }


                    else if (statusIdInt === 3) {


                        forAction =
                            dTODashboardCountlstForAction.filter(function(e) {

                                return e.actionId == 11;

                            });


                    }


                    else if (statusIdInt === 22) {


                        forAction =
                            dTODashboardCountlstForAction.filter(function(e) {

                                return e.actionId == 3 &&
                                    e.stagesId == 2;

                            });


                    }


                    else if (statusIdInt === 31) {


                        forAction =
                            dTODashboardCountlstForAction.filter(function(e) {

                                return e.actionId == 3 &&
                                    e.stagesId == 3;

                            });


                    }


                    else if (statusIdInt === 37) {


                        forAction =
                            dTODashboardCountlstForAction.filter(function(e) {

                                return e.actionId == 3 &&
                                    e.stagesId == 1;

                            });

                    }





                    for (var j = 0; j < forAction.length; j++) {



                        tot += forAction[j].tot;



                        if (forAction[j].isComplete === false) {

                            peding += forAction[j].tot;

                        }



                        if (forAction[j].isComplete === true) {

                            sent += forAction[j].tot;

                        }


                    }



                }

                else {



                    var dashboardCount =

                        dtoDashboardCountlst.filter(function(element) {


                            return element.stagesId == header.stageId &&
                                element.statusId == header.statusId;


                        });




                    if (dashboardCount.length > 0) {


                        peding =
                            dashboardCount[0].pendingTot || 0;



                        sent =
                            dashboardCount[0].sentTot || 0;



                        tot =
                            peding + sent;



                        pendingComment =
                            dashboardCount[0].pendingCommentTot || 0;



                        sentComment =
                            dashboardCount[0].sentCommentTot || 0;


                    }

                }

                /*
                    ICON + APPROVED COUNT SECTION
                */


                listitem +=

                    '<div class="icon-container ApprovedProj cursorpointer">' +

                    '<span class="d-none" id="spnstatusId">' +
                    header.statusId +
                    '</span>';




                if (isCombinedStage) {



                    if (statusIdInt === 3) {


                        listitem +=

                            '<img src="/assets/images/icons/prog.png" ' +
                            'class="db-icon-25" ' +
                            'data-toggle="tooltip" ' +
                            'title="Total No of proj approved at this stage">';


                    }
                    else {


                        listitem +=

                            '<img src="/assets/images/icons/rejec.png" ' +
                            'class="db-icon-25">';


                    }



                    listitem +=

                        '<h5 class="db-h5-mt25"></h5>';



                }

                else {



                    listitem +=

                        '<img src="/assets/images/icons/prog.png" ' +
                        'class="db-icon-25" ' +
                        'data-toggle="tooltip" ' +
                        'title="Total No of proj approved at this stage">';



                    var approvedcount =
                        dTOApprovedCountlst.filter(function(element) {


                            if (statusIdInt == 55) {


                                return element.statusId == 55 &&
                                    element.is_AI_ML == true;


                            }

                            else if (statusIdInt == 1) {


                                return element.statusId == 1 &&
                                    element.is_AI_ML == false;


                            }

                            else {


                                return element.statusId == header.statusId;


                            }


                        });




                    if (approvedcount.length > 0) {



                        var approvedTotal =

                            approvedcount.reduce(function(sum, item) {


                                return sum + (item.total || 0);


                            }, 0);




                        listitem +=

                            '<span class="d-none" id="spnstatusActionsMappingId">' +
                            approvedcount[0].statusActionsMappingId +
                            '</span>' +


                            '<h5 class="db-h5-mt8" ' +
                            'data-toggle="tooltip" ' +
                            'title="Total No of proj approved at this stage">' +
                            approvedTotal +
                            '</h5>';



                    }

                    else {



                        listitem +=

                            '<span class="d-none" id="spnstatusActionsMappingId">0</span>' +

                            '<h5 class="db-h5-mt8">0</h5>';


                    }


                }



                listitem +=

                    '<div class="t-1 statusprojsummry d-none">' +
                    header.status +
                    '</div>';



                listitem += '</div>';




                /*
                    STATUS NAME
                */



                listitem +=

                    '<div class="cursorpointer btnGetsummay">' +

                    '<span class="d-none" id="spnstatusId">' +
                    header.statusId +
                    '</span>' +

                    '<div>';





                var isNamedStatus =

                    header.status.includes("BISAG-N") ||
                    header.status.includes("Re-Whitelisting") ||
                    header.status.includes("Foreclosed") ||
                    header.status.includes("Obsn") ||
                    header.status.includes("Fielded In Next STC");




                if (isNamedStatus) {


                    listitem +=

                        '<div class="t-1 statusprojsummry db-status-pt7">' +
                        header.status +
                        '</div>';


                }

                else {


                    listitem +=

                        '<div class="t-1 statusprojsummry">' +
                        header.status +
                        '</div>';


                }





                /*
                    COMMENTS STATUS
                */


                var isCommentStatus =

                    dtoDashboardCountlst.filter(function(e) {


                        return e.stagesId == header.stageId &&
                            e.statusId == header.statusId &&
                            e.iscomment == 1;


                    }).length > 0;




                var isCounterOnlyStatus =


                    header.status.includes("BISAG-N") ||
                    header.status.includes("Foreclosed") ||
                    header.status.includes("Obsn") ||
                    header.status.includes("Re-Whitelisting") ||
                    header.status.includes("Fielded In Next STC");




                if (isCommentStatus) {



                    listitem +=


                        '<span class="badge badge-light text-black db-badge-18">' +


                        '<span class="badge bg-danger get_Pen_Sen_Comments count-up" ' +
                        'data-commenttype="3" ' +
                        'data-count="' + pendingComment + '" ' +
                        'data-status-id="' + header.statusId + '" ' +
                        'title="Pending Comments">' +
                        pendingComment +
                        '</span>' +


                        ' / ' +



                        '<span class="badge bg-success get_Pen_Sen_Comments count-up" ' +
                        'data-commenttype="4" ' +
                        'data-count="' + sentComment + '" ' +
                        'data-status-id="' + header.statusId + '" ' +
                        'title="Actioned Comments">' +
                        sentComment +
                        '</span>' +


                        '</span>';



                }


                else {



                    /*
                        INBOX RED
                        SENTBOX GREEN
                    */
                    if (!isNamedStatus) {

                        listitem +=


                            '<span class="badge badge-light text-black db-badge-18">' +



                            '<span class="badge bg-danger send count-up" ' +
                            'data-type="1" ' +
                            'data-count="' + peding + '" ' +
                            'data-status-id="' + header.statusId + '" ' +
                            'title="Inbox">' +
                            peding +
                            '</span>' +



                            ' / ' +



                            '<span class="badge bg-success send count-up" ' +
                            'data-type="2" ' +
                            'data-count="' + sent + '" ' +
                            'data-status-id="' + header.statusId + '" ' +
                            'title="SentBox">' +
                            sent +
                            '</span>' +



                            '</span>';

                    }

                }




                /*
                    TOTAL COUNT HIDDEN
                */
                var shototal =


                    header.status.includes("Foreclosed") ||
                    header.status.includes("Obsn");
                 



                if (shototal) {
                    listitem +=


                        '<div class="mb-2">' +


                        '<span class="badge badge-primary mr-2 send db-badge-14 " ' +
                        'data-count="' + tot + '" ' +
                        'data-type="1" title="Total No of transaction at this stage" data-status-id="' + header.statusId + '">' +
                        tot +
                        '</span><div class="t-1 d-none statusprojsummry" > ' + header.status + '</div > ' +
                        

                        '</div>';

                }


                listitem += '</div>';

                listitem += '</div>';

                listitem += '</div>';




                stageId = header.stageId;


            }



            $("#carddashboardcount").html(listitem);



            $('.count-up').each(function() {

                animateCount($(this));

            });


            // ================= APPROVED CLICK EVENT =================


            $(document).on("click", ".ApprovedProj", function() {
                debugger;
                let spnstatusId = $(this).closest("div").find("#spnstatusId").html();

                let spnstatusActionsMappingId = $(this).closest("div").find("#spnstatusActionsMappingId").html();

                tittle =  $(this).closest("div").find(".statusprojsummry").html();
                tittleIPA =  $(this).closest("div").find(".statusprojsummry").html() + " (Parallel Processing)";



                let mappingId =
                    parseInt(spnstatusActionsMappingId);



                if (mappingId === 1)
                    Status = "Accepted";

                else if (mappingId === 9)
                    Status = "obsn Raised";

                else if (mappingId === 113)
                    Status = "Rectified";

                else if (mappingId === 48 || mappingId === 53)
                    Status = "Approved";

                else if (mappingId === 60) {

                    tittle = "Closed Project";
                    Status = "Closed";

                }

                else if ([68, 73, 63, 78, 83, 88].includes(mappingId))
                    Status = "Completed";

                else if ([26, 31, 37].includes(mappingId))
                    Status = "Approved";




                if (mappingId === 0) {


                    Swal.fire({

                        icon: "error",
                        title: "Oops...",
                        text: "Data Not Found!"

                    });


                    return;

                }
                else {
                    $('#CertName').html("Cert&Att");

                    var spnstatusIdInt = parseInt(spnstatusId);
                    var mappingIdInt = parseInt(spnstatusActionsMappingId);

                    // Status: 2, 3, 22
                    if ([2, 3, 22].includes(spnstatusIdInt)) {
                        return;
                    }

                    // BISAG-N
                    else if ([44, 46].includes(spnstatusIdInt)) {
                        $('#ProjectApprovedTittleBisag').html(tittle);
                        $('#BISAG-N').modal('show');

                        getProjBisagN(
                            spnstatusId,
                            spnstatusActionsMappingId,
                            
                        );
                    }

                    // IPA
                    else if (spnstatusIdInt === 21) {
                        $('.IPAProjectApprovedTittle').html(tittle);
                        $('#IPAProjApproved').modal('show');

                        getProjApproved(
                            spnstatusId,
                            spnstatusActionsMappingId,1
                        );
                    }

                    // Process Approved
                    else if ([1, 55, 60].includes(spnstatusIdInt)) {
                        $('.IPAProjectApprovedTittle').html(tittle);

                        $('#ProcessApproved').modal('show');

                        if (spnstatusIdInt === 60) {
                            $('.btnaimltoggle').removeClass('d-none');
                        }

                        getProjApproved(
                            spnstatusId,
                            spnstatusActionsMappingId,
                            1
                        );
                    }

                    // Project Approved with IPA mapping
                    else if ([26, 31, 37].includes(mappingIdInt)) {
                        $('#ProjectApprovedTittle').html(tittleIPA);
                        $('#ProjApproved').modal('show');

                        $('.timestampheader').html('Approved Date');

                        getProjApproved(
                            spnstatusId,
                            spnstatusActionsMappingId,
                            1
                        );
                    }

                    // Certificate / Normal Project
                    else {
                        const hascert = [24, 25, 26, 27, 28, 29].includes(spnstatusIdInt);

                        if (hascert) {
                            $('.IPAProjectApprovedTittle').html(tittle);
                            $('#IPAProjApproved').modal('show');
                        } else {
                            $('#ProjectApprovedTittle').html(tittle);
                            $('#ProjApproved').modal('show');

                            $('.timestampheader').html('Approved Date');
                        }

                        getProjApproved(
                            spnstatusId,
                            spnstatusActionsMappingId,
                            1
                        );
                    }
                }
                });






            // ================= INBOX / SENTBOX CLICK =================


            $(document)
                .off("click", ".send")
                .on("click", ".send", function() {

                    debugger;

                    var statusId =
                        parseInt($(this).data("status-id"));



                    var listType =
                        parseInt($(this).data("type"));



                    var total =
                        parseInt($(this).data("count")) || 0;



                    if (!statusId ||
                        !listType ||
                        total === 0) {


                        return;

                    }





                    if (statusId !== 1041 &&
                        statusId !== 44 &&
                        statusId !== 46) {



                        var status =
                            $(this)
                                .closest("div")
                                .find(".statusprojsummry")
                                .html();




                        $("#ProjectSummaryTittle")
                            .html(status);



                        $("#IsNotduplicate")
                            .prop("checked", false);




                        if ([2, 3, 37, 22, 31].includes(statusId)) {


                            $("#ProjGetsummayForClosed_Obsn")
                                .modal("show");


                        }

                        else {


                            $("#ProjGetsummay")
                                .modal("show");


                        }




                        getProjGetsummay(
                            statusId,
                            listType
                        );


                    }


                });





            // ================= COMMENTS CLICK =================


            $(document)
                .off("click", ".get_Pen_Sen_Comments")
                .on("click", ".get_Pen_Sen_Comments", function() {



                    var statusId =
                        parseInt($(this).data("status-id"));



                    var commentType =
                        parseInt($(this).data("commenttype"));



                    var total =
                        parseInt($(this).data("count")) || 0;




                    if (!statusId ||
                        !commentType ||
                        total === 0) {


                        return;

                    }




                    var status =
                        $(this)
                            .closest("div")
                            .find(".statusprojsummry")
                            .html();




                    $("#ProjectSummaryTittle")
                        .html(status);



                    $("#ProjGetsummay")
                        .modal("show");




                    getProjGetsummay(
                        statusId,
                        commentType
                    );



                });



        },

        error: function() {


            alert('Error fetching Count.');


        }

    });

}



    function Getbarchart() {
        $.ajax({
            url: '/Home/indexToBarChartS',
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                if (data.error) {
                    console.error('Error fetching data:', data.error);
                    return;
                }

                let monthNames = [...new Set(data.map(item => item.MonthNameYr))];
                let unitNames = [...new Set(data.map(item => item.unitname))];

                let datasets = unitNames.map(unitName => {
                    let totalInData = [];
                    let totalOutData = [];

                    monthNames.forEach(month => {
                        let monthData = data.find(item => item.MonthNameYr === month && item.unitname === unitName);
                        if (monthData) {
                            totalInData.push(monthData.TotalIn);
                            totalOutData.push(monthData.TotalOut);
                        } else {
                            totalInData.push(0);
                            totalOutData.push(0);
                        }
                    });

                    let totalInColor = getRandomColor();
                    let totalOutColor = getRandomColor();

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

                const canvas = document.getElementById("myChart");

                if (!canvas) {
                    console.warn("Canvas #myChart not found");
                    return;
                }

                const ctx = canvas.getContext("2d");
                let myChart = new Chart(ctx, {
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
                $('#chartTitle').text('Bar Chart Title');
            }
        });
       
    }
   



function getProjApproved(spnstatusId, spnstatusActionsMappingId, IsAIML) {

    let listItem = "";
    var table = new DataTable('#dashboardApproved');
    table.destroy();
    var table = new DataTable('#IPAdashboardApproved');
    table.destroy();
    var table = new DataTable('#dashboardApprovedSTC');
    table.destroy();



    $.ajax({
        url: '/Home/GetDashboardApproved',
        contentType: 'application/x-www-form-urlencoded',
        data: {
            StatusId: encryptData(spnstatusId),
            statusActionsMappingId: encryptData(spnstatusActionsMappingId),
            projecttype: encryptData(IsAIML)
        },
        type: 'POST',
        success: function(response) {
            debugger;
            if (response != "null" && response != null) {

                const hasIPA = Array.isArray(response) && response.some(item => [53, 63, 68, 73, 78, 83, 88].includes(item.statusactionMappingid));
                var hasProccesedWithSTC = Array.isArray(response) && response.some(item => [21, 60, 159].includes(item.statusactionMappingid));
                if (response == -1) {
                    Swal.fire({ text: "" });
                } else if (response == 0) {

                    $('#DetailBodyApproved').empty();
                    $('#DetailBodyApprovedWithSTCCheck').empty();

                    $('#IPADetailBodyApproved').empty();
                    listItem += "<tr><td class='text-center' colspan='7'>No Record Found</td></tr>";


                    $("#DetailBodyApproved").html(listItem);
                    $("#IPADetailBodyApproved").html(listItem);
                    $("#DetailBodyApprovedWithSTCCheck").html(listItem);
                    $("#lblTotal").html(0);

                } else {
                    let count = 1;

                    const unitId = $('#spndashboardUnitId').text().trim();

                    $('#DetailBodyApproved').empty();
                    $('#dashboardApproved').dataTable().fnClearTable();
                    $('#dashboardApproved').dataTable().fnDestroy();

                    for (let i = 0; i < response.length; i++) {

                        const projName = response[i].projName;
                        const words = projName.split(" ");
                        const shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;

                        listItem += "<tr>";
                        listItem += "<td class='align-middle dp-w-3'>" + count + "</td>";
                        listItem += "<td class='align-middle dp-w-3'>" + response[i].projId + "</td>";
                        if (unitId == 1 || unitId == 2 || unitId == 3 || unitId == 4 || unitId == 5 || unitId == 7) {
                            listItem += "<td class='align-middle nowrap'>" +
                                "<a class='ProjName' title='" + projName + "' data-proj-id='" + response[i].projId + "' data-proj-name='" + shortProjName + "' " +
                                "href='/Projects/ProjHistory?EncyID=" + response[i].encyID + "&Type=XRDC'>" +
                                shortProjName + "</a><span class='d-none'>'" + projName + "'</span></td>";
                        } else {
                            listItem += "<td class='align-middle nowrap'><span id='ProjName' title='" + projName + "'>" + shortProjName + "</span></td>";
                        }

                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stakeHolder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + DateFormateddMMyyyyhhmmss(response[i].timeStamp) + "</span></td>";
                        // listItem += "<td ><span class='badge badge-success' id='divName'>" + Status + "</span></td>";

                        if (response[i].statusactionMappingid == 53) {
                            if (response[i].approvedDt != null && response[i].approvedRemarks != null) {
                                const visClass = (response[i].statusactionMappingid == 53) ? "dp-vis-visible" : "dp-vis-hidden";

                                listItem += `
<td class="text-center noExport">
  <button 
      class="badge badge-success generateCertificate ${visClass}"
      data-project="${encodeURIComponent(response[i].projName)}"
      data-remarks="${encodeURIComponent(response[i].approvedRemarks)}"
      data-date="${encodeURIComponent(response[i].approvedDt)}">
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
                                listItem += `<td>No Attachemnts</td>`;
                            }


                        } else if (response[i].statusactionMappingid == 21 || response[i].statusactionMappingid == 60 || response[i].statusactionMappingid == 159) {
                            if (unitId == 1 || unitId == 2) {
                                listItem += `
<td class="text-center noExport">   
    <div class="toggle-container d-flex">
        <span class="toggle-label text-danger mr-2">NO</span>

        <div class="form-check form-switch">
            <input class="form-check-input toggleSwitch"
                   data-projid="${response[i].encyID}"
                   type="checkbox"
                   ${response[i].fieldInSTC ? 'checked' : ''}>
            <label class="form-check-label"></label>
        </div>

        <span class="toggle-label text-success">YES</span>
    </div>
</td>`;
                            }
                            else {
                                listItem += `
<td class="text-center noExport">
    <div class="toggle-container d-flex">
        <span class="toggle-label text-danger mr-2">NO</span>

        <div class="form-check form-switch">
            <input class="form-check-input"
                   data-projid="${response[i].encyID}"
                   type="checkbox"
                   ${response[i].fieldInSTC ? 'checked' : ''} disabled>
            <label class="form-check-label"></label>
        </div>

        <span class="toggle-label text-success">YES</span>
    </div>
</td>`;
                            }

                        }

                        listItem += "</tr>";
                        count++;
                    }

                    if (hasIPA) {
                        refreshDataTable('#IPAdashboardApproved');
                        $("#IPADetailBodyApproved").html(listItem);

                        initializeDataTable('#IPAdashboardApproved')

                    }

                    else if (hasProccesedWithSTC) {

                        $("#DetailBodyApprovedWithSTCCheck").html(listItem);
                        initializeDataTable('#dashboardApprovedSTC');
                    }
                    else {

                        refreshDataTable('#dashboardApproved');
                        $("#DetailBodyApproved").html(listItem);

                        initializeDataTable('#dashboardApproved')

                    }
                }
            }
            else {

                $('#DetailBodyApproved').empty();
                listItem += "<tr><td class='text-center' colspan='7'>No Record Found</td></tr>";
                $("#DetailBodyApproved").html(listItem);

            }
        },
        error: function(result) {
            Swal.fire({ text: "" });
        }
    });
}
document.addEventListener("click", function (e) {
    if (!e.target.classList.contains("generateCertificate")) return;

    const btn = e.target;

    const url = `/Home/Generate?ProjectName=${btn.dataset.project}` +
        `&ApprovedRemarks=${btn.dataset.remarks}` +
        `&ApprovedDt=${btn.dataset.date}`;

    window.open(url, "_blank");
});

function getProjGetsummay(spnstatusId, IsDuplicate) {

    let listItem = "";

    $("#spndashboardstatusId").html(spnstatusId);
    let userdata = {
        "StatusId": spnstatusId,
        "IsDuplicate": IsDuplicate
    };

    let encrypted_payload = encryptData(userdata);

    $.ajax({
        url: '/Home/GetDashboardStatusDetails',
        contentType: 'application/x-www-form-urlencoded',
        data: { encrypted_payload: encrypted_payload },
        type: 'POST',
        success: function(response) {

            debugger;
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({ text: "" });
                } else if (response == 0) {

                    $('#DetailBodysummary1').empty();
                    listItem += "<tr><td class='text-center' colspan='10'>No Record Found</td></tr>";
                    $("#DetailBodysummary1").html(listItem);
                    $("#lblTotal").html(0);

                    $('#DetailBodysummary2').empty();
                    listItem += "<tr><td class='text-center' colspan='8'>No Record Found</td></tr>";
                    $("#DetailBodysummary2").html(listItem);
                    $("#lblTotal").html(0);


                } else {
                    let count = 1;
                    $('#dashboardDeatils').dataTable().fnClearTable();
                    $('#dashboardDeatils').dataTable().fnDestroy();
                    $('#dashboardDeatilsforClosed').dataTable().fnClearTable();
                    $('#dashboardDeatilsforClosed').dataTable().fnDestroy();

                    let unitId = $('#spndashboardUnitId').text().trim();
                    for (let i = 0; i < response.length; i++) {

                        let projName = response[i].projName;
                        let words = projName.split(" ");
                        let shortProjName = words.length > 6 ? words.slice(0, 6).join(" ") + "..." : projName;

                        listItem += "<tr>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + count + "</span></td>";
                        if (unitId == 1 || unitId == 2 || unitId == 3 || unitId == 4 || unitId == 5 || unitId == 7) {
                            listItem += "<td class='align-middle'>" +
                                "<a class='ProjName' title='" + projName + "' data-proj-id='" + response[i].projId + "' data-proj-name='" + projName + "' " +
                                "href='/Projects/ProjHistory?EncyID=" + response[i].encyID + "&Type=XRDC'>" +
                                shortProjName + "</a><span class='d-none'>" + projName + "</span></td>";
                        }
                        else {
                            listItem += "<td class='align-middle'><span id='ProjName' title='" + projName + "'>" + shortProjName + "</span></td>";
                        }
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stakeHolder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].fromUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].toUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stage + "</span></td>";
                        if (![2, 3, 37, 22, 31].includes(parseInt(spnstatusId))) {

                            listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].status + "</span></td>";
                            listItem += "<td class='align-middle'><span id='divName'>" + response[i].action + "</span></td>";
                        }
                        if (parseInt(spnstatusId) === 1) {

                            listItem += "<td class='align-middle'><span id='divName'>" + DateFormateddMMyyyyhhmmss(response[i].initiatedDate) + "</span></td>";
                        } else {

                            listItem += "<td class='align-middle'><span id='divName'>" + DateFormateddMMyyyyhhmmss(response[i].dateTimeOfUpdate) + "</span></td>";
                        }

                        if ([2, 3, 37, 22, 31].includes(parseInt(spnstatusId))) {



                            listItem += `<td class="align-middle"><div id="divName" class="RefLetter-container" data-tooltip="${response[i].remarks}">
                                <span class="short-text">${trimByChars(response[i].remarks, 20)}</span>
                                <span class="RefLetter">${response[i].remarks}</span>
                            </div ></td>`;
                        }

                        else {
                            if (response[i].isComplete) {

                                if (response[i].stkStatusId == 2) {
                                    listItem += "<td ><span class='badge badge-warning' id='divName'>Obsn</span></td>";
                                } else if (response[i].stkStatusId == 3) {
                                    listItem += "<td ><span class='badge badge-danger' id='divName'>Rejected</span></td>";
                                }
                                else if (response[i].stkStatusId == 5) {
                                    listItem += "<td ><span class='badge badge-success' id='divName'>Info</span></td>";
                                }
                                else if (response[i].stkStatusId == 6) {
                                    listItem +=
                                        "<td>" +
                                        "<span class='badge bg-secondary' id='divName'>" +
                                        "Not Applicable" +
                                        "</span>" +
                                        "</td>";
                                }
                                else {
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
                                }
                                else if (response[i].stkStatusId == 6) {
                                    listItem +=
                                        "<td >" +
                                        "<span class='badge bg-secondary' id='divName'>" +
                                        "Not Applicable" +
                                        "</span>" +
                                        "</td>";
                                }
                                else {
                                    //listItem += "<td ><span class='badge badge-danger' id='divName'>Pending</span></td>";
                                    listItem += `<td ><span class='badge badge-danger' id='divName' title='Pending with ${response[i].toUnitName}'>${response[i].toUnitName}</span></td>`;
                                }
                            }
                        }



                        listItem += "</tr>";
                        count++;
                    }
                    if ([2, 3, 37, 22, 31].includes(parseInt(spnstatusId))) {

                        $("#DetailBodysummary2").html(listItem);
                        initializeDataTable('#dashboardDeatilsforClosed');
                    } else {

                        $("#DetailBodysummary1").html(listItem);
                        initializeDataTable('#dashboardDeatils');
                    }


                }
            }
            else {

                $('#DetailBodysummary1').empty();
                listItem += "<tr><td class='text-center' colspan='10'>No Record Found</td></tr>";
                $("#DetailBodysummary1").html(listItem);
                $('#DetailBodysummary2').empty();
                listItem += "<tr><td class='text-center' colspan='8'>No Record Found</td></tr>";
                $("#DetailBodysummary2").html(listItem);



            }
        },
        error: function(result) {
            Swal.fire({ text: "" });
        }
    });
}


function updatePieChart(data) {
    let titles = data.map(item => item.Status);
    let chartData = data.map(item => item.TotalProj);

  
    const canvas = document.getElementById("myChart1");

    if (!canvas) {
        console.warn("Canvas #myChart1 not found");
        return;
    }

   

    let backgroundColors = generateRandomColors(titles.length);

    let ctx = canvas.getContext('2d');

    let myChart1 = new Chart(ctx, {
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

    let todaysDate = new Date();
    let datef1 = new Date(date);
    let datef2 = new Date(date);
    let months = "" + `${(datef2.getMonth() + 1)}`;
    let days = "" + `${(datef2.getDate())}`;
    let pad = "00"
    let monthsans = pad.substring(0, pad.length - months.length) + months
    let dayans = pad.substring(0, pad.length - days.length) + days
    let year = `${datef2.getFullYear()}`;
    let hh = `${datef2.getHours()}`;
    let mm = `${datef2.getMinutes()}`;
    let ss = `${datef2.getSeconds()}`;
    if (hh < 10) hh = "0" + hh;
    if (mm < 10) mm = "0" + mm;
    if (ss < 10) ss = "0" + ss;
    if (year > 1902) {

        let datemmddyyyy = dayans + `/` + monthsans + `/` + year + ` ` + hh + `:` + mm + `:` + ss
        return datemmddyyyy;
    }
    else {
        return '';
    }
}


function getProjBisagN(spnstatusId, spnstatusActionsMappingId) {
    let listItem = "";
    let table = $('#dashboardApprovedBisagN').DataTable(); // Initialize DataTable
    table.clear().destroy(); // Destroy the existing table instance

    let userdata = {
        "StatusId": spnstatusId,
        "statusActionsMappingId": spnstatusActionsMappingId,
        "projecttype":1
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
                    let count = 1;

                    $('#DetailBodyBisagN').empty();
                    for (let i = 0; i < response.length; i++) {
                        listItem += "<tr>";
                        listItem += "<td class='align-middle'>" + count + "</td>";
                        listItem += "<td class='align-middle nowrap'><span id='ProjName'>" + response[i].projName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + response[i].stakeHolder + "</span></td>";
                        listItem += "<td class='align-middle'><span id='ProjName'>" + DateFormateddMMyyyyhhmmss(response[i].timeStamp) + "</span></td>";
                        listItem += "</tr>";
                        count++;
                    }

                    $("#DetailBodyBisagN").html(listItem);
                    initializeDataTable('#dashboardApprovedBisagN');
                 
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
            const ApprovedProjects = data.approvedProjectsPre;
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
            const ApprovedProjectsPost = data.approvedProjectsPost;
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
            const WhitelistedProjects = data.whitelistedProjects;
            const colorspie = [
                "green", "red"]
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
            const TotalProjects = data.totalProjects;

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
document.getElementById('popupOverlay').addEventListener('click', function (event) {
    if (event.target === this) {
        closePopup();
    }
});
document.addEventListener('keydown', function (event) {
    if (event.key === 'Escape') {
        closePopup();
    }
});

$(document).ready(function () {

    let TeamDetailPostBackURL = '/Projects/AttDetails';
    $(document).on("click", ".anchorDetail", function (e) {
      
        e.preventDefault(); // prevent default anchor behavior
       
        
        let id = $(this).data('id'); // better than attr()

        $.ajax({
            type: "GET",
            url: TeamDetailPostBackURL,
            data: { Id: id }, // no quotes needed
            success: function (response) {
                
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
    $(document).on('click', '.pdf', function () {
        $('#ViewRecordsHistory').modal('show');
    });

});

function showPopup(segmentIndex) {

    const popupOverlay = document.getElementById('popupOverlay');
    const popupTitle = document.getElementById('popupTitle');
    const projectList = document.getElementById('projectList');

    let projects = [];
    let title = '';
    let statusActionsMappingId = 0;
    if (segmentIndex === 0) {
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

$(document).on('change', '.toggleSwitch', function () {

    var checkbox = $(this);
    var projId = checkbox.data('projid');

    var isChecked = checkbox.is(':checked');

    $.ajax({
        url: '/Projects/UpdateFieldInSTC',
        type: 'POST',
        data: {
            projId: projId,
            fieldInSTC: isChecked
        },
        success: function (response) {

            if (response.success) {

                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: response.message,
                    timer: 1500,
                    showConfirmButton: false
                });
                GetAllDashbaordCount();

            } else {

                checkbox.prop('checked', !isChecked);

                Swal.fire({
                    icon: 'error',
                    title: 'Update Failed',
                    text: response.message
                });
            }
        },
        error: function () {

            checkbox.prop('checked', !isChecked);

            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Something went wrong. Please try again.'
            });
        }
    });

});
// NOTE: now that the toggle re-fetches from the server via
// getProjApproved(), the client-side filtering below (stcApprovedProjects /
// renderApprovedSTCTable) is no longer on the toggle's code path — your
// existing getProjApproved success handler is what re-renders the table
// rows each time. Kept here only if you still want an initial render
// helper; otherwise you can delete loadApprovedSTCTable/renderApprovedSTCTable.
var stcApprovedProjects = [];
var stcShowingAIML = false; // default view: regular (non-AI/ML) projects

function setToggleButtonState(showingAIML) {
    var $btn = $('#btnToggleAIML');
    var $label = $btn.find('.btn-aiml-toggle-label');
    var $icon = $btn.find('i');

    $btn.attr('data-mode', showingAIML ? 'aiml' : 'regular');
    $btn.attr('aria-pressed', showingAIML ? 'true' : 'false');

    if (showingAIML) {
        $label.text('Show Regular Projects');
        $icon.removeClass('fa-microchip').addClass('fa-list');
        $('#lblSTCTableMode').text('Showing: AI/ML Projects');
    } else {
        $label.text('Show AI/ML Projects');
        $icon.removeClass('fa-list').addClass('fa-microchip');
        $('#lblSTCTableMode').text('Showing:Auto Projects');

    }
}

//$(document).on('click', '#btnToggleAIML', function () {
//    stcShowingAIML = !stcShowingAIML;
//    setToggleButtonState(stcShowingAIML);

//    // Re-fetch from the server with the flag flipped each click:
//    // 1st click -> true (AI/ML only), 2nd click -> false (regular), etc.
//    // Adjust statusId/statusActionsMappingId to whatever this modal's
//    // context actually needs.
//    getProjApproved(60, 1, stcShowingAIML);
//});

$('#ProcessApproved').on('hidden.bs.modal', function () {
    /*   setToggleButtonState(false);*/
    $('.btnaimltoggle').addClass('d-none');
});

$(document).on("change", "#ddlProjectdashboardType", function () {
    let projectSearchTypeId =
        Number($("#ddlProjectdashboardType").val()) || 1;

    getProjApproved(60, 1, projectSearchTypeId);
});