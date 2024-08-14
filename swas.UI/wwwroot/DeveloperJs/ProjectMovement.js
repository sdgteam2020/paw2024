$(document).ready(function () {

    GetProjectMovement();
});

function GetProjectMovement()
{

        var listItem = "";
     
    $.ajax({
        url: '/Projects/GetProjectMov',
        type: 'Post',
        data: {
            "Id": 0
        },
        success: function (response) {
           
            console.log(response);
            if (response != "null" && response != null) {

                if (response == -1) {
                    Swal.fire({
                        text: ""
                    });
                }
                else if (response == 0) {

                    listItem += "<tr><td class='text-center' colspan=5>No Record Found</td></tr>";

                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(0);
                }

                else {
                    var count = 1;
                    for (var i = 0; i < response.length; i++) {

                        listItem += "<tr>";
                        listItem += "<td class='d-none'><span id='spnpsmId' class='d-none'>" + response[i].psmIds + "</span>";
                        listItem += "<span id='spnStageId' class='d-none'>" + response[i].stageId + "</span>";
                        listItem += "<span id='spnStatusId' class='d-none'>" + response[i].statusId + "</span>";
                        listItem += "<span id='spnActionId' class='d-none'>" + response[i].actionId + "</span>";
                        listItem += "<span id='spnToUnitId' class='d-none'>" + response[i].toUnitId + "</span>";
                       
                        listItem += "</td>";
                        listItem += "<td>" + count +"</td>";
                        listItem += "<td class=''><span id='Date'>" + response[i].dateTimeOfUpdate + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].fromUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].toUnitName + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].stage + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].status + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'>" + response[i].action + "</span></td>";
                        listItem += "<td class='align-middle'><span id='spnremarks'>" + response[i].remarks + "</span></td>";
                        listItem += "<td class='align-middle'><span id='FromUnitName'><span class='btn btn-primary cls-btnedit'>Edit</span></td>";


                        listItem += "</tr>";
                        count++;
                    }

                    $("#ProjectMovement").html(listItem);


                    $("body").unbind().on("click", ".cls-btnedit", function () {
                        $('#ProjFwdEdit').modal('show');
                        $("#spanEditPslmId").html($(this).closest("tr").find("#spnpsmId").html());
                        $("#txtRemarksfwd").val($(this).closest("tr").find("#spnremarks").html());
                        mMsaterfwdStage($(this).closest("tr").find("#spnStageId").html(), "ddlfwdStage", 5, 0, 1)
                        mMsaterStage($(this).closest("tr").find("#spnStatusId").html(), "ddlfwdSubStage", 6, $(this).closest("tr").find("#spnStageId").html(), 0)
                        mMsater($(this).closest("tr").find("#spnActionId").html(), "ddlfwdAction", 7, $(this).closest("tr").find("#spnStatusId").html())
                        mMsaterFwdTo($(this).closest("tr").find("#spnToUnitId").html(), "ddlfwdFwdTo", 8, 0, $("#SpnFwdStakeHolderId").html(), $(this).closest("tr").find("#spnToUnitId").html());

                    });

                }

            }

        }

    }
    );
    
}
