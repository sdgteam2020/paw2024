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
            if (data != null) {
                for (var i = 0; i < dtoDashboardHeaderlst.length; i++) {
                    if (stageId != dtoDashboardHeaderlst[i].stageId) {
                        if (stageId != 0) {
                            listitem += '</div>';
                        }
                        listitem += '<div class="newprojectheading text-center">' + dtoDashboardHeaderlst[i].stages + ' </div>';
                        listitem += '<div class="r-1">';
                    }

                    listitem += '<div class="cd-1" onclick="" class="cd-1-container">';
                    listitem += '<div class="icon-container">';
                    listitem += '<img src="/assets/images/icons/adduser.png" alt="Icon" class="icon-img">';
                    listitem += '</div>';
                    tot = 0;
                    var DTODashboardCount = data.dtoDashboardCountlst.filter(function (element) { return element.stagesId == dtoDashboardHeaderlst[i].stageId && element.statusId == dtoDashboardHeaderlst[i].statusId; });
                    for (var j = 0; j < DTODashboardCount.length; j++) {
                        tot = DTODashboardCount[j].tot;
                    }
                    listitem += '<h5 class="tot-count">' + tot + ' </h5>';
                    listitem += '<div class="t-1">' + dtoDashboardHeaderlst[i].status + '</div> ';
                    listitem += '<div class="mb-2">';

                    var DTODashboardActionlst = data.dtoDashboardActionlst.filter(function (element) { return element.statusId == dtoDashboardHeaderlst[i].statusId });

                    for (var j = 0; j < DTODashboardActionlst.length; j++) {
                        var tot1 = 0;
                        var DTODashboardCount1 = data.dtoDashboardCountlst.filter(function (element) { return element.stagesId == dtoDashboardHeaderlst[i].stageId && element.statusId == dtoDashboardHeaderlst[i].statusId && element.actionId == DTODashboardActionlst[j].actionId; });
                        for (var x = 0; x < DTODashboardCount1.length; x++) {
                            tot1 = DTODashboardCount1[x].tot;
                        }
                        listitem += '<div class="badge badge-primary mr-2 ml-2 mb-2">';
                        listitem += '<span class="badge action-badge">' + DTODashboardActionlst[j].action + '</span> ';
                        listitem += '<span class="badge badge-light text-black action-count-badge">' + tot1 + '</span>';
                        listitem += '  </div>';
                    }
                    listitem += '</div>';
                    listitem += '</div>';
                    stageId = dtoDashboardHeaderlst[i].stageId;
                }

                $("#carddashboardcount").html(listitem);

            }

        },
        error: function () {
            alert('Error fetching comments.');
        }
    });
}
