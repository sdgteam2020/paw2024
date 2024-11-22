$(document).ready(function () {

    mMsaterfwdStage(0, "ddlfwdStage", 5, 0)

    $("#ddlfwdStage").change(function () {

        mMsaterStage(0, "ddlfwdSubStage", 6, $("#ddlfwdStage").val(), $("#SpnStakeHolderId").html())
    });


    $("#ddlfwdSubStage").change(function () {

        mMsater(0, "ddlfwdAction", 7, $("#ddlfwdSubStage").val())
    });


    $("#ddlfwdAction").change(function () {

        mMsaterFwdTo(0, "ddlfwdFwdTo", 8, 0, $("#SpnFwdStakeHolderId").html());
    });

    //$("#ddlfwdSubStage").change(function () {

    //    mMsater(0, "ddlfwdAction", 9, $("#ddlfwdSubStage").val())
    //});

    //$("#ddlfwdAction").change(function () {

    //    mMsaterFwdTo(0, "ddlfwdFwdTo", 8, 0, $("#SpnStakeHolderId").html())
    //});
    $(".btn-Undo").click(function () {

        IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());
        Swal.fire({
            title: "Enter Undo Remarks",
            input: "text",
            inputAttributes: {
                autocapitalize: "off"
            },
            showCancelButton: true,
            confirmButtonText: "Undo",
            showLoaderOnConfirm: true,
            preConfirm: async (login) => {
                if (login == "") {
                    Swal.showValidationMessage(`
               Please Enter Remarks: `);
                }
            },
            allowOutsideClick: () => !Swal.isLoading()
        }).then((result) => {
            if (result.isConfirmed) {

                Updateundo($(this).closest("tr").find("#SpnCurrentProjId").html(), $(this).closest("tr").find("#SpnCurrentpsmId").html(), result.value, $(this).closest("tr").find("#SpnprojectStageId").html());
                UndoNotification($(this).closest("tr").find("#SpnCurrentProjId").html(), 2, $(this).closest("tr").find("#SpnprojectToUnitId").html());
            }
        });
    });
    $(".btn-FwdHistory").click(function () {
        $('#ProjFwdHistory').modal('show');


        GetProjectMovHistory($(this).closest("tr").find("#SpnCurrentProjId").html());
        IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());

        IsReadNotification($(this).closest("tr").find("#SpnCurrentProjId").html(), 2);

        $(this).closest("tr").removeClass("bold-text")


    });
    $(".btn-Fwd").click(function () {

        var projName = $(this).data('proj-name') + "  " + "Fwd";
        var projNameDetail = $(this).data('proj-name') + " " + "Move Details";
        $('#fwdModal').text(projName);
        $('.fwdtitle').text(projNameDetail);


        mMsaterfwdStage($(this).closest("tr").find("#SpnStageId").html(), "ddlfwdStage", 5, 0, 1)
        mMsaterStage($(this).closest("tr").find("#SpnTimeStatusId").html(), "ddlfwdSubStage", 6, $(this).closest("tr").find("#SpnStageId").html(), $("#SpnStakeHolderId").html())
        /* mMsater(0, "ddlfwdAction", 9, $("#ddlfwdSubStage").val())*/

        mMsater($(this).closest("tr").find("#SpnTimeActionId").html(), "ddlfwdAction", 7, $(this).closest("tr").find("#SpnTimeStatusId").html())
        /*mMsater($(this).closest("tr").find("#SpnTimeActionId").html(), "ddlfwdAction", 9, $(this).closest("tr").find("#SpnTimeStatusId").html())*/
        mMsaterFwdTo($(this).closest("tr").find("#SpnTimeToUnitId").html(), "ddlfwdFwdTo", 8, 0, $(this).closest("tr").find("#SpnStakeHolderId").html());


        $("#spanFwdCurrentPslmId").html($(this).closest("tr").find("#SpnCurrentpsmId").html())
        $("#spanFwdProjectId").html($(this).closest("tr").find("#SpnCurrentProjId").html())
        $("#SpnFwdStakeHolderId").html($(this).closest("tr").find("#SpnStakeHolderId").html())

        IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());
        IsReadNotification($(this).closest("tr").find("#SpnCurrentProjId").html(), 2);

        $(this).closest("tr").removeClass("bold-text")

        $('#ProjFwd').modal('show');

        $(".Fwdtitle").html("Projects Move Details");
        $(".ProjectsFwd").removeClass("d-none");
        $(".Attmenthistory").addClass("d-none");



        // alert($(this).closest("tr").find("#SpnprojectStageId").html())
        //GetAllComments($(this).closest("tr").find("#SpnCurrentProjId").html());
    });
    $(".btn-Obsn").click(function () {

        var projName = $(this).data('proj-name') + "  " + "Fwd";
        var projNameDetail = $(this).data('proj-name') + " " + "Move Details";

        $('#fwdModal').text(projName);
        $('.fwdtitle').text(projNameDetail);

        mMsaterfwdStage($(this).closest("tr").find("#SpnStageId").html(), "ddlfwdStage", 5, 0, 2)
        mMsaterStage($(this).closest("tr").find("#SpnTimeStatusId").html(), "ddlfwdSubStage", 6, $(this).closest("tr").find("#SpnStageId").html(), $("#SpnStakeHolderId").html())

        /*mMsater($(this).closest("tr").find("#SpnTimeActionId").html(), "ddlfwdAction", 9, $(this).closest("tr").find("#SpnTimeStatusId").html())*/
        mMsater($(this).closest("tr").find("#SpnTimeActionId").html(), "ddlfwdAction", 7, $(this).closest("tr").find("#SpnTimeStatusId").html())
        mMsaterFwdTo($(this).closest("tr").find("#SpnTimeToUnitId").html(), "ddlfwdFwdTo", 8, 0, $(this).closest("tr").find("#SpnStakeHolderId").html());

        $("#spanFwdCurrentPslmId").html($(this).closest("tr").find("#SpnCurrentpsmId").html())
        $("#spanFwdProjectId").html($(this).closest("tr").find("#SpnCurrentProjId").html())
        $("#SpnFwdStakeHolderId").html($(this).closest("tr").find("#SpnStakeHolderId").html())


        $('#ProjFwd').modal('show');

        $(".Fwdtitle").html("Projects Obsn To Unit");

        $(".ProjectsFwd").removeClass("d-none");
        $(".Attmenthistory").addClass("d-none");





        //GetAllComments($(this).closest("tr").find("#SpnCurrentProjId").html());
    });

    $("#ProjectName").click(function () {
      
        var row = $(this).closest('tr');
        IsReadInbox($(this).closest("tr").find("#SpnCurrentpsmId").html());  
        IsReadNotification($(this).closest("tr").find("#SpnCurrentProjId").html(), 2);
        $(this).closest("tr").removeClass("bold-text")
        window.location.reload();



        /*$(this).closest("tr").removeClass("bold-text");*/
    });

    //$(".ProjName").click(function () {
    //    debugger;
    //    var row = $(this).closest("tr");
    //    var currentPsmIdElement = row.find("#SpnCurrentpsmId");
    //    console.log("Divyanshu",currentPsmIdElement); // Check if the element is found
    //    console.log("DivyanshuHtml",currentPsmIdElement.html()); // Check the content of the element
    //});

    $("#btn-ibutton").click(function () {
        $('#Projibutton').modal('show');
    });
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

            CheckFwdCondition($("#spanFwdCurrentPslmId").html());

        }
    });

    $("#btnAttchMultiforpsmid").click(function () {


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
function CheckFwdCondition(CurrentPslmId) {

    var userdata =
    {
        "ProjId": $("#spanFwdProjectId").html(),
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
                    if ($("#ddlfwdSubStage").val() != 1) {
                        Swal.fire({
                            icon: "error",
                            title: "Oops...",
                            text: "Sub Stage Already Approved / Completed!",

                        });
                    }
                    else {
                        Swal.fire({
                            icon: "error",
                            title: "Oops...",
                            text: "Project Already Sent For Comments!",

                        });
                    }
                }
                else if (response == false) {

                    AttechHistory();
                    SaveFwdTo(CurrentPslmId);
                }
            }

        }
    });


}
function SaveFwdTo(CurrentPslmId) {


    var userdata =
    {
        "ProjId": $("#spanFwdProjectId").html(),
        /* "StatusId": $("#ddlfwdSubStage").val(),*/
        "StatusActionsMappingId": $("#ddlfwdAction").val(),
        "Remarks": $("#txtRemarksfwd").val(),
        "ToUnitId": $("#ddlfwdFwdTo").val(),

        "TimeStamp": $("#TimeStampToProjfwd").val()
    };
    $.ajax({
        url: '/Projects/FwdToProject',
        type: 'POST',
        data: userdata,
        success: function (response) {
            console.log(response);
            if (response != null) {
                $("#spanCurrentPslmId").html(response.psmId);
                FwdProjConfirm(CurrentPslmId);
                $(".Fwdtitle").html("Projects Attch Details");
                $(".ProjectsFwd").addClass("d-none");
                $(".Attmenthistory").removeClass("d-none");
                AddNotification($("#spanFwdProjectId").html(), 2, $("#ddlfwdFwdTo").val());
                IsReadNotification($("#spanFwdProjectId").html(), 2);
            }

        }
    });
}
function Updateundo(ProjId, PslmId, UndoRemarks, StageId) {
    var userdata =
    {
        "ProjectId": ProjId,
        "PsmId": PslmId,
        "Remarks": UndoRemarks,
        "StageId": StageId


    };
    $.ajax({
        url: '/Projects/UndoProject',
        type: 'POST',
        data: userdata,
        success: function (response) {
            console.log(response);
            if (response != null) {
                if (response == 2) {
                    alert("Project Successfully Undo");
                    location.reload();
                }


            }

        }
    });

}
function reset() {

    $("#ddlfwdStage").val('');
    $("#ddlfwdSubStage").val('');
    $("#ddlfwdAction").val('');
    $("#ddlfwdFwdTo").val('');
    $("#txtRemarksfwd").val('');
    $("#TimeStampToProjfwd").val('');
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
function UndoNotification(ProjId, type, ToUnitId) {

    $.ajax({
        url: '/Notification/UndoNotification',
        type: 'POST',
        data: {
            "ProjId": ProjId,
            "type": type,
            "ToUnitId": ToUnitId
        },
        success: function (response) {
            console.log(response);

        }
    });
}