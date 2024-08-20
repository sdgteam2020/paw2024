$(document).ready(function () {

    GetProjectMovement();

 
    $("#btnFwdNext").click(function () {
        requiredFields = $('#ProjFwd').find('.requiredField');
        var allFieldsComplete = true;
        requiredFields.each(function (index) {
            if (this.value.length == 0) {
                $(this).addClass('is-invalid');
                allFieldsComplete = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        if (allFieldsComplete) {
           
            CheckFwdCondition($("#spanEditPslmId").html());
           
        }
    });

    function CheckFwdCondition(CurrentPslmId) {
       
        var userdata =
        {
            "ProjId": $("#spanProjectId").html(),
            "StatusId": $("#ddlfwdSubStage").val(),
        };

        $.ajax({
            url: '/Projects/CheckFwdCondition',
            type: 'POST',
            data: userdata,
            success: function (response) {
                console.log(response);
                if (response != null) {

                    if (response == true) {
                        Swal.fire({
                            icon: "error",
                            title: "Oops...",
                            text: "Sub Stage Allready Approved / Completed!",

                        });
                    }
                    else if (response == false) {

                        AttechHistory();
                        SaveFwdTo(spanEditPslmId);
                    }
                }

            }
        });


    }

    function AttechHistory() {
        var listItem = "";
        var userdata =
        {
            "PslmId": $("#spanEditPslmId").html(),

        };
        $.ajax({
            url: '/Projects/GetAtthHistoryByProjectId',
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

                        listItem += "<tr><td class='text-center' colspan=5>No Record Found</td></tr>";

                        $("#DetailBody").html(listItem);
                        $("#lblTotal").html(0);
                    }

                    else {


                        // { attId: 8, psmId: 8, attPath: 'Swas_22ed1265-b2a0-4008-b7ff-b3eb5f704849.pdf', actionId: 0, timeStamp: '2024-05-02T16:17:45.6016413', … }
                        for (var i = 0; i < response.length; i++) {

                            listItem += "<tr>";
                            listItem += "<td class='d-none'><span id='spnattId'>" + response[i].attId + "</span><span id='spnpsmId'>" + response[i].psmId + "</span> </td>";
                            listItem += "<td class='align-middle'><span id='btnedit'><button type='button' class='cls-btnDelete btn-icon btn-round btn-danger mr-1'><i class='fas fa-trash-alt'></i></button></td>";
                            listItem += "<td class='align-middle'><span id='comdName'>" + response[i].reamarks + "</span></td>";
                            listItem += "<td class='align-middle'><span id='corpsName'><a class='link-success' target='_blank' href=/uploads/" + response[i].attPath + ">" + response[i].actFileName + "</a></span></td>";
                            listItem += "<td class='align-middle'><span id='divName'>" + response[i].timeStamp + "</span></td>";



                            /*    listItem += "<td class='nowrap'><button type='button' class='cls-btnSend btn btn-outline-success mr-1'>Send To Verification</button></td>";*/
                            listItem += "</tr>";
                        }

                        $("#DetailBody").html(listItem);
                        $("#lblTotal").html(response.length);



                        var rows;





                        $("body").on("click", ".cls-btnDelete", function () {

                            Swal.fire({
                                title: 'Are you sure?',
                                text: "You want to Delete ",
                                icon: 'warning',
                                showCancelButton: true,
                                confirmButtonColor: '#072697',
                                cancelButtonColor: '#d33',
                                confirmButtonText: 'Yes, Delete It!'
                            }).then((result) => {
                                if (result.value) {

                                    Deleteattechment($(this).closest("tr").find("#spnattId").html());

                                }
                            });
                        });


                    }
                }
                else {
                    listItem += "<tr><td class='text-center' colspan=7>No Record Found</td></tr>";
                    $("#SoftwareTypes").DataTable().destroy();
                    $("#DetailBody").html(listItem);
                    $("#lblTotal").html(0);
                }
            },
            error: function (result) {
                Swal.fire({
                    text: ""
                });
            }
        });
    }

    //$('#UpdateProjectMovement').click(function () {
    //    alert("Hihui");
    //    var formData = new FormData();
        
    //    formData.append("StageId", $("#ddlfwdStage").val());
    //    formData.append("StatusId", $("#ddlfwdSubStage").val());
    //    formData.append("ProjectId", $("#ddlfwdAction").html());
    //    formData.append("psmid", $("#ddlfwdFwdTo").html());
    //    formData.append("CommentDate", $("#txtRemarksfwd").val());
    //    formData.append("", $("#TimeStampToProjfwd").val());
    //    $.ajax({
    //        url: '/Projects/UpdateProjectMovement',
    //        type: 'Post',
    //        data: {

    //        },
    //        success: function (response) {

    //        }
    //    })
    //})

    $("#btnAttchMultiforpsmid").click(function () {

        alert("Amit3");
        requiredFields = $('#ProjFwd').find('.requiredFieldAttch');
        var allFieldsComplete = true;
        requiredFields.each(function (index) {
            if (this.value.length == 0) {
                $(this).addClass('is-invalid');
                allFieldsComplete = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        if (allFieldsComplete) {
            Swal.fire({
                title: "Are you sure?",
                text: "Do you Want Upload Pdf File",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes, Upload it!"
            }).then((result) => {
                if (result.isConfirmed) {
                    UploadFiles();
                }
            });
        }
    });

    $("#btnFwdConfirm").click(function () {
        location.reload();
        $('#ProjFwd').modal('hide');
    });

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
                        listItem += "<span id='spanProjId' class='d-none'>" + response[i].projId + "</span>";
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
                        $("#spanProjectId").html($(this).closest("tr").find("#spanProjId").html());
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
